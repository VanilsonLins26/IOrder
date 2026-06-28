export interface ResponseError {
  errors: string[];
  tokenIsExpired: boolean;
}

export interface PagedList<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PaginationQuery {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  isDescending?: boolean;
}

export interface ProductSearchQuery extends PaginationQuery {
  name?: string;
  price?: number;
  priceFilter?: number;
}

export interface StoreSearchQuery extends PaginationQuery {
  name?: string;
  categoryId?: string;
}
