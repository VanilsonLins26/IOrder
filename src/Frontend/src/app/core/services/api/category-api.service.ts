import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  CategoryResponse,
  CategoryRequest,
  AddProductsToCategoryRequest,
  UpdateCategoryPositionsRequest,
} from '../../models';

@Injectable({ providedIn: 'root' })
export class CategoryApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/category`;

  create(dto: CategoryRequest): Observable<CategoryResponse> {
    return this.http.post<CategoryResponse>(this.baseUrl, dto);
  }

  getById(id: string): Observable<CategoryResponse> {
    return this.http.get<CategoryResponse>(`${this.baseUrl}/${id}`);
  }

  getByStoreId(storeId: string): Observable<CategoryResponse[]> {
    return this.http.get<CategoryResponse[]>(`${this.baseUrl}/store/${storeId}`);
  }

  update(id: string, dto: CategoryRequest): Observable<CategoryResponse> {
    return this.http.put<CategoryResponse>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<CategoryResponse> {
    return this.http.delete<CategoryResponse>(`${this.baseUrl}/${id}`);
  }

  addProducts(categoryId: string, dto: AddProductsToCategoryRequest): Observable<CategoryResponse> {
    return this.http.post<CategoryResponse>(`${this.baseUrl}/${categoryId}/products`, dto);
  }

  removeProducts(categoryId: string): Observable<CategoryResponse> {
    return this.http.delete<CategoryResponse>(`${this.baseUrl}/${categoryId}/products`);
  }

  updatePositions(dto: UpdateCategoryPositionsRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/positions`, dto);
  }
}
