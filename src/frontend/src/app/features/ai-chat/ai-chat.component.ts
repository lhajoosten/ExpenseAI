import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-ai-chat',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <div class="ai-chat-container">
      <h1>AI Assistant</h1>
      <mat-card>
        <mat-card-content>
          <p>AI chat functionality will be implemented here.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .ai-chat-container {
      max-width: 800px;
      margin: 0 auto;
    }
  `]
})
export class AiChatComponent {}
