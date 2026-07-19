import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject, input, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { SlicePipe, DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '@auth0/auth0-angular';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ChatApiService } from '../../../../core/services/api/chat-api.service';
import { DeliveryApiService } from '../../../../core/services/api/delivery-api.service';
import { ChatSignalRService } from '../../../../core/services/chat-signalr.service';
import { ToastService } from '../../../../core/services/toast.service';
import { CurrencyInputDirective } from '../../../../shared/directives/currency-input.directive';
import { OrderStatusDto, MessageTypeDto, OrderMessageResponseDto } from '../../../../core/models';
import type { OrderResponseDto } from '../../../../core/models';
import { OrderChatOffcanvasComponent } from '../../../../shared/components/order-chat-offcanvas/order-chat-offcanvas.component';
import { OrderTimelineComponent } from '../../../../shared/components/order-timeline/order-timeline.component';
import { getOrderStatusLabel, getOrderStatusClass, getOrderNextStatuses } from '../../../../shared/utils/order-status.utils';
import { AdminStore } from '../../store/admin.store';
import { generateAvailableDates, generateTimeSlots } from '../../../../core/utils/opening-hours.utils';
import { effect, untracked } from '@angular/core';

@Component({
  selector: 'app-store-order-detail',
  standalone: true,
  imports: [SlicePipe, DatePipe, CurrencyPipe, RouterLink, FormsModule, CurrencyInputDirective, OrderChatOffcanvasComponent, OrderTimelineComponent],
  templateUrl: './store-order-detail.component.html',
  styleUrl: './store-order-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreOrderDetailComponent implements OnInit, OnDestroy {
  readonly id = input.required<string>();
  private readonly auth = inject(AuthService);
  private readonly orderApi = inject(OrderApiService);
  private readonly chatApi = inject(ChatApiService);
  private readonly deliveryApi = inject(DeliveryApiService);
  protected readonly chatSignalr = inject(ChatSignalRService);
  private readonly toast = inject(ToastService);
  private readonly destroyRef = inject(DestroyRef);

  readonly order = signal<OrderResponseDto | null>(null);
  readonly loading = signal(false);
  readonly sendingMessage = signal(false);
  readonly typingUser = signal<string | null>(null);
  readonly expandedImage = signal<string | null>(null);
  readonly chatOpen = signal(false);

  readonly showAssignCourier = signal(false);
  readonly courierUserId = signal('');
  readonly assigningCourier = signal(false);
  readonly earlyDeliveryRequested = signal(false);
  readonly broadcastingOffer = signal(false);

  readonly isSearchingCourier = computed(() => {
    return this.order()?.isSearchingCourier ?? false;
  });

  readonly canBroadcastDeliveryOffer = computed(() => {
    const o = this.order();
    if (!o) return false;

    let isTimeAllowed = o.requestedEarlyDelivery;
    if (!isTimeAllowed && o.deliveryDate) {
      const deliveryTime = new Date(o.deliveryDate).getTime();
      const currentTime = new Date().getTime();
      const thirtyMinsInMs = 30 * 60 * 1000;
      if (currentTime >= deliveryTime - thirtyMinsInMs) {
        isTimeAllowed = true;
      }
    }

    return (o.status === OrderStatusDto.Paid || o.status === OrderStatusDto.Preparing || o.status === OrderStatusDto.Ready)
      && o.deliveryType === 0
      && o.deliveryPartner === 0
      && !o.activeAssignment
      && !o.isSearchingCourier
      && isTimeAllowed;
  });

  readonly showNegotiate = signal(false);
  readonly proposedAmount = signal<number | null>(null);
  readonly proposedDateString = signal('');
  readonly proposedTimeString = signal('');
  readonly shopkeeperNotes = signal('');
  readonly currentUser = toSignal(this.auth.user$);
  readonly adminStore = inject(AdminStore);

  readonly availableDates = computed(() => {
    return generateAvailableDates(this.adminStore.myStore()?.openingHours || [], 7);
  });

  readonly availableTimes = computed(() => {
    return generateTimeSlots(this.proposedDateString(), this.adminStore.myStore()?.openingHours || [], 30);
  });

  constructor() {
    effect(() => {
      const dates = this.availableDates();
      if (dates.length > 0 && !this.proposedDateString()) {
        untracked(() => this.proposedDateString.set(dates[0].date));
      }
    });

    effect(() => {
      const times = this.availableTimes();
      if (times.length > 0 && !times.some(t => t.value === this.proposedTimeString())) {
        untracked(() => this.proposedTimeString.set(times[0].value));
      }
    });
  }

  readonly unreadMessagesCount = computed(() => {
    const o = this.order();
    const uid = this.currentUser()?.sub;
    if (!o || !uid) return 0;
    return o.messages.filter(m => m.readAt == null && m.userId !== uid).length;
  });

  readonly canMarkReady = computed(() => {
    const o = this.order();
    if (!o) return false;
    return o.status === OrderStatusDto.Preparing;
  });



  readonly canSendForDelivery = computed(() => {
    const o = this.order();
    if (!o) return false;
    return o.status === OrderStatusDto.Ready
      && o.deliveryType === 0
      && o.deliveryPartner === 1;
  });

  readonly canMarkDelivered = computed(() => {
    const o = this.order();
    if (!o) return false;
    return o.status === OrderStatusDto.OutForDelivery;
  });

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

    this.chatSignalr.onMessageReceived.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((message) => {
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
        if (this.chatOpen()) {
          this.chatApi.markAsRead(this.id()).subscribe({
            next: () => {
              const current = this.order();
              if (current) {
                const now = new Date().toISOString();
                const updatedMessages = current.messages.map(m => {
                  if (m.userId !== this.currentUser()?.sub && !m.readAt) {
                    return { ...m, readAt: now };
                  }
                  return m;
                });
                this.order.set({ ...current, messages: updatedMessages });
              }
              this.chatSignalr.markOrderRead(this.id());
            }
          });
        }
      }
    });

    this.chatSignalr.onMessagesRead.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((orderId) => {
      if (orderId.toLowerCase() === this.id().toLowerCase()) {
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
    });

    this.chatSignalr.onUserTyping.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((orderId) => {
      if (orderId.toLowerCase() !== this.id().toLowerCase()) return;
      this.typingUser.set('Cliente');
    });

    this.chatSignalr.onUserStoppedTyping.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((orderId) => {
      if (orderId.toLowerCase() !== this.id().toLowerCase()) return;
      this.typingUser.set(null);
    });

    this.chatSignalr.onPaymentStatusChanged.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((event) => {
      if (event.orderId.toLowerCase() !== this.id().toLowerCase()) return;
      this.loadOrder();
    });

    this.chatSignalr.onOrderStatusChanged.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((event) => {
      const eId = event.orderId || (event as any).OrderId;
      if (eId && eId.toLowerCase() === this.id().toLowerCase()) {
        this.loadOrder();
      }
    });

    this.chatSignalr.onEarlyDeliveryRequested.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((event) => {
      if (event.orderId.toLowerCase() === this.id().toLowerCase()) {
        this.earlyDeliveryRequested.set(true);
        this.loadOrder();
        this.toast.info('O cliente quer receber o pedido antes!');
      }
    });
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
        this.proposedAmount.set(o.totalAmount - o.deliveryFee);
        if (o.deliveryDate) {
          const date = new Date(o.deliveryDate);
          date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
          const isoStr = date.toISOString();
          this.proposedDateString.set(isoStr.slice(0, 10));
          this.proposedTimeString.set(isoStr.slice(11, 16));
        } else {
          this.proposedDateString.set('');
          this.proposedTimeString.set('');
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
    let proposedDeliveryDate: string | null = null;
    const dateStr = this.proposedDateString();
    const timeStr = this.proposedTimeString();
    if (dateStr && timeStr) {
      const [yyyy, mm, dd] = dateStr.split('-').map(Number);
      const [hh, min] = timeStr.split(':').map(Number);
      const d = new Date(yyyy, mm - 1, dd, hh, min);
      proposedDeliveryDate = d.toISOString();
    }

    this.orderApi.negotiate(this.id(), {
      proposedTotalAmount: this.proposedAmount(),
      proposedDeliveryDate,
      shopkeeperNotes: this.shopkeeperNotes() || null,
    }).subscribe({
      next: () => {
        this.toast.success('Proposta enviada!');
        this.showNegotiate.set(false);
        this.proposedAmount.set(null);
        this.proposedDateString.set('');
        this.proposedTimeString.set('');
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
    this.chatApi.markAsRead(this.id()).subscribe({
      next: () => {
        const current = this.order();
        if (current) {
          const now = new Date().toISOString();
          const updatedMessages = current.messages.map(m => {
            if (m.userId !== this.currentUser()?.sub && !m.readAt) {
              return { ...m, readAt: now };
            }
            return m;
          });
          this.order.set({ ...current, messages: updatedMessages });
        }
        this.chatSignalr.markOrderRead(this.id());
      }
    });
  }



  protected readonly OrderStatusDto = OrderStatusDto;
  protected readonly MessageTypeDto = MessageTypeDto;

  markOutForDelivery() {
    this.orderApi.markAsOutForDelivery(this.id()).subscribe({
      next: () => {
        this.toast.success('Pedido saiu para entrega!');
        this.loadOrder();
      },
      error: (err) => this.toast.error(err.error?.errors?.[0] || 'Erro ao marcar saída para entrega.'),
    });
  }

  markDelivered() {
    this.updateStatus(OrderStatusDto.Delivered);
  }

  broadcastDeliveryOffer() {
    this.broadcastingOffer.set(true);
    this.deliveryApi.broadcastDeliveryOffer(this.id()).subscribe({
      next: () => {
        this.toast.success('Entregadores notificados! Aguardando aceite...');
        this.broadcastingOffer.set(false);
        this.loadOrder();
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao solicitar entregador.');
        this.broadcastingOffer.set(false);
      },
    });
  }
}
