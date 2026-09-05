import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <h1 class="page-title">Reports</h1>

      <div class="tab-bar">
        <button [class.active]="tab === 'projects'" (click)="switchTab('projects')">Projects</button>
        <button [class.active]="tab === 'budgets'" (click)="switchTab('budgets')">Budgets</button>
        <button [class.active]="tab === 'expenses'" (click)="switchTab('expenses')">Expenses</button>
        <button [class.active]="tab === 'tasks'" (click)="switchTab('tasks')">Tasks</button>
      </div>

      @if (loading()) {
        <div class="loading">Loading report...</div>
      }

      @if (!loading() && tab === 'projects') {
        <div class="table-card">
          <table class="data-table">
            <thead>
              <tr>
                <th>Project</th>
                <th>Status</th>
                <th>Progress</th>
                <th>Total Tasks</th>
                <th>Completed</th>
                <th>Budget</th>
                <th>Expenses</th>
                <th>Utilization</th>
              </tr>
            </thead>
            <tbody>
              @for (r of reportData(); track r.projectId) {
                <tr>
                  <td class="font-medium">{{ r.projectName }}</td>
                  <td>{{ r.status }}</td>
                  <td>{{ r.progressPercentage }}%</td>
                  <td>{{ r.totalTasks }}</td>
                  <td>{{ r.completedTasks }}</td>
                  <td>{{ r.totalBudget | number:'1.2-2' }}</td>
                  <td>{{ r.totalExpenses | number:'1.2-2' }}</td>
                  <td>{{ r.budgetUtilization }}%</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }

      @if (!loading() && tab === 'tasks') {
        <div class="summary-row">
          <div class="stat-card">
            <div class="stat-label">Total Tasks</div>
            <div class="stat-value">{{ taskReport()?.totalTasks }}</div>
          </div>
          <div class="stat-card">
            <div class="stat-label">Completed</div>
            <div class="stat-value green">{{ taskReport()?.completedTasks }}</div>
          </div>
          <div class="stat-card">
            <div class="stat-label">In Progress</div>
            <div class="stat-value blue">{{ taskReport()?.inProgressTasks }}</div>
          </div>
          <div class="stat-card">
            <div class="stat-label">Overdue</div>
            <div class="stat-value red">{{ taskReport()?.overdueTasks }}</div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .page { max-width: 1200px; margin: 0 auto; }
    .page-title { font-size: 24px; font-weight: 700; color: #111827; margin: 0 0 24px; }
    .tab-bar { display: flex; gap: 4px; margin-bottom: 20px; border-bottom: 2px solid #e5e7eb; }
    .tab-bar button { padding: 10px 20px; border: none; background: none; cursor: pointer; font-size: 14px; color: #6b7280; font-weight: 500; }
    .tab-bar button.active { color: #3b82f6; border-bottom: 2px solid #3b82f6; margin-bottom: -2px; }
    .loading { text-align: center; padding: 40px; color: #6b7280; }
    .table-card { background: #fff; border-radius: 10px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); overflow: hidden; }
    .data-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .data-table th { background: #f9fafb; padding: 12px 16px; text-align: left; font-size: 12px; font-weight: 600; color: #6b7280; text-transform: uppercase; }
    .data-table td { padding: 14px 16px; border-top: 1px solid #f3f4f6; color: #374151; }
    .font-medium { font-weight: 500; }
    .summary-row { display: flex; gap: 16px; }
    .stat-card { flex: 1; background: #fff; border-radius: 10px; padding: 20px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
    .stat-label { font-size: 12px; color: #6b7280; }
    .stat-value { font-size: 32px; font-weight: 700; color: #111827; margin-top: 6px; }
    .green { color: #16a34a; }
    .blue { color: #2563eb; }
    .red { color: #dc2626; }
  `]
})
export class ReportsComponent implements OnInit {
  tab = 'projects';
  loading = signal(false);
  reportData = signal<any[]>([]);
  taskReport = signal<any>(null);

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadReport();
  }

  switchTab(t: string): void {
    this.tab = t;
    this.loadReport();
  }

  loadReport(): void {
    this.loading.set(true);

    if (this.tab === 'projects') {
      this.api.get<any[]>('/reports/projects').subscribe({
        next: (d) => { this.reportData.set(d); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
    } else if (this.tab === 'tasks') {
      this.api.get<any>('/reports/tasks').subscribe({
        next: (d) => { this.taskReport.set(d); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
    } else {
      this.loading.set(false);
    }
  }
}
