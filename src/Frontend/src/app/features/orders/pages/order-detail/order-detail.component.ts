import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '@auth0/auth0-angular';
import { OrdersStore } from '../../store/orders.store';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ChatApiService } from '../../../../core/services/api/chat-api.service';
import { ChatSignalRService } from '../../../../core/services/chat-signalr.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OrderStatusDto, MessageTypeDto } from '../../../../core/models';
import { OrderChatOffcanvasComponent } from '../../../../shared/components/order-chat-offcanvas/order-chat-offcanvas.component';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, OrderChatOffcanvasComponent],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderDetailComponent implements OnInit, OnDestroy {
  readonly id = input.required<string>();
  readonly store = inject(OrdersStore);
  private readonly auth = inject(AuthService);
  private readonly orderApi = inject(OrderApiService);
  private readonly chatApi = inject(ChatApiService);
  protected readonly chatSignalr = inject(ChatSignalRService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  readonly sendingMessage = signal(false);
  readonly cancelling = signal(false);
  readonly updatingStatus = signal(false);
  readonly typingUser = signal<string | null>(null);
  readonly expandedImage = signal<string | null>(null);
  readonly chatOpen = signal(false);


  private currentUserId: string | null = null;

  ngOnInit() {
    this.auth.user$.subscribe(user => { this.currentUserId = user?.sub ?? null; });
    this.store.loadById(this.id());
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
      this.store.appendMessage(message);
      if (this.chatOpen()) {
        this.chatApi.markAsRead(this.id()).subscribe();
        this.chatSignalr.markOrderRead(this.id());
      }
    };

    this.chatSignalr.onMessagesRead = (orderId) => {
      if (orderId === this.id() && this.currentUserId) {
        this.store.markMessagesAsRead(this.currentUserId);
      }
    };

    this.chatSignalr.onUserTyping = (orderId) => {
      if (orderId !== this.id()) return;
      this.typingUser.set('Lojista');
    };

    this.chatSignalr.onUserStoppedTyping = (orderId) => {
      if (orderId !== this.id()) return;
      this.typingUser.set(null);
    };
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

  acceptProposal() {
    if (!confirm('Aceitar esta proposta de valor e entrega?')) return;
    this.updatingStatus.set(true);
    this.orderApi.updateStatus(this.id(), { status: OrderStatusDto.AwaitingPayment }).subscribe({
      next: () => {
        this.updatingStatus.set(false);
        this.toast.success('Proposta aceita! O pedido aguarda pagamento.');
        this.store.loadById(this.id());
      },
      error: (err) => {
        this.updatingStatus.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao aceitar proposta.');
      }
    });
  }

  declineProposal() {
    if (!confirm('Tem certeza que deseja recusar a proposta? O pedido será recusado.')) return;
    this.updatingStatus.set(true);
    this.orderApi.updateStatus(this.id(), { status: OrderStatusDto.Declined }).subscribe({
      next: () => {
        this.updatingStatus.set(false);
        this.toast.success('Proposta recusada.');
        this.store.loadById(this.id());
      },
      error: (err) => {
        this.updatingStatus.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao recusar proposta.');
      }
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

  goToOrders() {
    this.router.navigate(['/orders']);
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
