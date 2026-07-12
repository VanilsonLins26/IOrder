import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { SlicePipe, DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { OrdersStore } from '../../../orders/store/orders.store';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OrderStatusDto } from '../../../../core/models';
import type { OrderResponseDto } from '../../../../core/models';
import { getOrderStatusLabel, getOrderStatusClass, getOrderNextStatuses } from '../../../../shared/utils/order-status.utils';

@Component({
  selector: 'app-store-orders',
  standalone: true,
  imports: [SlicePipe, DatePipe, CurrencyPipe, RouterLink],
  templateUrl: './store-orders.component.html',
  styleUrl: './store-orders.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreOrdersComponent implements OnInit {
  readonly store = inject(OrdersStore);
  private readonly orderApi = inject(OrderApiService);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);

  readonly currentUser = toSignal(this.auth.user$);

  ngOnInit() {
    this.store.loadStoreOrders({ page: 1 });
  }

  getStatusLabel(status: OrderStatusDto): string {
    return getOrderStatusLabel(status);
  }

  getStatusClass(status: OrderStatusDto): string {
    return getOrderStatusClass(status);
  }

  getUnreadCount(order: OrderResponseDto): number {
    const uid = this.currentUser()?.sub;
    if (!uid || !order.messages) return 0;
    return order.messages.filter(m => m.readAt == null && m.userId !== uid).length;
  }

  getNextStatuses(status: OrderStatusDto): { status: OrderStatusDto; label: string }[] {
    return getOrderNextStatuses(status);
  }

  updateStatus(orderId: string, status: OrderStatusDto) {
    this.orderApi.updateStatus(orderId, { status }).subscribe({
      next: () => {
        this.toast.success('Status atualizado!');
        this.store.loadStoreOrders({ page: this.store.currentPage() });
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao atualizar status.');
      },
    });
  }

  changePage(page: number) {
    if (page < 1 || page > this.store.totalPages()) return;
    this.store.goToPage(page);
    this.store.loadStoreOrders({ page });
  }

  protected readonly trackByOrderId = (_: number, item: { id: string }) => item.id;
  protected readonly OrderStatusDto = OrderStatusDto;
}
