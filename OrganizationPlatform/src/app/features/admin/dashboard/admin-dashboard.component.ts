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
  data = signal<any>(null);
  now = new Date();

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.get<any>('/platform/dashboard').subscribe({
      next: (d) => { this.data.set(d); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  utilizationPct(org: any): number {
    return org.totalBudget > 0
      ? Math.min(100, Math.round((org.totalExpenses / org.totalBudget) * 100))
      : 0;
  }
}
