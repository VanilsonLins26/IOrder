export interface ProductRequest {
  name: string;
  price: number;
  storeId: string;
  unitOfMeasure: UnitOfMeasure;
  imageUrl: string;
  description: string;
}

export interface ProductResponse {
  id: string;
  name: string;
  price: number;
  storeId: string;
  unitOfMeasure: UnitOfMeasure;
  imageUrl: string;
  description: string;
  active: boolean;
  currentPromotionalPrice: number | null;
  promotions: PromotionPriceResponse[];
}

export interface UpdateProductRequest {
  name: string;
  price: number;
  unitOfMeasure: UnitOfMeasure;
  imageUrl: string;
  description: string;
  active: boolean;
}

export interface PromotionPriceRequest {
  productId: string;
  price: number;
  initialTime: string;
  finalTime: string;
}

export interface PromotionPriceResponse {
  id: string;
  productId: string;
  price: number;
  initialTime: string;
  finalTime: string;
}

export enum UnitOfMeasure {
  Unit = 0,
  Kg = 1,
  Gram = 2,
  Liter = 3,
  Ml = 4,
  Meter = 5,
}

export enum PriceFilterType {
  Higher = 0,
  Lower = 1,
}
