import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { LoadingService } from '../services/loading.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  private loadingService = inject(LoadingService);
  private activeRequests = 0;

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Don't show loading for certain requests
    if (this.shouldSkipLoading(req)) {
      return next.handle(req);
    }

    this.startLoading();

    return next.handle(req).pipe(
      finalize(() => this.stopLoading())
    );
  }

  private shouldSkipLoading(req: HttpRequest<any>): boolean {
    // Skip loading for background requests, polling, or specific endpoints
    const skipPatterns = [
      '/health',
      '/api/v1/auth/refresh',
      '/api/v1/notifications/poll'
    ];

    return skipPatterns.some(pattern => req.url.includes(pattern)) ||
           req.headers.has('X-Skip-Loading');
  }

  private startLoading(): void {
    this.activeRequests++;
    if (this.activeRequests === 1) {
      this.loadingService.setLoading(true);
    }
  }

  private stopLoading(): void {
    this.activeRequests--;
    if (this.activeRequests === 0) {
      this.loadingService.setLoading(false);
    }
  }
}
