import { Component, signal, computed } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: string[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  template: `
    <aside class="sidebar" [class.collapsed]="collapsed()">
      <div class="sidebar-header">
        <div class="brand">
          <div class="brand-logo">
            <span class="logo-icon">⬡</span>
          </div>
          @if (!collapsed()) {
            <div class="brand-text">
              <span class="brand-name">OrgPlatform</span>
              <span class="brand-tagline">Operations Suite</span>
            </div>
          }
        </div>
        <button class="collapse-btn" (click)="collapsed.set(!collapsed())" [attr.aria-label]="collapsed() ? 'Expand' : 'Collapse'">
          {{ collapsed() ? '›' : '‹' }}
        </button>
      </div>

      @if (!collapsed() && authService.currentUser()) {
        <div class="user-pill">
          <div class="avatar-sm">{{ initials() }}</div>
          <div class="user-info">
            <span class="user-name">{{ authService.currentUser()!.firstName }} {{ authService.currentUser()!.lastName }}</span>
            <span class="user-role">{{ primaryRole() }}</span>
          </div>
        </div>
      }

      <nav class="nav-section">
        @if (!collapsed()) {
          <span class="nav-section-label">Main</span>
        }
        @for (item of visibleNavItems(); track item.route) {
          <a
            [routerLink]="item.route"
            routerLinkActive="active"
            class="nav-item"
            [class.icon-only]="collapsed()"
            [title]="collapsed() ? item.label : ''"
          >
            <span class="nav-icon">{{ item.icon }}</span>
            @if (!collapsed()) {
              <span class="nav-label">{{ item.label }}</span>
            }
          </a>
        }
      </nav>

      <div class="sidebar-footer">
        <button class="nav-item logout-btn" [class.icon-only]="collapsed()" (click)="authService.logout()" title="Logout">
          <span class="nav-icon">⎋</span>
          @if (!collapsed()) { <span class="nav-label">Logout</span> }
        </button>
      </div>
    </aside>
  `,
  styles: [`
    .sidebar {
      width: 240px;
      min-height: 100vh;
      background: var(--sidebar-bg);
      display: flex;
      flex-direction: column;
      flex-shrink: 0;
      transition: width 0.25s cubic-bezier(0.4,0,0.2,1);
      border-right: 1px solid var(--sidebar-border);
    }
    .sidebar.collapsed { width: 64px; }

    .sidebar-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 18px 14px 14px;
      border-bottom: 1px solid var(--sidebar-border);
      min-height: 64px;
    }

    .brand { display: flex; align-items: center; gap: 10px; overflow: hidden; }

    .brand-logo {
      width: 36px; height: 36px;
      background: linear-gradient(135deg, #6366f1, #8b5cf6);
      border-radius: 10px;
      display: flex; align-items: center; justify-content: center;
      flex-shrink: 0;
    }
    .logo-icon { color: #fff; font-size: 18px; }

    .brand-text { display: flex; flex-direction: column; line-height: 1.2; }
    .brand-name { font-size: 14px; font-weight: 700; color: #f1f5f9; }
    .brand-tagline { font-size: 10px; color: #64748b; font-weight: 500; }

    .collapse-btn {
      background: none; border: none; cursor: pointer;
      color: #64748b; font-size: 18px; padding: 2px 4px;
      border-radius: 4px; line-height: 1;
      transition: color 0.15s;
      &:hover { color: #f1f5f9; }
    }

    .user-pill {
      margin: 12px 12px 8px;
      padding: 10px 12px;
      background: rgba(255,255,255,0.04);
      border-radius: 10px;
      display: flex; align-items: center; gap: 10px;
      border: 1px solid var(--sidebar-border);
    }

    .avatar-sm {
      width: 30px; height: 30px;
      border-radius: 8px;
      background: linear-gradient(135deg, #6366f1, #8b5cf6);
      color: #fff; font-size: 12px; font-weight: 700;
      display: flex; align-items: center; justify-content: center;
      flex-shrink: 0;
    }

    .user-info { display: flex; flex-direction: column; overflow: hidden; }
    .user-name { font-size: 12px; font-weight: 600; color: #e2e8f0; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .user-role { font-size: 10px; color: #64748b; }

    .nav-section {
      flex: 1;
      padding: 8px 10px;
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .nav-section-label {
      font-size: 9px;
      font-weight: 700;
      color: #475569;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      padding: 8px 8px 4px;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 9px 10px;
      border-radius: 8px;
      color: var(--sidebar-text);
      font-size: 13px;
      font-weight: 500;
      transition: all 0.15s;
      cursor: pointer;
      border: none;
      background: none;
      width: 100%;
      text-align: left;
      text-decoration: none;

      &:hover {
        background: var(--sidebar-hover);
        color: var(--sidebar-text-active);
      }

      &.active {
        background: var(--sidebar-active);
        color: #a5b4fc;
        font-weight: 600;
      }

      &.icon-only {
        justify-content: center;
        padding: 10px;
      }
    }

    .nav-icon { font-size: 16px; flex-shrink: 0; }
    .nav-label { flex: 1; white-space: nowrap; }

    .sidebar-footer {
      padding: 10px;
      border-top: 1px solid var(--sidebar-border);
    }

    .logout-btn {
      color: #64748b;
      &:hover { color: #f87171; background: rgba(239,68,68,0.1); }
    }
  `]
})
export class SidebarComponent {
  collapsed = signal(false);

  constructor(public authService: AuthService) {}

  private readonly allNavItems: NavItem[] = [
    { label: 'Admin Dashboard', icon: '🛡️', route: '/admin',       roles: ['SystemAdmin'] },
    { label: 'Dashboard',       icon: '📊', route: '/dashboard' },
    { label: 'Projects',        icon: '📁', route: '/projects' },
    { label: 'Tasks',           icon: '✅', route: '/tasks' },
    { label: 'Finance',         icon: '💰', route: '/finance' },
    { label: 'Users',           icon: '👥', route: '/users' },
    { label: 'Roles',           icon: '🔑', route: '/roles' },
    { label: 'Reports',         icon: '📈', route: '/reports' },
    { label: 'Notifications',   icon: '🔔', route: '/notifications' },
  ];

  visibleNavItems = computed(() => {
    const user = this.authService.currentUser();
    const userRoles = user?.roles ?? [];
    return this.allNavItems.filter(item => {
      if (!item.roles) return true;
      return item.roles.some(r => userRoles.includes(r));
    });
  });

  initials = computed(() => {
    const u = this.authService.currentUser();
    if (!u) return '?';
    return `${u.firstName[0]}${u.lastName[0]}`.toUpperCase();
  });

  primaryRole = computed(() => {
    const roles = this.authService.currentUser()?.roles ?? [];
    return roles[0] ?? 'User';
  });
}
