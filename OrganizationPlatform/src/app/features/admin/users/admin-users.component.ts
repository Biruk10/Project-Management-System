import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss'
})
export class AdminUsersComponent implements OnInit {
  loading  = signal(true);
  allUsers = signal<any[]>([]);
  filtered = signal<any[]>([]);
  total    = signal(0);
  actionId = signal<number | null>(null);
  search = '';

  constructor(private api: ApiService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.api.get<any>('/platform/users', { pageSize: 200 }).subscribe({
      next: (data) => {
        const list = data?.items ?? [];
        this.allUsers.set(list);
        this.total.set(data?.totalCount ?? list.length);
        this.applyFilter();
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  applyFilter(): void {
    if (!this.search.trim()) { this.filtered.set(this.allUsers()); return; }
    const s = this.search.toLowerCase();
    this.filtered.set(this.allUsers().filter(u =>
      `${u.firstName} ${u.lastName}`.toLowerCase().includes(s) ||
      u.email.toLowerCase().includes(s) ||
      (u.organizationName ?? '').toLowerCase().includes(s)
    ));
  }

  activate(id: number): void {
    this.actionId.set(id);
    this.api.patch<void>(`/platform/users/${id}/activate`, {}).subscribe({
      next: () => { this.actionId.set(null); this.load(); },
      error: () => this.actionId.set(null)
    });
  }

  deactivate(id: number): void {
    this.actionId.set(id);
    this.api.patch<void>(`/platform/users/${id}/deactivate`, {}).subscribe({
      next: () => { this.actionId.set(null); this.load(); },
      error: () => this.actionId.set(null)
    });
  }
}
