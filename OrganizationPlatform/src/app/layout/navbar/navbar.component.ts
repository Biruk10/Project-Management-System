import { Component, OnInit, signal, HostListener, ElementRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  notifOpen = signal(false);
  userOpen = signal(false);
  recentNotifications = signal<any[]>([]);

  initials() {
    const u = this.authService.currentUser();
    if (!u) return '?';
    return `${u.firstName[0]}${u.lastName[0]}`.toUpperCase();
  }

  constructor(
    public authService: AuthService,
    public notificationService: NotificationService,
    private elRef: ElementRef
  ) {}

  ngOnInit(): void {
    this.notificationService.getUnreadCount().subscribe();
    this.notificationService.getMyNotifications().subscribe({
      next: (r) => this.recentNotifications.set(r.items.slice(0, 5))
    });
  }

  readNotif(n: any): void {
    if (!n.isRead) {
      this.notificationService.markAsRead(n.id).subscribe();
      n.isRead = true;
    }
  }

  markAll(): void {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.recentNotifications.update(ns => ns.map(n => ({ ...n, isRead: true })));
    });
  }

  logout(): void {
    this.userOpen.set(false);
    this.authService.logout();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (!this.elRef.nativeElement.contains(e.target)) {
      this.notifOpen.set(false);
      this.userOpen.set(false);
    }
  }
}
