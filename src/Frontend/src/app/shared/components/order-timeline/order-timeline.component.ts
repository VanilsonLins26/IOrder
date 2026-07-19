import {
  Component,
  ChangeDetectionStrategy,
  input,
  computed,
} from '@angular/core';
import { OrderStatusDto } from '../../../core/models';

interface TimelineStep {
  label: string;
  sublabel: string;
  statuses: OrderStatusDto[];
  icon: string;
}

const STEPS: TimelineStep[] = [
  {
    label: 'Pedido Realizado',
    sublabel: 'Em análise',
    statuses: [OrderStatusDto.Pending, OrderStatusDto.Negotiating],
    icon: '🧾',
  },
  {
    label: 'Aguardando Pagamento',
    sublabel: 'Confirme o pagamento',
    statuses: [OrderStatusDto.AwaitingPayment],
    icon: '⏳',
  },
  {
    label: 'Em Preparação',
    sublabel: 'Seu pedido está sendo feito',
    statuses: [OrderStatusDto.Paid, OrderStatusDto.Preparing],
    icon: '⚡',
  },
  {
    label: 'Encomenda Pronta',
    sublabel: 'Pronto para entrega/retirada',
    statuses: [OrderStatusDto.Ready],
    icon: '📦',
  },
  {
    label: 'Saiu para Entrega',
    sublabel: 'O entregador está a caminho',
    statuses: [OrderStatusDto.OutForDelivery],
    icon: '🚚',
  },
  {
    label: 'Entregue',
    sublabel: 'Aproveite!',
    statuses: [OrderStatusDto.Delivered],
    icon: '🎉',
  },
];

@Component({
  selector: 'app-order-timeline',
  standalone: true,
  imports: [],
  templateUrl: './order-timeline.component.html',
  styleUrl: './order-timeline.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderTimelineComponent {
  readonly status = input.required<OrderStatusDto>();

  readonly steps = STEPS;

  readonly hasError = computed(() => {
    const s = this.status();
    return s === OrderStatusDto.Cancelled || s === OrderStatusDto.Declined;
  });

  readonly errorLabel = computed(() =>
    this.status() === OrderStatusDto.Cancelled ? 'Cancelado' : 'Recusado',
  );

  readonly activeIndex = computed(() => {
    if (this.hasError()) return -1;
    return STEPS.findIndex((s) => s.statuses.includes(this.status()));
  });

  /** 0–100 progress for the connecting bar */
  readonly progressPercent = computed(() => {
    const idx = this.activeIndex();
    if (idx < 0) return 0;
    return Math.round((idx / (STEPS.length - 1)) * 100);
  });

  isCompleted(i: number): boolean {
    return !this.hasError() && i < this.activeIndex();
  }

  isActive(i: number): boolean {
    return !this.hasError() && i === this.activeIndex();
  }
}
