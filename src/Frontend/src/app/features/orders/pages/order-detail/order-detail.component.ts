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

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
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
  private readonly chatSignalr = inject(ChatSignalRService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  readonly messageText = signal('');
  readonly sendingMessage = signal(false);
  readonly cancelling = signal(false);
  readonly typingUser = signal<string | null>(null);
  private typingTimeout: ReturnType<typeof setTimeout> | null = null;
  private lastTypingNotify = 0;
  private currentUserId: string | null = null;

  ngOnInit() {
    this.auth.user$.subscribe(user => { this.currentUserId = user?.sub ?? null; });
    this.store.loadById(this.id());
    this.initChat();
  }

  ngOnDestroy() {
    this.chatSignalr.leaveOrderGroup(this.id());
    this.cleanupTyping();
  }

  private async initChat() {
    await this.chatSignalr.start();
    await this.chatSignalr.joinOrderGroup(this.id());
    this.chatApi.markAsRead(this.id()).subscribe();
    this.chatSignalr.markOrderRead(this.id());

    this.chatSignalr.onMessageReceived = (message) => {
      if (message.userId === this.currentUserId) return;
      this.store.appendMessage(message);
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

  onMessageInput() {
    const now = Date.now();
    if (now - this.lastTypingNotify > 3000) {
      this.lastTypingNotify = now;
      this.chatSignalr.userTyping(this.id());
    }
    if (this.typingTimeout) clearTimeout(this.typingTimeout);
    this.typingTimeout = setTimeout(() => {
      this.chatSignalr.userStoppedTyping(this.id());
    }, 1500);
  }

  private cleanupTyping() {
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
      this.typingTimeout = null;
    }
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
    this.chatSignalr.userStoppedTyping(this.id());
    this.orderApi.sendMessage(this.id(), { message: text, type: MessageTypeDto.Text }).subscribe({
      next: () => {
        this.messageText.set('');
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

  protected readonly OrderStatusDto = OrderStatusDto;
  protected readonly MessageTypeDto = MessageTypeDto;
}
