import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  loading = signal(true);
  refreshing = signal(false);
  data = signal<any>(null);
  now = new Date();

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.api.get<any>('/platform/dashboard').subscribe({
      next: (d) => {
        this.data.set(d);
        this.loading.set(false);
        this.refreshing.set(false);
        this.now = new Date();
      },
      error: () => {
        this.loading.set(false);
        this.refreshing.set(false);
      }
    });
  }

  refresh(): void {
    this.refreshing.set(true);
    this.loadData();
  }

  overallUtilization(): number {
    const d = this.data();
    if (!d || !d.totalBudget || d.totalBudget <= 0) return 0;
    return Math.min(100, Math.round((d.totalExpenses / d.totalBudget) * 100));
  }

  remainingBudget(): number {
    const d = this.data();
    if (!d) return 0;
    return Math.max(0, (d.totalBudget ?? 0) - (d.totalExpenses ?? 0));
  }
}
