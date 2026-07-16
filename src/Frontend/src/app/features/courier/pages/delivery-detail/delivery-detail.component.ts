import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  OnDestroy,
  inject,
  signal,
  input,
} from '@angular/core';
import { CurrencyPipe, DatePipe, SlicePipe } from '@angular/common';
import { Router } from '@angular/router';
import { DeliveryApiService } from '../../../../core/services/api/delivery-api.service';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { GeolocationService } from '../../../../core/services/geolocation.service';
import {
  getAssignmentStatusLabel,
  getAssignmentStatusClass,
  canAccept,
  canReject,
  canPickup,
  canDeliver,
} from '../../../../shared/utils/assignment-status.utils';
import { getOrderStatusLabel, getOrderStatusClass } from '../../../../shared/utils/order-status.utils';
import { AssignmentStatusDto } from '../../../../core/models';
import type { DeliveryAssignmentResponseDto, OrderResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-delivery-detail',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, SlicePipe],
  templateUrl: './delivery-detail.component.html',
  styleUrl: './delivery-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeliveryDetailComponent implements OnInit, OnDestroy {
  readonly id = input.required<string>();

  private readonly api = inject(DeliveryApiService);
  private readonly orderApi = inject(OrderApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly geolocation = inject(GeolocationService);

  readonly loading = signal(true);
  readonly assignment = signal<DeliveryAssignmentResponseDto | null>(null);
  readonly order = signal<OrderResponseDto | null>(null);
  readonly processing = signal(false);

  private locationInterval: ReturnType<typeof setInterval> | null = null;

  ngOnInit() {
    this.loadAssignment();
  }

  ngOnDestroy() {
    this.stopLocationPolling();
  }

  loadAssignment() {
    this.loading.set(true);
    // The my-deliveries endpoint returns assignments, but we need to get one by id
    // We'll fetch from my-deliveries and find by id, or use the order endpoint
    this.api.getMyDeliveries(1, 100).subscribe({
      next: (res) => {
        const found = res.items.find(a => a.id === this.id());
        if (found) {
          this.assignment.set(found);
          this.orderApi.getById(found.orderId).subscribe({
            next: (order) => {
              this.order.set(order);
              this.loading.set(false);
              if (this.isActive()) {
                this.startLocationPolling();
              }
            },
            error: () => this.loading.set(false),
          });
        } else {
          this.loading.set(false);
        }
      },
      error: () => this.loading.set(false),
    });
  }

  isActive(): boolean {
    const a = this.assignment();
    if (!a) return false;
    return a.status === AssignmentStatusDto.Accepted ||
           a.status === AssignmentStatusDto.PickedUp ||
           a.status === AssignmentStatusDto.InTransit;
  }

  getStatusLabel(status: number): string {
    return getAssignmentStatusLabel(status);
  }

  getStatusClass(status: number): string {
    return getAssignmentStatusClass(status);
  }

  getOrderStatusLabel(status: number): string {
    return getOrderStatusLabel(status);
  }

  getOrderStatusClass(status: number): string {
    return getOrderStatusClass(status);
  }

  canAccept(): boolean {
    return canAccept(this.assignment()?.status ?? -1);
  }

  canReject(): boolean {
    return canReject(this.assignment()?.status ?? -1);
  }

  canPickup(): boolean {
    return canPickup(this.assignment()?.status ?? -1);
  }

  canDeliver(): boolean {
    return canDeliver(this.assignment()?.status ?? -1);
  }

  acceptAssignment() {
    const a = this.assignment();
    if (!a) return;
    this.processing.set(true);
    this.api.acceptAssignment(a.id).subscribe({
      next: (updated) => {
        this.assignment.set(updated);
        this.toast.success('Entrega aceita com sucesso!');
        this.startLocationPolling();
        this.processing.set(false);
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao aceitar entrega.');
        this.processing.set(false);
      },
    });
  }

  rejectAssignment() {
    const a = this.assignment();
    if (!a) return;
    this.processing.set(true);
    this.api.rejectAssignment(a.id).subscribe({
      next: (updated) => {
        this.assignment.set(updated);
        this.toast.success('Entrega rejeitada.');
        this.processing.set(false);
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao rejeitar entrega.');
        this.processing.set(false);
      },
    });
  }

  pickupOrder() {
    const a = this.assignment();
    if (!a) return;
    this.processing.set(true);
    this.api.pickupOrder(a.id).subscribe({
      next: (updated) => {
        this.assignment.set(updated);
        this.toast.success('Pedido coletado! Siga para a entrega.');
        this.processing.set(false);
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao coletar pedido.');
        this.processing.set(false);
      },
    });
  }

  deliverOrder() {
    const a = this.assignment();
    if (!a) return;
    this.processing.set(true);
    this.api.deliverOrder(a.id).subscribe({
      next: (updated) => {
        this.assignment.set(updated);
        this.stopLocationPolling();
        this.toast.success('Entrega confirmada!');
        this.processing.set(false);
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao confirmar entrega.');
        this.processing.set(false);
      },
    });
  }

  goBack() {
    this.router.navigate(['/courier/deliveries']);
  }

  private startLocationPolling() {
    this.stopLocationPolling();
    this.sendLocation();
    this.locationInterval = setInterval(() => this.sendLocation(), 5000);
  }

  private stopLocationPolling() {
    if (this.locationInterval) {
      clearInterval(this.locationInterval);
      this.locationInterval = null;
    }
  }

  private sendLocation() {
    this.geolocation.getCurrentPosition().subscribe({
      next: (pos) => {
        this.api.updateLocation({
          latitude: pos.latitude,
          longitude: pos.longitude,
        }).subscribe();
      },
    });
  }

  protected readonly AssignmentStatusDto = AssignmentStatusDto;
}
