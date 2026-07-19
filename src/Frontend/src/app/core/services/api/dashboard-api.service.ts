import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { DashboardMetricsResponseDto } from '../../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Dashboard`;

  getMetrics(): Observable<DashboardMetricsResponseDto> {
    return this.http.get<DashboardMetricsResponseDto>(this.baseUrl);
  }
}
