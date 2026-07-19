export interface CreateReviewRequestDto {
  storeRating: number;
  courierRating?: number;
  comment?: string;
}

export interface ReviewResponseDto {
  id: string;
  orderId: string;
  storeId: string;
  userId: string;
  courierUserId?: string;
  storeRating: number;
  courierRating?: number;
  comment?: string;
  createdAt: string;
}
