import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { CreatePaymentRequestDto, PaymentResponseDto, PublicKeyResponseDto } from '../../models';

@Injectable({ providedIn: 'root' })
export class PaymentApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Payment`;

  create(dto: CreatePaymentRequestDto): Observable<PaymentResponseDto> {
    return this.http.post<PaymentResponseDto>(this.baseUrl, dto);
  }

  getByOrder(orderId: string): Observable<PaymentResponseDto> {
    return this.http.get<PaymentResponseDto>(`${this.baseUrl}/${orderId}`);
  }

  getPublicKey(): Observable<PublicKeyResponseDto> {
    return this.http.get<PublicKeyResponseDto>(`${this.baseUrl}/public-key`);
  }
}
