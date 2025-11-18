export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
}

export type ErrorType = 
  | 'network'
  | 'timeout'
  | 'connection'
  | 'validation'
  | 'unauthorized'
  | 'forbidden'
  | 'notFound'
  | 'conflict'
  | 'rateLimit'
  | 'server'
  | 'serviceUnavailable'
  | 'unknown';

export interface ApiError {
  message: string;
  status?: number;
  code?: string;
  type?: ErrorType;
  retryable?: boolean;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Location {
  latitude: number;
  longitude: number;
}

