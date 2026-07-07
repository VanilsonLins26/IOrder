import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { OrdersStore } from '../../store/orders.store';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [CommonModule, RouterLink, EmptyStateComponent, LoadingSkeletonComponent],
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
    const labels: Record<number, string> = {
      0: 'Pendente',
      1: 'Negociando',
      2: 'Aguardando Pagamento',
      3: 'Pago',
      4: 'Preparando',
      5: 'Pronto',
      6: 'Entregue',
      7: 'Cancelado',
      8: 'Recusado',
    };
    return labels[status] ?? 'Desconhecido';
  }

  getStatusClass(status: number): string {
    const classes: Record<number, string> = {
      0: 'status--pending',
      1: 'status--negotiating',
      2: 'status--awaiting',
      3: 'status--paid',
      4: 'status--preparing',
      5: 'status--ready',
      6: 'status--delivered',
      7: 'status--cancelled',
      8: 'status--declined',
    };
    return classes[status] ?? '';
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
