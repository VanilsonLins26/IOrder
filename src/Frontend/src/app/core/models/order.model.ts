import type { SelectedOption } from './customization.model';
import type { DeliveryAssignmentResponseDto } from './delivery.model';

export enum OrderStatusDto {
  Pending = 0,
  Negotiating = 1,
  AwaitingPayment = 2,
  Paid = 3,
  Preparing = 4,
  Ready = 5,
  Delivered = 6,
  Cancelled = 7,
  Declined = 8,
}

export enum DeliveryTypeDto {
  Delivery = 0,
  Pickup = 1,
}

export enum MessageTypeDto {
  Text = 0,
  Proposal = 1,
}

export interface CreateOrderRequestDto {
  customerNotes?: string | null;
  deliveryDate?: string | null;
  customerPhone?: string | null;
  deliveryType: DeliveryTypeDto;
  deliveryFee: number;
}

export interface UpdateOrderStatusRequestDto {
  status: OrderStatusDto;
}

export interface NegotiateOrderRequestDto {
  proposedTotalAmount?: number | null;
  proposedDeliveryDate?: string | null;
  shopkeeperNotes?: string | null;
}

export interface SendOrderMessageRequestDto {
  message: string;
  type: MessageTypeDto;
  proposedTotalAmount?: number | null;
  proposedDeliveryDate?: string | null;
}

export interface OrderMessageResponseDto {
  id: string;
  userId: string;
  userRole: string;
  message: string;
  sentAt: string;
  type: MessageTypeDto;
  proposedTotalAmount: number | null;
  proposedDeliveryDate: string | null;
  readAt: string | null;
  readByUserId: string | null;
}

export interface OrderItemResponseDto {
  id: string;
  productId: string;
  productName: string;
  productImageUrl: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  selectedOptions: SelectedOption[];
  customize: string;
  imageUrls: string[];
}

export interface OrderResponseDto {
  id: string;
  userId: string;
  storeId: string;
  status: OrderStatusDto;
  totalAmount: number;
  originalAmount: number;
  couponCode?: string | null;
  discountValue?: number | null;
  discountedTotal?: number | null;
  deliveryType: DeliveryTypeDto;
  deliveryFee: number;
  deliveryDate?: string | null;
  customerNotes?: string | null;
  shopkeeperNotes?: string | null;
  createdAt: string;
  updatedAt: string;
  items: OrderItemResponseDto[];
  messages: OrderMessageResponseDto[];
  activeAssignment: DeliveryAssignmentResponseDto | null;
}

export interface OrderStatusChangedEvent {
  orderId: string;
  status: OrderStatusDto;
}
