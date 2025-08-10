import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <div class="analytics-container">
      <h1>Analytics</h1>
      <mat-card>
        <mat-card-content>
          <p>Analytics and reporting will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .analytics-container {
      max-width: 1200px;
      margin: 0 auto;
    }
  `]
})
export class AnalyticsComponent {}
