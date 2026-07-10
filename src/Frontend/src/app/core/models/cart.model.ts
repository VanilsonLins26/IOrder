export interface AddItemToCartRequestDto {
  quantity: number;
  productId: string;
  imageUrls: string[];
  customize: string;
  selectedOptionIds: string[];
}

export interface ChangeCartItemQuantityRequestDto {
  cartItemId: string;
  newQuantity: number;
}

export interface ApplyCouponRequestDto {
  couponCode: string;
}

import type { SelectedOption } from './customization.model';

export interface CartItemResponseDto {
  id: string;
  quantity: number;
  unitPrice: number;
  productId: string;
  totalPrice: number;
  imageUrls: string[];
  selectedOptions: SelectedOption[];
  customize: string;
  productName: string;
  productImageUrl: string;
  storeId: string;
}

export interface CartResponseDto {
  userId: string;
  cartTotal: number;
  couponCode: string | null;
  discountValue: number | null;
  discountedTotal: number | null;
  items: CartItemResponseDto[];
}
