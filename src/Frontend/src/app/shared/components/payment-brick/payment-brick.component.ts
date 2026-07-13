import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject, input, output, signal } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ModalComponent } from '../modal/modal.component';
import { PaymentApiService } from '../../../core/services/api/payment-api.service';
import { UserCardApiService } from '../../../core/services/api/user-card-api.service';
import { ToastService } from '../../../core/services/toast.service';
import { PaymentMethodDto, PaymentStatusDto, type PaymentResponseDto, type UserCardResponseDto } from '../../../core/models';

@Component({
  selector: 'app-payment-brick',
  standalone: true,
  imports: [ModalComponent, CurrencyPipe, DecimalPipe, FormsModule],
  templateUrl: './payment-brick.component.html',
  styleUrl: './payment-brick.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaymentBrickComponent implements OnInit, OnDestroy {
  readonly orderId = input.required<string>();
  readonly totalAmount = input.required<number>();
  readonly isOpen = input.required<boolean>();
  readonly payerEmail = input<string>('');

  readonly close = output<void>();
  readonly paymentCreated = output<PaymentResponseDto>();

  private readonly paymentApi = inject(PaymentApiService);
  private readonly userCardApi = inject(UserCardApiService);
  private readonly toast = inject(ToastService);

  readonly selectedMethod = signal<PaymentMethodDto>(PaymentMethodDto.Pix);
  readonly loading = signal(false);
  readonly paymentResult = signal<PaymentResponseDto | null>(null);
  readonly error = signal<string | null>(null);
  readonly userEmail = signal('');
  readonly cardProcessing = signal(false);
  readonly mpReady = signal(false);

  readonly savedCards = signal<UserCardResponseDto[]>([]);
  readonly selectedCardId = signal<string | null>('new');
  readonly saveNewCard = signal<boolean>(false);
  readonly installmentsForSavedCard = signal<number>(1);

  private cardBrickInstance: any = null;
  private mpInitialized = false;

  ngOnInit(): void {
    this.loadSavedCards();
  }

  private loadSavedCards(): void {
    this.userCardApi.getAll().subscribe({
      next: (cards) => {
        this.savedCards.set(cards);
        if (cards.length > 0) {
          this.selectedCardId.set(cards[0].id);
        }
      },
      error: () => console.error('Failed to load saved cards'),
    });
  }

  async selectMethod(method: PaymentMethodDto): Promise<void> {
    this.cleanupCardBrick();
    this.selectedMethod.set(method);
    this.paymentResult.set(null);
    this.error.set(null);

    if (method === PaymentMethodDto.CreditCard && this.selectedCardId() === 'new') {
      await this.initCardBrick();
    }
  }

  async onCardSelectionChange(id: string): Promise<void> {
    this.selectedCardId.set(id);
    this.error.set(null);
    if (id === 'new') {
      await this.initCardBrick();
    } else {
      this.cleanupCardBrick();
    }
  }

  private async initCardBrick(): Promise<void> {
    if (this.mpInitialized) {
      this.mountCardBrick();
      return;
    }

    try {
      const { publicKey } = await firstValueFrom(this.paymentApi.getPublicKey());
      await this.loadMpSdk();

      const mp = new (window as any).MercadoPago(publicKey, { locale: 'pt-BR' });
      this.cardBrickInstance = mp.bricks();
      this.mpInitialized = true;
      this.mountCardBrick();
    } catch {
      this.error.set('Erro ao carregar processador de cartão.');
    }
  }

  private mountCardBrick(): void {
    this.mpReady.set(false);
    const container = document.getElementById('cardPaymentBrick_container');
    if (!container) {
      setTimeout(() => this.mountCardBrick(), 200);
      return;
    }

    try {
      this.cardBrickInstance?.create('cardPayment', 'cardPaymentBrick_container', {
        initialization: { amount: this.totalAmount() },
        callbacks: {
          onSubmit: (formData: any) => {
            return new Promise<void>((resolve, reject) => {
              this.processNewCardPayment(formData.token, formData.installments ?? 1, resolve, reject);
            });
          },
          onError: (error: any) => {
            this.error.set(error?.message || 'Erro no formulário de cartão.');
          },
          onReady: () => this.mpReady.set(true),
        },
      });
    } catch {
      this.error.set('Erro ao montar formulário de cartão.');
    }
  }

  private async processNewCardPayment(token: string, installments: number, resolve: () => void, reject: () => void): Promise<void> {
    this.cardProcessing.set(true);
    this.error.set(null);

    let savedCardId: string | null = null;
    let actualToken: string | null = token;

    if (this.saveNewCard()) {
      try {
        const savedCard = await firstValueFrom(this.userCardApi.save({ cardToken: token }));
        savedCardId = savedCard.id;
        actualToken = null; // Token was consumed, use savedCardId instead
      } catch (err: any) {
        this.error.set(err.error?.errors?.[0] || 'Erro ao salvar o cartão.');
        this.cardProcessing.set(false);
        reject();
        return;
      }
    }

    this.paymentApi.create({
      orderId: this.orderId(),
      method: PaymentMethodDto.CreditCard,
      cardToken: actualToken,
      savedCardId: savedCardId,
      installments,
      payerEmail: this.userEmail() || this.payerEmail(),
    }).subscribe({
      next: (result) => {
        this.paymentResult.set(result);
        this.cardProcessing.set(false);
        this.paymentCreated.emit(result);
        this.toast.success('Pagamento processado com sucesso!');
        if (this.saveNewCard()) {
          this.loadSavedCards(); // Refresh list if card was saved
        }
        resolve();
      },
      error: (err) => {
        this.cardProcessing.set(false);
        this.error.set(err.error?.errors?.[0] || 'Erro ao processar pagamento.');
        reject();
      },
    });
  }

  processSavedCardPayment(): void {
    const cardId = this.selectedCardId();
    if (!cardId || cardId === 'new') return;

    this.cardProcessing.set(true);
    this.error.set(null);

    this.paymentApi.create({
      orderId: this.orderId(),
      method: PaymentMethodDto.CreditCard,
      savedCardId: cardId,
      installments: this.installmentsForSavedCard(),
      payerEmail: this.userEmail() || this.payerEmail(),
    }).subscribe({
      next: (result) => {
        this.paymentResult.set(result);
        this.cardProcessing.set(false);
        this.paymentCreated.emit(result);
        this.toast.success('Pagamento processado com sucesso!');
      },
      error: (err) => {
        this.cardProcessing.set(false);
        this.error.set(err.error?.errors?.[0] || 'Erro ao processar pagamento com cartão salvo.');
      },
    });
  }

  processPix(): void {
    this.loading.set(true);
    this.error.set(null);

    this.paymentApi.create({
      orderId: this.orderId(),
      method: PaymentMethodDto.Pix,
      payerEmail: this.userEmail() || this.payerEmail(),
    }).subscribe({
      next: (result) => {
        this.paymentResult.set(result);
        this.loading.set(false);
        this.paymentCreated.emit(result);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.errors?.[0] || 'Erro ao gerar PIX.');
      },
    });
  }

  processBoleto(): void {
    this.loading.set(true);
    this.error.set(null);

    this.paymentApi.create({
      orderId: this.orderId(),
      method: PaymentMethodDto.Boleto,
      payerEmail: this.userEmail() || this.payerEmail(),
    }).subscribe({
      next: (result) => {
        this.paymentResult.set(result);
        this.loading.set(false);
        this.paymentCreated.emit(result);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.errors?.[0] || 'Erro ao gerar boleto.');
      },
    });
  }

  copyPixKey(): void {
    const key = this.paymentResult()?.pixCopyPaste;
    if (key) {
      navigator.clipboard.writeText(key);
      this.toast.success('Chave PIX copiada!');
    }
  }

  private async loadMpSdk(): Promise<void> {
    const src = 'https://sdk.mercadopago.com/js/v2';
    if (document.querySelector(`script[src="${src}"]`)) return;
    return new Promise((resolve, reject) => {
      const script = document.createElement('script');
      script.src = src;
      script.onload = () => resolve();
      script.onerror = () => reject();
      document.head.appendChild(script);
    });
  }

  private cleanupCardBrick(): void {
    const container = document.getElementById('cardPaymentBrick_container');
    if (container) container.innerHTML = '';
    this.mpReady.set(false);
  }

  ngOnDestroy(): void {
    this.cleanupCardBrick();
  }

  protected readonly PaymentMethodDto = PaymentMethodDto;
  protected readonly PaymentStatusDto = PaymentStatusDto;
}
