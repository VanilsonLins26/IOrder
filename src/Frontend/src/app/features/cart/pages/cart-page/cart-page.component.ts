import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CartStore } from '../../store/cart.store';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, EmptyStateComponent, LoadingSkeletonComponent],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartPageComponent implements OnInit {
  readonly cartStore = inject(CartStore);
  private readonly router = inject(Router);
  private readonly orderApi = inject(OrderApiService);
  private readonly toast = inject(ToastService);

  readonly couponInput = signal('');
  readonly creatingOrder = signal(false);

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
    this.orderApi.create({}).subscribe({
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
}
