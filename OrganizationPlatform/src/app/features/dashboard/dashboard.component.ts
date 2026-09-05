import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardSummary, ProjectProgress } from '../../core/models/dashboard.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  data = signal<DashboardSummary | null>(null);
  loading = signal(true);

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe({
      next: (d) => { this.data.set(d); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  statusBadge(status: string): string {
    const map: Record<string, string> = {
      InProgress: 'badge badge-primary',
      Completed: 'badge badge-success',
      OnHold: 'badge badge-warning',
      Cancelled: 'badge badge-danger',
      NotStarted: 'badge badge-gray'
    };
    return map[status] ?? 'badge badge-gray';
  }
}
