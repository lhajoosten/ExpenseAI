export interface Money {
  amount: number;
  currency: string;
}

export interface Category {
  name: string;
  description?: string;
  color: string;
  icon: string;
  isSystemCategory: boolean;
}

export interface Expense {
  id: string;
  description: string;
  amount: Money;
  category: Category;
  expenseDate: string;
  notes?: string;
  receiptUrl?: string;
  merchantName?: string;
  paymentMethod?: string;
  isReimbursable: boolean;
  isReimbursed: boolean;
  status: ExpenseStatus;
  rejectionReason?: string;
  isAiCategorized: boolean;
  aiConfidenceScore?: number;
  aiExtractedData?: string;
  userId: string;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface ExpenseDto {
  id: string;
  description: string;
  amount: Money;
  category: Category;
  expenseDate: string;
  notes?: string;
  receiptUrl?: string;
  merchantName?: string;
  paymentMethod?: string;
  isReimbursable: boolean;
  isReimbursed: boolean;
  status: ExpenseStatus;
  rejectionReason?: string;
  isAiCategorized: boolean;
  aiConfidenceScore?: number;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateExpenseRequest {
  description: string;
  amount: Money;
  categoryName: string;
  expenseDate: string;
  notes?: string;
  merchantName?: string;
  paymentMethod?: string;
  isReimbursable?: boolean;
  tags?: string[];
}

export interface UpdateExpenseRequest {
  description: string;
  amount: Money;
  categoryName: string;
  expenseDate: string;
  notes?: string;
  merchantName?: string;
  paymentMethod?: string;
  isReimbursable?: boolean;
  tags?: string[];
}

export interface ExpenseFilters {
  searchTerm?: string;
  category?: string;
  minAmount?: number;
  maxAmount?: number;
  startDate?: Date;
  endDate?: Date;
  paymentMethod?: string;
  isReimbursable?: boolean;
  status?: ExpenseStatus;
  tags?: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ExpenseStatsDto {
  totalExpenses: number;
  totalAmount: Money;
  averageExpense: Money;
  expensesByCategory: CategorySummary[];
  expensesByMonth: MonthlySummary[];
  recentExpenses: ExpenseDto[];
  topMerchants: MerchantSummary[];
  expensesByPaymentMethod: PaymentMethodSummary[];
}

export interface CategorySummary {
  category: Category;
  totalAmount: Money;
  count: number;
  percentage: number;
}

export interface MonthlySummary {
  year: number;
  month: number;
  totalAmount: Money;
  count: number;
}

export interface MerchantSummary {
  merchantName: string;
  totalAmount: Money;
  count: number;
}

export interface PaymentMethodSummary {
  paymentMethod: string;
  totalAmount: Money;
  count: number;
  percentage: number;
}

export enum ExpenseStatus {
  Draft = 'Draft',
  Submitted = 'Submitted',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Paid = 'Paid'
}

export interface ExpenseAnalytics {
  totalSpent: Money;
  monthlyAverage: Money;
  largestExpense: ExpenseDto;
  mostFrequentCategory: Category;
  spendingTrend: 'up' | 'down' | 'stable';
  budgetUtilization: number;
  predictedMonthlySpend: Money;
}

// Form models for reactive forms
export interface ExpenseFormData {
  description: string;
  amount: number;
  currency: string;
  categoryName: string;
  expenseDate: Date;
  notes: string;
  merchantName: string;
  paymentMethod: string;
  isReimbursable: boolean;
  tags: string;
}
