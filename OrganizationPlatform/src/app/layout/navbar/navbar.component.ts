import { Component, OnInit, signal, HostListener, ElementRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  template: `
    <header class="navbar">
      <div class="navbar-left">
        <div class="breadcrumb">
          <span class="org-badge">
            <span class="org-dot"></span>
            {{ authService.currentUser()?.roles?.[0] ?? 'User' }}
          </span>
        </div>
      </div>

      <div class="navbar-right">
        <div class="notif-wrapper" #notifRef>
          <button class="icon-btn" (click)="notifOpen.set(!notifOpen())" aria-label="Notifications">
            <span class="notif-icon">🔔</span>
            @if (notificationService.unreadCount() > 0) {
              <span class="notif-badge">{{ notificationService.unreadCount() > 9 ? '9+' : notificationService.unreadCount() }}</span>
            }
          </button>

          @if (notifOpen()) {
            <div class="dropdown notif-dropdown">
              <div class="dropdown-header">
                <span>Notifications</span>
                @if (notificationService.unreadCount() > 0) {
                  <button class="mark-all-btn" (click)="markAll()">Mark all read</button>
                }
              </div>
              <div class="dropdown-body">
                @if (recentNotifications().length === 0) {
                  <div class="notif-empty">No notifications</div>
                }
                @for (n of recentNotifications(); track n.id) {
                  <div class="notif-item" [class.unread]="!n.isRead" (click)="readNotif(n)">
                    <div class="notif-dot-small" [class.show]="!n.isRead"></div>
                    <div class="notif-content">
                      <div class="notif-title-sm">{{ n.title }}</div>
                      <div class="notif-time">{{ n.createdAt | date:'shortTime' }}</div>
                    </div>
                  </div>
                }
              </div>
              <a routerLink="/notifications" class="dropdown-footer" (click)="notifOpen.set(false)">
                View all notifications →
              </a>
            </div>
          }
        </div>

        <div class="user-menu-wrapper" #userRef>
          <button class="user-trigger" (click)="userOpen.set(!userOpen())">
            <div class="nav-avatar">{{ initials() }}</div>
            <div class="user-meta">
              <span class="nav-name">{{ authService.currentUser()?.firstName }} {{ authService.currentUser()?.lastName }}</span>
            </div>
            <span class="chevron" [class.open]="userOpen()">▾</span>
          </button>

          @if (userOpen()) {
            <div class="dropdown user-dropdown">
              <div class="dropdown-user-header">
                <div class="nav-avatar large">{{ initials() }}</div>
                <div>
                  <div class="duh-name">{{ authService.currentUser()?.firstName }} {{ authService.currentUser()?.lastName }}</div>
                  <div class="duh-email">{{ authService.currentUser()?.email }}</div>
                </div>
              </div>
              <div class="dropdown-body">
                <a class="dropdown-item" routerLink="/notifications" (click)="userOpen.set(false)">
                  <span>🔔</span> Notifications
                </a>
              </div>
              <div class="dropdown-divider"></div>
              <div class="dropdown-body">
                <button class="dropdown-item danger" (click)="logout()">
                  <span>⎋</span> Sign out
                </button>
              </div>
            </div>
          }
        </div>
      </div>
    </header>
  `,
  styles: [`
    .navbar {
      height: 58px;
      background: #fff;
      border-bottom: 1px solid var(--gray-200);
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 24px;
      flex-shrink: 0;
      position: sticky;
      top: 0;
      z-index: 40;
    }

    .navbar-right { display: flex; align-items: center; gap: 8px; }

    .org-badge {
      display: flex; align-items: center; gap: 6px;
      font-size: 12px; font-weight: 600; color: var(--gray-500);
      background: var(--gray-100); padding: 5px 10px; border-radius: 20px;
    }
    .org-dot {
      width: 6px; height: 6px; border-radius: 50%;
      background: var(--success);
    }

    .icon-btn {
      position: relative;
      width: 36px; height: 36px;
      background: var(--gray-100);
      border: none; border-radius: 10px;
      cursor: pointer; display: flex; align-items: center; justify-content: center;
      font-size: 16px;
      transition: var(--transition);
      &:hover { background: var(--gray-200); }
    }

    .notif-badge {
      position: absolute; top: -4px; right: -4px;
      background: var(--danger); color: #fff;
      border-radius: 99px; min-width: 18px; height: 18px;
      font-size: 10px; font-weight: 700;
      display: flex; align-items: center; justify-content: center;
      border: 2px solid #fff;
      padding: 0 3px;
    }

    .notif-wrapper, .user-menu-wrapper { position: relative; }

    .dropdown {
      position: absolute; top: calc(100% + 8px); right: 0;
      background: #fff;
      border: 1px solid var(--gray-200);
      border-radius: var(--border-radius);
      box-shadow: var(--card-shadow-lg);
      z-index: 100;
      animation: fadeIn 0.15s ease;
      min-width: 280px;
    }

    .dropdown-header {
      padding: 14px 16px 10px;
      font-size: 13px; font-weight: 700; color: var(--gray-800);
      border-bottom: 1px solid var(--gray-100);
      display: flex; align-items: center; justify-content: space-between;
    }

    .mark-all-btn {
      background: none; border: none; cursor: pointer;
      font-size: 11px; color: var(--primary); font-weight: 600;
      &:hover { text-decoration: underline; }
    }

    .dropdown-body { padding: 6px; }

    .notif-item {
      display: flex; align-items: center; gap: 10px;
      padding: 10px 10px; border-radius: 8px; cursor: pointer;
      transition: background 0.12s;
      &:hover { background: var(--gray-50); }
      &.unread { background: rgba(99,102,241,0.04); }
    }

    .notif-dot-small {
      width: 6px; height: 6px; border-radius: 50%;
      background: var(--primary); flex-shrink: 0; opacity: 0;
      &.show { opacity: 1; }
    }

    .notif-content { flex: 1; overflow: hidden; }
    .notif-title-sm { font-size: 12px; font-weight: 500; color: var(--gray-800); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .notif-time { font-size: 11px; color: var(--gray-400); margin-top: 1px; }
    .notif-empty { padding: 20px; text-align: center; font-size: 13px; color: var(--gray-400); }

    .dropdown-footer {
      display: block; padding: 10px 16px;
      font-size: 12px; color: var(--primary); font-weight: 600;
      border-top: 1px solid var(--gray-100);
      text-align: center;
      &:hover { background: var(--gray-50); }
    }

    .user-trigger {
      display: flex; align-items: center; gap: 8px;
      background: none; border: none; cursor: pointer;
      padding: 5px 10px 5px 5px;
      border-radius: 10px; transition: background 0.12s;
      &:hover { background: var(--gray-100); }
    }

    .nav-avatar {
      width: 32px; height: 32px; border-radius: 9px;
      background: linear-gradient(135deg, #6366f1, #8b5cf6);
      color: #fff; font-size: 12px; font-weight: 700;
      display: flex; align-items: center; justify-content: center;
      flex-shrink: 0;
      &.large { width: 42px; height: 42px; font-size: 15px; border-radius: 12px; }
    }

    .nav-name { font-size: 13px; font-weight: 600; color: var(--gray-800); }
    .chevron { font-size: 11px; color: var(--gray-400); transition: transform 0.2s; &.open { transform: rotate(180deg); } }

    .user-dropdown { min-width: 240px; }

    .dropdown-user-header {
      display: flex; align-items: center; gap: 12px;
      padding: 14px 14px 12px;
      border-bottom: 1px solid var(--gray-100);
    }
    .duh-name { font-size: 13px; font-weight: 700; color: var(--gray-900); }
    .duh-email { font-size: 11px; color: var(--gray-500); margin-top: 1px; }

    .dropdown-item {
      display: flex; align-items: center; gap: 10px;
      width: 100%; padding: 9px 10px;
      border-radius: 8px; font-size: 13px; font-weight: 500;
      color: var(--gray-700); background: none; border: none; cursor: pointer;
      text-align: left; transition: background 0.12s; text-decoration: none;
      &:hover { background: var(--gray-100); color: var(--gray-900); }
      &.danger { color: var(--danger); &:hover { background: var(--danger-light); } }
    }

    .dropdown-divider { height: 1px; background: var(--gray-100); margin: 2px 0; }
  `]
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
