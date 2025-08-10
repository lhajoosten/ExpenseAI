import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <div class="invoices-container">
      <h1>Invoices</h1>
      <mat-card>
        <mat-card-content>
          <p>Invoice management will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .invoices-container {
      max-width: 1200px;
      margin: 0 auto;
    }
  `]
})
export class InvoicesComponent {}
