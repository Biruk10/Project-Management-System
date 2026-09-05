import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProjectService } from '../../../core/services/project.service';
import { TaskService } from '../../../core/services/task.service';
import { Project } from '../../../core/models/project.models';
import { Task } from '../../../core/models/task.models';
import { PagedResult } from '../../../core/models/api.models';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page animate-in">
      @if (loading()) {
        <div class="loading-state"><div class="spinner"></div>Loading project…</div>
      }

      @if (project()) {
        <div class="page-header">
          <div>
            <a routerLink="/projects" class="back-btn">← Projects</a>
            <div class="page-title" style="margin-top:8px">
              <div class="proj-icon-lg">{{ project()!.name[0].toUpperCase() }}</div>
              {{ project()!.name }}
            </div>
            @if (project()!.description) {
              <div class="page-subtitle" style="margin-top:4px">{{ project()!.description }}</div>
            }
          </div>
          <span class="badge" [ngClass]="statusClass(project()!.status)" style="font-size:13px;padding:6px 14px">
            {{ project()!.status }}
          </span>
        </div>

        <div class="stat-strip">
          <div class="stat-box">
            <div class="stat-box-label">Progress</div>
            <div class="stat-box-val">{{ project()!.progressPercentage }}%</div>
            <div class="progress-bar-track" style="width:100%;margin-top:6px">
              <div class="progress-bar-fill" [style.width.%]="project()!.progressPercentage"></div>
            </div>
          </div>
          <div class="stat-box">
            <div class="stat-box-label">Manager</div>
            <div class="stat-box-val">{{ project()!.projectManagerName ?? '—' }}</div>
          </div>
          <div class="stat-box">
            <div class="stat-box-label">Start Date</div>
            <div class="stat-box-val">{{ project()!.startDate | date:'MMM d, y' }}</div>
          </div>
          <div class="stat-box">
            <div class="stat-box-label">End Date</div>
            <div class="stat-box-val">{{ project()!.endDate ? (project()!.endDate! | date:'MMM d, y') : 'Open-ended' }}</div>
          </div>
          <div class="stat-box">
            <div class="stat-box-label">Members</div>
            <div class="stat-box-val">{{ project()!.members.length }}</div>
          </div>
          <div class="stat-box">
            <div class="stat-box-label">Tasks</div>
            <div class="stat-box-val">{{ tasks()?.totalCount ?? '…' }}</div>
          </div>
        </div>

        <div class="detail-grid">
          <div class="card detail-card">
            <div class="detail-card-header">👥 Team Members</div>
            <div class="detail-card-body">
              @if (project()!.members.length === 0) {
                <div class="empty-state">
                  <div class="empty-icon">👤</div>
                  <div class="empty-title">No members yet</div>
                </div>
              }
              @for (m of project()!.members; track m.userId) {
                <div class="member-row">
                  <div class="avatar">{{ m.fullName[0] }}</div>
                  <div class="member-info">
                    <div class="member-name">{{ m.fullName }}</div>
                    <div class="member-role-tag">{{ m.projectRole }}</div>
                  </div>
                  <div class="member-email">{{ m.email }}</div>
                </div>
              }
            </div>
          </div>

          <div class="card detail-card">
            <div class="detail-card-header">✅ Recent Tasks</div>
            <div class="detail-card-body">
              @if (!tasks() || tasks()!.items.length === 0) {
                <div class="empty-state">
                  <div class="empty-icon">📋</div>
                  <div class="empty-title">No tasks yet</div>
                </div>
              }
              @for (t of tasks()?.items; track t.id) {
                <div class="task-row">
                  <div class="task-status-dot" [ngClass]="taskStatusDot(t.status)"></div>
                  <div class="task-info">
                    <div class="task-title-sm">{{ t.title }}</div>
                    <div class="task-meta-row">
                      <span class="badge badge-gray" style="font-size:10px;padding:1px 7px">{{ t.priority }}</span>
                      @if (t.assignedToUserName) {
                        <span class="task-assignee">{{ t.assignedToUserName }}</span>
                      }
                    </div>
                  </div>
                  @if (t.dueDate) {
                    <div class="task-due" [class.overdue]="isOverdue(t)">
                      {{ t.dueDate | date:'MMM d' }}
                    </div>
                  }
                </div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .back-btn {
      font-size: 13px; color: var(--primary); font-weight: 600;
      &:hover { text-decoration: underline; }
    }

    .proj-icon-lg {
      width: 40px; height: 40px;
      border-radius: 12px;
      background: linear-gradient(135deg, #6366f1, #8b5cf6);
      color: #fff; font-size: 18px; font-weight: 700;
      display: inline-flex; align-items: center; justify-content: center;
      margin-right: 10px; vertical-align: middle;
    }

    .stat-strip {
      display: grid;
      grid-template-columns: repeat(6, 1fr);
      gap: 12px;
      margin-bottom: 24px;
    }

    .stat-box {
      background: #fff;
      border: 1px solid var(--gray-200);
      border-radius: var(--border-radius);
      padding: 16px;
      box-shadow: var(--card-shadow);
    }

    .stat-box-label { font-size: 11px; font-weight: 700; color: var(--gray-400); text-transform: uppercase; letter-spacing: 0.06em; margin-bottom: 4px; }
    .stat-box-val   { font-size: 17px; font-weight: 700; color: var(--gray-900); }

    .detail-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
    .detail-card { overflow: hidden; padding: 0; }
    .detail-card-header {
      padding: 16px 20px 14px;
      font-size: 14px; font-weight: 700; color: var(--gray-800);
      border-bottom: 1px solid var(--gray-100);
    }
    .detail-card-body { padding: 8px; }

    .member-row {
      display: flex; align-items: center; gap: 10px;
      padding: 10px 12px; border-radius: 8px;
      &:hover { background: var(--gray-50); }
    }
    .member-info { flex: 1; }
    .member-name { font-size: 13px; font-weight: 600; color: var(--gray-900); }
    .member-role-tag {
      display: inline-block; margin-top: 2px;
      font-size: 10px; font-weight: 600;
      background: var(--primary-light); color: var(--primary-dark);
      padding: 1px 7px; border-radius: 10px;
    }
    .member-email { font-size: 11px; color: var(--gray-400); }

    .task-row {
      display: flex; align-items: center; gap: 10px;
      padding: 10px 12px; border-radius: 8px;
      &:hover { background: var(--gray-50); }
    }
    .task-status-dot {
      width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0;
      &.dot-todo       { background: var(--gray-300); }
      &.dot-inprogress { background: var(--primary); }
      &.dot-inreview   { background: var(--warning); }
      &.dot-completed  { background: var(--success); }
    }
    .task-info { flex: 1; }
    .task-title-sm  { font-size: 13px; font-weight: 500; color: var(--gray-800); margin-bottom: 3px; }
    .task-meta-row  { display: flex; align-items: center; gap: 6px; }
    .task-assignee  { font-size: 11px; color: var(--gray-400); }
    .task-due       { font-size: 11px; font-weight: 600; color: var(--gray-500); }
    .task-due.overdue { color: var(--danger); }

    @media (max-width: 900px) {
      .stat-strip { grid-template-columns: repeat(3, 1fr); }
      .detail-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class ProjectDetailComponent implements OnInit {
  project = signal<Project | null>(null);
  tasks = signal<PagedResult<Task> | null>(null);
  loading = signal(true);

  constructor(private route: ActivatedRoute, private projectService: ProjectService, private taskService: TaskService) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.projectService.getById(id).subscribe({
      next: (p) => { this.project.set(p); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
    this.taskService.getAll({ projectId: id, pageSize: 10 }).subscribe({ next: (t) => this.tasks.set(t) });
  }

  statusClass(s: string): string {
    const m: Record<string,string> = { InProgress:'badge badge-primary', Completed:'badge badge-success', OnHold:'badge badge-warning', Cancelled:'badge badge-danger', NotStarted:'badge badge-gray' };
    return m[s] ?? 'badge badge-gray';
  }

  taskStatusDot(s: string): string {
    const m: Record<string,string> = { Todo:'dot-todo', InProgress:'dot-inprogress', InReview:'dot-inreview', Completed:'dot-completed' };
    return m[s] ?? 'dot-todo';
  }

  isOverdue(t: Task): boolean {
    return !!t.dueDate && t.status !== 'Completed' && new Date(t.dueDate) < new Date();
  }
}
