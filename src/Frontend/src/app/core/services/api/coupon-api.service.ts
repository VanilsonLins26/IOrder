import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { CouponRequestDto, CouponResponseDto } from '../../models/coupon.model';

@Injectable({ providedIn: 'root' })
export class CouponApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Coupon`;

  create(dto: CouponRequestDto): Observable<CouponResponseDto> {
    return this.http.post<CouponResponseDto>(this.baseUrl, dto);
  }

  getActive(): Observable<CouponResponseDto[]> {
    return this.http.get<CouponResponseDto[]>(`${this.baseUrl}/active`);
  }

  getAllAdmin(): Observable<CouponResponseDto[]> {
    return this.http.get<CouponResponseDto[]>(`${this.baseUrl}/admin`);
  }
}
