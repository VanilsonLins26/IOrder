import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { SlicePipe, DatePipe, CurrencyPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { OrdersStore } from '../../store/orders.store';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { getOrderStatusLabel, getOrderStatusClass } from '../../../../shared/utils/order-status.utils';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [SlicePipe, DatePipe, CurrencyPipe, RouterLink, EmptyStateComponent, LoadingSkeletonComponent],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyOrdersComponent implements OnInit {
  readonly store = inject(OrdersStore);
  private readonly router = inject(Router);

  ngOnInit() {
    this.store.loadUserOrders({ page: 1 });
  }

  getStatusLabel(status: number): string {
    return getOrderStatusLabel(status);
  }

  getStatusClass(status: number): string {
    return getOrderStatusClass(status);
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
