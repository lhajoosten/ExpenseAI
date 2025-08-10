import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Observable, startWith, map } from 'rxjs';
import { inject } from '@angular/core';

import { CategoryService } from '../../core/services/category.service';
import { Category, Money, ExpenseFormData } from '../models/expense.models';

@Component({
  selector: 'app-expense-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatChipsModule,
    MatIconModule,
    MatAutocompleteModule
  ],
  template: `
    <h1 mat-dialog-title>{{ data?.expense ? 'Edit Expense' : 'Add New Expense' }}</h1>

    <div mat-dialog-content>
      <form [formGroup]="expenseForm" class="expense-form">
        <!-- Description -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Description</mat-label>
          <input matInput formControlName="description" placeholder="Enter expense description">
          <mat-error *ngIf="expenseForm.get('description')?.hasError('required')">
            Description is required
          </mat-error>
        </mat-form-field>

        <!-- Amount and Currency -->
        <div class="amount-row">
          <mat-form-field appearance="outline" class="amount-field">
            <mat-label>Amount</mat-label>
            <input matInput type="number" formControlName="amount" placeholder="0.00" step="0.01">
            <mat-error *ngIf="expenseForm.get('amount')?.hasError('required')">
              Amount is required
            </mat-error>
            <mat-error *ngIf="expenseForm.get('amount')?.hasError('min')">
              Amount must be greater than 0
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="currency-field">
            <mat-label>Currency</mat-label>
            <mat-select formControlName="currency">
              <mat-option value="USD">USD</mat-option>
              <mat-option value="EUR">EUR</mat-option>
              <mat-option value="GBP">GBP</mat-option>
              <mat-option value="CAD">CAD</mat-option>
              <mat-option value="AUD">AUD</mat-option>
            </mat-select>
          </mat-form-field>
        </div>

        <!-- Category -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Category</mat-label>
          <mat-select formControlName="categoryName">
            <mat-option *ngFor="let category of categories$ | async" [value]="category.name">
              <mat-icon [style.color]="category.color">{{ category.icon }}</mat-icon>
              {{ category.name }}
            </mat-option>
          </mat-select>
          <mat-error *ngIf="expenseForm.get('categoryName')?.hasError('required')">
            Category is required
          </mat-error>
        </mat-form-field>

        <!-- Date -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Expense Date</mat-label>
          <input matInput [matDatepicker]="picker" formControlName="expenseDate">
          <mat-hint>MM/DD/YYYY</mat-hint>
          <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
          <mat-datepicker #picker></mat-datepicker>
          <mat-error *ngIf="expenseForm.get('expenseDate')?.hasError('required')">
            Expense date is required
          </mat-error>
        </mat-form-field>

        <!-- Merchant -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Merchant/Vendor</mat-label>
          <input matInput formControlName="merchantName" placeholder="Where was this expense incurred?">
        </mat-form-field>

        <!-- Payment Method -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Payment Method</mat-label>
          <mat-select formControlName="paymentMethod">
            <mat-option value="">None</mat-option>
            <mat-option value="Cash">Cash</mat-option>
            <mat-option value="Credit Card">Credit Card</mat-option>
            <mat-option value="Debit Card">Debit Card</mat-option>
            <mat-option value="Bank Transfer">Bank Transfer</mat-option>
            <mat-option value="Check">Check</mat-option>
            <mat-option value="Corporate Card">Corporate Card</mat-option>
            <mat-option value="Personal Card">Personal Card</mat-option>
          </mat-select>
        </mat-form-field>

        <!-- Reimbursable -->
        <mat-checkbox formControlName="isReimbursable" class="full-width">
          This is a reimbursable business expense
        </mat-checkbox>

        <!-- Tags -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Tags (comma-separated)</mat-label>
          <input matInput formControlName="tags" placeholder="e.g., client-meeting, travel, conference">
          <mat-hint>Separate multiple tags with commas</mat-hint>
        </mat-form-field>

        <!-- Notes -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Notes</mat-label>
          <textarea matInput formControlName="notes" rows="3" placeholder="Additional notes or details"></textarea>
        </mat-form-field>
      </form>
    </div>

    <div mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary"
              [disabled]="expenseForm.invalid || isSubmitting"
              (click)="onSubmit()">
        {{ isSubmitting ? 'Saving...' : (data?.expense ? 'Update' : 'Create') }}
      </button>
    </div>
  `,
  styles: [`
    .expense-form {
      min-width: 500px;
      max-width: 600px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .amount-row {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
    }

    .amount-field {
      flex: 2;
    }

    .currency-field {
      flex: 1;
    }

    mat-option mat-icon {
      margin-right: 8px;
      vertical-align: middle;
    }

    .mat-mdc-dialog-content {
      padding: 20px 24px;
    }

    .mat-mdc-dialog-actions {
      padding: 8px 24px 20px;
    }
  `]
})
export class ExpenseFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);
  private dialogRef = inject(MatDialogRef<ExpenseFormDialogComponent>);

  @inject(MAT_DIALOG_DATA) data: { expense?: any } | null = null;

  expenseForm!: FormGroup;
  categories$!: Observable<Category[]>;
  isSubmitting = false;

  ngOnInit() {
    this.categories$ = this.categoryService.getCategories();
    this.initializeForm();
  }

  private initializeForm() {
    const expense = this.data?.expense;

    this.expenseForm = this.fb.group({
      description: [expense?.description || '', [Validators.required, Validators.maxLength(500)]],
      amount: [expense?.amount?.amount || '', [Validators.required, Validators.min(0.01)]],
      currency: [expense?.amount?.currency || 'USD', Validators.required],
      categoryName: [expense?.category?.name || '', Validators.required],
      expenseDate: [expense ? new Date(expense.expenseDate) : new Date(), Validators.required],
      merchantName: [expense?.merchantName || ''],
      paymentMethod: [expense?.paymentMethod || ''],
      isReimbursable: [expense?.isReimbursable || false],
      tags: [expense?.tags ? expense.tags.join(', ') : ''],
      notes: [expense?.notes || '']
    });
  }

  onSubmit() {
    if (this.expenseForm.valid) {
      this.isSubmitting = true;
      const formValue = this.expenseForm.value;

      const expenseData = {
        description: formValue.description,
        amount: {
          amount: parseFloat(formValue.amount),
          currency: formValue.currency
        },
        categoryName: formValue.categoryName,
        expenseDate: formValue.expenseDate.toISOString(),
        merchantName: formValue.merchantName || undefined,
        paymentMethod: formValue.paymentMethod || undefined,
        isReimbursable: formValue.isReimbursable,
        tags: formValue.tags ? formValue.tags.split(',').map((tag: string) => tag.trim()).filter((tag: string) => tag) : [],
        notes: formValue.notes || undefined
      };

      this.dialogRef.close(expenseData);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
