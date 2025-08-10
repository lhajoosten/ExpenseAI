import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { Observable, BehaviorSubject, combineLatest } from 'rxjs';
import { map, startWith, switchMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

import { ExpenseService } from '../../core/services/expense.service';
import { CategoryService } from '../../core/services/category.service';

interface DashboardStats {
  totalExpenses: number;
  totalAmount: number;
  averageExpense: number;
  monthlyGrowth: number;
  topCategories: Array<{ category: string; amount: number; percentage: number }>;
  recentExpenses: Array<{ description: string; amount: number; date: Date; category: string }>;
  monthlyTrends: Array<{ month: string; amount: number }>;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatGridListModule,
    MatProgressSpinnerModule,
    MatListModule,
    ReactiveFormsModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private expenseService = inject(ExpenseService);
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  periodControl = this.fb.control('month');
  loading$ = new BehaviorSubject<boolean>(false);

  private refreshSubject = new BehaviorSubject<void>(undefined);

  stats$ = combineLatest([
    this.refreshSubject,
    this.periodControl.valueChanges.pipe(startWith(this.periodControl.value))
  ]).pipe(
    switchMap(([_, period]) => {
      this.loading$.next(true);
      // For now, use mock data directly since backend isn't ready
      return of(this.getMockStats()).pipe(
        map(stats => {
          this.loading$.next(false);
          return stats;
        })
      );

      // TODO: Enable this when backend statistics endpoint is ready
      // return this.expenseService.getExpenseStatistics(period as any).pipe(
      //   map((data: any) => this.transformStatsData(data)),
      //   catchError(() => of(this.getMockStats())),
      //   map(stats => {
      //     this.loading$.next(false);
      //     return stats;
      //   })
      // );
    })
  );

  ngOnInit() {
    this.refreshData();
  }

  onPeriodChange() {
    this.refreshData();
  }

  refreshData() {
    this.refreshSubject.next();
  }

  getChangeClass(growth: number): string {
    if (growth > 0) return 'positive';
    if (growth < 0) return 'negative';
    return '';
  }

  formatGrowth(growth: number): string {
    const sign = growth >= 0 ? '+' : '';
    return `${sign}${growth.toFixed(1)}%`;
  }

  exportReport() {
    console.log('Exporting dashboard report...');
  }

  private transformStatsData(apiData: any): DashboardStats {
    return {
      totalExpenses: apiData.totalExpenses || 0,
      totalAmount: apiData.totalAmount || 0,
      averageExpense: apiData.averageExpense || 0,
      monthlyGrowth: apiData.monthlyGrowth || 0,
      topCategories: apiData.topCategories || [],
      recentExpenses: apiData.recentExpenses || [],
      monthlyTrends: apiData.monthlyTrends || []
    };
  }

  private getMockStats(): DashboardStats {
    return {
      totalExpenses: 156,
      totalAmount: 12450.75,
      averageExpense: 79.81,
      monthlyGrowth: 8.2,
      topCategories: [
        { category: 'Food & Dining', amount: 3200, percentage: 25.7 },
        { category: 'Transportation', amount: 2100, percentage: 16.9 },
        { category: 'Entertainment', amount: 1800, percentage: 14.5 },
        { category: 'Shopping', amount: 1500, percentage: 12.1 },
        { category: 'Utilities', amount: 1200, percentage: 9.6 }
      ],
      recentExpenses: [
        { description: 'Coffee at Starbucks', amount: 5.75, date: new Date(), category: 'Food & Dining' },
        { description: 'Uber ride to office', amount: 12.50, date: new Date(Date.now() - 86400000), category: 'Transportation' },
        { description: 'Netflix subscription', amount: 15.99, date: new Date(Date.now() - 172800000), category: 'Entertainment' },
        { description: 'Grocery shopping', amount: 89.34, date: new Date(Date.now() - 259200000), category: 'Food & Dining' },
        { description: 'Gas station', amount: 45.00, date: new Date(Date.now() - 345600000), category: 'Transportation' }
      ],
      monthlyTrends: [
        { month: 'Jan', amount: 2800 },
        { month: 'Feb', amount: 3200 },
        { month: 'Mar', amount: 2900 },
        { month: 'Apr', amount: 3500 },
        { month: 'May', amount: 3100 },
        { month: 'Jun', amount: 3800 }
      ]
    };
  }
}
