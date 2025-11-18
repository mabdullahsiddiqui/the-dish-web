import { Search } from 'lucide-react';
import { Place } from '@/types/place';
import { PlaceCard } from '@/components/features/places/place-card';
import { Button } from '@/components/ui/button';

interface SearchResultsProps {
  places: Place[];
  totalCount: number;
  isLoading: boolean;
  error?: any;
  onRetry?: () => void;
  viewMode?: 'grid' | 'list';
}

export function SearchResults({ 
  places, 
  totalCount, 
  isLoading, 
  error, 
  onRetry,
  viewMode = 'grid' 
}: SearchResultsProps) {
  if (isLoading) {
    return (
      <div className={`grid gap-6 ${
        viewMode === 'grid' 
          ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
          : 'grid-cols-1'
      }`}>
        {[...Array(6)].map((_, i) => (
          <div key={i} className="border rounded-lg p-4 animate-pulse">
            <div className="h-48 bg-gray-200 rounded mb-4"></div>
            <div className="space-y-2">
              <div className="h-4 bg-gray-200 rounded w-3/4"></div>
              <div className="h-4 bg-gray-200 rounded w-1/2"></div>
              <div className="h-4 bg-gray-200 rounded w-2/3"></div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-12">
        <div className="text-red-500 mb-4">
          <p className="text-lg">Error loading search results</p>
          <p className="text-sm">{error.message || 'Please try again later'}</p>
        </div>
        {onRetry && (
          <Button onClick={onRetry}>Try Again</Button>
        )}
      </div>
    );
  }

  if (places.length === 0) {
    return (
      <div className="text-center py-12">
        <Search className="w-12 h-12 mx-auto mb-4 text-gray-300" />
        <p className="text-lg text-gray-500 mb-2">No restaurants found</p>
        <p className="text-sm text-gray-400">
          Try adjusting your search terms or filters
        </p>
      </div>
    );
  }

  return (
    <div className={`grid gap-6 ${
      viewMode === 'grid' 
        ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
        : 'grid-cols-1 max-w-4xl'
    }`}>
      {places.map((place) => (
        <PlaceCard key={place.id} place={place} />
      ))}
    </div>
  );
}

