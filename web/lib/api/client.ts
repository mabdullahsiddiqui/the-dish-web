import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';
import { ApiResponse, ApiError } from '@/types/api';

// Get API base URL from environment
const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000/api/v1';

// Create axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config) => {
    // Get token from localStorage
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('auth_token');
      console.log('[API Client] Interceptor running for:', config.url);
      console.log('[API Client] Token exists:', !!token);
      if (token) {
        // Ensure headers object exists and set Authorization header
        config.headers = config.headers || {};
        config.headers['Authorization'] = `Bearer ${token}`;
        console.log('[API Client] Authorization header set:', config.headers['Authorization']?.substring(0, 20) + '...');
      } else {
        console.warn('[API Client] No token found in localStorage!');
      }
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
apiClient.interceptors.response.use(
  (response: AxiosResponse<ApiResponse>) => {
    // Check if the API returned success: false even with 200 status
    if (response.data && response.data.success === false) {
      // Convert to error format
      const error: any = new Error(response.data.message || 'Request failed');
      error.response = {
        status: 400,
        data: response.data,
      };
      return Promise.reject(error);
    }
    return response;
  },
  (error) => {
    let apiError: ApiError = {
      message: 'An unexpected error occurred',
      status: error.response?.status,
      code: error.code,
    };

    // Network errors (no response from server)
    if (!error.response) {
      if (error.code === 'ECONNABORTED' || error.message.includes('timeout')) {
        apiError.message = 'Request timed out. Please check your connection and try again.';
        apiError.type = 'timeout';
      } else if (error.message.includes('Network Error') || error.code === 'ERR_NETWORK') {
        apiError.message = 'Network error. Please check your internet connection.';
        apiError.type = 'network';
      } else {
        apiError.message = 'Unable to connect to server. Please try again later.';
        apiError.type = 'connection';
      }
    } else {
      // HTTP errors (server responded with error status)
      const status = error.response.status;
      
      // Extract error message from response
      // Handle the API response format: { success: false, message: "...", data: null }
      if (error.response.data?.message) {
        apiError.message = error.response.data.message;
      } else if (error.response.data?.data?.message) {
        // Sometimes the message is nested in data
        apiError.message = error.response.data.data.message;
      } else if (error.response.data?.errors && Array.isArray(error.response.data.errors)) {
        apiError.message = error.response.data.errors.join(', ');
      } else if (error.response.data?.error) {
        apiError.message = error.response.data.error;
      } else if (error.message) {
        apiError.message = error.message;
      } else {
        // Fallback: show the status code
        apiError.message = `Request failed with status ${status}`;
      }

      // Provide user-friendly messages for common status codes
      switch (status) {
        case 400:
          apiError.type = 'validation';
          if (!apiError.message || apiError.message === 'An unexpected error occurred') {
            apiError.message = 'Invalid request. Please check your input and try again.';
          }
          break;
        case 401:
          apiError.type = 'unauthorized';
          // Only redirect to login for protected routes
          // Public routes like search should not redirect or show session expired message
          const currentPath = typeof window !== 'undefined' ? window.location.pathname : '';
          const publicRoutes = ['/', '/search', '/login', '/register'];
          const isPublicRoute = publicRoutes.includes(currentPath) || currentPath.startsWith('/places/');
          
          if (isPublicRoute) {
            // For public routes, show a generic error instead of "session expired"
            apiError.message = 'Unable to access this resource. Please try again.';
          } else {
            apiError.message = 'Your session has expired. Please log in again.';
            if (typeof window !== 'undefined') {
              localStorage.removeItem('auth_token');
              localStorage.removeItem('user');
              // Use router if available, otherwise use window.location
              setTimeout(() => {
                window.location.href = '/login';
              }, 100);
            }
          }
          break;
        case 403:
          apiError.type = 'forbidden';
          apiError.message = 'You do not have permission to perform this action.';
          break;
        case 404:
          apiError.type = 'notFound';
          apiError.message = 'The requested resource was not found.';
          break;
        case 409:
          apiError.type = 'conflict';
          apiError.message = 'This action conflicts with existing data.';
          break;
        case 422:
          apiError.type = 'validation';
          apiError.message = 'Validation failed. Please check your input.';
          break;
        case 429:
          apiError.type = 'rateLimit';
          apiError.message = 'Too many requests. Please wait a moment and try again.';
          break;
        case 500:
          apiError.type = 'server';
          apiError.message = 'Server error. Please try again later.';
          break;
        case 502:
        case 503:
        case 504:
          apiError.type = 'serviceUnavailable';
          apiError.message = 'Service temporarily unavailable. Please try again later.';
          break;
        default:
          apiError.type = 'unknown';
      }
    }

    return Promise.reject(apiError);
  }
);

// Generic API methods
export const api = {
  get: <T>(url: string, config?: AxiosRequestConfig) => 
    apiClient.get<ApiResponse<T>>(url, config),
  
  post: <T>(url: string, data?: any, config?: AxiosRequestConfig) => 
    apiClient.post<ApiResponse<T>>(url, data, config),
  
  put: <T>(url: string, data?: any, config?: AxiosRequestConfig) => 
    apiClient.put<ApiResponse<T>>(url, data, config),
  
  delete: <T>(url: string, config?: AxiosRequestConfig) => 
    apiClient.delete<ApiResponse<T>>(url, config),
  
  patch: <T>(url: string, data?: any, config?: AxiosRequestConfig) => 
    apiClient.patch<ApiResponse<T>>(url, data, config),
};

export default apiClient;

