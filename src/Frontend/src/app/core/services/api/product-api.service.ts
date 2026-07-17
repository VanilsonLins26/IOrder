import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  ProductResponse,
  ProductRequest,
  UpdateProductRequest,
  PromotionPriceRequest,
  PromotionPriceResponse,
  ProductSearchQuery,
  PagedList,
} from '../../models';

@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/product`;

  create(dto: ProductRequest): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(this.baseUrl, dto);
  }

  getById(id: string): Observable<ProductResponse> {
    return this.http.get<ProductResponse>(`${this.baseUrl}/${id}`);
  }

  getPaged(query: ProductSearchQuery): Observable<PagedList<ProductResponse>> {
    let params = new HttpParams();
    if (query.pageNumber)              params = params.set('PageNumber',   query.pageNumber);
    if (query.pageSize)                params = params.set('PageSize',     query.pageSize);
    if (query.name)                    params = params.set('Name',         query.name);
    if (query.orderBy)                 params = params.set('OrderBy',      query.orderBy);
    if (query.isDescending !== undefined) params = params.set('IsDescending', query.isDescending);
    if (query.price !== undefined)     params = params.set('Price',        query.price);
    if (query.priceFilter !== undefined) params = params.set('PriceFilter', query.priceFilter);
    if (query.storeId)                 params = params.set('StoreId',      query.storeId);
    return this.http.get<PagedList<ProductResponse>>(`${this.baseUrl}/paged`, { params });
  }

  update(id: string, dto: UpdateProductRequest): Observable<ProductResponse> {
    return this.http.put<ProductResponse>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<ProductResponse> {
    return this.http.delete<ProductResponse>(`${this.baseUrl}/${id}`);
  }

  createPromotion(dto: PromotionPriceRequest): Observable<PromotionPriceResponse> {
    return this.http.post<PromotionPriceResponse>(`${this.baseUrl}/promotion`, dto);
  }

  updateImage(id: string, file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.put<{ imageUrl: string }>(`${this.baseUrl}/image/${id}`, formData);
  }
}
