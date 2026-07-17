import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  CreateOrderRequestDto,
  UpdateOrderStatusRequestDto,
  NegotiateOrderRequestDto,
  SendOrderMessageRequestDto,
  OrderResponseDto,
  PagedList,
} from '../../models';

@Injectable({ providedIn: 'root' })
export class OrderApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Order`;

  create(dto: CreateOrderRequestDto): Observable<OrderResponseDto> {
    return this.http.post<OrderResponseDto>(this.baseUrl, dto);
  }

  getById(id: string): Observable<OrderResponseDto> {
    return this.http.get<OrderResponseDto>(`${this.baseUrl}/${id}`);
  }

  getUserOrders(pageNumber = 1, pageSize = 10): Observable<PagedList<OrderResponseDto>> {
    return this.http.get<PagedList<OrderResponseDto>>(`${this.baseUrl}/user`, {
      params: { pageNumber, pageSize },
    });
  }

  getStoreOrders(pageNumber = 1, pageSize = 10): Observable<PagedList<OrderResponseDto>> {
    return this.http.get<PagedList<OrderResponseDto>>(`${this.baseUrl}/store`, {
      params: { pageNumber, pageSize },
    });
  }

  updateStatus(id: string, dto: UpdateOrderStatusRequestDto): Observable<OrderResponseDto> {
    return this.http.patch<OrderResponseDto>(`${this.baseUrl}/${id}/status`, dto);
  }

  negotiate(id: string, dto: NegotiateOrderRequestDto): Observable<OrderResponseDto> {
    return this.http.patch<OrderResponseDto>(`${this.baseUrl}/${id}/negotiate`, dto);
  }

  sendMessage(id: string, dto: SendOrderMessageRequestDto): Observable<OrderResponseDto> {
    return this.http.post<OrderResponseDto>(`${this.baseUrl}/${id}/message`, dto);
  }

  markAsOutForDelivery(id: string): Observable<OrderResponseDto> {
    return this.http.patch<OrderResponseDto>(`${this.baseUrl}/${id}/out-for-delivery`, {});
  }

  requestEarlyDelivery(id: string): Observable<OrderResponseDto> {
    return this.http.patch<OrderResponseDto>(`${this.baseUrl}/${id}/request-early-delivery`, {});
  }
}
