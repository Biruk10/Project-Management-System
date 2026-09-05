import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.models';
import { Notification } from '../models/notification.models';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  unreadCount = signal<number>(0);

  constructor(private api: ApiService) {}

  getMyNotifications(isRead?: boolean): Observable<PagedResult<Notification>> {
    const params = isRead !== undefined ? { isRead } : undefined;
    return this.api.get<PagedResult<Notification>>('/notifications', params as Record<string, string | number | boolean | undefined>);
  }

  getUnreadCount(): Observable<{ unreadCount: number }> {
    return this.api.get<{ unreadCount: number }>('/notifications/unread-count').pipe(
      tap(result => this.unreadCount.set(result.unreadCount))
    );
  }

  markAsRead(id: number): Observable<void> {
    return this.api.post<void>(`/notifications/${id}/read`, {}).pipe(
      tap(() => this.unreadCount.update(c => Math.max(0, c - 1)))
    );
  }

  markAllAsRead(): Observable<void> {
    return this.api.post<void>('/notifications/read-all', {}).pipe(
      tap(() => this.unreadCount.set(0))
    );
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`/notifications/${id}`);
  }
}
