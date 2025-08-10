import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-expense-form',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <div class="expense-form-container">
      <h1>Add/Edit Expense</h1>
      <mat-card>
        <mat-card-content>
          <p>Expense form will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .expense-form-container {
      max-width: 800px;
      margin: 0 auto;
    }
  `]
})
export class ExpenseFormComponent {}
