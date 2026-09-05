import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../../core/services/project.service';
import { Project } from '../../../core/models/project.models';
import { PagedResult } from '../../../core/models/api.models';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="page animate-in">
      <div class="page-header">
        <div>
          <div class="page-title">📁 Projects</div>
          <div class="page-subtitle">{{ result()?.totalCount ?? 0 }} projects total</div>
        </div>
        <button class="btn-primary">+ New Project</button>
      </div>

      <div class="filters">
        <input class="filter-input" [(ngModel)]="search" (ngModelChange)="onSearch()" placeholder="🔍  Search projects…" />
        <select class="filter-select" [(ngModel)]="statusFilter" (ngModelChange)="onSearch()">
          <option value="">All Status</option>
          <option value="NotStarted">Not Started</option>
          <option value="InProgress">In Progress</option>
          <option value="OnHold">On Hold</option>
          <option value="Completed">Completed</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </div>

      @if (loading()) {
        <div class="loading-state"><div class="spinner"></div>Loading projects…</div>
      } @else if (result()?.items?.length === 0) {
        <div class="card empty-state">
          <div class="empty-icon">📭</div>
          <div class="empty-title">No projects found</div>
          <div class="empty-desc">Try adjusting your search or create a new project.</div>
        </div>
      } @else {
        <div class="table-wrapper">
          <table class="data-table">
            <thead>
              <tr>
                <th>Project</th>
                <th>Status</th>
                <th>Manager</th>
                <th>Progress</th>
                <th>Members</th>
                <th>Timeline</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (p of result()!.items; track p.id) {
                <tr>
                  <td>
                    <div class="proj-cell">
                      <div class="proj-icon">{{ p.name[0].toUpperCase() }}</div>
                      <div>
                        <div class="proj-name">{{ p.name }}</div>
                        @if (p.description) {
                          <div class="proj-desc">{{ p.description | slice:0:50 }}{{ p.description!.length > 50 ? '…' : '' }}</div>
                        }
                      </div>
                    </div>
                  </td>
                  <td>
                    <span class="badge" [ngClass]="statusClass(p.status)">{{ p.status }}</span>
                  </td>
                  <td>
                    @if (p.projectManagerName) {
                      <div class="manager-cell">
                        <div class="avatar-xs">{{ p.projectManagerName[0] }}</div>
                        <span>{{ p.projectManagerName }}</span>
                      </div>
                    } @else {
                      <span class="text-muted">Unassigned</span>
                    }
                  </td>
                  <td>
                    <div class="progress-cell">
                      <div class="progress-bar-track" style="width:80px">
                        <div class="progress-bar-fill" [style.width.%]="p.progressPercentage"></div>
                      </div>
                      <span class="prog-label">{{ p.progressPercentage }}%</span>
                    </div>
                  </td>
                  <td>
                    <span class="members-chip">
                      👤 {{ p.members.length }}
                    </span>
                  </td>
                  <td>
                    <div class="timeline-cell">
                      <span>{{ p.startDate | date:'MMM d' }}</span>
                      @if (p.endDate) {
                        <span class="arrow">→</span>
                        <span>{{ p.endDate | date:'MMM d, y' }}</span>
                      }
                    </div>
                  </td>
                  <td>
                    <a [routerLink]="['/projects', p.id]" class="btn-ghost btn-sm">View →</a>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>

        <div class="pagination">
          <button [disabled]="page === 1" (click)="changePage(page - 1)">← Prev</button>
          <span>Page {{ page }} of {{ result()?.totalPages }}</span>
          <button [disabled]="!result()?.hasNextPage" (click)="changePage(page + 1)">Next →</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .proj-cell { display: flex; align-items: center; gap: 10px; }
    .proj-icon {
      width: 34px; height: 34px;
      border-radius: 9px;
      background: linear-gradient(135deg, #6366f1, #8b5cf6);
      color: #fff; font-size: 14px; font-weight: 700;
      display: flex; align-items: center; justify-content: center;
      flex-shrink: 0;
    }
    .proj-name { font-size: 13px; font-weight: 600; color: var(--gray-900); }
    .proj-desc { font-size: 11px; color: var(--gray-400); margin-top: 1px; }

    .manager-cell { display: flex; align-items: center; gap: 6px; font-size: 13px; color: var(--gray-700); }
    .avatar-xs {
      width: 24px; height: 24px;
      border-radius: 6px;
      background: var(--primary-light);
      color: var(--primary);
      font-size: 10px; font-weight: 700;
      display: flex; align-items: center; justify-content: center;
    }

    .progress-cell { display: flex; align-items: center; gap: 8px; }
    .prog-label { font-size: 12px; font-weight: 600; color: var(--gray-600); width: 32px; }

    .members-chip { font-size: 12px; color: var(--gray-600); font-weight: 500; }

    .timeline-cell {
      display: flex; align-items: center; gap: 4px;
      font-size: 12px; color: var(--gray-500);
    }
    .arrow { color: var(--gray-300); }
    .text-muted { font-size: 12px; color: var(--gray-400); }
  `]
})
export class ProjectsListComponent implements OnInit {
  result = signal<PagedResult<Project> | null>(null);
  loading = signal(true);
  showCreate = false;
  page = 1;
  search = '';
  statusFilter = '';

  constructor(private projectService: ProjectService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.projectService.getAll({ page: this.page, search: this.search || undefined, status: this.statusFilter || undefined }).subscribe({
      next: (d) => { this.result.set(d); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  onSearch(): void { this.page = 1; this.load(); }
  changePage(p: number): void { this.page = p; this.load(); }

  statusClass(status: string): string {
    const map: Record<string, string> = {
      InProgress: 'badge badge-primary', Completed: 'badge badge-success',
      OnHold: 'badge badge-warning', Cancelled: 'badge badge-danger', NotStarted: 'badge badge-gray'
    };
    return map[status] ?? 'badge badge-gray';
  }
}
