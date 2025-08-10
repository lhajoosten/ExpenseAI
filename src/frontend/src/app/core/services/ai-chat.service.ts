import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../../environments/environment';

export interface ChatMessage {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  timestamp: Date;
  type?: 'text' | 'chart' | 'table' | 'action';
  metadata?: any;
}

export interface ChatResponse {
  message: string;
  type: 'text' | 'chart' | 'table' | 'action';
  data?: any;
  suggestions?: string[];
}

export interface ExpenseInsight {
  type: 'trend' | 'anomaly' | 'suggestion' | 'warning';
  title: string;
  description: string;
  data?: any;
  actionable?: boolean;
  action?: string;
}

export interface AICategorization {
  suggestedCategory: string;
  confidence: number;
  reasoning: string;
  alternatives: Array<{ category: string; confidence: number }>;
}

@Injectable({
  providedIn: 'root'
})
export class AiChatService {
  private readonly apiUrl = `${environment.apiUrl}/ai`;

  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  private isTypingSubject = new BehaviorSubject<boolean>(false);
  public isTyping$ = this.isTypingSubject.asObservable();

  constructor(private http: HttpClient) {
    this.initializeChat();
  }

  private initializeChat() {
    const welcomeMessage: ChatMessage = {
      id: this.generateId(),
      role: 'assistant',
      content: `Hello! I'm your AI financial assistant. I can help you with:

• Analyzing your expenses and spending patterns
• Categorizing expenses automatically
• Generating financial insights and reports
• Answering questions about your financial data
• Creating budgets and financial plans

What would you like to know about your finances today?`,
      timestamp: new Date(),
      type: 'text'
    };

    this.messagesSubject.next([welcomeMessage]);
  }

  sendMessage(content: string): Observable<ChatResponse> {
    const userMessage: ChatMessage = {
      id: this.generateId(),
      role: 'user',
      content,
      timestamp: new Date(),
      type: 'text'
    };

    this.addMessage(userMessage);
    this.isTypingSubject.next(true);

    return this.http.post<ChatResponse>(`${this.apiUrl}/chat`, { message: content }).pipe(
      map(response => {
        const assistantMessage: ChatMessage = {
          id: this.generateId(),
          role: 'assistant',
          content: response.message,
          timestamp: new Date(),
          type: response.type,
          metadata: response.data
        };

        this.addMessage(assistantMessage);
        this.isTypingSubject.next(false);

        return response;
      })
    );
  }

  categorizeExpense(description: string, amount: number, merchantName?: string): Observable<AICategorization> {
    return this.http.post<AICategorization>(`${this.apiUrl}/categorize`, {
      description,
      amount,
      merchantName
    });
  }

  getExpenseInsights(period: 'week' | 'month' | 'quarter' | 'year' = 'month'): Observable<ExpenseInsight[]> {
    return this.http.get<ExpenseInsight[]>(`${this.apiUrl}/insights`, {
      params: { period }
    });
  }

  generateReport(type: 'spending' | 'category' | 'trends' | 'budget', period: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reports`, { type, period });
  }

  askQuestion(question: string, context?: any): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(`${this.apiUrl}/question`, {
      question,
      context
    });
  }

  getSuggestions(input: string): Observable<string[]> {
    if (!input || input.length < 2) {
      return new Observable(observer => {
        observer.next(this.getDefaultSuggestions());
        observer.complete();
      });
    }

    return this.http.get<string[]>(`${this.apiUrl}/suggestions`, {
      params: { input }
    });
  }

  analyzeSentiment(text: string): Observable<{ sentiment: 'positive' | 'negative' | 'neutral'; confidence: number }> {
    return this.http.post<{ sentiment: 'positive' | 'negative' | 'neutral'; confidence: number }>
      (`${this.apiUrl}/sentiment`, { text });
  }

  detectDuplicateExpenses(): Observable<Array<{ expenses: any[]; similarity: number }>> {
    return this.http.get<Array<{ expenses: any[]; similarity: number }>>(`${this.apiUrl}/duplicates`);
  }

  getBudgetRecommendations(): Observable<Array<{ category: string; suggested: number; current: number; reasoning: string }>> {
    return this.http.get<Array<{ category: string; suggested: number; current: number; reasoning: string }>>
      (`${this.apiUrl}/budget-recommendations`);
  }

  getFraudAlerts(): Observable<Array<{ type: string; description: string; severity: 'low' | 'medium' | 'high'; expenseId?: string }>> {
    return this.http.get<Array<{ type: string; description: string; severity: 'low' | 'medium' | 'high'; expenseId?: string }>>
      (`${this.apiUrl}/fraud-alerts`);
  }

  // Chat management methods
  clearChat() {
    this.messagesSubject.next([]);
    this.initializeChat();
  }

  deleteMessage(messageId: string) {
    const currentMessages = this.messagesSubject.value;
    const updatedMessages = currentMessages.filter(msg => msg.id !== messageId);
    this.messagesSubject.next(updatedMessages);
  }

  regenerateResponse(messageId: string) {
    const messages = this.messagesSubject.value;
    const messageIndex = messages.findIndex(msg => msg.id === messageId);

    if (messageIndex > 0) {
      const userMessage = messages[messageIndex - 1];
      if (userMessage.role === 'user') {
        // Remove the old response
        const updatedMessages = messages.slice(0, messageIndex);
        this.messagesSubject.next(updatedMessages);

        // Send the message again
        this.sendMessage(userMessage.content).subscribe();
      }
    }
  }

  private addMessage(message: ChatMessage) {
    const currentMessages = this.messagesSubject.value;
    this.messagesSubject.next([...currentMessages, message]);
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  private getDefaultSuggestions(): string[] {
    return [
      "Show me my spending trends for this month",
      "What's my biggest expense category?",
      "Categorize my recent expenses",
      "Find duplicate expenses",
      "Generate a spending report",
      "How much did I spend on travel this quarter?",
      "Show me unusual expenses",
      "What's my average weekly spending?",
      "Help me create a budget",
      "Find tax-deductible expenses"
    ];
  }

  // Natural language processing helpers
  extractIntent(message: string): string {
    const lowerMessage = message.toLowerCase();

    if (lowerMessage.includes('trend') || lowerMessage.includes('pattern')) {
      return 'trends';
    } else if (lowerMessage.includes('budget')) {
      return 'budget';
    } else if (lowerMessage.includes('category') || lowerMessage.includes('categorize')) {
      return 'categorization';
    } else if (lowerMessage.includes('report') || lowerMessage.includes('summary')) {
      return 'reporting';
    } else if (lowerMessage.includes('duplicate')) {
      return 'duplicates';
    } else if (lowerMessage.includes('fraud') || lowerMessage.includes('suspicious')) {
      return 'fraud';
    } else {
      return 'general';
    }
  }

  extractTimeperiod(message: string): string {
    const lowerMessage = message.toLowerCase();

    if (lowerMessage.includes('today') || lowerMessage.includes('day')) {
      return 'day';
    } else if (lowerMessage.includes('week')) {
      return 'week';
    } else if (lowerMessage.includes('month')) {
      return 'month';
    } else if (lowerMessage.includes('quarter')) {
      return 'quarter';
    } else if (lowerMessage.includes('year')) {
      return 'year';
    } else {
      return 'month';
    }
  }
}
