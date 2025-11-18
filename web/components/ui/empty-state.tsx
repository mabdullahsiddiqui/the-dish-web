import { ReactNode } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Search, Utensils, MapPin } from 'lucide-react';

interface EmptyStateProps {
  icon?: ReactNode;
  title: string;
  description: string;
  action?: {
    label: string;
    onClick: () => void;
  };
  children?: ReactNode;
  variant?: 'default' | 'search' | 'places';
}

export function EmptyState({ 
  icon, 
  title, 
  description, 
  action, 
  children,
  variant = 'default'
}: EmptyStateProps) {
  const getDefaultIcon = () => {
    switch (variant) {
      case 'search':
        return <Search className="w-16 h-16 text-gray-300" />;
      case 'places':
        return <Utensils className="w-16 h-16 text-gray-300" />;
      default:
        return icon || <MapPin className="w-16 h-16 text-gray-300" />;
    }
  };

  return (
    <Card className="border-dashed border-2">
      <CardContent className="flex flex-col items-center text-center py-16 px-8">
        <div className="mb-6">
          {icon || getDefaultIcon()}
        </div>
        
        <h3 className="text-2xl font-bold text-gray-900 mb-3">{title}</h3>
        <p className="text-gray-600 mb-8 max-w-md text-lg leading-relaxed">
          {description}
        </p>
        
        {variant === 'search' && (
          <div className="mb-6 p-4 bg-blue-50 rounded-lg border border-blue-100 max-w-md">
            <p className="text-sm text-blue-800">
              <strong>Tip:</strong> Try searching with different keywords, or browse by cuisine type or dietary preferences.
            </p>
          </div>
        )}
        
        {action && (
          <Button 
            onClick={action.onClick}
            size="lg"
            className="min-w-[150px]"
          >
            {action.label}
          </Button>
        )}
        
        {children}
      </CardContent>
    </Card>
  );
}

