import { ProductResponse } from './product.model';

export interface CategoryRequest {
  name: string;
  position: number;
  imageUrl?: string;
}

export interface CategoryResponse {
  id: string;
  name: string;
  storeId: string;
  position: number;
  imageUrl?: string;
  products: ProductResponse[];
}

export interface AddProductsToCategoryRequest {
  productIds: string[];
}

export interface UpdateCategoryPositionsRequest {
  positions: CategoryPosition[];
}

export interface CategoryPosition {
  categoryId: string;
  position: number;
}
