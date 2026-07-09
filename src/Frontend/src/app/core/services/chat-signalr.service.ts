import { Injectable, inject, signal } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import type { OrderMessageResponseDto } from '../models';

@Injectable({ providedIn: 'root' })
export class ChatSignalRService {
  private readonly auth = inject(AuthService);

  private hubConnection: HubConnection | null = null;
  readonly connected = signal(false);

  onMessageReceived: ((message: OrderMessageResponseDto) => void) | null = null;
  onConversationUpdated: ((message: OrderMessageResponseDto) => void) | null = null;
  onMessagesRead: ((orderId: string) => void) | null = null;
  onUserTyping: ((orderId: string) => void) | null = null;
  onUserStoppedTyping: ((orderId: string) => void) | null = null;

  async start(): Promise<void> {
    if (this.hubConnection?.state === 'Connected') return;

    const token = await this.auth.getAccessTokenSilently().toPromise();

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.signalrUrl}`, {
        accessTokenFactory: () => token,
      })
      .configureLogging(LogLevel.Warning)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .build();

    this.hubConnection.on('MessageReceived', (message: OrderMessageResponseDto) => {
      this.onMessageReceived?.(message);
    });

    this.hubConnection.on('ConversationUpdated', (message: OrderMessageResponseDto) => {
      this.onConversationUpdated?.(message);
    });

    this.hubConnection.on('MessagesRead', (orderId: string) => {
      this.onMessagesRead?.(orderId);
    });

    this.hubConnection.on('UserTyping', (orderId: string) => {
      this.onUserTyping?.(orderId);
    });

    this.hubConnection.on('UserStoppedTyping', (orderId: string) => {
      this.onUserStoppedTyping?.(orderId);
    });

    this.hubConnection.onreconnecting(() => this.connected.set(false));
    this.hubConnection.onreconnected(() => this.connected.set(true));
    this.hubConnection.onclose(() => this.connected.set(false));

    try {
      await this.hubConnection.start();
      this.connected.set(true);
    } catch (err) {
      console.error('SignalR connection failed:', err);
      this.connected.set(false);
    }
  }

  async stop(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.hubConnection = null;
      this.connected.set(false);
    }
  }

  async joinOrderGroup(orderId: string): Promise<void> {
    try {
      await this.hubConnection?.invoke('JoinOrderGroup', orderId);
    } catch (err) {
      console.error('Failed to join order group:', err);
    }
  }

  async leaveOrderGroup(orderId: string): Promise<void> {
    try {
      await this.hubConnection?.invoke('LeaveOrderGroup', orderId);
    } catch (err) {
      console.error('Failed to leave order group:', err);
    }
  }

  async markOrderRead(orderId: string): Promise<void> {
    try {
      await this.hubConnection?.invoke('MarkOrderRead', orderId);
    } catch (err) {
      console.error('Failed to mark order read:', err);
    }
  }

  async userTyping(orderId: string): Promise<void> {
    try {
      await this.hubConnection?.invoke('UserTyping', orderId);
    } catch {
    }
  }

  async userStoppedTyping(orderId: string): Promise<void> {
    try {
      await this.hubConnection?.invoke('UserStoppedTyping', orderId);
    } catch {
    }
  }
}
