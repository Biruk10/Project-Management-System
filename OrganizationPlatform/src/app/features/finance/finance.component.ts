import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceService } from '../../core/services/finance.service';
import { Expense } from '../../core/models/finance.models';
import { PagedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-finance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1 class="page-title">Finance & Budget</h1>
      </div>

      <div class="tab-bar">
        <button [class.active]="tab === 'expenses'" (click)="tab = 'expenses'">Expenses</button>
        <button [class.active]="tab === 'budgets'" (click)="tab = 'budgets'">Budgets</button>
      </div>

      @if (tab === 'expenses') {
        @if (loading()) {
          <div class="loading">Loading expenses...</div>
        } @else {
          <div class="table-card">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Project</th>
                  <th>Budget Line</th>
                  <th>Amount</th>
                  <th>Description</th>
                  <th>Date</th>
                  <th>Recorded By</th>
                </tr>
              </thead>
              <tbody>
                @for (e of expenses()?.items; track e.id) {
                  <tr>
                    <td>{{ e.projectName }}</td>
                    <td>{{ e.budgetLineCategory ?? '—' }}</td>
                    <td class="amount">{{ e.amount | number:'1.2-2' }}</td>
                    <td>{{ e.description }}</td>
                    <td>{{ e.expenseDate | date:'mediumDate' }}</td>
                    <td>{{ e.recordedByUserName ?? '—' }}</td>
                  </tr>
                }
                @if (!expenses()?.items?.length) {
                  <tr>
                    <td colspan="6" class="empty-cell">No expenses recorded.</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      }

      @if (tab === 'budgets') {
        <div class="empty-state">Select a project from the Projects page to view its budget details.</div>
      }
    </div>
  `,
  styles: [`
    .page { max-width: 1200px; margin: 0 auto; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .page-title { font-size: 24px; font-weight: 700; color: #111827; margin: 0; }
    .tab-bar { display: flex; gap: 4px; margin-bottom: 20px; border-bottom: 2px solid #e5e7eb; }
    .tab-bar button { padding: 10px 20px; border: none; background: none; cursor: pointer; font-size: 14px; color: #6b7280; font-weight: 500; }
    .tab-bar button.active { color: #3b82f6; border-bottom: 2px solid #3b82f6; margin-bottom: -2px; }
    .loading { text-align: center; padding: 40px; color: #6b7280; }
    .table-card { background: #fff; border-radius: 10px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); overflow: hidden; }
    .data-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .data-table th { background: #f9fafb; padding: 12px 16px; text-align: left; font-size: 12px; font-weight: 600; color: #6b7280; text-transform: uppercase; }
    .data-table td { padding: 14px 16px; border-top: 1px solid #f3f4f6; color: #374151; }
    .amount { font-weight: 600; color: #111827; }
    .empty-cell { text-align: center; color: #9ca3af; }
    .empty-state { text-align: center; padding: 60px; color: #9ca3af; font-size: 14px; }
  `]
})
export class FinanceComponent implements OnInit {
  expenses = signal<PagedResult<Expense> | null>(null);
  loading = signal(true);
  tab = 'expenses';

  constructor(private financeService: FinanceService) {}

  ngOnInit(): void {
    this.financeService.getExpenses({ pageSize: 50 }).subscribe({
      next: (data) => { this.expenses.set(data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }
}
