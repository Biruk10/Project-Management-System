import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { Notification } from '../../core/models/notification.models';
import { PagedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss'
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
