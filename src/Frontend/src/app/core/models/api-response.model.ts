// ResponseError — formato de erro retornado pela API
export interface ResponseError {
  errors: string[];
  tokenIsExpired: boolean;
}

// O backend retorna um JSON na forma de objeto { items: [...], totalCount: ... }
export interface PagedList<T> {
  currentPage: number;
  totalPages:  number;
  pageSize:    number;
  totalCount:  number;
  hasPrevious: boolean;
  hasNext:     boolean;
  items:       T[];
}

export interface PaginationQuery {
  pageNumber?:   number;
  pageSize?:     number;
  orderBy?:      string;
  isDescending?: boolean;
}

export interface ProductSearchQuery extends PaginationQuery {
  name?:        string;
  price?:       number;
  priceFilter?: number;
  storeId?:     string;
}

export interface StoreSearchQuery extends PaginationQuery {
  name?:          string;
  categoryId?:    string;
  userLatitude?:  number;
  userLongitude?: number;
}
