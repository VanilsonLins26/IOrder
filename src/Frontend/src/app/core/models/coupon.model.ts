export interface CouponRequestDto {
  code: string;
  discountType: string;
  discountValue: number;
  maxDiscountAmount: number | null;
  minPurchaseAmount: number | null;
  expiresAt: string | null;
  maxUsageCount: number;
}

export interface CouponResponseDto {
  id: string;
  code: string;
  discountType: string;
  discountValue: number;
  maxDiscountAmount: number | null;
  minPurchaseAmount: number | null;
  expiresAt: string | null;
  maxUsageCount: number;
  currentUsageCount: number;
  active: boolean;
}
