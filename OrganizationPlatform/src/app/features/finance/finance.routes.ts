import { Routes } from '@angular/router';

export const financeRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./finance.component').then(m => m.FinanceComponent)
  }
];
