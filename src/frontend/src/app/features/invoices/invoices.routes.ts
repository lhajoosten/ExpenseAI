import { Routes } from '@angular/router';

export const invoicesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./invoices.component').then(m => m.InvoicesComponent)
  }
];
