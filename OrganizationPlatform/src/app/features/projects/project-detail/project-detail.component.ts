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
  templateUrl: './project-detail.component.html',
  styleUrl: './project-detail.component.scss'
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
