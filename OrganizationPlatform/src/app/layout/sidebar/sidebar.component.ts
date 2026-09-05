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
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
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
