import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  OnDestroy,
  inject,
  signal,
  input,
  effect,
  DestroyRef
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CurrencyPipe, DatePipe, SlicePipe } from '@angular/common';
import { Router } from '@angular/router';
import { DeliveryApiService } from '../../../../core/services/api/delivery-api.service';
import { OrderApiService } from '../../../../core/services/api/order-api.service';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { FakeGpsService, Coordinates } from '../../../../core/services/fake-gps.service';
import { DeliveryMapComponent } from '../../../../shared/components/delivery-map/delivery-map.component';
import {
  getAssignmentStatusLabel,
  getAssignmentStatusClass,
  canAccept,
  canReject,
  canPickup,
  canStartTransit,
  canDeliver,
} from '../../../../shared/utils/assignment-status.utils';
import { getOrderStatusLabel, getOrderStatusClass } from '../../../../shared/utils/order-status.utils';
import { AssignmentStatusDto } from '../../../../core/models';
import type { DeliveryAssignmentResponseDto, OrderResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-delivery-detail',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, SlicePipe, DeliveryMapComponent],
  templateUrl: './delivery-detail.component.html',
  styleUrl: './delivery-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeliveryDetailComponent implements OnInit, OnDestroy {
  readonly id = input.required<string>();

  private readonly api = inject(DeliveryApiService);
  private readonly orderApi = inject(OrderApiService);
  private readonly storeApi = inject(StoreApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly fakeGps = inject(FakeGpsService);
  private readonly destroyRef = inject(DestroyRef);

  readonly loading = signal(true);
  readonly assignment = signal<DeliveryAssignmentResponseDto | null>(null);
  readonly order = signal<OrderResponseDto | null>(null);
  readonly processing = signal(false);

  readonly storeLocation = signal<Coordinates | null>(null);
  readonly clientLocation = signal<Coordinates | null>(null);
  readonly courierLocation = signal<Coordinates | null>(null);

  constructor() {
    this.fakeGps.currentPos$.pipe(takeUntilDestroyed()).subscribe(pos => {
      this.courierLocation.set(pos);
    });
  }

  ngOnInit() {
    this.loadAssignment();
  }

  ngOnDestroy() {
    this.fakeGps.stopTracking();
  }

  loadAssignment() {
    this.loading.set(true);
    this.api.getMyDeliveries(1, 100).subscribe({
      next: (res) => {
        const found = res.items.find(a => a.id === this.id());
        if (found) {
          this.assignment.set(found);
          this.orderApi.getById(found.orderId).subscribe({
            next: (order) => {
              this.order.set(order);
              this.loading.set(false);
              
              if (this.isOutForDelivery()) {
                this.initLocations(order.storeId, found.courierUserId);
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

  isOutForDelivery(): boolean {
    const a = this.assignment();
    if (!a) return false;
    return a.status === AssignmentStatusDto.PickedUp || a.status === AssignmentStatusDto.InTransit;
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

  canStartTransit(): boolean {
    return canStartTransit(this.assignment()?.status ?? -1);
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
        const orderInfo = this.order();
        if (orderInfo) {
          this.initLocations(orderInfo.storeId, updated.courierUserId);
        }
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao coletar pedido.');
        this.processing.set(false);
      },
    });
  }

  startTransit() {
    const a = this.assignment();
    if (!a) return;
    this.processing.set(true);
    this.api.startTransit(a.id).subscribe({
      next: (updated) => {
        this.assignment.set(updated);
        this.toast.success('Saiu para entrega!');
        this.processing.set(false);
      },
      error: (err) => {
        this.toast.error(err.error?.errors?.[0] || 'Erro ao iniciar trǽnsito.');
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
        this.fakeGps.stopTracking();
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

  private initLocations(storeId: string, courierUserId: string) {
    this.storeApi.getById(storeId).subscribe({
      next: (store) => {
        if (store.latitude && store.longitude) {
          const sLoc = { lat: store.latitude, lng: store.longitude };
          this.storeLocation.set(sLoc);
          
          // Fake client location (portfolio sim)
          const cLoc = { lat: store.latitude - 0.015, lng: store.longitude + 0.020 };
          this.clientLocation.set(cLoc);

          this.fakeGps.startTracking(courierUserId, sLoc, cLoc, 1.5);
        }
      }
    });
  }

  protected readonly AssignmentStatusDto = AssignmentStatusDto;
}
