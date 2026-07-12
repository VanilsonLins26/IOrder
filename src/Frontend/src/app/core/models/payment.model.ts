export enum PaymentMethodDto {
  Pix = 0,
  CreditCard = 1,
  Boleto = 2,
}

export enum PaymentStatusDto {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Refunded = 3,
  Cancelled = 4,
}

export interface PaymentResponseDto {
  id: string;
  orderId: string;
  amount: number;
  method: PaymentMethodDto;
  status: PaymentStatusDto;
  pixQrCode: string | null;
  pixCopyPaste: string | null;
  boletoUrl: string | null;
  boletoBarcode: string | null;
  cardLastFourDigits: string | null;
  installments: number | null;
  installmentAmount: string | null;
  createdAt: string;
  paidAt: string | null;
}

export interface CreatePaymentRequestDto {
  orderId: string;
  method: PaymentMethodDto;
  cardToken?: string | null;
  installments?: number | null;
  payerEmail: string;
  payerIdentificationType?: string | null;
  payerIdentificationNumber?: string | null;
}

export interface PublicKeyResponseDto {
  publicKey: string;
}

export interface PaymentStatusChangedEvent {
  orderId: string;
  paymentId: string;
  status: PaymentStatusDto;
  paidAt: string | null;
}
