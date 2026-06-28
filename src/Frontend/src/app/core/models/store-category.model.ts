export interface StoreCategoryRequest {
  name: string;
  imageUrl: string;
  position: number;
}

export interface StoreCategoryResponse {
  id: string;
  name: string;
  imageUrl: string;
  position: number;
}

export interface UpdateStoreCategoryPositionsRequest {
  positions: StoreCategoryPosition[];
}

export interface StoreCategoryPosition {
  storeCategoryId: string;
  position: number;
}
