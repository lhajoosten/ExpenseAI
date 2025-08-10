import { Money } from './expense.models';

export interface Invoice {
  id: string;
  invoiceNumber: string;
  clientName: string;
  clientEmail: string;
  clientAddress?: string;
  issueDate: string;
  dueDate: string;
  subtotalAmount: Money;
  taxAmount: Money;
  totalAmount: Money;
  taxRate: number;
  notes?: string;
  status: InvoiceStatus;
  paidDate?: string;
  paymentMethod?: string;
  paymentReference?: string;
  userId: string;
  lineItems: InvoiceLineItem[];
  createdAt: string;
  updatedAt: string;
}

export interface InvoiceLineItem {
  id: string;
  description: string;
  quantity: number;
  unitPrice: Money;
  totalPrice: Money;
  notes?: string;
}

export interface CreateInvoiceRequest {
  clientName: string;
  clientEmail: string;
  clientAddress?: string;
  issueDate: string;
  dueDate: string;
  taxRate: number;
  notes?: string;
  lineItems: CreateInvoiceLineItemRequest[];
}

export interface CreateInvoiceLineItemRequest {
  description: string;
  quantity: number;
  unitPrice: Money;
  notes?: string;
}

export interface UpdateInvoiceRequest {
  clientName: string;
  clientEmail: string;
  clientAddress?: string;
  issueDate: string;
  dueDate: string;
  taxRate: number;
  notes?: string;
  lineItems: UpdateInvoiceLineItemRequest[];
}

export interface UpdateInvoiceLineItemRequest {
  id?: string;
  description: string;
  quantity: number;
  unitPrice: Money;
  notes?: string;
}

export interface InvoiceFilters {
  searchTerm?: string;
  status?: InvoiceStatus;
  clientName?: string;
  startDate?: Date;
  endDate?: Date;
  minAmount?: number;
  maxAmount?: number;
}

export interface InvoiceStatsDto {
  totalInvoices: number;
  totalAmount: Money;
  paidAmount: Money;
  pendingAmount: Money;
  overdueAmount: Money;
  averageInvoiceAmount: Money;
  invoicesByStatus: InvoiceStatusSummary[];
  invoicesByMonth: InvoiceMonthSummary[];
  topClients: ClientSummary[];
}

export interface InvoiceStatusSummary {
  status: InvoiceStatus;
  count: number;
  totalAmount: Money;
  percentage: number;
}

export interface InvoiceMonthSummary {
  year: number;
  month: number;
  totalAmount: Money;
  count: number;
  paidAmount: Money;
  pendingAmount: Money;
}

export interface ClientSummary {
  clientName: string;
  totalAmount: Money;
  invoiceCount: number;
  lastInvoiceDate: string;
}

export enum InvoiceStatus {
  Draft = 'Draft',
  Sent = 'Sent',
  Viewed = 'Viewed',
  Paid = 'Paid',
  Overdue = 'Overdue',
  Cancelled = 'Cancelled'
}

// Form models
export interface InvoiceFormData {
  clientName: string;
  clientEmail: string;
  clientAddress: string;
  issueDate: Date;
  dueDate: Date;
  taxRate: number;
  notes: string;
}

export interface InvoiceLineItemFormData {
  description: string;
  quantity: number;
  unitPrice: number;
  currency: string;
  notes: string;
}
