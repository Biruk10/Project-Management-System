import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';

interface PermModule {
  name: string;
  icon: string;
  isPlatform: boolean;
  permissions: { id: number; key: string; description: string }[];
}

@Component({
  selector: 'app-admin-permissions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-permissions.component.html',
  styleUrl: './admin-permissions.component.scss'
})
export class AdminPermissionsComponent implements OnInit {
  loading = signal(true);
  modules = signal<PermModule[]>([]);
  total   = signal(0);

  private readonly icons: Record<string, { icon: string; order: number }> = {
    Platform:     { icon: '🛡️', order: 0 },
    Project:      { icon: '📁', order: 1 },
    Task:         { icon: '✅', order: 2 },
    Budget:       { icon: '💰', order: 3 },
    Expense:      { icon: '🧾', order: 4 },
    Report:       { icon: '📈', order: 5 },
    User:         { icon: '👥', order: 6 },
    Role:         { icon: '🎭', order: 7 },
    Organization: { icon: '🏢', order: 8 },
    AuditLog:     { icon: '📋', order: 9 },
    Notification: { icon: '🔔', order: 10 }
  };

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.get<any[]>('/permissions').subscribe({
      next: (data) => {
        const list = Array.isArray(data) ? data : [];
        this.total.set(list.length);

        const groups: Record<string, any[]> = {};
        for (const p of list) {
          const mod = p.key.split('.')[0];
          if (!groups[mod]) groups[mod] = [];
          groups[mod].push(p);
        }

        const mods: PermModule[] = Object.entries(groups)
          .map(([name, perms]) => ({
            name,
            icon: this.icons[name]?.icon ?? '⚙️',
            isPlatform: name === 'Platform',
            order: this.icons[name]?.order ?? 99,
            permissions: perms
          }))
          .sort((a, b) => a.order - b.order);

        this.modules.set(mods);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
