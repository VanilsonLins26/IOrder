import { OrderStatusDto } from '../../core/models';

const LABELS: Record<number, string> = {
  [OrderStatusDto.Pending]: 'Pendente',
  [OrderStatusDto.Negotiating]: 'Negociando',
  [OrderStatusDto.AwaitingPayment]: 'Aguardando Pagamento',
  [OrderStatusDto.Paid]: 'Pago',
  [OrderStatusDto.Preparing]: 'Preparando',
  [OrderStatusDto.Ready]: 'Pronto',
  [OrderStatusDto.Delivered]: 'Entregue',
  [OrderStatusDto.Cancelled]: 'Cancelado',
  [OrderStatusDto.Declined]: 'Recusado',
  [OrderStatusDto.OutForDelivery]: 'Saiu para Entrega',
};

const STATUS_CLASSES: Record<number, string> = {
  [OrderStatusDto.Pending]: 'status--pending',
  [OrderStatusDto.Negotiating]: 'status--negotiating',
  [OrderStatusDto.AwaitingPayment]: 'status--awaiting',
  [OrderStatusDto.Paid]: 'status--paid',
  [OrderStatusDto.Preparing]: 'status--preparing',
  [OrderStatusDto.Ready]: 'status--ready',
  [OrderStatusDto.Delivered]: 'status--delivered',
  [OrderStatusDto.Cancelled]: 'status--cancelled',
  [OrderStatusDto.Declined]: 'status--declined',
  [OrderStatusDto.OutForDelivery]: 'status--out-for-delivery',
};

const NEXT_STATUSES: Record<number, { status: OrderStatusDto; label: string }[]> = {
  [OrderStatusDto.Pending]: [
    { status: OrderStatusDto.AwaitingPayment, label: 'Aceitar' },
    { status: OrderStatusDto.Declined, label: 'Recusar' },
  ],
  [OrderStatusDto.Negotiating]: [],
  [OrderStatusDto.AwaitingPayment]: [
    { status: OrderStatusDto.Cancelled, label: 'Cancelar' },
  ],
  [OrderStatusDto.Paid]: [
    { status: OrderStatusDto.Preparing, label: 'Iniciar Preparo' },
  ],
  [OrderStatusDto.Preparing]: [
    { status: OrderStatusDto.Ready, label: 'Marcar como Pronto' },
  ],
  [OrderStatusDto.Ready]: [],
  [OrderStatusDto.OutForDelivery]: [],
};

export function getOrderStatusLabel(status: number): string {
  return LABELS[status] ?? 'Desconhecido';
}

export function getOrderStatusClass(status: number): string {
  return STATUS_CLASSES[status] ?? '';
}

export function getOrderNextStatuses(status: number): { status: OrderStatusDto; label: string }[] {
  return NEXT_STATUSES[status] ?? [];
}
