import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, Subject, interval } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { environment } from '../../../environments/environment';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
  persistent?: boolean;
  actionable?: boolean;
  action?: {
    label: string;
    callback: () => void;
  };
  metadata?: any;
}

export interface SystemAlert {
  id: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
  category: 'budget' | 'fraud' | 'duplicate' | 'system' | 'reminder';
  title: string;
  description: string;
  timestamp: Date;
  acknowledged: boolean;
  data?: any;
}

export interface NotificationSettings {
  email: {
    enabled: boolean;
    frequency: 'immediate' | 'daily' | 'weekly';
    types: string[];
  };
  push: {
    enabled: boolean;
    types: string[];
  };
  inApp: {
    enabled: boolean;
    sound: boolean;
    desktop: boolean;
  };
  budgetAlerts: {
    enabled: boolean;
    threshold: number; // percentage
    categories: string[];
  };
  expenseAlerts: {
    enabled: boolean;
    unusualSpending: boolean;
    duplicateDetection: boolean;
    largeTransactions: {
      enabled: boolean;
      threshold: number;
    };
  };
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly apiUrl = `${environment.apiUrl}/notifications`;

  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  public notifications$ = this.notificationsSubject.asObservable();

  private alertsSubject = new BehaviorSubject<SystemAlert[]>([]);
  public alerts$ = this.alertsSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  private settingsSubject = new BehaviorSubject<NotificationSettings | null>(null);
  public settings$ = this.settingsSubject.asObservable();

  private destroy$ = new Subject<void>();
  private pollingInterval = 30000; // 30 seconds

  constructor(private http: HttpClient) {
    this.loadSettings();
    this.loadNotifications();
    this.loadAlerts();
    this.startPolling();
    this.requestNotificationPermission();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Notification Management
  showNotification(notification: Omit<Notification, 'id' | 'timestamp' | 'read'>): void {
    const newNotification: Notification = {
      ...notification,
      id: this.generateId(),
      timestamp: new Date(),
      read: false
    };

    const currentNotifications = this.notificationsSubject.value;
    this.notificationsSubject.next([newNotification, ...currentNotifications]);
    this.updateUnreadCount();

    // Show browser notification if enabled
    if (this.settingsSubject.value?.inApp.desktop) {
      this.showBrowserNotification(newNotification);
    }

    // Play sound if enabled
    if (this.settingsSubject.value?.inApp.sound) {
      this.playNotificationSound();
    }

    // Auto-remove non-persistent notifications
    if (!notification.persistent) {
      setTimeout(() => {
        this.removeNotification(newNotification.id);
      }, 5000);
    }
  }

  showSuccess(title: string, message: string, persistent: boolean = false): void {
    this.showNotification({
      type: 'success',
      title,
      message,
      persistent
    });
  }

  showError(title: string, message: string, persistent: boolean = true): void {
    this.showNotification({
      type: 'error',
      title,
      message,
      persistent
    });
  }

  showWarning(title: string, message: string, persistent: boolean = true): void {
    this.showNotification({
      type: 'warning',
      title,
      message,
      persistent
    });
  }

  showInfo(title: string, message: string, persistent: boolean = false): void {
    this.showNotification({
      type: 'info',
      title,
      message,
      persistent
    });
  }

  removeNotification(id: string): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = currentNotifications.filter(n => n.id !== id);
    this.notificationsSubject.next(updatedNotifications);
    this.updateUnreadCount();
  }

  markAsRead(id: string): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = currentNotifications.map(n =>
      n.id === id ? { ...n, read: true } : n
    );
    this.notificationsSubject.next(updatedNotifications);
    this.updateUnreadCount();

    // Update on server
    this.http.patch(`${this.apiUrl}/${id}/read`, {}).subscribe();
  }

  markAllAsRead(): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = currentNotifications.map(n => ({ ...n, read: true }));
    this.notificationsSubject.next(updatedNotifications);
    this.updateUnreadCount();

    // Update on server
    this.http.patch(`${this.apiUrl}/mark-all-read`, {}).subscribe();
  }

  clearAll(): void {
    this.notificationsSubject.next([]);
    this.updateUnreadCount();
  }

  // Alert Management
  addAlert(alert: Omit<SystemAlert, 'id' | 'timestamp' | 'acknowledged'>): void {
    const newAlert: SystemAlert = {
      ...alert,
      id: this.generateId(),
      timestamp: new Date(),
      acknowledged: false
    };

    const currentAlerts = this.alertsSubject.value;
    this.alertsSubject.next([newAlert, ...currentAlerts]);

    // Also show as notification if high severity
    if (alert.severity === 'high' || alert.severity === 'critical') {
      this.showNotification({
        type: 'warning',
        title: alert.title,
        message: alert.description,
        persistent: true,
        actionable: true,
        action: {
          label: 'View Alert',
          callback: () => this.acknowledgeAlert(newAlert.id)
        }
      });
    }
  }

  acknowledgeAlert(id: string): void {
    const currentAlerts = this.alertsSubject.value;
    const updatedAlerts = currentAlerts.map(a =>
      a.id === id ? { ...a, acknowledged: true } : a
    );
    this.alertsSubject.next(updatedAlerts);

    // Update on server
    this.http.patch(`${this.apiUrl}/alerts/${id}/acknowledge`, {}).subscribe();
  }

  dismissAlert(id: string): void {
    const currentAlerts = this.alertsSubject.value;
    const updatedAlerts = currentAlerts.filter(a => a.id !== id);
    this.alertsSubject.next(updatedAlerts);

    // Update on server
    this.http.delete(`${this.apiUrl}/alerts/${id}`).subscribe();
  }

  // Settings Management
  getSettings(): Observable<NotificationSettings> {
    return this.http.get<NotificationSettings>(`${this.apiUrl}/settings`);
  }

  updateSettings(settings: Partial<NotificationSettings>): Observable<NotificationSettings> {
    return this.http.patch<NotificationSettings>(`${this.apiUrl}/settings`, settings);
  }

  // Server Communication
  private loadNotifications(): void {
    this.http.get<Notification[]>(`${this.apiUrl}`).subscribe(
      notifications => {
        this.notificationsSubject.next(notifications);
        this.updateUnreadCount();
      },
      error => console.error('Failed to load notifications:', error)
    );
  }

  private loadAlerts(): void {
    this.http.get<SystemAlert[]>(`${this.apiUrl}/alerts`).subscribe(
      alerts => this.alertsSubject.next(alerts),
      error => console.error('Failed to load alerts:', error)
    );
  }

  private loadSettings(): void {
    this.getSettings().subscribe(
      settings => this.settingsSubject.next(settings),
      error => console.error('Failed to load notification settings:', error)
    );
  }

  private startPolling(): void {
    interval(this.pollingInterval)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.loadNotifications();
        this.loadAlerts();
      });
  }

  // Browser Notifications
  private async requestNotificationPermission(): Promise<void> {
    if ('Notification' in window) {
      const permission = await Notification.requestPermission();
      console.log('Notification permission:', permission);
    }
  }

  private showBrowserNotification(notification: Notification): void {
    if ('Notification' in window && Notification.permission === 'granted') {
      const browserNotification = new Notification(notification.title, {
        body: notification.message,
        icon: '/assets/icons/notification-icon.png',
        badge: '/assets/icons/badge-icon.png',
        tag: notification.id,
        requireInteraction: notification.persistent
      });

      browserNotification.onclick = () => {
        window.focus();
        browserNotification.close();
        this.markAsRead(notification.id);
      };
    }
  }

  private playNotificationSound(): void {
    const audio = new Audio('/assets/sounds/notification.mp3');
    audio.volume = 0.3;
    audio.play().catch(error => console.log('Could not play notification sound:', error));
  }

  // Utility Methods
  private updateUnreadCount(): void {
    const unreadCount = this.notificationsSubject.value.filter(n => !n.read).length;
    this.unreadCountSubject.next(unreadCount);
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  // Quick Actions
  notifyBudgetExceeded(category: string, percentage: number, amount: number): void {
    this.addAlert({
      severity: percentage > 100 ? 'high' : 'medium',
      category: 'budget',
      title: `Budget Alert: ${category}`,
      description: `You've spent ${percentage.toFixed(1)}% of your ${category} budget ($${amount.toFixed(2)})`,
      data: { category, percentage, amount }
    });
  }

  notifyLargeTransaction(description: string, amount: number): void {
    this.addAlert({
      severity: 'medium',
      category: 'system',
      title: 'Large Transaction Detected',
      description: `Unusual transaction: ${description} - $${amount.toFixed(2)}`,
      data: { description, amount }
    });
  }

  notifyDuplicateExpense(expenseId: string, description: string): void {
    this.addAlert({
      severity: 'low',
      category: 'duplicate',
      title: 'Possible Duplicate Expense',
      description: `Similar expense detected: ${description}`,
      data: { expenseId, description }
    });
  }

  notifyFraudAlert(description: string, reason: string): void {
    this.addAlert({
      severity: 'critical',
      category: 'fraud',
      title: 'Fraud Alert',
      description: `Suspicious activity: ${description}. ${reason}`,
      data: { description, reason }
    });
  }

  notifyExpenseReminder(description: string, dueDate: Date): void {
    this.showNotification({
      type: 'info',
      title: 'Expense Reminder',
      message: `Don't forget: ${description} (Due: ${dueDate.toLocaleDateString()})`,
      persistent: true,
      actionable: true,
      action: {
        label: 'Add Expense',
        callback: () => {
          // This would typically navigate to expense form
          console.log('Navigate to expense form');
        }
      }
    });
  }
}
