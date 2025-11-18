import { ReactNode } from 'react';
import { AlertTriangle, RefreshCw, Wifi } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';

interface ErrorStateProps {
  title?: string;
  message: string;
  action?: {
    label: string;
    onClick: () => void;
  };
  type?: 'error' | 'network' | 'not-found';
  children?: ReactNode;
}

export function ErrorState({ 
  title,
  message, 
  action, 
  type = 'error',
  children 
}: ErrorStateProps) {
  const getIcon = () => {
    switch (type) {
      case 'network':
        return <Wifi className="w-12 h-12 text-gray-400" />;
      case 'not-found':
        return <div className="text-6xl text-gray-300">404</div>;
      default:
        return <AlertTriangle className="w-12 h-12 text-red-400" />;
    }
  };

  const getDefaultTitle = () => {
    switch (type) {
      case 'network':
        return 'Connection Problem';
      case 'not-found':
        return 'Not Found';
      default:
        return 'Something went wrong';
    }
  };

  return (
    <Card className="border-dashed">
      <CardContent className="flex flex-col items-center text-center py-12">
        <div className="mb-4">
          {getIcon()}
        </div>
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          {title || getDefaultTitle()}
        </h3>
        <p className="text-gray-600 mb-6 max-w-sm">{message}</p>
        
        {action && (
          <Button onClick={action.onClick} className="flex items-center space-x-2">
            <RefreshCw className="w-4 h-4" />
            <span>{action.label}</span>
          </Button>
        )}
        
        {children}
      </CardContent>
    </Card>
  );
}

