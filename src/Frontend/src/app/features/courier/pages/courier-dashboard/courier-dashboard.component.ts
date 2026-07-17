import { Component, ChangeDetectionStrategy, OnInit, OnDestroy, inject, signal, DestroyRef } from '@angular/core';
import { DatePipe, SlicePipe, CurrencyPipe } from '@angular/common';
import { Router } from '@angular/router';
import { DeliveryApiService } from '../../../../core/services/api/delivery-api.service';
import { ChatSignalRService } from '../../../../core/services/chat-signalr.service';
import { AuthService } from '@auth0/auth0-angular';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { getAssignmentStatusLabel, getAssignmentStatusClass } from '../../../../shared/utils/assignment-status.utils';
import { AssignmentStatusDto } from '../../../../core/models';
import type { DeliveryAssignmentResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-courier-dashboard',
  standalone: true,
  imports: [DatePipe, SlicePipe, CurrencyPipe, EmptyStateComponent, LoadingSkeletonComponent],
  templateUrl: './courier-dashboard.component.html',
  styleUrl: './courier-dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourierDashboardComponent implements OnInit, OnDestroy {
  private readonly api = inject(DeliveryApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly chatSignalr = inject(ChatSignalRService);
  private readonly destroyRef = inject(DestroyRef);

  readonly user = toSignal(this.auth.user$);
  readonly loading = signal(true);
  readonly pendingAssignments = signal<DeliveryAssignmentResponseDto[]>([]);
  readonly activeAssignments = signal<DeliveryAssignmentResponseDto[]>([]);
  readonly availableDeliveries = signal<any[]>([]);

  ngOnInit() {
    this.loadData();

    // The group "Couriers" is joined in ChatSignalRService or needs to be joined?
    // Let's add a method to join group "Couriers"
    this.chatSignalr.joinGroup('Couriers');

    this.chatSignalr.onNewDeliveryAvailable.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.loadAvailableDeliveries();
    });
  }

  ngOnDestroy() {
    this.chatSignalr.leaveGroup('Couriers');
  }

  private loadData() {
    this.loading.set(true);
    this.api.getMyDeliveries(1, 50).subscribe({
      next: (res) => {
        this.pendingAssignments.set(res.items.filter(a => a.status === AssignmentStatusDto.Pending));
        this.activeAssignments.set(res.items.filter(a =>
          a.status === AssignmentStatusDto.Accepted ||
          a.status === AssignmentStatusDto.PickedUp ||
          a.status === AssignmentStatusDto.InTransit
        ));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
    
    this.loadAvailableDeliveries();
  }

  private loadAvailableDeliveries() {
    this.api.getAvailableDeliveries().subscribe({
      next: (res) => {
        this.availableDeliveries.set(res.items);
      },
      error: () => {}
    });
  }

  getStatusLabel(status: number): string {
    return getAssignmentStatusLabel(status);
  }

  getStatusClass(status: number): string {
    return getAssignmentStatusClass(status);
  }

  goToDeliveryDetail(assignmentId: string): void {
    this.router.navigate(['/courier/deliveries', assignmentId]);
  }

  acceptDelivery(orderId: string): void {
    this.api.acceptAssignment(orderId).subscribe({
      next: (res) => {
        this.goToDeliveryDetail(res.id);
      },
      error: () => {
        this.loadAvailableDeliveries();
      }
    });
  }

  goToMyDeliveries(): void {
    this.router.navigate(['/courier/deliveries']);
  }

  protected readonly AssignmentStatusDto = AssignmentStatusDto;
}
