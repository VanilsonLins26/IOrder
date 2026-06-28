import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  StoreCategoryResponse,
  StoreCategoryRequest,
  UpdateStoreCategoryPositionsRequest,
} from '../../models';

@Injectable({ providedIn: 'root' })
export class StoreCategoryApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/storecategory`;

  getAll(): Observable<StoreCategoryResponse[]> {
    return this.http.get<StoreCategoryResponse[]>(this.baseUrl);
  }

  getById(id: string): Observable<StoreCategoryResponse> {
    return this.http.get<StoreCategoryResponse>(`${this.baseUrl}/${id}`);
  }

  create(dto: StoreCategoryRequest): Observable<StoreCategoryResponse> {
    return this.http.post<StoreCategoryResponse>(this.baseUrl, dto);
  }

  update(id: string, dto: StoreCategoryRequest): Observable<StoreCategoryResponse> {
    return this.http.put<StoreCategoryResponse>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<StoreCategoryResponse> {
    return this.http.delete<StoreCategoryResponse>(`${this.baseUrl}/${id}`);
  }

  updatePositions(dto: UpdateStoreCategoryPositionsRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/positions`, dto);
  }
}
