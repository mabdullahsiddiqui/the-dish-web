'use client'

import { useState, useEffect } from 'react';
import { CheckCircle, XCircle, Loader2, RefreshCw } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { checkBackendHealth, checkApiHealth, ServiceHealth } from '@/lib/utils/health-check';

/**
 * Backend Health Status Component
 * Only shown in development mode
 */
export function BackendHealthStatus() {
  const [healthStatus, setHealthStatus] = useState<ServiceHealth[]>([]);
  const [apiHealth, setApiHealth] = useState<boolean | null>(null);
  const [loading, setLoading] = useState(true);
  const [lastChecked, setLastChecked] = useState<Date | null>(null);

  const checkHealth = async () => {
    setLoading(true);
    try {
      const [services, api] = await Promise.all([
        checkBackendHealth(),
        checkApiHealth(),
      ]);
      setHealthStatus(services);
      setApiHealth(api);
      setLastChecked(new Date());
    } catch (error) {
      console.error('Health check failed:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // Only show in development
    if (process.env.NODE_ENV === 'development') {
      checkHealth();
      // Auto-refresh every 30 seconds
      const interval = setInterval(checkHealth, 30000);
      return () => clearInterval(interval);
    }
  }, []);

  // Don't render in production
  if (process.env.NODE_ENV !== 'development') {
    return null;
  }

  const allHealthy = healthStatus.every(s => s.status === 'healthy') && apiHealth === true;

  return (
    <Card className="fixed bottom-4 right-4 w-80 z-50 shadow-lg border-2">
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <CardTitle className="text-sm font-semibold">Backend Health</CardTitle>
          <Button
            variant="ghost"
            size="sm"
            onClick={checkHealth}
            disabled={loading}
            className="h-6 w-6 p-0"
          >
            <RefreshCw className={`w-3 h-3 ${loading ? 'animate-spin' : ''}`} />
          </Button>
        </div>
      </CardHeader>
      <CardContent className="space-y-2">
        {loading && healthStatus.length === 0 ? (
          <div className="flex items-center space-x-2 text-sm text-gray-500">
            <Loader2 className="w-4 h-4 animate-spin" />
            <span>Checking services...</span>
          </div>
        ) : (
          <>
            {/* Overall Status */}
            <div className="flex items-center space-x-2 pb-2 border-b">
              {allHealthy ? (
                <>
                  <CheckCircle className="w-4 h-4 text-green-500" />
                  <span className="text-sm font-medium text-green-700">All services healthy</span>
                </>
              ) : (
                <>
                  <XCircle className="w-4 h-4 text-red-500" />
                  <span className="text-sm font-medium text-red-700">Some services unavailable</span>
                </>
              )}
            </div>

            {/* Service Status */}
            <div className="space-y-1.5 max-h-48 overflow-y-auto">
              {healthStatus.map((service) => (
                <div key={service.name} className="flex items-center justify-between text-xs">
                  <div className="flex items-center space-x-2">
                    {service.status === 'healthy' ? (
                      <CheckCircle className="w-3 h-3 text-green-500" />
                    ) : (
                      <XCircle className="w-3 h-3 text-red-500" />
                    )}
                    <span className="text-gray-700">{service.name}</span>
                  </div>
                  {service.responseTime && (
                    <span className="text-gray-500">{service.responseTime}ms</span>
                  )}
                </div>
              ))}
              
              {/* API Health - Only show if different from API Gateway service check */}
              {healthStatus.find(s => s.name === 'API Gateway')?.status !== (apiHealth ? 'healthy' : 'unhealthy') && (
                <div className="flex items-center justify-between text-xs pt-1 border-t">
                  <div className="flex items-center space-x-2">
                    {apiHealth === true ? (
                      <CheckCircle className="w-3 h-3 text-green-500" />
                    ) : apiHealth === false ? (
                      <XCircle className="w-3 h-3 text-red-500" />
                    ) : (
                      <Loader2 className="w-3 h-3 animate-spin text-gray-400" />
                    )}
                    <span className="text-gray-700">API Gateway (Connectivity)</span>
                  </div>
                </div>
              )}
            </div>

            {/* Last Checked */}
            {lastChecked && (
              <div className="text-xs text-gray-400 pt-2 border-t">
                Last checked: {lastChecked.toLocaleTimeString()}
              </div>
            )}

            {/* Help Text */}
            {!allHealthy && (
              <div className="text-xs text-amber-600 bg-amber-50 p-2 rounded mt-2">
                <p className="font-medium mb-1">Services not running?</p>
                <p>Start backend services:</p>
                <code className="block mt-1 text-xs bg-white p-1 rounded">
                  cd ../backend<br />
                  .\scripts\start-services.ps1
                </code>
              </div>
            )}
          </>
        )}
      </CardContent>
    </Card>
  );
}
