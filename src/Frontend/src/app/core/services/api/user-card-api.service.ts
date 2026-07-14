import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { SaveCardRequestDto, UserCardResponseDto } from '../../models';

@Injectable({ providedIn: 'root' })
export class UserCardApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/UserCard`;

  save(dto: SaveCardRequestDto): Observable<UserCardResponseDto> {
    return this.http.post<UserCardResponseDto>(this.baseUrl, dto);
  }

  getAll(): Observable<UserCardResponseDto[]> {
    return this.http.get<UserCardResponseDto[]>(this.baseUrl);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
