import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardSummary, ProjectProgress } from '../../core/models/dashboard.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page animate-in">
      <div class="page-header">
        <div>
          <div class="page-title">📊 Dashboard</div>
          <div class="page-subtitle">Organization overview and key metrics</div>
        </div>
        <a routerLink="/projects" class="btn-primary">+ New Project</a>
      </div>

      @if (loading()) {
        <div class="loading-state"><div class="spinner"></div>Loading dashboard…</div>
      }

      @if (data()) {
        <div class="kpi-grid">
          <div class="kpi-card" style="--accent:#6366f1">
            <div class="kpi-icon" style="background:#ede9fe">📁</div>
            <div class="kpi-label">Total Projects</div>
            <div class="kpi-value">{{ data()!.totalProjects }}</div>
            <div class="kpi-sub">
              <span class="kpi-trend-up">▲ {{ data()!.activeProjects }}</span> active &nbsp;·&nbsp;
              <span class="kpi-trend-up" style="color:var(--success)">{{ data()!.completedProjects }}</span> done
            </div>
          </div>

          <div class="kpi-card" style="--accent:#10b981">
            <div class="kpi-icon" style="background:#d1fae5">✅</div>
            <div class="kpi-label">Total Tasks</div>
            <div class="kpi-value">{{ data()!.totalTasks }}</div>
            <div class="kpi-sub">
              <span class="kpi-trend-down">⚠ {{ data()!.overdueTasks }} overdue</span>&nbsp;·&nbsp;
              {{ data()!.upcomingTasks }} due soon
            </div>
          </div>

          <div class="kpi-card" style="--accent:#f59e0b">
            <div class="kpi-icon" style="background:#fef3c7">💰</div>
            <div class="kpi-label">Total Budget</div>
            <div class="kpi-value">\${{ data()!.totalBudget | number:'1.0-0' }}</div>
            <div class="kpi-sub">
              \${{ data()!.totalExpenses | number:'1.0-0' }} spent &nbsp;·&nbsp;
              <span [class.kpi-trend-up]="data()!.budgetUtilizationPercentage < 80"
                    [class.kpi-trend-down]="data()!.budgetUtilizationPercentage >= 80">
                {{ data()!.budgetUtilizationPercentage }}% used
              </span>
            </div>
          </div>

          <div class="kpi-card" style="--accent:#3b82f6">
            <div class="kpi-icon" style="background:#dbeafe">👥</div>
            <div class="kpi-label">Team Members</div>
            <div class="kpi-value">{{ data()!.totalUsers }}</div>
            <div class="kpi-sub">
              <span class="kpi-trend-up">● {{ data()!.activeUsers }} active</span>
            </div>
          </div>
        </div>

        <div class="dash-grid">
          <div class="card dash-card">
            <div class="dash-card-header">
              <span class="section-title" style="margin:0">📈 Project Progress</span>
              <a routerLink="/projects" class="view-all">View all →</a>
            </div>
            <div class="project-list">
              @for (p of data()!.projectProgress; track p.projectId) {
                <div class="project-row">
                  <div class="project-row-top">
                    <span class="project-row-name">{{ p.projectName }}</span>
                    <span class="badge" [ngClass]="statusBadge(p.status)">{{ p.status }}</span>
                  </div>
                  <div class="project-row-stats">
                    <span class="stat-chip">{{ p.completedTasks }}/{{ p.totalTasks }} tasks</span>
                    <span class="stat-chip">{{ p.budgetUtilization }}% budget</span>
                  </div>
                  <div class="progress-row-bar">
                    <div class="progress-bar-track">
                      <div class="progress-bar-fill" [style.width.%]="p.progressPercentage"></div>
                    </div>
                    <span class="prog-pct">{{ p.progressPercentage }}%</span>
                  </div>
                </div>
              }
              @if (data()!.projectProgress.length === 0) {
                <div class="empty-state">
                  <div class="empty-icon">📭</div>
                  <div class="empty-title">No projects yet</div>
                  <a routerLink="/projects" class="btn-primary btn-sm" style="margin-top:8px">Create one</a>
                </div>
              }
            </div>
          </div>

          <div class="card dash-card">
            <div class="dash-card-header">
              <span class="section-title" style="margin:0">⚡ Recent Activity</span>
            </div>
            <div class="activity-list">
              @for (a of data()!.recentActivities; track $index) {
                <div class="activity-row">
                  <div class="activity-dot"></div>
                  <div class="activity-body">
                    <span class="activity-text">
                      <strong>{{ a.action }}</strong> {{ a.entityName }}
                    </span>
                    <span class="activity-meta">
                      {{ a.userName ?? 'System' }} · {{ a.occurredAt | date:'MMM d, h:mm a' }}
                    </span>
                  </div>
                </div>
              }
              @if (data()!.recentActivities.length === 0) {
                <div class="empty-state">
                  <div class="empty-icon">📋</div>
                  <div class="empty-title">No activity yet</div>
                </div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .kpi-card {
      border-top: 3px solid var(--accent, var(--primary));
      &::before { background: var(--accent, var(--primary)); }
    }

    .dash-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 20px;
    }

    .dash-card { padding: 0; overflow: hidden; }

    .dash-card-header {
      display: flex; align-items: center; justify-content: space-between;
      padding: 18px 20px 14px;
      border-bottom: 1px solid var(--gray-100);
    }

    .view-all { font-size: 12px; color: var(--primary); font-weight: 600; &:hover { text-decoration: underline; } }

    .project-list, .activity-list { padding: 8px; }

    .project-row {
      padding: 12px;
      border-radius: 8px;
      transition: background 0.12s;
      &:hover { background: var(--gray-50); }
    }

    .project-row-top {
      display: flex; align-items: center; justify-content: space-between;
      margin-bottom: 6px;
    }

    .project-row-name {
      font-size: 13px; font-weight: 600; color: var(--gray-800);
    }

    .project-row-stats {
      display: flex; gap: 6px; margin-bottom: 8px;
    }

    .stat-chip {
      font-size: 11px;
      background: var(--gray-100);
      color: var(--gray-500);
      padding: 2px 8px;
      border-radius: 20px;
      font-weight: 500;
    }

    .progress-row-bar {
      display: flex; align-items: center; gap: 8px;
    }

    .progress-bar-track { flex: 1; }

    .prog-pct {
      font-size: 11px; font-weight: 700;
      color: var(--primary); width: 30px;
      text-align: right; flex-shrink: 0;
    }

    .activity-row {
      display: flex; gap: 12px; align-items: flex-start;
      padding: 10px 12px;
      border-radius: 8px;
      &:hover { background: var(--gray-50); }
    }

    .activity-dot {
      width: 8px; height: 8px;
      border-radius: 50%;
      background: var(--primary);
      margin-top: 4px;
      flex-shrink: 0;
    }

    .activity-body { flex: 1; min-width: 0; }

    .activity-text {
      font-size: 13px; color: var(--gray-700);
      display: block; margin-bottom: 2px;
      strong { color: var(--gray-900); }
    }

    .activity-meta { font-size: 11px; color: var(--gray-400); }

    @media (max-width: 900px) {
      .dash-grid { grid-template-columns: 1fr; }
    }
  `]
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
