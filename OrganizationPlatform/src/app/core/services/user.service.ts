import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.models';
import { User, CreateUserRequest, UpdateUserRequest, UserFilterParams } from '../models/user.models';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private api: ApiService) {}

  getAll(filters?: UserFilterParams): Observable<PagedResult<User>> {
    return this.api.get<PagedResult<User>>('/users', filters as Record<string, string | number | boolean | undefined>);
  }

  getById(id: number): Observable<User> {
    return this.api.get<User>(`/users/${id}`);
  }

  create(request: CreateUserRequest): Observable<User> {
    return this.api.post<User>('/users', request);
  }

  update(id: number, request: UpdateUserRequest): Observable<User> {
    return this.api.put<User>(`/users/${id}`, request);
  }

  activate(id: number): Observable<void> {
    return this.api.post<void>(`/users/${id}/activate`, {});
  }

  deactivate(id: number): Observable<void> {
    return this.api.post<void>(`/users/${id}/deactivate`, {});
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`/users/${id}`);
  }

  assignRoles(id: number, roleIds: number[]): Observable<void> {
    return this.api.put<void>(`/users/${id}/roles`, { roleIds });
  }
}
