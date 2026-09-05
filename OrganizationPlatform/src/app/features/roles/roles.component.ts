import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';

interface Role {
  id: number;
  name: string;
  description?: string;
  isSystemRole: boolean;
  permissions: string[];
}

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Roles & Permissions</h1>
        <button class="btn-primary">+ New Role</button>
      </div>

      <div class="roles-grid">
        @for (role of roles(); track role.id) {
          <div class="role-card">
            <div class="role-header">
              <h3 class="role-name">{{ role.name }}</h3>
              @if (role.isSystemRole) {
                <span class="system-badge">System</span>
              }
            </div>
            <p class="role-desc">{{ role.description ?? 'No description.' }}</p>
            <div class="permissions">
              @for (p of role.permissions; track p) {
                <span class="perm-tag">{{ p }}</span>
              }
              @if (role.permissions.length === 0) {
                <span class="no-perms">No permissions assigned</span>
              }
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .page { max-width: 1200px; margin: 0 auto; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .page-title { font-size: 24px; font-weight: 700; color: #111827; margin: 0; }
    .btn-primary { background: #3b82f6; color: #fff; border: none; padding: 10px 18px; border-radius: 6px; cursor: pointer; font-weight: 500; }
    .roles-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(340px, 1fr)); gap: 16px; }
    .role-card { background: #fff; border-radius: 10px; padding: 20px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
    .role-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 8px; }
    .role-name { font-size: 16px; font-weight: 600; color: #111827; margin: 0; }
    .system-badge { font-size: 11px; background: #ede9fe; color: #7c3aed; padding: 2px 8px; border-radius: 10px; font-weight: 500; }
    .role-desc { font-size: 13px; color: #6b7280; margin: 0 0 14px; }
    .permissions { display: flex; flex-wrap: wrap; gap: 6px; }
    .perm-tag { font-size: 11px; padding: 3px 8px; background: #f3f4f6; color: #374151; border-radius: 4px; }
    .no-perms { font-size: 12px; color: #9ca3af; }
  `]
})
export class RolesComponent implements OnInit {
  roles = signal<Role[]>([]);

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.get<Role[]>('/roles').subscribe({
      next: (data) => this.roles.set(data)
    });
  }
}
