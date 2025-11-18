/**
 * Retry utility for failed API requests
 * Useful for transient errors like network issues or 5xx server errors
 */

export interface RetryOptions {
  maxRetries?: number;
  retryDelay?: number;
  retryableStatuses?: number[];
  retryableErrors?: string[];
}

const DEFAULT_OPTIONS: Required<RetryOptions> = {
  maxRetries: 3,
  retryDelay: 1000, // 1 second
  retryableStatuses: [408, 429, 500, 502, 503, 504],
  retryableErrors: ['ECONNABORTED', 'ETIMEDOUT', 'ENOTFOUND', 'ECONNRESET'],
};

/**
 * Check if an error is retryable
 */
export function isRetryableError(error: any, options: RetryOptions = {}): boolean {
  const opts = { ...DEFAULT_OPTIONS, ...options };
  
  // Network errors are usually retryable
  if (error.code && opts.retryableErrors.includes(error.code)) {
    return true;
  }
  
  // Check status code
  if (error.status && opts.retryableStatuses.includes(error.status)) {
    return true;
  }
  
  // Check error type
  if (error.type) {
    const retryableTypes = ['network', 'timeout', 'connection', 'server', 'serviceUnavailable', 'rateLimit'];
    return retryableTypes.includes(error.type);
  }
  
  return false;
}

/**
 * Calculate exponential backoff delay
 */
export function calculateRetryDelay(attempt: number, baseDelay: number = 1000): number {
  // Exponential backoff: 1s, 2s, 4s, etc.
  return baseDelay * Math.pow(2, attempt);
}

/**
 * Retry a function with exponential backoff
 */
export async function retryWithBackoff<T>(
  fn: () => Promise<T>,
  options: RetryOptions = {}
): Promise<T> {
  const opts = { ...DEFAULT_OPTIONS, ...options };
  let lastError: any;
  
  for (let attempt = 0; attempt <= opts.maxRetries; attempt++) {
    try {
      return await fn();
    } catch (error: any) {
      lastError = error;
      
      // Don't retry if error is not retryable
      if (!isRetryableError(error, opts)) {
        throw error;
      }
      
      // Don't retry on last attempt
      if (attempt === opts.maxRetries) {
        break;
      }
      
      // Calculate delay with exponential backoff
      const delay = calculateRetryDelay(attempt, opts.retryDelay);
      
      // Wait before retrying
      await new Promise(resolve => setTimeout(resolve, delay));
    }
  }
  
  throw lastError;
}
