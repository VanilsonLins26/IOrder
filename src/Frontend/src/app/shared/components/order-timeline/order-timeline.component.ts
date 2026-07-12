import { Component, ChangeDetectionStrategy, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderStatusDto } from '../../../core/models';

interface TimelineStep {
  label: string;
  statuses: OrderStatusDto[];
  icon: string;
}

@Component({
  selector: 'app-order-timeline',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-timeline.component.html',
  styleUrls: ['./order-timeline.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderTimelineComponent {
  status = input.required<OrderStatusDto>();

  steps: TimelineStep[] = [
    { label: 'Pedido Realizado', statuses: [OrderStatusDto.Pending, OrderStatusDto.Negotiating], icon: '📝' },
    { label: 'Aguardando Pagamento', statuses: [OrderStatusDto.AwaitingPayment], icon: '💳' },
    { label: 'Pagamento Confirmado', statuses: [OrderStatusDto.Paid], icon: '💰' },
    { label: 'Preparando', statuses: [OrderStatusDto.Preparing, OrderStatusDto.Ready], icon: '🍳' },
    { label: 'Finalizado', statuses: [OrderStatusDto.Delivered], icon: '✅' },
  ];

  isStepCompleted(stepIndex: number): boolean {
    const currentStatus = this.status();
    
    if (currentStatus === OrderStatusDto.Cancelled || currentStatus === OrderStatusDto.Declined) {
      return false; // Error state handled differently
    }

    const currentStepIndex = this.steps.findIndex(s => s.statuses.includes(currentStatus));
    return currentStepIndex >= stepIndex;
  }

  isStepActive(stepIndex: number): boolean {
    const currentStatus = this.status();
    const currentStepIndex = this.steps.findIndex(s => s.statuses.includes(currentStatus));
    return currentStepIndex === stepIndex;
  }
  
  hasError(): boolean {
    const s = this.status();
    return s === OrderStatusDto.Cancelled || s === OrderStatusDto.Declined;
  }
}
