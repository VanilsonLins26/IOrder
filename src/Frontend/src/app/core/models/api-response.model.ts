// ResponseError — formato de erro retornado pela API
export interface ResponseError {
  errors: string[];
  tokenIsExpired: boolean;
}

// PagedList<T> no backend herda de List<T>, então o JSON é um ARRAY
// com propriedades extras de paginação — NÃO tem { items: [] }
export type PagedList<T> = T[] & {
  currentPage: number;
  totalPages:  number;
  pageSize:    number;
  totalCount:  number;
  hasPrevious: boolean;
  hasNext:     boolean;
};

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
  name?:       string;
  categoryId?: string;
}
