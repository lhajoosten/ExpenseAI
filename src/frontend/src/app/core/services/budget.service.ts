import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { PagedResult } from '../../shared/models/expense.models';

export interface Budget {
  id: string;
  name: string;
  description?: string;
  category: string;
  amount: number;
  spent: number;
  remaining: number;
  percentage: number;
  period: 'weekly' | 'monthly' | 'quarterly' | 'yearly';
  startDate: Date;
  endDate: Date;
  alertThreshold: number; // percentage
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  tags?: string[];
  color?: string;
  icon?: string;
}

export interface CreateBudgetRequest {
  name: string;
  description?: string;
  category: string;
  amount: number;
  period: 'weekly' | 'monthly' | 'quarterly' | 'yearly';
  startDate: Date;
  alertThreshold?: number;
  tags?: string[];
  color?: string;
  icon?: string;
}

export interface UpdateBudgetRequest {
  name?: string;
  description?: string;
  amount?: number;
  alertThreshold?: number;
  isActive?: boolean;
  tags?: string[];
  color?: string;
  icon?: string;
}

export interface BudgetAlert {
  id: string;
  budgetId: string;
  budgetName: string;
  category: string;
  type: 'threshold' | 'exceeded' | 'depleted';
  message: string;
  percentage: number;
  amount: number;
  createdAt: Date;
  acknowledged: boolean;
}

export interface BudgetSummary {
  totalBudgets: number;
  activeBudgets: number;
  totalBudgeted: number;
  totalSpent: number;
  totalRemaining: number;
  overBudgetCount: number;
  nearLimitCount: number;
  onTrackCount: number;
  monthlyAverage: number;
  projectedEndOfMonth: number;
}

export interface BudgetPerformance {
  budgetId: string;
  budgetName: string;
  category: string;
  performance: Array<{
    period: string;
    budgeted: number;
    spent: number;
    percentage: number;
    variance: number;
  }>;
  trend: 'improving' | 'declining' | 'stable';
  averageUtilization: number;
  recommendations: string[];
}

export interface BudgetTemplate {
  id: string;
  name: string;
  description: string;
  categories: Array<{
    category: string;
    percentage: number;
    amount?: number;
  }>;
  period: 'monthly' | 'yearly';
  isDefault: boolean;
  tags: string[];
}

export interface BudgetGoal {
  id: string;
  name: string;
  targetAmount: number;
  currentAmount: number;
  targetDate: Date;
  category?: string;
  description?: string;
  priority: 'low' | 'medium' | 'high';
  isAchieved: boolean;
  milestones: Array<{
    amount: number;
    date: Date;
    achieved: boolean;
  }>;
}

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private readonly apiUrl = `${environment.apiUrl}/budgets`;

  constructor(private http: HttpClient) {}

  // Budget CRUD Operations
  getBudgets(
    page: number = 1,
    pageSize: number = 20,
    search?: string,
    category?: string,
    status?: 'active' | 'inactive' | 'over' | 'near-limit',
    sortBy?: string,
    sortDesc?: boolean
  ): Observable<PagedResult<Budget>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) params = params.set('search', search);
    if (category) params = params.set('category', category);
    if (status) params = params.set('status', status);
    if (sortBy) params = params.set('sortBy', sortBy);
    if (sortDesc !== undefined) params = params.set('sortDesc', sortDesc.toString());

    return this.http.get<PagedResult<Budget>>(`${this.apiUrl}`, { params });
  }

  getBudgetById(id: string): Observable<Budget> {
    return this.http.get<Budget>(`${this.apiUrl}/${id}`);
  }

  createBudget(request: CreateBudgetRequest): Observable<Budget> {
    return this.http.post<Budget>(`${this.apiUrl}`, request);
  }

  updateBudget(id: string, request: UpdateBudgetRequest): Observable<Budget> {
    return this.http.put<Budget>(`${this.apiUrl}/${id}`, request);
  }

  deleteBudget(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  duplicateBudget(id: string, name: string): Observable<Budget> {
    return this.http.post<Budget>(`${this.apiUrl}/${id}/duplicate`, { name });
  }

  // Budget Analysis
  getBudgetSummary(period?: 'current' | 'month' | 'quarter' | 'year'): Observable<BudgetSummary> {
    const params = period ? new HttpParams().set('period', period) : undefined;
    return this.http.get<BudgetSummary>(`${this.apiUrl}/summary`, { params });
  }

  getBudgetPerformance(
    budgetId?: string,
    months: number = 12
  ): Observable<BudgetPerformance[]> {
    let params = new HttpParams().set('months', months.toString());
    if (budgetId) params = params.set('budgetId', budgetId);

    return this.http.get<BudgetPerformance[]>(`${this.apiUrl}/performance`, { params });
  }

  getBudgetUtilization(
    period: 'week' | 'month' | 'quarter' | 'year' = 'month'
  ): Observable<Array<{
    category: string;
    budgeted: number;
    spent: number;
    remaining: number;
    percentage: number;
    status: 'under' | 'over' | 'warning';
    trend: number[];
  }>> {
    return this.http.get<Array<{
      category: string;
      budgeted: number;
      spent: number;
      remaining: number;
      percentage: number;
      status: 'under' | 'over' | 'warning';
      trend: number[];
    }>>(`${this.apiUrl}/utilization`, {
      params: { period }
    });
  }

  // Budget Alerts
  getBudgetAlerts(
    page: number = 1,
    pageSize: number = 20,
    unacknowledgedOnly: boolean = false
  ): Observable<PagedResult<BudgetAlert>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('unacknowledgedOnly', unacknowledgedOnly.toString());

    return this.http.get<PagedResult<BudgetAlert>>(`${this.apiUrl}/alerts`, { params });
  }

  acknowledgeBudgetAlert(alertId: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/alerts/${alertId}/acknowledge`, {});
  }

  dismissBudgetAlert(alertId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/alerts/${alertId}`);
  }

  // Budget Templates
  getBudgetTemplates(): Observable<BudgetTemplate[]> {
    return this.http.get<BudgetTemplate[]>(`${this.apiUrl}/templates`);
  }

  createBudgetFromTemplate(
    templateId: string,
    totalAmount: number,
    startDate: Date
  ): Observable<Budget[]> {
    return this.http.post<Budget[]>(`${this.apiUrl}/templates/${templateId}/apply`, {
      totalAmount,
      startDate
    });
  }

  saveBudgetAsTemplate(
    budgetIds: string[],
    templateName: string,
    description: string
  ): Observable<BudgetTemplate> {
    return this.http.post<BudgetTemplate>(`${this.apiUrl}/templates`, {
      budgetIds,
      name: templateName,
      description
    });
  }

  // Budget Goals
  getBudgetGoals(): Observable<BudgetGoal[]> {
    return this.http.get<BudgetGoal[]>(`${this.apiUrl}/goals`);
  }

  createBudgetGoal(goal: Omit<BudgetGoal, 'id' | 'currentAmount' | 'isAchieved'>): Observable<BudgetGoal> {
    return this.http.post<BudgetGoal>(`${this.apiUrl}/goals`, goal);
  }

  updateBudgetGoal(id: string, goal: Partial<BudgetGoal>): Observable<BudgetGoal> {
    return this.http.put<BudgetGoal>(`${this.apiUrl}/goals/${id}`, goal);
  }

  deleteBudgetGoal(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/goals/${id}`);
  }

  // Budget Forecasting
  getBudgetForecast(
    budgetId: string,
    months: number = 3
  ): Observable<{
    projections: Array<{
      month: string;
      projected: number;
      confidence: number;
      factors: string[];
    }>;
    recommendations: string[];
    riskLevel: 'low' | 'medium' | 'high';
  }> {
    return this.http.get<{
      projections: Array<{
        month: string;
        projected: number;
        confidence: number;
        factors: string[];
      }>;
      recommendations: string[];
      riskLevel: 'low' | 'medium' | 'high';
    }>(`${this.apiUrl}/${budgetId}/forecast`, {
      params: { months: months.toString() }
    });
  }

  getRecommendedBudgets(
    totalIncome: number,
    existingBudgets?: string[]
  ): Observable<Array<{
    category: string;
    recommendedAmount: number;
    percentage: number;
    priority: 'essential' | 'important' | 'optional';
    reasoning: string;
  }>> {
    const body: any = { totalIncome };
    if (existingBudgets) body.existingBudgets = existingBudgets;

    return this.http.post<Array<{
      category: string;
      recommendedAmount: number;
      percentage: number;
      priority: 'essential' | 'important' | 'optional';
      reasoning: string;
    }>>(`${this.apiUrl}/recommendations`, body);
  }

  // Budget Import/Export
  exportBudgets(budgetIds?: string[], format: 'csv' | 'excel' | 'json' = 'csv'): Observable<Blob> {
    let params = new HttpParams().set('format', format);
    if (budgetIds && budgetIds.length > 0) {
      params = params.set('budgetIds', budgetIds.join(','));
    }

    return this.http.get(`${this.apiUrl}/export`, {
      params,
      responseType: 'blob'
    });
  }

  importBudgets(file: File): Observable<{
    imported: number;
    failed: number;
    errors: string[];
    budgets: Budget[];
  }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{
      imported: number;
      failed: number;
      errors: string[];
      budgets: Budget[];
    }>(`${this.apiUrl}/import`, formData);
  }

  // Utility Methods
  calculateBudgetHealth(budget: Budget): {
    status: 'healthy' | 'warning' | 'over' | 'depleted';
    color: string;
    message: string;
  } {
    const percentage = budget.percentage;

    if (percentage >= 100) {
      return {
        status: budget.remaining < 0 ? 'depleted' : 'over',
        color: '#f44336',
        message: budget.remaining < 0 ? 'Budget depleted' : 'Budget exceeded'
      };
    } else if (percentage >= budget.alertThreshold) {
      return {
        status: 'warning',
        color: '#ff9800',
        message: `${percentage.toFixed(1)}% of budget used`
      };
    } else {
      return {
        status: 'healthy',
        color: '#4caf50',
        message: `${percentage.toFixed(1)}% of budget used`
      };
    }
  }

  getRemainingDays(budget: Budget): number {
    const now = new Date();
    const endDate = new Date(budget.endDate);
    const diffTime = endDate.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  getDailyBudget(budget: Budget): number {
    const remainingDays = this.getRemainingDays(budget);
    return remainingDays > 0 ? budget.remaining / remainingDays : 0;
  }
}
