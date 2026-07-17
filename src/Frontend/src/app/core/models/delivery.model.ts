export enum AssignmentStatusDto {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
  PickedUp = 3,
  InTransit = 4,
  Delivered = 5,
  Failed = 6,
}

export interface DeliveryAssignmentResponseDto {
  id: string;
  orderId: string;
  courierUserId: string;
  status: AssignmentStatusDto;
  assignedAt: string;
  acceptedAt: string | null;
  pickedUpAt: string | null;
  inTransitAt: string | null;
  deliveredAt: string | null;
  courierNotes: string | null;
}

export interface CourierLocationResponseDto {
  courierUserId: string;
  latitude: number;
  longitude: number;
  updatedAt: string;
}

export interface AssignCourierRequestDto {
  courierUserId: string;
}

export interface UpdateCourierLocationRequestDto {
  latitude: number;
  longitude: number;
}

export interface AvailableCourierResponseDto {
  courierUserId: string;
  courierName?: string;
  distanceKm?: number;
  lastLocationAt: string;
}
