import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { DashboardApiService } from '../../../../core/services/api/dashboard-api.service';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import type { DashboardMetricsResponseDto, OrderResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CurrencyPipe, LoadingSkeletonComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly dashboardApi = inject(DashboardApiService);
  private readonly orderApi = inject(OrderApiService);

  readonly metrics = signal<DashboardMetricsResponseDto | null>(null);
  readonly recentOrders = signal<OrderResponseDto[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    this.loading.set(true);
    
    // Carregar as métricas do dashboard
    this.dashboardApi.getMetrics().subscribe({
      next: (data) => {
        this.metrics.set(data);
        
        // Carregar pedidos recentes (os 5 últimos) para exibir no dashboard
        this.orderApi.getStoreOrders().subscribe({
          next: (orders) => {
            this.recentOrders.set(orders.items.slice(0, 5));
            this.loading.set(false);
          },
          error: () => {
            this.loading.set(false);
          }
        });
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  // Função auxiliar para mapear o status do pedido para cores e labels amigáveis no HTML
  getStatusLabel(status: number | string): string {
    const statusMap: Record<string, string> = {
      '0': 'Pendente', 'Pending': 'Pendente',
      '1': 'Negociando', 'Negotiating': 'Negociando',
      '2': 'Aguard. Pgto', 'AwaitingPayment': 'Aguard. Pgto',
      '3': 'Pago', 'Paid': 'Pago',
      '4': 'Preparando', 'Preparing': 'Preparando',
      '5': 'Pronto', 'Ready': 'Pronto',
      '6': 'Entregue', 'Delivered': 'Entregue',
      '7': 'Cancelado', 'Cancelled': 'Cancelado',
      '8': 'Recusado', 'Declined': 'Recusado',
      '9': 'Em Rota', 'OutForDelivery': 'Em Rota'
    };
    return statusMap[status.toString()] || 'Desconhecido';
  }

  getStatusClass(status: number | string): string {
    const s = status.toString();
    if (s === '0' || s === 'Pending') return 'status--pending';
    if (s === '4' || s === 'Preparing' || s === '5' || s === 'Ready') return 'status--preparing';
    if (s === '9' || s === 'OutForDelivery') return 'status--delivery';
    if (s === '6' || s === 'Delivered') return 'status--delivered';
    if (s === '7' || s === 'Cancelled' || s === '8' || s === 'Declined') return 'status--cancelled';
    return 'status--default';
  }
}
