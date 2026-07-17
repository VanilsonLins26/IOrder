import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { UserProfileResponseDto, UserProfileRequestDto } from '../../models';

@Injectable({ providedIn: 'root' })
export class ProfileApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Profile`;

  get(): Observable<UserProfileResponseDto> {
    return this.http.get<UserProfileResponseDto>(this.baseUrl);
  }

  update(dto: UserProfileRequestDto): Observable<UserProfileResponseDto> {
    return this.http.put<UserProfileResponseDto>(this.baseUrl, dto);
  }
}
