import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-admin-organizations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-organizations.component.html',
  styleUrl: './admin-organizations.component.scss'
})
export class AdminOrganizationsComponent implements OnInit {
  loading  = signal(true);
  allOrgs  = signal<any[]>([]);
  filtered = signal<any[]>([]);
  actionId = signal<number | null>(null);
  search = '';
  statusFilter = '';

  constructor(private api: ApiService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.api.get<any[]>('/platform/organizations').subscribe({
      next: (data) => {
        const list = Array.isArray(data) ? data : [];
        this.allOrgs.set(list);
        this.applyFilter();
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  applyFilter(): void {
    let list = this.allOrgs();
    if (this.search.trim()) {
      const s = this.search.toLowerCase();
      list = list.filter(o => o.name.toLowerCase().includes(s) || o.email.toLowerCase().includes(s));
    }
    if (this.statusFilter === 'active')    list = list.filter(o => o.isActive);
    if (this.statusFilter === 'suspended') list = list.filter(o => !o.isActive);
    this.filtered.set(list);
  }

  activate(id: number): void {
    this.actionId.set(id);
    this.api.patch<void>(`/platform/organizations/${id}/activate`, {}).subscribe({
      next: () => { this.actionId.set(null); this.load(); },
      error: () => this.actionId.set(null)
    });
  }

  suspend(id: number): void {
    this.actionId.set(id);
    this.api.patch<void>(`/platform/organizations/${id}/suspend`, {}).subscribe({
      next: () => { this.actionId.set(null); this.load(); },
      error: () => this.actionId.set(null)
    });
  }
}
