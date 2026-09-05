import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-admin-audit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-audit.component.html',
  styleUrl: './admin-audit.component.scss'
})
export class AdminAuditComponent implements OnInit {
  loading     = signal(true);
  allLogs     = signal<any[]>([]);
  filtered    = signal<any[]>([]);
  total       = signal(0);
  entityNames = signal<string[]>([]);
  search = '';
  entityFilter = '';

  constructor(private api: ApiService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.api.get<any>('/platform/audit-logs', { pageSize: 200 }).subscribe({
      next: (data) => {
        const list = data?.items ?? [];
        this.allLogs.set(list);
        this.total.set(data?.totalCount ?? list.length);
        const names = [...new Set<string>(list.map((l: any) => l.entityName))].sort();
        this.entityNames.set(names);
        this.applyFilter();
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  applyFilter(): void {
    let list = this.allLogs();
    if (this.search.trim()) {
      const s = this.search.toLowerCase();
      list = list.filter(l =>
        l.action.toLowerCase().includes(s) ||
        l.entityName.toLowerCase().includes(s) ||
        (l.organizationName ?? '').toLowerCase().includes(s)
      );
    }
    if (this.entityFilter) list = list.filter(l => l.entityName === this.entityFilter);
    this.filtered.set(list);
  }

  actionClass(action: string): string {
    const a = action.toLowerCase();
    if (a.includes('create')) return 'create';
    if (a.includes('update')) return 'update';
    if (a.includes('delete')) return 'delete';
    if (a.includes('login'))  return 'login';
    return 'other';
  }
}
