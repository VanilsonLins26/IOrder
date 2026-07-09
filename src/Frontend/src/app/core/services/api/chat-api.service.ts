import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { ConversationResponseDto, OrderMessageResponseDto, MarkAsReadResponse, PagedList } from '../../models';

@Injectable({ providedIn: 'root' })
export class ChatApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/chat`;

  getConversations(pageNumber = 1, pageSize = 20): Observable<PagedList<ConversationResponseDto>> {
    return this.http.get<PagedList<ConversationResponseDto>>(`${this.baseUrl}/conversations`, {
      params: { pageNumber, pageSize },
    });
  }

  getMessages(orderId: string, pageNumber = 1, pageSize = 50): Observable<PagedList<OrderMessageResponseDto>> {
    return this.http.get<PagedList<OrderMessageResponseDto>>(`${this.baseUrl}/${orderId}/messages`, {
      params: { pageNumber, pageSize },
    });
  }

  markAsRead(orderId: string): Observable<MarkAsReadResponse> {
    return this.http.post<MarkAsReadResponse>(`${this.baseUrl}/${orderId}/read`, {});
  }
}
