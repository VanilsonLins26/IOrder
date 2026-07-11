import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject, input, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { SlicePipe, DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '@auth0/auth0-angular';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ChatApiService } from '../../../../core/services/api/chat-api.service';
import { ChatSignalRService } from '../../../../core/services/chat-signalr.service';
import { ToastService } from '../../../../core/services/toast.service';
import { CurrencyInputDirective } from '../../../../shared/directives/currency-input.directive';
import { OrderStatusDto, MessageTypeDto, OrderMessageResponseDto } from '../../../../core/models';
import type { OrderResponseDto } from '../../../../core/models';
import { OrderChatOffcanvasComponent } from '../../../../shared/components/order-chat-offcanvas/order-chat-offcanvas.component';
import { getOrderStatusLabel, getOrderStatusClass, getOrderNextStatuses } from '../../../../shared/utils/order-status.utils';

@Component({
  selector: 'app-store-order-detail',
  standalone: true,
  imports: [SlicePipe, DatePipe, CurrencyPipe, RouterLink, FormsModule, CurrencyInputDirective, OrderChatOffcanvasComponent],
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
  readonly currentUser = toSignal(this.auth.user$);


  ngOnInit() {
    this.loadOrder();
    this.initChat();
  }

  ngOnDestroy() {
    this.chatSignalr.leaveOrderGroup(this.id());
  }

  private async initChat() {
    await this.chatSignalr.start();
    await this.chatSignalr.joinOrderGroup(this.id());

    this.chatSignalr.onMessageReceived = (message) => {
      const current = this.order();
      if (!current) return;
      const uid = this.currentUser()?.sub ?? '';
      if (uid && message.userId === uid) {
        const existingIdx = current.messages.findIndex(
          m => m.id.startsWith('temp-') && m.message === message.message && m.userId === uid
        );
        if (existingIdx !== -1) {
          const updated = [...current.messages];
          updated[existingIdx] = message;
          this.order.set({ ...current, messages: updated });
        } else {
          this.order.set({ ...current, messages: [...current.messages, message] });
        }
      } else {
        const alreadyExists = current.messages.some(m => m.id === message.id);
        if (!alreadyExists) {
          this.order.set({ ...current, messages: [...current.messages, message] });
        }
      }
      if (this.chatOpen()) {
        this.chatApi.markAsRead(this.id()).subscribe();
        this.chatSignalr.markOrderRead(this.id());
      }
    };

    this.chatSignalr.onMessagesRead = (orderId) => {
      if (orderId === this.id()) {
        const current = this.order();
        if (!current) return;
        const now = new Date().toISOString();
        const updatedMessages = current.messages.map(m => {
          if (m.userId === this.currentUser()?.sub && !m.readAt) {
            return { ...m, readAt: now };
          }
          return m;
        });
        this.order.set({ ...current, messages: updatedMessages });
      }
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
    return getOrderStatusLabel(status);
  }

  getStatusClass(status: OrderStatusDto): string {
    return getOrderStatusClass(status);
  }

  getNextStatuses(status: OrderStatusDto): { status: OrderStatusDto; label: string }[] {
    return getOrderNextStatuses(status);
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

    const tempId = `temp-${crypto.randomUUID()}`;
    const uid = this.currentUser()?.sub ?? '';
    const tempMessage: OrderMessageResponseDto = {
      id: tempId,
      userId: uid,
      userRole: 'ShopKeeper',
      message: text,
      sentAt: new Date().toISOString(),
      type: MessageTypeDto.Text,
      proposedTotalAmount: null,
      proposedDeliveryDate: null,
      readAt: null,
      readByUserId: null,
    };
    const current = this.order();
    if (current) {
      this.order.set({ ...current, messages: [...current.messages, tempMessage] });
    }

    this.orderApi.sendMessage(this.id(), { message: text, type: MessageTypeDto.Text }).subscribe({
      next: () => {
        this.sendingMessage.set(false);
      },
      error: (err) => {
        this.sendingMessage.set(false);
        const c = this.order();
        if (c) {
          this.order.set({ ...c, messages: c.messages.filter(m => m.id !== tempId) });
        }
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

  openChat() {
    this.chatOpen.set(true);
    this.chatApi.markAsRead(this.id()).subscribe();
    this.chatSignalr.markOrderRead(this.id());
  }

  protected readonly OrderStatusDto = OrderStatusDto;
  protected readonly MessageTypeDto = MessageTypeDto;
}
