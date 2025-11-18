'use client'

import { useState, useEffect, Suspense } from 'react';
import { Grid, List, SlidersHorizontal, MapPin, Sparkles, TrendingUp } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { SearchFilters, SearchFilters as SearchFiltersType } from '@/components/features/search/search-filters';
import { PlaceCard } from '@/components/features/places/place-card';
import { LoadingSpinner, LoadingSkeleton } from '@/components/ui/loading';
import { Pagination } from '@/components/ui/pagination';
import { EmptyState } from '@/components/ui/empty-state';
import { ErrorState } from '@/components/ui/error-state';
import { usePlaces } from '@/hooks/usePlaces';
import { SearchPlacesRequest } from '@/types/place';

function PlacesPageContent() {
  const { useSearchPlaces } = usePlaces();

  // State
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

  // Build search params (no search term - show all places)
  const searchRequest: SearchPlacesRequest = {
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

  const handleFiltersChange = (newFilters: SearchFiltersType) => {
    setFilters(newFilters);
    setCurrentPage(1); // Reset to first page when filters change
  };

  const places = searchResponse?.success && searchResponse.data ? searchResponse.data.places : [];
  const totalCount = searchResponse?.success && searchResponse.data ? searchResponse.data.totalCount : 0;
  const totalPages = searchResponse?.success && searchResponse.data ? searchResponse.data.totalPages : 0;

  const activeFiltersCount = 
    filters.cuisineTypes.length + 
    filters.dietaryTags.length + 
    (filters.minRating > 0 ? 1 : 0) + 
    (filters.priceRange[0] !== 1 || filters.priceRange[1] !== 4 ? 1 : 0) +
    (filters.radiusKm !== 25 ? 1 : 0);

  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50 to-white">
      {/* Hero Section */}
      <div className="bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
          <div className="text-center max-w-3xl mx-auto">
            <div className="flex items-center justify-center gap-2 mb-4">
              <Sparkles className="w-6 h-6 text-yellow-300" />
              <h1 className="text-4xl md:text-5xl lg:text-6xl font-bold">
                Discover Amazing Restaurants
              </h1>
              <Sparkles className="w-6 h-6 text-yellow-300" />
            </div>
            <p className="text-xl md:text-2xl text-blue-100 mb-8">
              Explore a curated collection of restaurants that match your taste and dietary preferences
            </p>
            <div className="flex flex-wrap items-center justify-center gap-4 text-sm">
              <div className="flex items-center gap-2 bg-white/10 backdrop-blur-sm rounded-full px-4 py-2">
                <MapPin className="w-4 h-4" />
                <span>{totalCount} Restaurants</span>
              </div>
              <div className="flex items-center gap-2 bg-white/10 backdrop-blur-sm rounded-full px-4 py-2">
                <TrendingUp className="w-4 h-4" />
                <span>Verified Reviews</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Controls Bar */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-4 mb-6">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            {/* Left: Filters */}
            <div className="flex items-center gap-3 flex-wrap">
              <SearchFilters
                onFiltersChange={handleFiltersChange}
                initialFilters={filters}
              />
              {activeFiltersCount > 0 && (
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => {
                    setFilters({
                      cuisineTypes: [],
                      dietaryTags: [],
                      priceRange: [1, 4],
                      minRating: 0,
                      radiusKm: 25,
                    });
                    setCurrentPage(1);
                  }}
                  className="text-gray-600 hover:text-gray-900"
                >
                  Clear all
                </Button>
              )}
            </div>

            {/* Right: View Toggle & Results Count */}
            <div className="flex items-center gap-4">
              {!isLoading && places.length > 0 && (
                <div className="hidden sm:block text-sm text-gray-600">
                  <span className="font-medium text-gray-900">{totalCount}</span>{' '}
                  {totalCount === 1 ? 'restaurant' : 'restaurants'} found
                </div>
              )}
              <div className="flex items-center gap-1 bg-gray-100 rounded-lg p-1">
                <Button
                  variant={viewMode === 'grid' ? 'default' : 'ghost'}
                  size="sm"
                  onClick={() => setViewMode('grid')}
                  className="px-3"
                >
                  <Grid className="w-4 h-4" />
                </Button>
                <Button
                  variant={viewMode === 'list' ? 'default' : 'ghost'}
                  size="sm"
                  onClick={() => setViewMode('list')}
                  className="px-3"
                >
                  <List className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </div>
        </div>

        {/* Results Section */}
        <div className="space-y-6">
          {isLoading ? (
            <div className={`grid gap-6 ${
              viewMode === 'grid' 
                ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4' 
                : 'grid-cols-1 max-w-4xl'
            }`}>
              {[...Array(8)].map((_, i) => (
                <LoadingSkeleton key={i} />
              ))}
            </div>
          ) : error ? (
            <div className="py-12">
              <ErrorState
                type="error"
                title="Error loading restaurants"
                message={error.message || 'Please try again later'}
                action={{
                  label: 'Try Again',
                  onClick: () => refetch(),
                }}
              />
            </div>
          ) : places.length === 0 ? (
            <div className="py-12">
              <EmptyState
                variant="places"
                title="No Restaurants Found"
                description="We couldn't find any restaurants matching your filters. Try adjusting your filters to discover great places near you."
                action={{
                  label: 'Clear Filters',
                  onClick: () => {
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
            </div>
          ) : (
            <>
              {/* Results Grid */}
              <div className={`grid gap-6 ${
                viewMode === 'grid' 
                  ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4' 
                  : 'grid-cols-1 max-w-4xl'
              }`}>
                {places.map((place, index) => (
                  <div
                    key={place.id}
                    className="opacity-0 animate-[fadeIn_0.5s_ease-in-out_forwards]"
                    style={{ animationDelay: `${index * 50}ms` }}
                  >
                    <PlaceCard place={place} />
                  </div>
                ))}
              </div>

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="mt-12 flex justify-center">
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
    </div>
  );
}

export default function PlacesPage() {
  return (
    <Suspense fallback={
      <div className="min-h-screen bg-gradient-to-b from-gray-50 to-white flex items-center justify-center">
        <LoadingSpinner />
      </div>
    }>
      <PlacesPageContent />
    </Suspense>
  );
}

