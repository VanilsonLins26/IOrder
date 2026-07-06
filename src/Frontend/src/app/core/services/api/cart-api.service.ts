import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  AddItemToCartRequestDto,
  ChangeCartItemQuantityRequestDto,
  ApplyCouponRequestDto,
  CartResponseDto,
} from '../../models';

@Injectable({ providedIn: 'root' })
export class CartApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Cart`;

  get(): Observable<CartResponseDto> {
    return this.http.get<CartResponseDto>(this.baseUrl);
  }

  addItem(dto: AddItemToCartRequestDto): Observable<CartResponseDto> {
    return this.http.patch<CartResponseDto>(`${this.baseUrl}/AddItem`, dto);
  }

  changeQuantity(dto: ChangeCartItemQuantityRequestDto): Observable<CartResponseDto> {
    return this.http.patch<CartResponseDto>(`${this.baseUrl}/ChangeQuantity`, dto);
  }

  applyCoupon(dto: ApplyCouponRequestDto): Observable<CartResponseDto> {
    return this.http.patch<CartResponseDto>(`${this.baseUrl}/Coupon`, dto);
  }

  removeItem(cartItemId: string): Observable<CartResponseDto> {
    return this.http.delete<CartResponseDto>(`${this.baseUrl}/${cartItemId}`);
  }

  clear(): Observable<void> {
    return this.http.delete<void>(this.baseUrl);
  }
}
