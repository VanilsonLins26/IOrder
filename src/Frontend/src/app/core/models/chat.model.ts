import type { OrderMessageResponseDto } from './order.model';

export type { OrderMessageResponseDto };

export interface ConversationResponseDto {
  orderId: string;
  storeId: string;
  storeName: string;
  storeImageUrl: string;
  status: string;
  lastMessage: string | null;
  lastMessageAt: string | null;
  lastMessageByRole: string | null;
  unreadCount: number;
  createdAt: string;
}

export interface MarkAsReadResponse {
  unreadCount: number;
}
