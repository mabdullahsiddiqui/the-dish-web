/**
 * Health check utility to verify backend connectivity
 * Can be used in development to ensure services are running
 */

export interface ServiceHealth {
  name: string;
  url: string;
  status: 'healthy' | 'unhealthy' | 'unknown';
  responseTime?: number;
  error?: string;
}

export async function checkBackendHealth(): Promise<ServiceHealth[]> {
  // Check services through API Gateway endpoints to avoid CORS issues
  // We'll test if each service is reachable through the API Gateway
  const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000/api/v1';
  
  const services: Array<{ name: string; testEndpoint: string }> = [
    { 
      name: 'API Gateway', 
      testEndpoint: `${apiBaseUrl}/places/search?page=1&pageSize=1` // Public endpoint
    },
    { 
      name: 'Place Service', 
      testEndpoint: `${apiBaseUrl}/places/search?page=1&pageSize=1` // Through API Gateway
    },
    { 
      name: 'Review Service', 
      testEndpoint: `${apiBaseUrl}/reviews/recent?page=1&pageSize=1` // Through API Gateway
    },
    { 
      name: 'User Service', 
      testEndpoint: `${apiBaseUrl}/users/register` // Will fail but shows if service is reachable
    },
  ];

  const healthChecks: ServiceHealth[] = [];

  for (const service of services) {
    const startTime = Date.now();
    try {
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 3000); // 3 second timeout
      
      const response = await fetch(service.testEndpoint, {
        method: 'GET',
        cache: 'no-store',
        signal: controller.signal,
        mode: 'cors',
      });
      
      clearTimeout(timeoutId);
      const responseTime = Date.now() - startTime;
      
      // If we get any response (even 400/401/500), the service is reachable
      // Status 0 means network error (service down)
      const isHealthy = response.status !== 0 && response.status < 500;
      
      healthChecks.push({
        name: service.name,
        url: service.testEndpoint,
        status: isHealthy ? 'healthy' : 'unhealthy',
        responseTime,
      });
    } catch (error) {
      const responseTime = Date.now() - startTime;
      healthChecks.push({
        name: service.name,
        url: service.testEndpoint,
        status: 'unhealthy',
        responseTime,
        error: error instanceof Error ? error.message : 'Unknown error',
      });
    }
  }

  return healthChecks;
}

export async function checkApiHealth(): Promise<boolean> {
  const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000/api/v1';
  
  try {
    // Try to hit the places search endpoint (public, doesn't require auth)
    // This is a better health check than a /health endpoint that might not exist
    const response = await fetch(`${apiBaseUrl}/places/search?page=1&pageSize=1`, {
      method: 'GET',
      cache: 'no-store',
      mode: 'cors',
    });
    
    // Even if we get 400/500, if we get a response, the API Gateway is working
    return response.status !== 0; // Status 0 means network error
  } catch (error) {
    return false;
  }
}
