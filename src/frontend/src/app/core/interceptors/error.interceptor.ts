import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { NotificationService } from '../services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private notificationService = inject(NotificationService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        this.handleError(error);
        return throwError(() => error);
      })
    );
  }

  private handleError(error: HttpErrorResponse): void {
    let errorMessage = 'An unexpected error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 0:
          errorMessage = 'Unable to connect to the server. Please check your internet connection.';
          break;
        case 400:
          errorMessage = this.extractValidationErrors(error) || 'Invalid request. Please check your input.';
          break;
        case 401:
          errorMessage = 'You are not authorized to perform this action.';
          break;
        case 403:
          errorMessage = 'You do not have permission to access this resource.';
          break;
        case 404:
          errorMessage = 'The requested resource was not found.';
          break;
        case 409:
          errorMessage = 'A conflict occurred while processing your request.';
          break;
        case 422:
          errorMessage = this.extractValidationErrors(error) || 'The data you provided is invalid.';
          break;
        case 500:
          errorMessage = 'An internal server error occurred. Please try again later.';
          break;
        case 502:
        case 503:
        case 504:
          errorMessage = 'The server is temporarily unavailable. Please try again later.';
          break;
        default:
          errorMessage = `Server error: ${error.status} ${error.statusText}`;
      }
    }

    // Don't show notifications for certain status codes that are handled elsewhere
    if (![401, 403].includes(error.status)) {
      this.notificationService.showError('Error', errorMessage);
    }
  }

  private extractValidationErrors(error: HttpErrorResponse): string | null {
    if (error.error && error.error.errors) {
      const errorMessages: string[] = [];

      // Handle .NET ModelState errors
      if (typeof error.error.errors === 'object') {
        Object.keys(error.error.errors).forEach(key => {
          const messages = error.error.errors[key];
          if (Array.isArray(messages)) {
            errorMessages.push(...messages);
          } else {
            errorMessages.push(messages);
          }
        });
      }

      return errorMessages.length > 0 ? errorMessages.join(', ') : null;
    }

    if (error.error && error.error.message) {
      return error.error.message;
    }

    return null;
  }
}
