export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
  errors?: string[];
}

export interface ApiError {
  message: string;
  errors?: { [key: string]: string[] };
  statusCode: number;
  traceId?: string;
}

export interface ValidationError {
  field: string;
  message: string;
  attemptedValue?: any;
}

export interface PaginationParams {
  skip?: number;
  take?: number;
}

export interface SortParams {
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface SearchParams extends PaginationParams, SortParams {
  searchTerm?: string;
}

export interface DateRange {
  startDate: Date;
  endDate: Date;
}

export interface FileUploadResponse {
  fileName: string;
  originalFileName: string;
  url: string;
  size: number;
  contentType: string;
  uploadedAt: string;
}

export interface HealthCheckResponse {
  status: 'Healthy' | 'Unhealthy' | 'Degraded';
  checks: HealthCheckItem[];
  totalDuration: string;
}

export interface HealthCheckItem {
  name: string;
  status: 'Healthy' | 'Unhealthy' | 'Degraded';
  description?: string;
  duration: string;
}

// Generic result pattern
export interface Result<T> {
  isSuccess: boolean;
  data?: T;
  error?: string;
  errors?: string[];
}

export interface OperationResult {
  success: boolean;
  message?: string;
  errors?: string[];
}
