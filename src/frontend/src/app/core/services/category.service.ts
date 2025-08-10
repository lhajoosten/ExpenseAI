import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { Category } from '../../shared/models/expense.models';

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  color?: string;
  icon?: string;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
  color?: string;
  icon?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly apiUrl = `${environment.apiUrl}/categories`;

  constructor(private http: HttpClient) {}

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  getCategory(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  getCategoryByName(name: string): Observable<Category | null> {
    return this.getCategories().pipe(
      map(categories => categories.find(c => c.name.toLowerCase() === name.toLowerCase()) || null)
    );
  }

  createCategory(request: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, request);
  }

  updateCategory(id: string, request: UpdateCategoryRequest): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}`, request);
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getSystemCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/system`);
  }

  getUserCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/user`);
  }

  // Helper methods for UI
  getCategoryIcon(categoryName: string): string {
    const iconMap: { [key: string]: string } = {
      'Office': 'business',
      'Travel': 'flight',
      'Meals': 'restaurant',
      'Software': 'computer',
      'Marketing': 'campaign',
      'Professional': 'work',
      'Food & Dining': 'restaurant',
      'Transportation': 'directions_car',
      'Entertainment': 'movie',
      'Shopping': 'shopping_cart',
      'Utilities': 'power',
      'Healthcare': 'local_hospital',
      'Education': 'school',
      'Insurance': 'security',
      'Maintenance': 'build',
      'Uncategorized': 'help_outline'
    };

    return iconMap[categoryName] || 'receipt';
  }

  getCategoryColor(categoryName: string): string {
    const colorMap: { [key: string]: string } = {
      'Office': '#2196F3',
      'Travel': '#FF9800',
      'Meals': '#4CAF50',
      'Software': '#9C27B0',
      'Marketing': '#F44336',
      'Professional': '#3F51B5',
      'Food & Dining': '#4CAF50',
      'Transportation': '#FF5722',
      'Entertainment': '#E91E63',
      'Shopping': '#00BCD4',
      'Utilities': '#607D8B',
      'Healthcare': '#009688',
      'Education': '#795548',
      'Insurance': '#8BC34A',
      'Maintenance': '#FFC107',
      'Uncategorized': '#9E9E9E'
    };

    return colorMap[categoryName] || '#6B7280';
  }

  // Get predefined system categories for offline usage
  getDefaultCategories(): Category[] {
    return [
      {
        name: 'Office',
        description: 'Office supplies and equipment',
        color: '#2196F3',
        icon: 'business',
        isSystemCategory: true
      },
      {
        name: 'Travel',
        description: 'Business travel expenses',
        color: '#FF9800',
        icon: 'flight',
        isSystemCategory: true
      },
      {
        name: 'Meals',
        description: 'Business meals and entertainment',
        color: '#4CAF50',
        icon: 'restaurant',
        isSystemCategory: true
      },
      {
        name: 'Software',
        description: 'Software licenses and subscriptions',
        color: '#9C27B0',
        icon: 'computer',
        isSystemCategory: true
      },
      {
        name: 'Marketing',
        description: 'Marketing and advertising expenses',
        color: '#F44336',
        icon: 'campaign',
        isSystemCategory: true
      },
      {
        name: 'Professional',
        description: 'Professional services and consulting',
        color: '#3F51B5',
        icon: 'work',
        isSystemCategory: true
      },
      {
        name: 'Transportation',
        description: 'Transportation and vehicle expenses',
        color: '#FF5722',
        icon: 'directions_car',
        isSystemCategory: true
      },
      {
        name: 'Utilities',
        description: 'Utilities and infrastructure',
        color: '#607D8B',
        icon: 'power',
        isSystemCategory: true
      },
      {
        name: 'Uncategorized',
        description: 'Expenses that need categorization',
        color: '#9E9E9E',
        icon: 'help_outline',
        isSystemCategory: true
      }
    ];
  }

  // Category suggestions based on description
  suggestCategory(description: string): Observable<Category | null> {
    const lowerDesc = description.toLowerCase();

    // Simple keyword matching for suggestions
    const suggestions: { [key: string]: string } = {
      'coffee': 'Meals',
      'starbucks': 'Meals',
      'restaurant': 'Meals',
      'food': 'Meals',
      'lunch': 'Meals',
      'dinner': 'Meals',
      'uber': 'Transportation',
      'taxi': 'Transportation',
      'flight': 'Travel',
      'hotel': 'Travel',
      'airbnb': 'Travel',
      'office': 'Office',
      'supplies': 'Office',
      'amazon': 'Shopping',
      'software': 'Software',
      'subscription': 'Software',
      'marketing': 'Marketing',
      'advertising': 'Marketing',
      'consulting': 'Professional',
      'legal': 'Professional'
    };

    for (const [keyword, categoryName] of Object.entries(suggestions)) {
      if (lowerDesc.includes(keyword)) {
        return this.getCategoryByName(categoryName);
      }
    }

    return of(null);
  }
}
