'use client'

import { useState, useEffect, Suspense } from 'react';
import { useSearchParams } from 'next/navigation';
import { Search, Grid, List, SlidersHorizontal } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent } from '@/components/ui/card';
import { SearchFilters, SearchFilters as SearchFiltersType } from '@/components/features/search/search-filters';
import { PlaceCard } from '@/components/features/places/place-card';
import { LoadingSpinner, LoadingSkeleton } from '@/components/ui/loading';
import { Pagination } from '@/components/ui/pagination';
import { EmptyState } from '@/components/ui/empty-state';
import { ErrorState } from '@/components/ui/error-state';
import { usePlaces } from '@/hooks/usePlaces';
import { SearchPlacesRequest } from '@/types/place';

function SearchPageContent() {
  const searchParams = useSearchParams();
  const { useSearchPlaces } = usePlaces();

  // State
  const [searchQuery, setSearchQuery] = useState(searchParams?.get('q') || '');
  const [currentPage, setCurrentPage] = useState(1);
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [showFilters, setShowFilters] = useState(false);
  const [filters, setFilters] = useState<SearchFiltersType>({
    cuisineTypes: [],
    dietaryTags: [],
    priceRange: [1, 4],
    minRating: 0,
    radiusKm: 25,
  });
  const [userLocation, setUserLocation] = useState<{ latitude: number; longitude: number } | null>(null);

  // Get user location for location-based search
  useEffect(() => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setUserLocation({
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
          });
        },
        (error) => {
          console.log('Location access denied or unavailable:', error);
        }
      );
    }
  }, []);

  // Build search params
  const searchRequest: SearchPlacesRequest = {
    searchTerm: searchQuery.trim() || undefined,
    cuisineTypes: filters.cuisineTypes.length > 0 ? filters.cuisineTypes : undefined,
    dietaryTags: filters.dietaryTags.length > 0 ? filters.dietaryTags : undefined,
    minPriceRange: filters.priceRange[0] !== 1 ? filters.priceRange[0] : undefined,
    maxPriceRange: filters.priceRange[1] !== 4 ? filters.priceRange[1] : undefined,
    minRating: filters.minRating > 0 ? filters.minRating : undefined,
    latitude: userLocation?.latitude,
    longitude: userLocation?.longitude,
    radiusKm: userLocation ? filters.radiusKm : undefined,
    page: currentPage,
    pageSize: 20,
  };

  // Search query
  const { data: searchResponse, isLoading, error, refetch } = useSearchPlaces(searchRequest);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setCurrentPage(1); // Reset to first page
    refetch();
  };

  const handleFiltersChange = (newFilters: SearchFiltersType) => {
    setFilters(newFilters);
    setCurrentPage(1); // Reset to first page when filters change
  };

  const places = searchResponse?.success && searchResponse.data ? searchResponse.data.places : [];
  const totalCount = searchResponse?.success && searchResponse.data ? searchResponse.data.totalCount : 0;
  const totalPages = searchResponse?.success && searchResponse.data ? searchResponse.data.totalPages : 0;

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">Search Restaurants</h1>
        
        {/* Search Bar */}
        <form onSubmit={handleSearch} className="flex gap-4 mb-6">
          <div className="flex-1 relative">
            <Input
              type="text"
              placeholder="Search restaurants, cuisines, or dishes..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-10"
            />
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
          </div>
          <Button type="submit">Search</Button>
        </form>

        {/* Controls */}
        <div className="flex justify-between items-center">
          <div className="flex items-center space-x-4">
            <SearchFilters
              onFiltersChange={handleFiltersChange}
              initialFilters={filters}
            />
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setShowFilters(!showFilters)}
              className="md:hidden"
            >
              <SlidersHorizontal className="w-4 h-4 mr-2" />
              Filters
            </Button>
          </div>

          <div className="flex items-center space-x-2">
            <span className="text-sm text-gray-600">
              {totalCount > 0 && `${totalCount} result${totalCount !== 1 ? 's' : ''}`}
            </span>
            <div className="border rounded flex">
              <Button
                variant={viewMode === 'grid' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => setViewMode('grid')}
              >
                <Grid className="w-4 h-4" />
              </Button>
              <Button
                variant={viewMode === 'list' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => setViewMode('list')}
              >
                <List className="w-4 h-4" />
              </Button>
            </div>
          </div>
        </div>
      </div>

      {/* Results */}
      <div className="space-y-6">
        {isLoading ? (
          <div className={`grid gap-6 ${
            viewMode === 'grid' 
              ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
              : 'grid-cols-1'
          }`}>
            {[...Array(6)].map((_, i) => (
              <Card key={i}>
                <CardContent className="p-0">
                  <div className="h-48 bg-gray-200 animate-pulse rounded-t-lg"></div>
                  <div className="p-4 space-y-3">
                    <div className="h-4 bg-gray-200 animate-pulse rounded w-3/4"></div>
                    <div className="h-4 bg-gray-200 animate-pulse rounded w-1/2"></div>
                    <div className="h-4 bg-gray-200 animate-pulse rounded w-2/3"></div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        ) : error ? (
          <ErrorState
            type="error"
            title="Error loading search results"
            message={error.message || 'Please try again later'}
            action={{
              label: 'Try Again',
              onClick: () => refetch(),
            }}
          />
        ) : places.length === 0 ? (
          <EmptyState
            variant="search"
            title="No Restaurants Found"
            description="We couldn't find any restaurants matching your search. Try adjusting your filters or search terms to discover great places near you."
            action={{
              label: 'Clear Search',
              onClick: () => {
                setSearchQuery('');
                setFilters({
                  cuisineTypes: [],
                  dietaryTags: [],
                  priceRange: [1, 4],
                  minRating: 0,
                  radiusKm: 25,
                });
                setCurrentPage(1);
              },
            }}
          />
        ) : (
          <>
            {/* Results Grid */}
            <div className={`grid gap-6 ${
              viewMode === 'grid' 
                ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3' 
                : 'grid-cols-1 max-w-4xl'
            }`}>
              {places.map((place) => (
                <PlaceCard key={place.id} place={place} />
              ))}
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="mt-8">
                <Pagination
                  currentPage={currentPage}
                  totalPages={totalPages}
                  onPageChange={setCurrentPage}
                />
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}

export default function SearchPage() {
  return (
    <Suspense fallback={<div className="flex justify-center py-8"><LoadingSpinner /></div>}>
      <SearchPageContent />
    </Suspense>
  );
}
