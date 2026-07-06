export interface AddItemToCartRequestDto {
  quantity: number;
  productId: string;
  imageUrls: string[];
  customize: string;
}

export interface ChangeCartItemQuantityRequestDto {
  cartItemId: string;
  newQuantity: number;
}

export interface ApplyCouponRequestDto {
  couponCode: string;
}

export interface CartItemResponseDto {
  id: string;
  quantity: number;
  unitPrice: number;
  productId: string;
  totalPrice: number;
  imageUrls: string[];
  customize: string;
  productName: string;
  productImageUrl: string;
  storeId: string;
}

export interface CartResponseDto {
  userId: string;
  cartTotal: number;
  couponCode: string | null;
  items: CartItemResponseDto[];
}
