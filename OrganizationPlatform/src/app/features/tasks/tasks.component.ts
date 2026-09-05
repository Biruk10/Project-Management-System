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
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.scss'
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
