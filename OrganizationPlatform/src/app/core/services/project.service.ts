import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.models';
import { Project, CreateProjectRequest, UpdateProjectRequest, ProjectFilterParams, ProjectMember } from '../models/project.models';

@Injectable({ providedIn: 'root' })
export class ProjectService {
  constructor(private api: ApiService) {}

  getAll(filters?: ProjectFilterParams): Observable<PagedResult<Project>> {
    return this.api.get<PagedResult<Project>>('/projects', filters as Record<string, string | number | boolean | undefined>);
  }

  getById(id: number): Observable<Project> {
    return this.api.get<Project>(`/projects/${id}`);
  }

  create(request: CreateProjectRequest): Observable<Project> {
    return this.api.post<Project>('/projects', request);
  }

  update(id: number, request: UpdateProjectRequest): Observable<Project> {
    return this.api.put<Project>(`/projects/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`/projects/${id}`);
  }

  addMember(projectId: number, userId: number, projectRole: string): Observable<Project> {
    return this.api.post<Project>(`/projects/${projectId}/members`, { userId, projectRole });
  }

  removeMember(projectId: number, userId: number): Observable<void> {
    return this.api.delete<void>(`/projects/${projectId}/members/${userId}`);
  }
}
