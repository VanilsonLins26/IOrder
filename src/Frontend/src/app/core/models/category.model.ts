import { ProductResponse } from './product.model';

export interface CategoryRequest {
  name: string;
  storeId: string;
  position: number;
}

export interface CategoryResponse {
  id: string;
  name: string;
  storeId: string;
  position: number;
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
