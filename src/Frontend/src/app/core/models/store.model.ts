export interface StoreRequest {
  name: string;
  about: string;
  imageUrl: string;
  address: AddressRequest;
  openingHours: OpeningHourRequest[];
  categoryId: string;
}

export interface StoreResponse {
  id: string;
  name: string;
  about: string;
  imageUrl: string;
  address: AddressResponse;
  openingHours: OpeningHourResponse[];
  userId: string;
  isOpen: boolean;
  categoryId: string;
  categoryName?: string;
  distanceKm?: number;
  deliveryFee?: number;
  baseDeliveryFee: number;
  feePerKm: number;
  maxDeliveryDistanceKm: number;
  latitude?: number | null;
  longitude?: number | null;
}

export interface UpdateStoreRequest {
  name: string;
  about: string;
  imageUrl: string;
  baseDeliveryFee?: number;
  feePerKm?: number;
  maxDeliveryDistanceKm?: number;
}

export interface AddressRequest {
  zipCode: string;
  street: string;
  number: string;
  complement: string;
  neighborhood: string;
  city: string;
  state: string;
}

export interface AddressResponse {
  zipCode: string;
  street: string;
  number: string;
  complement: string;
  neighborhood: string;
  city: string;
  state: string;
}

export interface OpeningHourRequest {
  dayOfWeek: number;
  openHour: string;
  closeHour: string;
}

export interface OpeningHourResponse {
  dayOfWeek: number;
  openHour: string;
  closeHour: string;
}

export interface UpdateOpeningHourRequest {
  openingHours: OpeningHourRequest[];
}
