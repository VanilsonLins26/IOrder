import { ChangeDetectionStrategy, Component, ElementRef, ViewChild, effect, input, output, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderMessageResponseDto, OrderStatusDto, MessageTypeDto } from '../../../core/models';

@Component({
  selector: 'app-order-chat-offcanvas',
  standalone: true,
  imports: [DatePipe, CurrencyPipe, FormsModule],
  templateUrl: './order-chat-offcanvas.component.html',
  styleUrl: './order-chat-offcanvas.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrderChatOffcanvasComponent {
  readonly isOpen = input<boolean>(false);
  readonly messages = input.required<OrderMessageResponseDto[]>();
  readonly orderStatus = input.required<OrderStatusDto>();
  readonly userRole = input.required<'Client' | 'ShopKeeper'>();
  readonly typingUser = input<string | null>(null);
  readonly sendingMessage = input<boolean>(false);
  readonly updatingStatus = input<boolean>(false);

  readonly closeOffcanvas = output<void>();
  readonly onSendMessage = output<string>();
  readonly typingNotify = output<void>();
  readonly stoppedTypingNotify = output<void>();
  readonly onAcceptProposal = output<void>();
  readonly onDeclineProposal = output<void>();

  @ViewChild('messagesContainer') private messagesContainer!: ElementRef<HTMLDivElement>;

  readonly messageText = signal('');
  private typingTimeout: ReturnType<typeof setTimeout> | null = null;
  private lastTypingNotify = 0;

  constructor() {
    effect(() => {
      const msgs = this.messages();
      if (msgs.length > 0 && this.isOpen()) {
        setTimeout(() => this.scrollToBottom(), 50);
      }
    });
  }

  close() {
    this.closeOffcanvas.emit();
  }

  isLastProposal(msg: OrderMessageResponseDto): boolean {
    if (msg.type !== MessageTypeDto.Proposal) return false;
    const proposals = this.messages().filter(m => m.type === MessageTypeDto.Proposal);
    if (proposals.length === 0) return false;
    return proposals[proposals.length - 1].id === msg.id;
  }

  sendMessage() {
    const text = this.messageText().trim();
    if (!text) return;
    this.onSendMessage.emit(text);
    this.messageText.set('');
    this.stoppedTypingNotify.emit();
  }

  onInput() {
    const now = Date.now();
    if (now - this.lastTypingNotify > 3000) {
      this.lastTypingNotify = now;
      this.typingNotify.emit();
    }
    if (this.typingTimeout) clearTimeout(this.typingTimeout);
    this.typingTimeout = setTimeout(() => {
      this.stoppedTypingNotify.emit();
    }, 1500);
  }

  acceptProposal() {
    this.onAcceptProposal.emit();
  }

  declineProposal() {
    this.onDeclineProposal.emit();
  }

  isOwnMessage(msg: OrderMessageResponseDto): boolean {
    if (this.userRole() === 'ShopKeeper') {
      return msg.userRole === 'ShopKeeper';
    }
    return msg.userRole !== 'ShopKeeper';
  }

  getAuthorName(msg: OrderMessageResponseDto): string {
    if (this.isOwnMessage(msg)) return 'Você';
    return msg.userRole === 'ShopKeeper' ? 'Lojista' : 'Cliente';
  }

  private scrollToBottom() {
    if (this.messagesContainer?.nativeElement) {
      this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
    }
  }

  protected readonly OrderStatusDto = OrderStatusDto;
  protected readonly MessageTypeDto = MessageTypeDto;
}
