import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, delay } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import {
  Expense,
  ExpenseDto,
  CreateExpenseRequest,
  UpdateExpenseRequest,
  ExpenseFilters,
  PagedResult,
  ExpenseStatsDto,
  ExpenseAnalytics
} from '../../shared/models/expense.models';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private readonly apiUrl = `${environment.apiUrl}/expenses`;

  constructor(private http: HttpClient) {}

  getExpenses(skip = 0, take = 50): Observable<PagedResult<ExpenseDto>> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());

    return this.http.get<PagedResult<ExpenseDto>>(this.apiUrl, { params });
  }

  getExpense(id: string): Observable<ExpenseDto> {
    return this.http.get<ExpenseDto>(`${this.apiUrl}/${id}`);
  }

  searchExpenses(filters: ExpenseFilters, skip = 0, take = 50): Observable<PagedResult<ExpenseDto>> {
    let params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());

    if (filters.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }
    if (filters.category) {
      params = params.set('category', filters.category);
    }
    if (filters.minAmount !== undefined) {
      params = params.set('minAmount', filters.minAmount.toString());
    }
    if (filters.maxAmount !== undefined) {
      params = params.set('maxAmount', filters.maxAmount.toString());
    }
    if (filters.startDate) {
      params = params.set('startDate', filters.startDate.toISOString());
    }
    if (filters.endDate) {
      params = params.set('endDate', filters.endDate.toISOString());
    }

    return this.http.get<PagedResult<ExpenseDto>>(`${this.apiUrl}/search`, { params });
  }

  getExpenseStatistics(startDate?: Date, endDate?: Date): Observable<ExpenseStatsDto> {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }

    return this.http.get<ExpenseStatsDto>(`${this.apiUrl}/stats`, { params });
  }

  createExpense(request: CreateExpenseRequest): Observable<ExpenseDto> {
    return this.http.post<ExpenseDto>(this.apiUrl, request);
  }

  updateExpense(id: string, request: UpdateExpenseRequest): Observable<ExpenseDto> {
    return this.http.put<ExpenseDto>(`${this.apiUrl}/${id}`, request);
  }

  deleteExpense(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  submitExpense(id: string): Observable<ExpenseDto> {
    return this.http.post<ExpenseDto>(`${this.apiUrl}/${id}/submit`, {});
  }

  approveExpense(id: string): Observable<ExpenseDto> {
    return this.http.post<ExpenseDto>(`${this.apiUrl}/${id}/approve`, {});
  }

  rejectExpense(id: string, reason: string): Observable<ExpenseDto> {
    return this.http.post<ExpenseDto>(`${this.apiUrl}/${id}/reject`, { reason });
  }

  categorizeExpense(id: string): Observable<ExpenseDto> {
    return this.http.post<ExpenseDto>(`${this.apiUrl}/${id}/categorize`, {});
  }

  uploadReceipt(id: string, file: File): Observable<ExpenseDto> {
    const formData = new FormData();
    formData.append('receipt', file);
    return this.http.post<ExpenseDto>(`${this.apiUrl}/${id}/receipt`, formData);
  }

  exportExpenses(format: 'csv' | 'excel' = 'csv', filters: ExpenseFilters = {}): Observable<Blob> {
    let params = new HttpParams().set('format', format);

    if (filters.category) {
      params = params.set('category', filters.category);
    }
    if (filters.startDate) {
      params = params.set('startDate', filters.startDate.toISOString());
    }
    if (filters.endDate) {
      params = params.set('endDate', filters.endDate.toISOString());
    }

    return this.http.get(`${this.apiUrl}/export`, {
      params,
      responseType: 'blob'
    });
  }

  getExpenseAnalytics(period: 'week' | 'month' | 'quarter' | 'year' = 'month'): Observable<ExpenseAnalytics> {
    const params = new HttpParams().set('period', period);
    return this.http.get<ExpenseAnalytics>(`${this.apiUrl}/analytics`, { params });
  }

  bulkDeleteExpenses(ids: string[]): Observable<void> {
    return this.http.request<void>('delete', this.apiUrl, {
      body: { ids }
    });
  }

  bulkCategorizeExpenses(ids: string[]): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/bulk-categorize`, { ids });
  }

  duplicateExpense(id: string): Observable<ExpenseDto> {
    return this.http.post<ExpenseDto>(`${this.apiUrl}/${id}/duplicate`, {});
  }

  getRecentExpenses(count = 5): Observable<ExpenseDto[]> {
    const params = new HttpParams()
      .set('take', count.toString())
      .set('sortBy', 'createdAt')
      .set('sortDirection', 'desc');

    return this.http.get<PagedResult<ExpenseDto>>(this.apiUrl, { params })
      .pipe(map(result => result.items));
  }

  // Analytics helpers
  getExpensesByCategory(period: 'week' | 'month' | 'quarter' | 'year' = 'month'): Observable<any[]> {
    return this.getExpenseStatistics().pipe(
      map(stats => stats.expensesByCategory)
    );
  }

  getMonthlyTrends(months = 12): Observable<any[]> {
    return this.getExpenseStatistics().pipe(
      map(stats => stats.expensesByMonth.slice(-months))
    );
  }

  getPendingReimbursements(): Observable<ExpenseDto[]> {
    const filters: ExpenseFilters = {
      status: 'Approved' as any
    };
    // Note: Add isReimbursable filter when backend supports it
    return this.searchExpenses(filters).pipe(
      map(result => result.items.filter(expense => expense.isReimbursable && !expense.isReimbursed))
    );
  }
}
