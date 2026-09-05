import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.models';
import { Budget, Expense, CreateBudgetRequest, CreateExpenseRequest, ExpenseFilterParams } from '../models/finance.models';

@Injectable({ providedIn: 'root' })
export class FinanceService {
  constructor(private api: ApiService) {}

  getBudgetsByProject(projectId: number): Observable<Budget[]> {
    return this.api.get<Budget[]>(`/budgets/project/${projectId}`);
  }

  getBudgetById(id: number): Observable<Budget> {
    return this.api.get<Budget>(`/budgets/${id}`);
  }

  createBudget(request: CreateBudgetRequest): Observable<Budget> {
    return this.api.post<Budget>('/budgets', request);
  }

  approveBudget(id: number): Observable<Budget> {
    return this.api.post<Budget>(`/budgets/${id}/approve`, {});
  }

  rejectBudget(id: number): Observable<Budget> {
    return this.api.post<Budget>(`/budgets/${id}/reject`, {});
  }

  getExpenses(filters?: ExpenseFilterParams): Observable<PagedResult<Expense>> {
    return this.api.get<PagedResult<Expense>>('/expenses', filters as Record<string, string | number | boolean | undefined>);
  }

  createExpense(request: CreateExpenseRequest): Observable<Expense> {
    return this.api.post<Expense>('/expenses', request);
  }

  deleteExpense(id: number): Observable<void> {
    return this.api.delete<void>(`/expenses/${id}`);
  }
}
