import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { CreateReviewRequestDto, ReviewResponseDto } from '../../models';

@Injectable({ providedIn: 'root' })
export class ReviewApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Reviews`;

  create(orderId: string, dto: CreateReviewRequestDto): Observable<ReviewResponseDto> {
    return this.http.post<ReviewResponseDto>(`${this.baseUrl}/${orderId}`, dto);
  }

  getByOrder(orderId: string): Observable<ReviewResponseDto | null> {
    return this.http.get<ReviewResponseDto | null>(`${this.baseUrl}/order/${orderId}`);
  }

  getStoreReviews(storeId: string, page = 1, pageSize = 20): Observable<ReviewResponseDto[]> {
    return this.http.get<ReviewResponseDto[]>(`${this.baseUrl}/store/${storeId}`, {
      params: { page, pageSize }
    });
  }
}
