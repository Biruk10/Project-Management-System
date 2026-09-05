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
  templateUrl: './projects-list.component.html',
  styleUrl: './projects-list.component.scss'
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
