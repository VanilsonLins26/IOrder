import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  DeliveryAssignmentResponseDto,
  CourierLocationResponseDto,
  AssignCourierRequestDto,
  UpdateCourierLocationRequestDto,
  AvailableCourierResponseDto,
  PagedList,
} from '../../models';

@Injectable({ providedIn: 'root' })
export class DeliveryApiService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Delivery`;

  getMyDeliveries(pageNumber = 1, pageSize = 10): Observable<PagedList<DeliveryAssignmentResponseDto>> {
    return this.http.get<PagedList<DeliveryAssignmentResponseDto>>(`${this.baseUrl}/my-deliveries`, {
      params: { pageNumber, pageSize },
    });
  }

  broadcastDeliveryOffer(orderId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/orders/${orderId}/broadcast`, {});
  }

  getAvailableDeliveries(): Observable<PagedList<any>> {
    // Reusing PagedList or just an array. Our backend returns ResponseListDto which matches PagedList basically but without paging info, or we can use any
    return this.http.get<PagedList<any>>(`${this.baseUrl}/available`);
  }

  acceptAssignment(orderId: string): Observable<DeliveryAssignmentResponseDto> {
    return this.http.post<DeliveryAssignmentResponseDto>(`${this.baseUrl}/orders/${orderId}/accept`, {});
  }

  rejectAssignment(assignmentId: string): Observable<DeliveryAssignmentResponseDto> {
    return this.http.patch<DeliveryAssignmentResponseDto>(`${this.baseUrl}/assignments/${assignmentId}/reject`, {});
  }

  pickupOrder(assignmentId: string): Observable<DeliveryAssignmentResponseDto> {
    return this.http.patch<DeliveryAssignmentResponseDto>(`${this.baseUrl}/assignments/${assignmentId}/pickup`, {});
  }

  deliverOrder(assignmentId: string): Observable<DeliveryAssignmentResponseDto> {
    return this.http.patch<DeliveryAssignmentResponseDto>(`${this.baseUrl}/assignments/${assignmentId}/deliver`, {});
  }

  updateLocation(dto: UpdateCourierLocationRequestDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/location`, dto);
  }

  getCourierLocation(courierUserId: string): Observable<CourierLocationResponseDto> {
    return this.http.get<CourierLocationResponseDto>(`${this.baseUrl}/courier/${courierUserId}/location`);
  }

  startTransit(assignmentId: string): Observable<DeliveryAssignmentResponseDto> {
    return this.http.patch<DeliveryAssignmentResponseDto>(`${this.baseUrl}/assignments/${assignmentId}/start-transit`, {});
  }
}
