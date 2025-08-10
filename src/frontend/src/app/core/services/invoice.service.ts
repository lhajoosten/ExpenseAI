import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import {
  Invoice,
  CreateInvoiceRequest,
  UpdateInvoiceRequest,
  InvoiceFilters,
  InvoiceStatsDto
} from '../../shared/models/invoice.models';
import { PagedResult } from '../../shared/models/expense.models';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly apiUrl = `${environment.apiUrl}/invoices`;

  constructor(private http: HttpClient) {}

  getInvoices(skip = 0, take = 50): Observable<PagedResult<Invoice>> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());

    return this.http.get<PagedResult<Invoice>>(this.apiUrl, { params });
  }

  getInvoice(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/${id}`);
  }

  searchInvoices(filters: InvoiceFilters, skip = 0, take = 50): Observable<PagedResult<Invoice>> {
    let params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());

    if (filters.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }
    if (filters.status) {
      params = params.set('status', filters.status);
    }
    if (filters.clientName) {
      params = params.set('clientName', filters.clientName);
    }
    if (filters.startDate) {
      params = params.set('startDate', filters.startDate.toISOString());
    }
    if (filters.endDate) {
      params = params.set('endDate', filters.endDate.toISOString());
    }
    if (filters.minAmount !== undefined) {
      params = params.set('minAmount', filters.minAmount.toString());
    }
    if (filters.maxAmount !== undefined) {
      params = params.set('maxAmount', filters.maxAmount.toString());
    }

    return this.http.get<PagedResult<Invoice>>(`${this.apiUrl}/search`, { params });
  }

  createInvoice(request: CreateInvoiceRequest): Observable<Invoice> {
    return this.http.post<Invoice>(this.apiUrl, request);
  }

  updateInvoice(id: string, request: UpdateInvoiceRequest): Observable<Invoice> {
    return this.http.put<Invoice>(`${this.apiUrl}/${id}`, request);
  }

  deleteInvoice(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  sendInvoice(id: string): Observable<Invoice> {
    return this.http.post<Invoice>(`${this.apiUrl}/${id}/send`, {});
  }

  markAsPaid(id: string, paymentDetails: { paidDate: string; paymentMethod?: string; paymentReference?: string }): Observable<Invoice> {
    return this.http.post<Invoice>(`${this.apiUrl}/${id}/mark-paid`, paymentDetails);
  }

  cancelInvoice(id: string, reason?: string): Observable<Invoice> {
    return this.http.post<Invoice>(`${this.apiUrl}/${id}/cancel`, { reason });
  }

  duplicateInvoice(id: string): Observable<Invoice> {
    return this.http.post<Invoice>(`${this.apiUrl}/${id}/duplicate`, {});
  }

  getInvoiceStatistics(startDate?: Date, endDate?: Date): Observable<InvoiceStatsDto> {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }

    return this.http.get<InvoiceStatsDto>(`${this.apiUrl}/stats`, { params });
  }

  exportInvoices(format: 'csv' | 'excel' = 'csv', filters: InvoiceFilters = {}): Observable<Blob> {
    let params = new HttpParams().set('format', format);

    if (filters.status) {
      params = params.set('status', filters.status);
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

  generatePDF(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/pdf`, {
      responseType: 'blob'
    });
  }

  getRecentInvoices(count = 5): Observable<Invoice[]> {
    const params = new HttpParams()
      .set('take', count.toString())
      .set('sortBy', 'createdAt')
      .set('sortDirection', 'desc');

    return this.http.get<PagedResult<Invoice>>(this.apiUrl, { params })
      .pipe(map(result => result.items));
  }

  getOverdueInvoices(): Observable<Invoice[]> {
    const filters: InvoiceFilters = {
      status: 'Overdue' as any
    };
    return this.searchInvoices(filters).pipe(
      map(result => result.items)
    );
  }

  getPendingInvoices(): Observable<Invoice[]> {
    const filters: InvoiceFilters = {
      status: 'Sent' as any
    };
    return this.searchInvoices(filters).pipe(
      map(result => result.items)
    );
  }

  // Helper methods
  calculateTotal(subtotal: number, taxRate: number): number {
    const taxAmount = subtotal * (taxRate / 100);
    return subtotal + taxAmount;
  }

  calculateTaxAmount(subtotal: number, taxRate: number): number {
    return subtotal * (taxRate / 100);
  }

  generateInvoiceNumber(): string {
    const date = new Date();
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0');

    return `INV-${year}${month}${day}-${random}`;
  }

  isOverdue(invoice: Invoice): boolean {
    const today = new Date();
    const dueDate = new Date(invoice.dueDate);
    return dueDate < today && invoice.status !== 'Paid' && invoice.status !== 'Cancelled';
  }

  getDaysOverdue(invoice: Invoice): number {
    if (!this.isOverdue(invoice)) return 0;

    const today = new Date();
    const dueDate = new Date(invoice.dueDate);
    const diffTime = today.getTime() - dueDate.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
}
