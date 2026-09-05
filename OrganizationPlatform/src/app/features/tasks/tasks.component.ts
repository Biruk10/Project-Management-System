import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../core/services/task.service';
import { Task } from '../../core/models/task.models';
import { PagedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Tasks</h1>
      </div>

      <div class="filters">
        <input type="text" [(ngModel)]="search" (input)="onSearch()" placeholder="Search tasks..." class="search-input" />
        <select [(ngModel)]="statusFilter" (change)="onSearch()" class="filter-select">
          <option value="">All Status</option>
          <option value="Todo">Todo</option>
          <option value="InProgress">In Progress</option>
          <option value="InReview">In Review</option>
          <option value="Completed">Completed</option>
        </select>
        <select [(ngModel)]="priorityFilter" (change)="onSearch()" class="filter-select">
          <option value="">All Priority</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
          <option value="Critical">Critical</option>
        </select>
      </div>

      @if (loading()) {
        <div class="loading">Loading tasks...</div>
      } @else {
        <div class="table-card">
          <table class="data-table">
            <thead>
              <tr>
                <th>Title</th>
                <th>Project</th>
                <th>Assigned To</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Due Date</th>
                <th>Progress</th>
              </tr>
            </thead>
            <tbody>
              @for (task of result()?.items; track task.id) {
                <tr>
                  <td class="task-title">{{ task.title }}</td>
                  <td>{{ task.projectName }}</td>
                  <td>{{ task.assignedToUserName ?? '—' }}</td>
                  <td>
                    <span class="priority-badge" [class]="'priority-' + task.priority.toLowerCase()">
                      {{ task.priority }}
                    </span>
                  </td>
                  <td>
                    <span class="badge" [class]="'badge-' + task.status.toLowerCase()">{{ task.status }}</span>
                  </td>
                  <td [class.overdue]="isOverdue(task)">
                    {{ task.dueDate ? (task.dueDate | date:'mediumDate') : '—' }}
                  </td>
                  <td>
                    <div class="mini-progress">
                      <div class="mini-bar" [style.width.%]="task.completionPercentage"></div>
                    </div>
                    {{ task.completionPercentage }}%
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
      }
    </div>
  `,
  styles: [`
    .page { max-width: 1200px; margin: 0 auto; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .page-title { font-size: 24px; font-weight: 700; color: #111827; margin: 0; }
    .filters { display: flex; gap: 12px; margin-bottom: 16px; }
    .search-input { flex: 1; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 6px; font-size: 14px; }
    .filter-select { padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 6px; font-size: 14px; }
    .loading { text-align: center; padding: 40px; color: #6b7280; }
    .table-card { background: #fff; border-radius: 10px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); overflow: hidden; }
    .data-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .data-table th { background: #f9fafb; padding: 12px 16px; text-align: left; font-size: 12px; font-weight: 600; color: #6b7280; text-transform: uppercase; }
    .data-table td { padding: 14px 16px; border-top: 1px solid #f3f4f6; color: #374151; }
    .task-title { font-weight: 500; }
    .badge { padding: 3px 10px; border-radius: 12px; font-size: 12px; font-weight: 500; background: #e5e7eb; }
    .badge-todo { background: #f3f4f6; color: #6b7280; }
    .badge-inprogress { background: #dbeafe; color: #1d4ed8; }
    .badge-completed { background: #d1fae5; color: #065f46; }
    .badge-inreview { background: #fef3c7; color: #92400e; }
    .priority-badge { padding: 2px 8px; border-radius: 4px; font-size: 11px; font-weight: 600; }
    .priority-critical { background: #fee2e2; color: #dc2626; }
    .priority-high { background: #ffedd5; color: #ea580c; }
    .priority-medium { background: #fef9c3; color: #ca8a04; }
    .priority-low { background: #f0fdf4; color: #16a34a; }
    .overdue { color: #ef4444; }
    .mini-progress { height: 4px; background: #e5e7eb; border-radius: 2px; width: 60px; display: inline-block; vertical-align: middle; margin-right: 4px; }
    .mini-bar { height: 100%; background: #3b82f6; border-radius: 2px; }
    .pagination { display: flex; align-items: center; gap: 12px; justify-content: center; margin-top: 16px; font-size: 13px; color: #6b7280; }
    .pagination button { padding: 6px 12px; border: 1px solid #d1d5db; border-radius: 6px; cursor: pointer; background: #fff; }
    .pagination button:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class TasksComponent implements OnInit {
  result = signal<PagedResult<Task> | null>(null);
  loading = signal(true);
  page = 1;
  search = '';
  statusFilter = '';
  priorityFilter = '';

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.taskService.getAll({
      page: this.page,
      search: this.search || undefined,
      status: this.statusFilter || undefined,
      priority: this.priorityFilter || undefined
    }).subscribe({
      next: (data) => { this.result.set(data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  onSearch(): void {
    this.page = 1;
    this.load();
  }

  changePage(p: number): void {
    this.page = p;
    this.load();
  }

  isOverdue(task: Task): boolean {
    if (!task.dueDate || task.status === 'Completed') return false;
    return new Date(task.dueDate) < new Date();
  }
}
