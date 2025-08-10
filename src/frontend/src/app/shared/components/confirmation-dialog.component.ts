import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ConfirmationDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'warning' | 'danger' | 'info';
  icon?: string;
}

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="confirmation-dialog">
      <div class="dialog-header" [ngClass]="'type-' + data.type">
        <mat-icon class="dialog-icon">{{ data.icon || getDefaultIcon() }}</mat-icon>
        <h1 mat-dialog-title>{{ data.title }}</h1>
      </div>

      <div mat-dialog-content>
        <p class="dialog-message">{{ data.message }}</p>
      </div>

      <div mat-dialog-actions align="end">
        <button mat-button (click)="onCancel()">
          {{ data.cancelText || 'Cancel' }}
        </button>
        <button mat-raised-button
                [color]="getButtonColor()"
                (click)="onConfirm()">
          {{ data.confirmText || 'Confirm' }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .confirmation-dialog {
      min-width: 350px;
      max-width: 500px;
    }

    .dialog-header {
      display: flex;
      align-items: center;
      margin-bottom: 16px;
    }

    .dialog-icon {
      margin-right: 12px;
      font-size: 28px;
      width: 28px;
      height: 28px;
    }

    .type-warning .dialog-icon {
      color: #ff9800;
    }

    .type-danger .dialog-icon {
      color: #f44336;
    }

    .type-info .dialog-icon {
      color: #2196f3;
    }

    .dialog-message {
      margin: 0;
      line-height: 1.5;
      color: rgba(0, 0, 0, 0.7);
    }

    .mat-mdc-dialog-title {
      margin: 0;
      font-size: 18px;
      font-weight: 500;
    }

    .mat-mdc-dialog-content {
      padding: 0 24px 20px;
    }

    .mat-mdc-dialog-actions {
      padding: 8px 24px 20px;
      gap: 8px;
    }
  `]
})
export class ConfirmationDialogComponent {
  private dialogRef = inject(MatDialogRef<ConfirmationDialogComponent>);

  @inject(MAT_DIALOG_DATA) data: ConfirmationDialogData = {
    title: 'Confirm Action',
    message: 'Are you sure you want to proceed?',
    type: 'info'
  };

  onConfirm() {
    this.dialogRef.close(true);
  }

  onCancel() {
    this.dialogRef.close(false);
  }

  getDefaultIcon(): string {
    switch (this.data.type) {
      case 'warning':
        return 'warning';
      case 'danger':
        return 'error';
      case 'info':
      default:
        return 'help';
    }
  }

  getButtonColor(): string {
    switch (this.data.type) {
      case 'danger':
        return 'warn';
      case 'warning':
        return 'accent';
      case 'info':
      default:
        return 'primary';
    }
  }
}
