import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type {
  DeliveryAssignmentResponseDto,
  CourierLocationResponseDto,
  AssignCourierRequestDto,
  UpdateCourierLocationRequestDto,
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

  assignCourier(orderId: string, dto: AssignCourierRequestDto): Observable<DeliveryAssignmentResponseDto> {
    return this.http.post<DeliveryAssignmentResponseDto>(`${this.baseUrl}/orders/${orderId}/assign`, dto);
  }

  acceptAssignment(assignmentId: string): Observable<DeliveryAssignmentResponseDto> {
    return this.http.patch<DeliveryAssignmentResponseDto>(`${this.baseUrl}/assignments/${assignmentId}/accept`, {});
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
}
