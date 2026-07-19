import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CurrencyPipe, DatePipe, NgClass } from '@angular/common';
import { CouponApiService } from '../../../../core/services/api/coupon-api.service';
import { AdminStore } from '../../store/admin.store';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import type { CouponResponseDto, CouponRequestDto } from '../../../../core/models';

@Component({
  selector: 'app-coupon-management',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe, NgClass, ModalComponent, LoadingSkeletonComponent],
  templateUrl: './coupon-management.component.html',
  styleUrl: './coupon-management.component.scss',
})
export class CouponManagementComponent implements OnInit {
  readonly adminStore = inject(AdminStore);
  private readonly couponApi = inject(CouponApiService);
  private readonly fb = inject(FormBuilder);

  readonly coupons = signal<CouponResponseDto[]>([]);
  readonly loadingCoupons = signal(true);
  
  // Modal states
  readonly isModalOpen = signal(false);
  readonly isSaving = signal(false);
  readonly apiError = signal<string | null>(null);

  readonly couponForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    discountType: ['Percentage', [Validators.required]],
    discountValue: [0, [Validators.required, Validators.min(0.01)]],
    maxDiscountAmount: [null as number | null, [Validators.min(0)]],
    minPurchaseAmount: [null as number | null, [Validators.min(0)]],
    expiresAt: [null as string | null],
    maxUsageCount: [0, [Validators.min(0)]]
  });

  ngOnInit(): void {
    this.loadCoupons();
  }

  loadCoupons(): void {
    this.loadingCoupons.set(true);
    this.couponApi.getAllAdmin().subscribe({
      next: (data) => {
        this.coupons.set(data);
        this.loadingCoupons.set(false);
      },
      error: () => {
        this.loadingCoupons.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.couponForm.reset({
      code: '',
      discountType: 'Percentage',
      discountValue: 0,
      maxDiscountAmount: null,
      minPurchaseAmount: null,
      expiresAt: null,
      maxUsageCount: 0
    });
    this.apiError.set(null);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
  }

  saveCoupon(): void {
    if (this.couponForm.invalid) {
      this.couponForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.apiError.set(null);

    const formValue = this.couponForm.getRawValue();
    
    // Ensure code is uppercase
    const dto: CouponRequestDto = {
      ...formValue,
      code: formValue.code.toUpperCase().trim(),
      maxUsageCount: formValue.maxUsageCount || 0
    };

    this.couponApi.create(dto).subscribe({
      next: (newCoupon) => {
        this.coupons.update(list => [newCoupon, ...list]);
        this.isSaving.set(false);
        this.closeModal();
      },
      error: (err) => {
        this.isSaving.set(false);
        this.apiError.set(err.error?.message || 'Erro ao criar o cupom.');
      }
    });
  }
}
