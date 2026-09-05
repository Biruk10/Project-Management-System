import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { User } from '../../core/models/user.models';
import { PagedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Users</h1>
        <button class="btn-primary">+ Invite User</button>
      </div>

      <div class="filters">
        <input type="text" [(ngModel)]="search" (input)="onSearch()" placeholder="Search users..." class="search-input" />
        <select [(ngModel)]="activeFilter" (change)="onSearch()" class="filter-select">
          <option value="">All</option>
          <option value="true">Active</option>
          <option value="false">Inactive</option>
        </select>
      </div>

      <div class="table-card">
        <table class="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Roles</th>
              <th>Status</th>
              <th>Last Login</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (user of result()?.items; track user.id) {
              <tr>
                <td class="user-name">
                  <div class="avatar">{{ user.firstName[0] }}</div>
                  {{ user.firstName }} {{ user.lastName }}
                </td>
                <td>{{ user.email }}</td>
                <td>{{ user.roles.join(', ') || '—' }}</td>
                <td>
                  <span class="status-dot" [class.active]="user.isActive"></span>
                  {{ user.isActive ? 'Active' : 'Inactive' }}
                </td>
                <td>{{ user.lastLoginAt ? (user.lastLoginAt | date:'mediumDate') : 'Never' }}</td>
                <td>
                  <button class="btn-sm" (click)="toggle(user)">
                    {{ user.isActive ? 'Deactivate' : 'Activate' }}
                  </button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      <div class="pagination">
        <button [disabled]="page === 1" (click)="changePage(page - 1)">Previous</button>
        <span>Page {{ page }} of {{ result()?.totalPages }}</span>
        <button [disabled]="!result()?.hasNextPage" (click)="changePage(page + 1)">Next</button>
      </div>
    </div>
  `,
  styles: [`
    .page { max-width: 1200px; margin: 0 auto; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .page-title { font-size: 24px; font-weight: 700; color: #111827; margin: 0; }
    .btn-primary { background: #3b82f6; color: #fff; border: none; padding: 10px 18px; border-radius: 6px; cursor: pointer; font-weight: 500; }
    .filters { display: flex; gap: 12px; margin-bottom: 16px; }
    .search-input { flex: 1; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 6px; font-size: 14px; }
    .filter-select { padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 6px; font-size: 14px; }
    .table-card { background: #fff; border-radius: 10px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); overflow: hidden; }
    .data-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .data-table th { background: #f9fafb; padding: 12px 16px; text-align: left; font-size: 12px; font-weight: 600; color: #6b7280; text-transform: uppercase; }
    .data-table td { padding: 14px 16px; border-top: 1px solid #f3f4f6; color: #374151; }
    .user-name { display: flex; align-items: center; gap: 10px; font-weight: 500; }
    .avatar { width: 30px; height: 30px; border-radius: 50%; background: #3b82f6; color: #fff; display: flex; align-items: center; justify-content: center; font-size: 13px; font-weight: 600; flex-shrink: 0; }
    .status-dot { width: 8px; height: 8px; border-radius: 50%; background: #d1d5db; display: inline-block; margin-right: 5px; }
    .status-dot.active { background: #22c55e; }
    .btn-sm { padding: 4px 10px; font-size: 12px; border: 1px solid #d1d5db; border-radius: 4px; cursor: pointer; background: #fff; }
    .btn-sm:hover { background: #f3f4f6; }
    .pagination { display: flex; align-items: center; gap: 12px; justify-content: center; margin-top: 16px; font-size: 13px; color: #6b7280; }
    .pagination button { padding: 6px 12px; border: 1px solid #d1d5db; border-radius: 6px; cursor: pointer; background: #fff; }
    .pagination button:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class UsersComponent implements OnInit {
  result = signal<PagedResult<User> | null>(null);
  page = 1;
  search = '';
  activeFilter = '';

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.userService.getAll({
      page: this.page,
      search: this.search || undefined,
      isActive: this.activeFilter !== '' ? this.activeFilter === 'true' : undefined
    }).subscribe({ next: (d) => this.result.set(d) });
  }

  onSearch(): void {
    this.page = 1;
    this.load();
  }

  changePage(p: number): void {
    this.page = p;
    this.load();
  }

  toggle(user: User): void {
    const action = user.isActive
      ? this.userService.deactivate(user.id)
      : this.userService.activate(user.id);

    action.subscribe({ next: () => this.load() });
  }
}
