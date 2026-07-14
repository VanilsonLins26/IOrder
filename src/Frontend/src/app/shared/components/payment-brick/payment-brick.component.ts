import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject, input, output, signal, effect, ElementRef, ViewChild } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ModalComponent } from '../modal/modal.component';
import { PaymentApiService } from '../../../core/services/api/payment-api.service';
import { UserCardApiService } from '../../../core/services/api/user-card-api.service';
import { ToastService } from '../../../core/services/toast.service';
import { PaymentStatusDto, type PaymentResponseDto, type UserCardResponseDto } from '../../../core/models';
import { loadStripe, Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js';

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

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly userEmail = signal('');

  readonly savedCards = signal<UserCardResponseDto[]>([]);
  readonly loadingCards = signal(false);
  readonly saveCardForFuture = signal(false);

  // Stripe
  private stripe: Stripe | null = null;
  private elements: StripeElements | null = null;
  private paymentElement: StripePaymentElement | null = null;
  private clientSecret: string | null = null;
  readonly stripeReady = signal(false);
  readonly processingPayment = signal(false);
  readonly selectedSavedCard = signal<string | null>(null);

  @ViewChild('paymentElementContainer') paymentElementContainer!: ElementRef;

  constructor() {
    effect(() => {
      if (this.isOpen()) {
        this.initializeStripe();
        this.loadSavedCards();
      } else {
        this.cleanupStripe();
      }
    });
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.cleanupStripe();
  }

  private loadSavedCards(): void {
    this.loadingCards.set(true);
    this.userCardApi.getAll().subscribe({
      next: (cards) => {
        this.savedCards.set(cards);
        this.loadingCards.set(false);
      },
      error: () => {
        console.error('Failed to load saved cards');
        this.loadingCards.set(false);
      },
    });
  }

  deleteCard(cardId: string): void {
    if (!confirm('Deseja realmente remover este cartão?')) return;

    this.userCardApi.delete(cardId).subscribe({
      next: () => {
        this.savedCards.update(cards => cards.filter(c => c.id !== cardId));
        this.toast.success('Cartão removido com sucesso.');
        if (this.selectedSavedCard() === cardId) {
          this.selectedSavedCard.set(null);
        }
      },
      error: () => this.toast.error('Erro ao remover o cartão.')
    });
  }

  selectCard(cardId: string): void {
    if (this.selectedSavedCard() === cardId) {
      this.selectedSavedCard.set(null);
    } else {
      this.selectedSavedCard.set(cardId);
    }
  }

  private async initializeStripe(): Promise<void> {
    this.cleanupStripe();
    this.loading.set(true);
    this.error.set(null);
    this.stripeReady.set(false);

    try {
      // 1. Get Publishable Key
      const keyResult = await firstValueFrom(this.paymentApi.getPublicKey());
      this.stripe = await loadStripe(keyResult.publicKey);

      if (!this.stripe) {
        throw new Error('Falha ao carregar o Stripe SDK.');
      }

      // 2. Create PaymentIntent on the backend
      const paymentIntentRes = await firstValueFrom(this.paymentApi.create({
        orderId: this.orderId()
      }));

      if (!paymentIntentRes.clientSecret) {
        throw new Error('Falha ao gerar o pagamento.');
      }
      this.clientSecret = paymentIntentRes.clientSecret;

      // 3. Initialize Elements
      this.elements = this.stripe.elements({
        clientSecret: paymentIntentRes.clientSecret,
        appearance: {
          theme: 'stripe',
          variables: {
            colorPrimary: '#ea580c',
            colorBackground: '#ffffff',
            colorText: '#1e293b',
          }
        },
        loader: 'auto'
      });

      // 4. Create and mount the Payment Element
      this.paymentElement = this.elements.create('payment', {
        layout: 'tabs',
        defaultValues: {
          billingDetails: {
            email: this.userEmail() || this.payerEmail()
          }
        }
      });

      // Wait a tick for the view to render the container
      setTimeout(() => {
        if (this.paymentElementContainer) {
          this.paymentElement!.mount(this.paymentElementContainer.nativeElement);
          this.paymentElement!.on('ready', () => {
            this.stripeReady.set(true);
            this.loading.set(false);
          });
        }
      }, 0);

    } catch (err: any) {
      this.error.set(err.message || 'Erro ao inicializar o pagamento.');
      this.loading.set(false);
    }
  }

  async processPayment(): Promise<void> {
    if (!this.stripe || (!this.elements && !this.selectedSavedCard())) return;

    this.processingPayment.set(true);
    this.error.set(null);

    // Get final email
    const emailEl = document.getElementById('payment-email') as HTMLInputElement;
    const finalEmail = emailEl?.value || this.userEmail() || this.payerEmail();

    try {
      if (this.selectedSavedCard() && this.clientSecret) {
        const { error, paymentIntent } = await this.stripe.confirmCardPayment(this.clientSecret, {
          payment_method: this.selectedSavedCard()!
        });

        if (error) {
          this.error.set(error.message || 'Falha ao processar o pagamento com o cartão salvo.');
          this.processingPayment.set(false);
        }
        return;
      }

      const { error, paymentIntent } = await this.stripe.confirmPayment({
        elements: this.elements!,
        confirmParams: {
          payment_method_data: {
            billing_details: {
              email: finalEmail
            }
          },
          setup_future_usage: this.saveCardForFuture() ? 'off_session' : undefined
        },
        redirect: 'if_required',
      });

      if (error) {
        this.error.set(error.message || 'Falha ao processar o pagamento.');
        this.processingPayment.set(false);
      } else if (paymentIntent && (paymentIntent.status === 'succeeded' || paymentIntent.status === 'processing' || paymentIntent.status === 'requires_action')) {
        // Se require_action for um PIX ou Boleto que não redireciona (Stripe vai mostrar UI). 
        // Mas se não usar redirect: 'if_required' ele redirecionaria.
        // No caso de redirect: 'if_required', PIX e Boleto mostrarão instruções na própria UI do Element?
        // Sim, o Stripe gerencia a exibição do QR Code / Boleto na própria tela ou dispara webhook.
        
        // Let's emit paymentCreated so the parent knows the payment flow has been finalized on UI.
        // It won't close immediately if it requires action within the modal.
        // Wait, if it requires action, we shouldn't close the modal immediately. 
        // Let's check status:
        if (paymentIntent.status === 'requires_action') {
           // Stripe shows the next step (Pix QR code, Boleto, 3DS) automatically.
           this.processingPayment.set(false);
        } else {
           // Succeeded or processing (e.g., waiting for async confirmation)
           this.toast.success('Processamento concluído.');
           this.paymentCreated.emit({ id: paymentIntent.id } as PaymentResponseDto); // dummy object, wait for webhook
           this.processingPayment.set(false);
           this.close.emit();
        }
      }
    } catch (err: any) {
      this.error.set(err.message || 'Erro inesperado.');
      this.processingPayment.set(false);
    }
  }

  private cleanupStripe(): void {
    if (this.paymentElement) {
      this.paymentElement.destroy();
      this.paymentElement = null;
    }
    this.elements = null;
  }
}
