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
  templateUrl: './finance.component.html',
  styleUrl: './finance.component.scss'
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
