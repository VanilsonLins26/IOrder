import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CartStore } from '../../store/cart.store';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { generateAvailableDates, generateTimeSlots } from '../../../../core/utils/opening-hours.utils';
import type { OpeningHourResponse } from '../../../../core/models';
import { effect, untracked, computed } from '@angular/core';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CurrencyPipe, FormsModule, EmptyStateComponent, LoadingSkeletonComponent],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartPageComponent implements OnInit {
  readonly cartStore = inject(CartStore);
  private readonly router = inject(Router);
  private readonly orderApi = inject(OrderApiService);
  private readonly storeApi = inject(StoreApiService);
  private readonly toast = inject(ToastService);

  readonly couponInput = signal('');
  readonly deliveryDate = signal('');
  readonly deliveryTime = signal('');
  readonly customerNotes = signal('');
  readonly creatingOrder = signal(false);
  readonly storeHours = signal<OpeningHourResponse[]>([]);

  readonly availableDates = computed(() => {
    return generateAvailableDates(this.storeHours(), 7);
  });

  readonly availableTimes = computed(() => {
    return generateTimeSlots(this.deliveryDate(), this.storeHours(), 30);
  });

  constructor() {
    effect(() => {
      const dates = this.availableDates();
      if (dates.length > 0 && !this.deliveryDate()) {
        untracked(() => this.deliveryDate.set(dates[0].date));
      }
    });

    effect(() => {
      const times = this.availableTimes();
      if (times.length > 0 && !times.includes(this.deliveryTime())) {
        untracked(() => this.deliveryTime.set(times[0]));
      }
    });

    effect(() => {
      const items = this.cartStore.items();
      if (items && items.length > 0) {
        const storeId = items[0].storeId;
        untracked(() => {
          this.storeApi.getById(storeId).subscribe({
            next: (store) => this.storeHours.set(store.openingHours)
          });
        });
      }
    });
  }

  ngOnInit() {
    this.cartStore.loadCart();
  }

  removeItem(itemId: string) {
    this.cartStore.removeItem(itemId);
  }

  changeQuantity(itemId: string, quantity: number) {
    if (quantity < 1) return;
    this.cartStore.changeQuantity({ cartItemId: itemId, newQuantity: quantity });
  }

  applyCoupon() {
    const code = this.couponInput().trim();
    if (!code) return;
    this.cartStore.applyCoupon(code);
    this.couponInput.set('');
  }

  clearCart() {
    this.cartStore.clearCart();
  }

  createOrder() {
    this.creatingOrder.set(true);

    let deliveryDate: string | null = null;
    const dateVal = this.deliveryDate();
    const timeVal = this.deliveryTime();
    if (dateVal && timeVal) {
      const [yyyy, mm, dd] = dateVal.split('-').map(Number);
      const [hh, min] = timeVal.split(':').map(Number);
      const d = new Date(yyyy, mm - 1, dd, hh, min);
      deliveryDate = d.toISOString();
    }

    this.orderApi.create({
      deliveryDate,
      customerNotes: this.customerNotes() || null,
    }).subscribe({
      next: (order) => {
        this.creatingOrder.set(false);
        this.cartStore.resetCart();
        this.toast.success('Pedido criado com sucesso!');
        this.router.navigate(['/orders', order.id]);
      },
      error: (err) => {
        this.creatingOrder.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao criar pedido.');
      },
    });
  }

  goToStores() {
    this.router.navigate(['/stores']);
  }

  protected readonly trackByItemId = (_: number, item: { id: string }) => item.id;

  protected today(): string {
    return new Date().toISOString().slice(0, 10);
  }
}
