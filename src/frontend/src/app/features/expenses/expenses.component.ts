import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { BehaviorSubject, debounceTime, distinctUntilChanged } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

import { ExpenseService } from '../../core/services/expense.service';
import { CategoryService } from '../../core/services/category.service';
import { ExpenseDto, ExpenseFilters, PagedResult } from '../../shared/models/expense.models';

@Component({
  selector: 'app-expenses',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatCardModule,
    MatChipsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatCheckboxModule
  ],
  templateUrl: './expenses.component.html',
  styleUrls: ['./expenses.component.scss']
})
export class ExpensesComponent implements OnInit {
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private expenseService = inject(ExpenseService);
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  displayedColumns: string[] = ['date', 'description', 'category', 'amount', 'actions'];
  pageSize = 25;
  pageIndex = 0;
  totalCount = 0;

  private filtersSubject = new BehaviorSubject<ExpenseFilters>({});

  loading$ = new BehaviorSubject<boolean>(false);

  filtersForm = this.fb.group({
    searchTerm: [''],
    categoryFilter: [''],
    startDate: [null as Date | null],
    endDate: [null as Date | null],
    minAmount: [null as number | null],
    maxAmount: [null as number | null]
  });

  categories$ = this.categoryService.getCategories();

  expenseData$ = this.filtersSubject.pipe(
    debounceTime(300),
    distinctUntilChanged(),
    switchMap(filters => {
      this.loading$.next(true);
      const skip = this.pageIndex * this.pageSize;
      return this.expenseService.searchExpenses(filters, skip, this.pageSize).pipe(
        map(data => {
          this.loading$.next(false);
          this.totalCount = data.totalCount;
          return data;
        }),
        catchError(error => {
          this.loading$.next(false);
          this.snackBar.open('Error loading expenses', 'Close', { duration: 3000 });
          return of({ items: [], totalCount: 0, pageIndex: 0, pageSize: this.pageSize, totalPages: 0 });
        })
      );
    })
  );

  expenses$ = this.expenseData$.pipe(
    map(data => data.items)
  );

  ngOnInit() {
    // Watch form changes and update filters
    this.filtersForm.valueChanges.pipe(
      startWith(this.filtersForm.value),
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(formValue => {
      const filters: ExpenseFilters = {
        searchTerm: formValue.searchTerm || undefined,
        category: formValue.categoryFilter || undefined,
        minAmount: formValue.minAmount || undefined,
        maxAmount: formValue.maxAmount || undefined,
        startDate: formValue.startDate || undefined,
        endDate: formValue.endDate || undefined
      };
      this.pageIndex = 0; // Reset to first page on filter change
      this.filtersSubject.next(filters);
    });
  }

  onPageChange(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.filtersSubject.next(this.filtersSubject.value);
  }

  clearFilters() {
    this.filtersForm.reset();
  }

  openAddExpenseDialog() {
    // TODO: Implement add expense dialog
    this.snackBar.open('Add expense dialog - to be implemented', 'Close', { duration: 3000 });
  }

  editExpense(expense: ExpenseDto) {
    // TODO: Implement edit expense dialog
    this.snackBar.open(`Edit expense: ${expense.description}`, 'Close', { duration: 3000 });
  }

  categorizeExpense(expense: ExpenseDto) {
    this.expenseService.categorizeExpense(expense.id).subscribe({
      next: (updatedExpense) => {
        this.snackBar.open('Expense categorized successfully', 'Close', { duration: 3000 });
        // Refresh data
        this.filtersSubject.next(this.filtersSubject.value);
      },
      error: (error) => {
        this.snackBar.open('Error categorizing expense', 'Close', { duration: 3000 });
      }
    });
  }

  viewReceipt(expense: ExpenseDto) {
    if (expense.receiptUrl) {
      window.open(expense.receiptUrl, '_blank');
    }
  }

  deleteExpense(expense: ExpenseDto) {
    if (confirm(`Are you sure you want to delete the expense: ${expense.description}?`)) {
      this.expenseService.deleteExpense(expense.id).subscribe({
        next: () => {
          this.snackBar.open('Expense deleted successfully', 'Close', { duration: 3000 });
          // Refresh data
          this.filtersSubject.next(this.filtersSubject.value);
        },
        error: (error) => {
          this.snackBar.open('Error deleting expense', 'Close', { duration: 3000 });
        }
      });
    }
  }

  exportExpenses(format: 'csv' | 'excel') {
    const currentFilters = this.filtersSubject.value;
    this.expenseService.exportExpenses(format, currentFilters).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `expenses.${format === 'excel' ? 'xlsx' : 'csv'}`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.snackBar.open(`Expenses exported as ${format.toUpperCase()}`, 'Close', { duration: 3000 });
      },
      error: (error) => {
        this.snackBar.open('Error exporting expenses', 'Close', { duration: 3000 });
      }
    });
  }

  getTotalAmount(): number {
    // This would be calculated from the current filtered results
    // For now, return 0 as placeholder
    return 0;
  }
}
