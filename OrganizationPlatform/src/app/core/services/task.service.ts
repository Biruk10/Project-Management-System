import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.models';
import { Task, CreateTaskRequest, UpdateTaskRequest, TaskFilterParams } from '../models/task.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  constructor(private api: ApiService) {}

  getAll(filters?: TaskFilterParams): Observable<PagedResult<Task>> {
    return this.api.get<PagedResult<Task>>('/tasks', filters as Record<string, string | number | boolean | undefined>);
  }

  getById(id: number): Observable<Task> {
    return this.api.get<Task>(`/tasks/${id}`);
  }

  create(request: CreateTaskRequest): Observable<Task> {
    return this.api.post<Task>('/tasks', request);
  }

  update(id: number, request: UpdateTaskRequest): Observable<Task> {
    return this.api.put<Task>(`/tasks/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`/tasks/${id}`);
  }
}
