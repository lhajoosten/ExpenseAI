import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface AnalyticsFilter {
  startDate?: Date;
  endDate?: Date;
  categories?: string[];
  tags?: string[];
  minAmount?: number;
  maxAmount?: number;
  paymentMethods?: string[];
}

export interface ExpenseAnalytics {
  totalAmount: number;
  transactionCount: number;
  averageAmount: number;
  periodComparison: {
    previousPeriod: number;
    percentageChange: number;
    trend: 'increasing' | 'decreasing' | 'stable';
  };
  categoryBreakdown: Array<{
    category: string;
    amount: number;
    percentage: number;
    transactionCount: number;
  }>;
  dailyTrends: Array<{
    date: Date;
    amount: number;
    transactionCount: number;
  }>;
  topExpenses: Array<{
    id: string;
    description: string;
    amount: number;
    category: string;
    date: Date;
  }>;
}

export interface BudgetAnalysis {
  budgets: Array<{
    category: string;
    budgeted: number;
    spent: number;
    remaining: number;
    percentage: number;
    status: 'under' | 'over' | 'warning';
    trend: number[];
  }>;
  totalBudgeted: number;
  totalSpent: number;
  projectedMonthEnd: number;
  recommendations: string[];
}

export interface TrendData {
  period: string;
  data: Array<{
    date: Date;
    amount: number;
    count: number;
    categories: Record<string, number>;
  }>;
  forecast: Array<{
    date: Date;
    predicted: number;
    confidence: number;
  }>;
  seasonality: {
    pattern: 'weekly' | 'monthly' | 'yearly' | 'none';
    strength: number;
    peakPeriods: string[];
  };
}

export interface ComparisonData {
  current: {
    period: string;
    amount: number;
    transactions: number;
  };
  previous: {
    period: string;
    amount: number;
    transactions: number;
  };
  change: {
    amount: number;
    percentage: number;
    transactions: number;
  };
  breakdown: Array<{
    category: string;
    current: number;
    previous: number;
    change: number;
    changePercentage: number;
  }>;
}

export interface CashFlowData {
  periods: Array<{
    period: string;
    income: number;
    expenses: number;
    netFlow: number;
    cumulativeFlow: number;
  }>;
  projections: Array<{
    period: string;
    projectedIncome: number;
    projectedExpenses: number;
    projectedNet: number;
  }>;
  insights: {
    averageMonthlyNet: number;
    volatility: number;
    trend: 'improving' | 'declining' | 'stable';
    recommendations: string[];
  };
}

export interface PaymentMethodAnalysis {
  methods: Array<{
    method: string;
    amount: number;
    percentage: number;
    transactionCount: number;
    averageTransaction: number;
    categories: Record<string, number>;
  }>;
  insights: {
    mostUsed: string;
    highestSpending: string;
    recommendations: string[];
  };
}

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private readonly apiUrl = `${environment.apiUrl}/analytics`;

  constructor(private http: HttpClient) {}

  getExpenseAnalytics(filter: AnalyticsFilter): Observable<ExpenseAnalytics> {
    const params = this.buildHttpParams(filter);
    return this.http.get<ExpenseAnalytics>(`${this.apiUrl}/expenses`, { params });
  }

  getBudgetAnalysis(period: 'month' | 'quarter' | 'year' = 'month'): Observable<BudgetAnalysis> {
    return this.http.get<BudgetAnalysis>(`${this.apiUrl}/budget`, {
      params: { period }
    });
  }

  getTrendAnalysis(
    period: 'daily' | 'weekly' | 'monthly' | 'yearly',
    timeRange: number = 12
  ): Observable<TrendData> {
    return this.http.get<TrendData>(`${this.apiUrl}/trends`, {
      params: { period, timeRange: timeRange.toString() }
    });
  }

  getComparisonData(
    currentPeriod: string,
    previousPeriod: string,
    filter?: AnalyticsFilter
  ): Observable<ComparisonData> {
    let params = new HttpParams()
      .set('currentPeriod', currentPeriod)
      .set('previousPeriod', previousPeriod);

    if (filter) {
      params = this.buildHttpParams(filter, params);
    }

    return this.http.get<ComparisonData>(`${this.apiUrl}/comparison`, { params });
  }

  getCashFlowAnalysis(months: number = 12): Observable<CashFlowData> {
    return this.http.get<CashFlowData>(`${this.apiUrl}/cash-flow`, {
      params: { months: months.toString() }
    });
  }

  getPaymentMethodAnalysis(filter: AnalyticsFilter): Observable<PaymentMethodAnalysis> {
    const params = this.buildHttpParams(filter);
    return this.http.get<PaymentMethodAnalysis>(`${this.apiUrl}/payment-methods`, { params });
  }

  getCategoryDistribution(filter: AnalyticsFilter): Observable<Array<{
    category: string;
    amount: number;
    percentage: number;
    color: string;
  }>> {
    const params = this.buildHttpParams(filter);
    return this.http.get<Array<{
      category: string;
      amount: number;
      percentage: number;
      color: string;
    }>>(`${this.apiUrl}/category-distribution`, { params });
  }

  getTopMerchants(filter: AnalyticsFilter, limit: number = 10): Observable<Array<{
    merchant: string;
    amount: number;
    transactionCount: number;
    averageAmount: number;
    lastTransaction: Date;
  }>> {
    const params = this.buildHttpParams(filter)
      .set('limit', limit.toString());

    return this.http.get<Array<{
      merchant: string;
      amount: number;
      transactionCount: number;
      averageAmount: number;
      lastTransaction: Date;
    }>>(`${this.apiUrl}/top-merchants`, { params });
  }

  getRecurringExpenses(filter: AnalyticsFilter): Observable<Array<{
    pattern: string;
    amount: number;
    frequency: 'daily' | 'weekly' | 'monthly' | 'yearly';
    nextExpected: Date;
    confidence: number;
    expenses: Array<{
      id: string;
      description: string;
      amount: number;
      date: Date;
    }>;
  }>> {
    const params = this.buildHttpParams(filter);
    return this.http.get<Array<{
      pattern: string;
      amount: number;
      frequency: 'daily' | 'weekly' | 'monthly' | 'yearly';
      nextExpected: Date;
      confidence: number;
      expenses: Array<{
        id: string;
        description: string;
        amount: number;
        date: Date;
      }>;
    }>>(`${this.apiUrl}/recurring`, { params });
  }

  getAnomalies(filter: AnalyticsFilter): Observable<Array<{
    type: 'amount' | 'frequency' | 'location' | 'timing';
    description: string;
    severity: 'low' | 'medium' | 'high';
    expense: {
      id: string;
      description: string;
      amount: number;
      date: Date;
      category: string;
    };
    reasoning: string;
    suggestion?: string;
  }>> {
    const params = this.buildHttpParams(filter);
    return this.http.get<Array<{
      type: 'amount' | 'frequency' | 'location' | 'timing';
      description: string;
      severity: 'low' | 'medium' | 'high';
      expense: {
        id: string;
        description: string;
        amount: number;
        date: Date;
        category: string;
      };
      reasoning: string;
      suggestion?: string;
    }>>(`${this.apiUrl}/anomalies`, { params });
  }

  exportAnalytics(
    type: 'expenses' | 'budget' | 'trends' | 'comparison',
    format: 'csv' | 'excel' | 'pdf',
    filter: AnalyticsFilter
  ): Observable<Blob> {
    const params = this.buildHttpParams(filter)
      .set('type', type)
      .set('format', format);

    return this.http.get(`${this.apiUrl}/export`, {
      params,
      responseType: 'blob'
    });
  }

  getInsights(filter: AnalyticsFilter): Observable<Array<{
    type: 'tip' | 'warning' | 'opportunity' | 'achievement';
    title: string;
    description: string;
    value?: number;
    action?: string;
    priority: 'low' | 'medium' | 'high';
  }>> {
    const params = this.buildHttpParams(filter);
    return this.http.get<Array<{
      type: 'tip' | 'warning' | 'opportunity' | 'achievement';
      title: string;
      description: string;
      value?: number;
      action?: string;
      priority: 'low' | 'medium' | 'high';
    }>>(`${this.apiUrl}/insights`, { params });
  }

  private buildHttpParams(filter: AnalyticsFilter, existingParams?: HttpParams): HttpParams {
    let params = existingParams || new HttpParams();

    if (filter.startDate) {
      params = params.set('startDate', filter.startDate.toISOString());
    }
    if (filter.endDate) {
      params = params.set('endDate', filter.endDate.toISOString());
    }
    if (filter.categories && filter.categories.length > 0) {
      params = params.set('categories', filter.categories.join(','));
    }
    if (filter.tags && filter.tags.length > 0) {
      params = params.set('tags', filter.tags.join(','));
    }
    if (filter.minAmount !== undefined) {
      params = params.set('minAmount', filter.minAmount.toString());
    }
    if (filter.maxAmount !== undefined) {
      params = params.set('maxAmount', filter.maxAmount.toString());
    }
    if (filter.paymentMethods && filter.paymentMethods.length > 0) {
      params = params.set('paymentMethods', filter.paymentMethods.join(','));
    }

    return params;
  }
}
