import { Component, ChangeDetectionStrategy, OnInit, inject, signal } from '@angular/core';
import { DatePipe, SlicePipe } from '@angular/common';
import { Router } from '@angular/router';
import { DeliveryApiService } from '../../../../core/services/api/delivery-api.service';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { getAssignmentStatusLabel, getAssignmentStatusClass } from '../../../../shared/utils/assignment-status.utils';
import type { DeliveryAssignmentResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-my-deliveries',
  standalone: true,
  imports: [DatePipe, SlicePipe],
  templateUrl: './my-deliveries.component.html',
  styleUrl: './my-deliveries.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyDeliveriesComponent implements OnInit {
  private readonly api = inject(DeliveryApiService);
  private readonly router = inject(Router);

  readonly loading = signal(true);
  readonly assignments = signal<DeliveryAssignmentResponseDto[]>([]);
  readonly currentPage = signal(1);
  readonly totalPages = signal(1);
  readonly hasNext = signal(false);
  readonly hasPrevious = signal(false);

  ngOnInit() {
    this.loadDeliveries(1);
  }

  loadDeliveries(page: number) {
    this.loading.set(true);
    this.api.getMyDeliveries(page, 10).subscribe({
      next: (res) => {
        this.assignments.set(res.items);
        this.currentPage.set(res.currentPage);
        this.totalPages.set(res.totalPages);
        this.hasNext.set(res.hasNext);
        this.hasPrevious.set(res.hasPrevious);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.loadDeliveries(page);
  }

  goToDetail(assignmentId: string) {
    this.router.navigate(['/courier/deliveries', assignmentId]);
  }

  getStatusLabel(status: number): string {
    return getAssignmentStatusLabel(status);
  }

  getStatusClass(status: number): string {
    return getAssignmentStatusClass(status);
  }

  protected readonly trackById = (_: number, item: DeliveryAssignmentResponseDto) => item.id;

  getPageRange(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const delta = 2;
    const range: number[] = [];
    for (let i = Math.max(1, current - delta); i <= Math.min(total, current + delta); i++) {
      range.push(i);
    }
    return range;
  }
}
