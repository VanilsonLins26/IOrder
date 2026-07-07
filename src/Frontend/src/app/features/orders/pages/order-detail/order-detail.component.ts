import { ChangeDetectionStrategy, Component, OnInit, inject, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { OrdersStore } from '../../store/orders.store';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OrderStatusDto, MessageTypeDto } from '../../../../core/models';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderDetailComponent implements OnInit {
  readonly id = input.required<string>();
  readonly store = inject(OrdersStore);
  private readonly orderApi = inject(OrderApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  readonly messageText = signal('');
  readonly sendingMessage = signal(false);
  readonly cancelling = signal(false);

  ngOnInit() {
    this.store.loadById(this.id());
  }

  getStatusLabel(status: OrderStatusDto): string {
    const labels: Record<number, string> = {
      0: 'Pendente', 1: 'Negociando', 2: 'Aguardando Pagamento', 3: 'Pago',
      4: 'Preparando', 5: 'Pronto', 6: 'Entregue', 7: 'Cancelado', 8: 'Recusado',
    };
    return labels[status] ?? 'Desconhecido';
  }

  getStatusClass(status: OrderStatusDto): string {
    const classes: Record<number, string> = {
      0: 'status--pending', 1: 'status--negotiating', 2: 'status--awaiting',
      3: 'status--paid', 4: 'status--preparing', 5: 'status--ready',
      6: 'status--delivered', 7: 'status--cancelled', 8: 'status--declined',
    };
    return classes[status] ?? '';
  }

  cancelOrder() {
    if (!confirm('Tem certeza que deseja cancelar este pedido?')) return;
    this.cancelling.set(true);
    this.orderApi.updateStatus(this.id(), { status: OrderStatusDto.Cancelled }).subscribe({
      next: () => {
        this.cancelling.set(false);
        this.toast.success('Pedido cancelado.');
        this.store.loadById(this.id());
      },
      error: (err) => {
        this.cancelling.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao cancelar pedido.');
      },
    });
  }

  sendMessage() {
    const text = this.messageText().trim();
    if (!text) return;
    this.sendingMessage.set(true);
    this.orderApi.sendMessage(this.id(), { message: text, type: MessageTypeDto.Text }).subscribe({
      next: () => {
        this.messageText.set('');
        this.sendingMessage.set(false);
        this.toast.success('Mensagem enviada!');
        this.store.loadById(this.id());
      },
      error: (err) => {
        this.sendingMessage.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao enviar mensagem.');
      },
    });
  }

  goToOrders() {
    this.router.navigate(['/orders']);
  }

  protected readonly OrderStatusDto = OrderStatusDto;
  protected readonly MessageTypeDto = MessageTypeDto;
}
