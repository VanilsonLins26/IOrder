import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrdersStore } from '../../../orders/store/orders.store';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OrderStatusDto } from '../../../../core/models';

@Component({
  selector: 'app-store-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './store-orders.component.html',
  styleUrl: './store-orders.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreOrdersComponent implements OnInit {
  readonly store = inject(OrdersStore);
  private readonly orderApi = inject(OrderApiService);
  private readonly toast = inject(ToastService);

  ngOnInit() {
    this.store.loadStoreOrders({ page: 1 });
  }

  getStatusLabel(status: OrderStatusDto): string {
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

  getStatusClass(status: OrderStatusDto): string {
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

  getNextStatuses(status: OrderStatusDto): { status: OrderStatusDto; label: string }[] {
    const map: Record<number, { status: OrderStatusDto; label: string }[]> = {
      [OrderStatusDto.Pending]: [
        { status: OrderStatusDto.AwaitingPayment, label: 'Aceitar' },
        { status: OrderStatusDto.Declined, label: 'Recusar' },
      ],
      [OrderStatusDto.AwaitingPayment]: [
        { status: OrderStatusDto.Paid, label: 'Confirmar Pagamento' },
        { status: OrderStatusDto.Cancelled, label: 'Cancelar' },
      ],
      [OrderStatusDto.Paid]: [
        { status: OrderStatusDto.Preparing, label: 'Iniciar Preparo' },
      ],
      [OrderStatusDto.Preparing]: [
        { status: OrderStatusDto.Ready, label: 'Marcar como Pronto' },
      ],
      [OrderStatusDto.Ready]: [
        { status: OrderStatusDto.Delivered, label: 'Confirmar Entrega' },
      ],
    };
    return map[status] ?? [];
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
