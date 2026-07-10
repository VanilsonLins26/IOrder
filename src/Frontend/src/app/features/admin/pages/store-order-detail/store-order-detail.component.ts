import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '@auth0/auth0-angular';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ChatApiService } from '../../../../core/services/api/chat-api.service';
import { ChatSignalRService } from '../../../../core/services/chat-signalr.service';
import { ToastService } from '../../../../core/services/toast.service';
import { CurrencyInputDirective } from '../../../../shared/directives/currency-input.directive';
import { OrderStatusDto, MessageTypeDto } from '../../../../core/models';
import type { OrderResponseDto } from '../../../../core/models';
import { OrderChatOffcanvasComponent } from '../../../../shared/components/order-chat-offcanvas/order-chat-offcanvas.component';

@Component({
  selector: 'app-store-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, CurrencyInputDirective, OrderChatOffcanvasComponent],
  templateUrl: './store-order-detail.component.html',
  styleUrl: './store-order-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreOrderDetailComponent implements OnInit, OnDestroy {
  readonly id = input.required<string>();
  private readonly auth = inject(AuthService);
  private readonly orderApi = inject(OrderApiService);
  private readonly chatApi = inject(ChatApiService);
  protected readonly chatSignalr = inject(ChatSignalRService);
  private readonly toast = inject(ToastService);

  readonly order = signal<OrderResponseDto | null>(null);
  readonly loading = signal(false);
  readonly sendingMessage = signal(false);
  readonly typingUser = signal<string | null>(null);
  readonly expandedImage = signal<string | null>(null);
  readonly chatOpen = signal(false);

  readonly showNegotiate = signal(false);
  readonly proposedAmount = signal<number | null>(null);
  readonly proposedDate = signal('');
  readonly shopkeeperNotes = signal('');

  private currentUserId: string | null = null;

  ngOnInit() {
    this.auth.user$.subscribe(user => { this.currentUserId = user?.sub ?? null; });
    this.loadOrder();
    this.initChat();
  }

  ngOnDestroy() {
    this.chatSignalr.leaveOrderGroup(this.id());
  }

  private async initChat() {
    await this.chatSignalr.start();
    await this.chatSignalr.joinOrderGroup(this.id());
    this.chatApi.markAsRead(this.id()).subscribe();
    this.chatSignalr.markOrderRead(this.id());

    this.chatSignalr.onMessageReceived = (message) => {
      const current = this.order();
      if (!current) return;
      const alreadyExists = current.messages.some(m => m.id === message.id);
      if (alreadyExists) return;
      this.order.set({ ...current, messages: [...current.messages, message] });
    };

    this.chatSignalr.onUserTyping = (orderId) => {
      if (orderId !== this.id()) return;
      this.typingUser.set('Cliente');
    };

    this.chatSignalr.onUserStoppedTyping = (orderId) => {
      if (orderId !== this.id()) return;
      this.typingUser.set(null);
    };
  }

  private loadOrder() {
    this.loading.set(true);
    this.orderApi.getById(this.id()).subscribe({
      next: (o) => { this.order.set(o); this.loading.set(false); },
      error: () => { this.loading.set(false); this.toast.error('Erro ao carregar pedido.'); },
    });
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

  getNextStatuses(status: OrderStatusDto): { status: OrderStatusDto; label: string }[] {
    const map: Record<number, { status: OrderStatusDto; label: string }[]> = {
      [OrderStatusDto.Pending]: [
        { status: OrderStatusDto.AwaitingPayment, label: 'Aceitar' },
        { status: OrderStatusDto.Declined, label: 'Recusar' },
      ],
      [OrderStatusDto.Negotiating]: [
        { status: OrderStatusDto.AwaitingPayment, label: 'Aceitar Proposta' },
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

  toggleNegotiate() {
    this.showNegotiate.set(!this.showNegotiate());
    if (this.showNegotiate()) {
      const o = this.order();
      if (o) {
        this.proposedAmount.set(o.totalAmount);
        if (o.deliveryDate) {
          const date = new Date(o.deliveryDate);
          date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
          this.proposedDate.set(date.toISOString().slice(0, 16));
        } else {
          this.proposedDate.set('');
        }
      }
    }
  }

  updateStatus(status: OrderStatusDto) {
    this.orderApi.updateStatus(this.id(), { status }).subscribe({
      next: () => {
        this.toast.success('Status atualizado!');
        this.loadOrder();
      },
      error: (err) => this.toast.error(err.error?.errors?.[0] || 'Erro ao atualizar status.'),
    });
  }

  sendMessage(text: string) {
    if (!text) return;
    this.sendingMessage.set(true);
    this.chatSignalr.userStoppedTyping(this.id());
    this.orderApi.sendMessage(this.id(), { message: text, type: MessageTypeDto.Text }).subscribe({
      next: () => {
        this.sendingMessage.set(false);
      },
      error: (err) => {
        this.sendingMessage.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao enviar mensagem.');
      },
    });
  }

  sendProposal() {
    this.orderApi.negotiate(this.id(), {
      proposedTotalAmount: this.proposedAmount(),
      proposedDeliveryDate: this.proposedDate() || null,
      shopkeeperNotes: this.shopkeeperNotes() || null,
    }).subscribe({
      next: () => {
        this.toast.success('Proposta enviada!');
        this.showNegotiate.set(false);
        this.proposedAmount.set(null);
        this.proposedDate.set('');
        this.shopkeeperNotes.set('');
        this.loadOrder();
      },
      error: (err) => this.toast.error(err.error?.errors?.[0] || 'Erro ao enviar proposta.'),
    });
  }

  openImage(url: string) {
    this.expandedImage.set(url);
  }

  closeImage() {
    this.expandedImage.set(null);
  }

  protected readonly OrderStatusDto = OrderStatusDto;
  protected readonly MessageTypeDto = MessageTypeDto;
}
