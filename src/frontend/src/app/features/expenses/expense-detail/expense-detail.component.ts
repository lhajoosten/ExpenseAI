import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-expense-detail',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <div class="expense-detail-container">
      <h1>Expense Details</h1>
      <mat-card>
        <mat-card-content>
          <p>Expense detail view will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .expense-detail-container {
      max-width: 800px;
      margin: 0 auto;
    }
  `]
})
export class ExpenseDetailComponent {}
