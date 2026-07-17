import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { UserAddressRequest, UserAddressResponse } from '../../models';

@Injectable({ providedIn: 'root' })
export class UserAddressApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/UserAddress`;

  getAll(): Observable<UserAddressResponse[]> {
    return this.http.get<UserAddressResponse[]>(this.baseUrl);
  }

  add(dto: UserAddressRequest): Observable<UserAddressResponse> {
    return this.http.post<UserAddressResponse>(this.baseUrl, dto);
  }

  setDefault(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/default`, {});
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
