import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject, input, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { SlicePipe, DatePipe, CurrencyPipe } from '@angular/common';
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
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';
import { getOrderStatusLabel, getOrderStatusClass } from '../../../../shared/utils/order-status.utils';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [SlicePipe, DatePipe, CurrencyPipe, RouterLink, FormsModule, OrderChatOffcanvasComponent, ConfirmationModalComponent],
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
  readonly currentUser = toSignal(this.auth.user$);

  readonly showCancelConfirm = signal(false);
  readonly showAcceptConfirm = signal(false);
  readonly showDeclineConfirm = signal(false);


  ngOnInit() {
    this.store.loadById(this.id());
    this.initChat();
  }

  ngOnDestroy() {
    this.chatSignalr.leaveOrderGroup(this.id());
  }

  private async initChat() {
    await this.chatSignalr.start();
    await this.chatSignalr.joinOrderGroup(this.id());

    this.chatSignalr.onMessageReceived = (message) => {
      this.store.appendMessage(message);
      if (this.chatOpen()) {
        this.chatApi.markAsRead(this.id()).subscribe();
        this.chatSignalr.markOrderRead(this.id());
      }
    };

    this.chatSignalr.onMessagesRead = (orderId) => {
      const uid = this.currentUser()?.sub ?? null;
      if (orderId === this.id() && uid) {
        this.store.markMessagesAsRead(uid);
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
    return getOrderStatusLabel(status);
  }

  getStatusClass(status: OrderStatusDto): string {
    return getOrderStatusClass(status);
  }

  cancelOrder() {
    this.cancelling.set(true);
    this.orderApi.updateStatus(this.id(), { status: OrderStatusDto.Cancelled }).subscribe({
      next: () => {
        this.cancelling.set(false);
        this.showCancelConfirm.set(false);
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
    this.updatingStatus.set(true);
    this.orderApi.updateStatus(this.id(), { status: OrderStatusDto.AwaitingPayment }).subscribe({
      next: () => {
        this.updatingStatus.set(false);
        this.showAcceptConfirm.set(false);
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
    this.updatingStatus.set(true);
    this.orderApi.updateStatus(this.id(), { status: OrderStatusDto.Declined }).subscribe({
      next: () => {
        this.updatingStatus.set(false);
        this.showDeclineConfirm.set(false);
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

  openChat() {
    this.chatOpen.set(true);
    this.chatApi.markAsRead(this.id()).subscribe();
    this.chatSignalr.markOrderRead(this.id());
  }

  protected readonly OrderStatusDto = OrderStatusDto;
  protected readonly MessageTypeDto = MessageTypeDto;
}
