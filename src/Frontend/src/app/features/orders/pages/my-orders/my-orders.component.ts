import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { SlicePipe, DatePipe, CurrencyPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { ChatNotificationService } from '../../../../core/services/chat-notification.service';
import { OrdersStore } from '../../store/orders.store';
import { getOrderStatusLabel, getOrderStatusClass } from '../../../../shared/utils/order-status.utils';
import type { OrderResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [SlicePipe, DatePipe, CurrencyPipe, RouterLink],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyOrdersComponent implements OnInit {
  readonly store = inject(OrdersStore);
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  private readonly chatNotification = inject(ChatNotificationService);
  
  readonly currentUser = toSignal(this.auth.user$);

  ngOnInit() {
    this.store.loadUserOrders({ page: 1 });
  }

  getStatusLabel(status: number): string {
    return getOrderStatusLabel(status);
  }

  getStatusClass(status: number): string {
    return getOrderStatusClass(status);
  }

  getUnreadCount(order: OrderResponseDto): number {
    const uid = this.currentUser()?.sub;
    if (!uid || !order.messages) return 0;
    return order.messages.filter(m => m.readAt == null && m.userId !== uid).length;
  }

  hasUnreadOnPreviousPage(): boolean {
    const orders = this.store.orders();
    if (!orders.length) return false;
    const firstOrderDate = new Date(orders[0].createdAt).getTime();
    
    const unread = this.chatNotification.unreadConversations();
    const result = unread.some(c => new Date(c.createdAt).getTime() > firstOrderDate);
    console.log('hasUnreadOnPreviousPage:', { firstOrderDate, unread, result });
    return result;
  }

  hasUnreadOnNextPage(): boolean {
    const orders = this.store.orders();
    if (!orders.length) return false;
    const lastOrderDate = new Date(orders[orders.length - 1].createdAt).getTime();
    
    const unread = this.chatNotification.unreadConversations();
    const result = unread.some(c => new Date(c.createdAt).getTime() < lastOrderDate);
    console.log('hasUnreadOnNextPage:', { lastOrderDate, unread, result });
    return result;
  }

  changePage(page: number) {
    if (page < 1 || page > this.store.totalPages()) return;
    this.store.goToPage(page);
    this.store.loadUserOrders({ page });
  }

  goToStores() {
    this.router.navigate(['/stores']);
  }

  protected readonly trackByOrderId = (_: number, item: { id: string }) => item.id;
}
