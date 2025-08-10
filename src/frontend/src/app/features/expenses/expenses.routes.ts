import { Routes } from '@angular/router';

export const expensesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./expenses.component').then(m => m.ExpensesComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./expense-form/expense-form.component').then(m => m.ExpenseFormComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./expense-detail/expense-detail.component').then(m => m.ExpenseDetailComponent)
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./expense-form/expense-form.component').then(m => m.ExpenseFormComponent)
  }
];
