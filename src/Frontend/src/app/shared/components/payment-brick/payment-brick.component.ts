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
  readonly userCpf = signal('');
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
            const email = formData.payer?.email || this.userEmail() || this.payerEmail();
            const identType = formData.payer?.identification?.type || 'CPF';
            const identNumber = formData.payer?.identification?.number || this.userCpf() || '';

            return new Promise<void>((resolve, reject) => {
              this.processNewCardPayment(
                formData.token,
                formData.installments ?? 1,
                formData.payment_method_id ?? formData.paymentMethodId,
                formData.issuer_id?.toString() ?? formData.issuerId?.toString(),
                identType,
                identNumber,
                email,
                resolve,
                reject
              );
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

  private async processNewCardPayment(
    token: string,
    installments: number,
    cardPaymentMethodId: string | undefined,
    issuerId: string | undefined,
    identType: string | undefined,
    identNumber: string | undefined,
    email: string,
    resolve: () => void,
    reject: () => void
  ): Promise<void> {
    this.cardProcessing.set(true);
    this.error.set(null);

    let savedCardId: string | null = null;
    let actualToken: string | null = token;

    if (this.saveNewCard()) {
      try {
        const savedCard = await firstValueFrom(this.userCardApi.save({ cardToken: token, payerEmail: email }));
        const { publicKey } = await firstValueFrom(this.paymentApi.getPublicKey());
        await this.loadMpSdk();
        const mp = new (window as any).MercadoPago(publicKey, { locale: 'pt-BR' });
        const tokenResponse = await mp.createCardToken({ cardId: savedCard.gatewayCardId });
        if (!tokenResponse?.id) throw new Error('Falha ao tokenizar cartão salvo.');
        actualToken = tokenResponse.id;
        savedCardId = null;
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
      payerEmail: email,
      cardPaymentMethodId,
      issuerId,
      payerIdentificationType: identType,
      payerIdentificationNumber: identNumber,
    }).subscribe({
      next: (result) => {
        this.paymentResult.set(result);
        this.cardProcessing.set(false);
        this.paymentCreated.emit(result);
        this.toast.success('Pagamento processado com sucesso!');
        if (this.saveNewCard()) {
          this.loadSavedCards();
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

  async processSavedCardPayment(): Promise<void> {
    const cardId = this.selectedCardId();
    if (!cardId || cardId === 'new') return;

    this.cardProcessing.set(true);
    this.error.set(null);

    try {
      const userCard = this.savedCards().find(c => c.id === cardId);
      if (!userCard) throw new Error('Cartão não encontrado.');

      // We need to fetch the public key again to use the SDK
      const { publicKey } = await firstValueFrom(this.paymentApi.getPublicKey());
      await this.loadMpSdk();
      const mp = new (window as any).MercadoPago(publicKey, { locale: 'pt-BR' });
      
      const tokenResponse = await mp.createCardToken({
        cardId: userCard.gatewayCardId
      });

      if (!tokenResponse || !tokenResponse.id) {
          throw new Error('Falha ao tokenizar cartão salvo.');
      }

      const savedEmailEl = document.getElementById('payment-email') as HTMLInputElement;
      const savedCpfEl = document.getElementById('payment-cpf') as HTMLInputElement;
      const savedEmail = savedEmailEl?.value || this.userEmail() || this.payerEmail();
      const savedCpf = savedCpfEl?.value || this.userCpf() || '';
      const payment = await firstValueFrom(this.paymentApi.create({
        orderId: this.orderId(),
        method: PaymentMethodDto.CreditCard,
        cardToken: tokenResponse.id,
        installments: this.installmentsForSavedCard(),
        payerEmail: savedEmail,
        payerIdentificationType: 'CPF',
        payerIdentificationNumber: savedCpf,
      }));

      this.paymentResult.set(payment);
      this.cardProcessing.set(false);
      this.paymentCreated.emit(payment);
      this.toast.success('Pagamento processado com sucesso!');
    } catch (err: any) {
      this.cardProcessing.set(false);
      this.error.set(err.error?.errors?.[0] || err.message || 'Erro ao processar pagamento com cartão salvo.');
    }
  }

  processPix(): void {
    this.loading.set(true);
    this.error.set(null);

    const pixEmailEl = document.getElementById('payment-email') as HTMLInputElement;
    const pixEmail = pixEmailEl?.value || this.userEmail() || this.payerEmail();
    this.paymentApi.create({
      orderId: this.orderId(),
      method: PaymentMethodDto.Pix,
      payerEmail: pixEmail,
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

    const boletoEmailEl = document.getElementById('payment-email') as HTMLInputElement;
    const boletoEmail = boletoEmailEl?.value || this.userEmail() || this.payerEmail();
    this.paymentApi.create({
      orderId: this.orderId(),
      method: PaymentMethodDto.Boleto,
      payerEmail: boletoEmail,
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
