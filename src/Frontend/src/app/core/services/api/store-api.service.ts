import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  StoreResponse,
  StoreRequest,
  UpdateStoreRequest,
  UpdateOpeningHourRequest,
  AddressRequest,
  StoreSearchQuery,
  PagedList,
} from '../../models';

@Injectable({ providedIn: 'root' })
export class StoreApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/store`;

  create(dto: StoreRequest): Observable<StoreResponse> {
    return this.http.post<StoreResponse>(this.baseUrl, dto);
  }

  getById(id: string): Observable<StoreResponse> {
    return this.http.get<StoreResponse>(`${this.baseUrl}/${id}`);
  }

  getPaged(query: StoreSearchQuery): Observable<PagedList<StoreResponse>> {
    let params = new HttpParams();
    if (query.pageNumber)              params = params.set('PageNumber',   query.pageNumber);
    if (query.pageSize)                params = params.set('PageSize',     query.pageSize);
    if (query.name)                    params = params.set('Name',         query.name);
    if (query.orderBy)                 params = params.set('OrderBy',      query.orderBy);
    if (query.isDescending !== undefined) params = params.set('IsDescending', query.isDescending);
    if (query.categoryId)              params = params.set('CategoryId',   query.categoryId);
    return this.http.get<PagedList<StoreResponse>>(`${this.baseUrl}/paged`, { params });
  }

  getMyStore(): Observable<StoreResponse> {
    return this.http.get<StoreResponse>(`${this.baseUrl}/mystore`);
  }

  update(id: string, dto: UpdateStoreRequest): Observable<StoreResponse> {
    return this.http.put<StoreResponse>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<StoreResponse> {
    return this.http.delete<StoreResponse>(`${this.baseUrl}/${id}`);
  }

  updateOpeningHours(storeId: string, dto: UpdateOpeningHourRequest): Observable<StoreResponse> {
    return this.http.put<StoreResponse>(`${this.baseUrl}/openinghour/${storeId}`, dto);
  }

  updateAddress(storeId: string, dto: AddressRequest): Observable<StoreResponse> {
    return this.http.put<StoreResponse>(`${this.baseUrl}/address/${storeId}`, dto);
  }

  updateImage(file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.put<{ imageUrl: string }>(`${this.baseUrl}/image`, formData);
  }
}
