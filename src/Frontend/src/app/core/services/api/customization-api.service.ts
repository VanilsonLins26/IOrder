import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { CustomizationGroup, SaveCustomizationGroupRequest } from '../../models';

@Injectable({ providedIn: 'root' })
export class CustomizationApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Customization`;

  getByProduct(productId: string): Observable<CustomizationGroup[]> {
    return this.http.get<CustomizationGroup[]>(`${this.baseUrl}/product/${productId}`);
  }

  save(dto: SaveCustomizationGroupRequest): Observable<CustomizationGroup> {
    return this.http.post<CustomizationGroup>(this.baseUrl, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
