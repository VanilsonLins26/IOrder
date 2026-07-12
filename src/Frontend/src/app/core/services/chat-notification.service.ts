import { Injectable, inject, signal } from '@angular/core';
import { ChatApiService } from './api/chat-api.service';
import { ChatSignalRService } from './chat-signalr.service';
import { AuthService } from '@auth0/auth0-angular';
import { take } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ChatNotificationService {
  private readonly chatApi = inject(ChatApiService);
  private readonly chatSignalR = inject(ChatSignalRService);
  private readonly auth = inject(AuthService);

  readonly totalUnread = signal(0);
  readonly unreadConversations = signal<import('../models/chat.model').ConversationResponseDto[]>([]);

  constructor() {
    this.auth.isAuthenticated$.subscribe(isAuth => {
      if (isAuth) {
        this.fetchUnreadCount();
        this.listenToSignalR();
      }
    });
  }

  private fetchUnreadCount() {
    this.chatApi.getConversations(1, 50).pipe(take(1)).subscribe({
      next: (res) => {
        const unread = res.items.filter(c => c.unreadCount > 0);
        const total = unread.reduce((sum, conv) => sum + conv.unreadCount, 0);
        this.unreadConversations.set(unread);
        this.totalUnread.set(total);
      },
      error: (err) => console.error('Failed to fetch unread count', err)
    });
  }

  private listenToSignalR() {
    // If a new message is received, refetch or just add 1
    // We will refetch to be safe and accurate
    const originalOnMessageReceived = this.chatSignalR.onMessageReceived;
    this.chatSignalR.onMessageReceived = (msg) => {
      if (originalOnMessageReceived) originalOnMessageReceived(msg);
      this.fetchUnreadCount();
    };

    const originalOnMessagesRead = this.chatSignalR.onMessagesRead;
    this.chatSignalR.onMessagesRead = (orderId) => {
      if (originalOnMessagesRead) originalOnMessagesRead(orderId);
      this.fetchUnreadCount();
    };
  }
}
