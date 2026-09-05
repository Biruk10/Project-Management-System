import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { Notification } from '../../core/models/notification.models';
import { PagedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Notifications</h1>
        <button class="btn-secondary" (click)="markAllRead()">Mark all as read</button>
      </div>

      @if (result()) {
        <div class="notif-list">
          @for (n of result()!.items; track n.id) {
            <div class="notif-item" [class.unread]="!n.isRead" (click)="read(n)">
              <div class="notif-dot" [class.visible]="!n.isRead"></div>
              <div class="notif-body">
                <div class="notif-title">{{ n.title }}</div>
                <div class="notif-message">{{ n.message }}</div>
                <div class="notif-time">{{ n.createdAt | date:'medium' }}</div>
              </div>
            </div>
          }
          @if (result()!.items.length === 0) {
            <div class="empty-state">No notifications.</div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .page { max-width: 800px; margin: 0 auto; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .page-title { font-size: 24px; font-weight: 700; color: #111827; margin: 0; }
    .btn-secondary { background: #fff; border: 1px solid #d1d5db; color: #374151; padding: 8px 16px; border-radius: 6px; cursor: pointer; font-size: 13px; }
    .btn-secondary:hover { background: #f3f4f6; }
    .notif-list { background: #fff; border-radius: 10px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); overflow: hidden; }
    .notif-item { display: flex; align-items: flex-start; gap: 12px; padding: 16px 20px; border-bottom: 1px solid #f3f4f6; cursor: pointer; transition: background 0.15s; }
    .notif-item:hover { background: #f9fafb; }
    .notif-item.unread { background: #eff6ff; }
    .notif-item.unread:hover { background: #dbeafe; }
    .notif-dot { width: 8px; height: 8px; border-radius: 50%; background: #3b82f6; margin-top: 6px; flex-shrink: 0; opacity: 0; }
    .notif-dot.visible { opacity: 1; }
    .notif-title { font-size: 14px; font-weight: 600; color: #111827; margin-bottom: 2px; }
    .notif-message { font-size: 13px; color: #6b7280; }
    .notif-time { font-size: 11px; color: #9ca3af; margin-top: 4px; }
    .empty-state { text-align: center; padding: 40px; color: #9ca3af; }
  `]
})
export class NotificationsComponent implements OnInit {
  result = signal<PagedResult<Notification> | null>(null);

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.notificationService.getMyNotifications().subscribe({
      next: (data) => this.result.set(data)
    });
  }

  read(n: Notification): void {
    if (!n.isRead) {
      this.notificationService.markAsRead(n.id).subscribe(() => {
        n.isRead = true;
      });
    }
  }

  markAllRead(): void {
    this.notificationService.markAllAsRead().subscribe(() => this.load());
  }
}
