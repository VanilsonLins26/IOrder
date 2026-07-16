import { Component, ChangeDetectionStrategy, OnInit, inject, signal } from '@angular/core';
import { DatePipe, SlicePipe } from '@angular/common';
import { Router } from '@angular/router';
import { DeliveryApiService } from '../../../../core/services/api/delivery-api.service';
import { AuthService } from '@auth0/auth0-angular';
import { toSignal } from '@angular/core/rxjs-interop';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { getAssignmentStatusLabel, getAssignmentStatusClass } from '../../../../shared/utils/assignment-status.utils';
import { AssignmentStatusDto } from '../../../../core/models';
import type { DeliveryAssignmentResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-courier-dashboard',
  standalone: true,
  imports: [DatePipe, SlicePipe, EmptyStateComponent, LoadingSkeletonComponent],
  templateUrl: './courier-dashboard.component.html',
  styleUrl: './courier-dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourierDashboardComponent implements OnInit {
  private readonly api = inject(DeliveryApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly user = toSignal(this.auth.user$);
  readonly loading = signal(true);
  readonly pendingAssignments = signal<DeliveryAssignmentResponseDto[]>([]);
  readonly activeAssignments = signal<DeliveryAssignmentResponseDto[]>([]);

  ngOnInit() {
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

  goToMyDeliveries(): void {
    this.router.navigate(['/courier/deliveries']);
  }

  protected readonly AssignmentStatusDto = AssignmentStatusDto;
}
