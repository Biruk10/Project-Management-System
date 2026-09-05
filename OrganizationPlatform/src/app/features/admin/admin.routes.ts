import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
  },
  {
    path: 'organizations',
    loadComponent: () => import('./organizations/admin-organizations.component').then(m => m.AdminOrganizationsComponent)
  },
  {
    path: 'users',
    loadComponent: () => import('./users/admin-users.component').then(m => m.AdminUsersComponent)
  },
  {
    path: 'audit',
    loadComponent: () => import('./audit/admin-audit.component').then(m => m.AdminAuditComponent)
  },
  {
    path: 'permissions',
    loadComponent: () => import('./permissions/admin-permissions.component').then(m => m.AdminPermissionsComponent)
  }
];
