'use client'

import { useState, useEffect, Suspense } from 'react';
import { useRouter } from 'next/navigation';
import { Grid, List, MapPin, TrendingUp } from 'lucide-react';
import { PlaceCard3D } from '@/components/cards/PlaceCard3D';
import { FilterPanel } from '@/components/search/FilterPanel';
import { Button3D } from '@/components/ui/Button3D';
import { GlassCard } from '@/components/cards/GlassCard';
import { StaggerChildren } from '@/components/animations/StaggerChildren';
import { LoadingSpinner } from '@/components/ui/loading';
import { PlaceCardSkeleton } from '@/components/ui/PlaceCardSkeleton';
import { Pagination } from '@/components/ui/pagination';
import { EmptyState } from '@/components/ui/empty-state';
import { ErrorState } from '@/components/ui/error-state';
import { usePlaces } from '@/hooks/usePlaces';
import { SearchPlacesRequest } from '@/types/place';
import { mapPlaceToCard3D } from '@/lib/utils/place-mapper';
import { SearchFilters as SearchFiltersType } from '@/components/features/search/search-filters';

function PlacesPageContent() {
  const { useSearchPlaces } = usePlaces();
  const router = useRouter();

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
    <div className="min-h-screen bg-[#0f172a]">
      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Controls Bar */}
        <GlassCard className="p-4 mb-6">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            {/* Left: Filters */}
            <div className="flex items-center gap-3 flex-wrap">
              <Button3D
                variant="outline"
                size="sm"
                onClick={() => setShowFilters(!showFilters)}
              >
                Filters {activeFiltersCount > 0 && `(${activeFiltersCount})`}
              </Button3D>
              {activeFiltersCount > 0 && (
                <Button3D
                  variant="outline"
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
                >
                  Clear all
                </Button3D>
              )}
            </div>

            {/* Right: View Toggle & Results Count */}
            <div className="flex items-center gap-4">
              {!isLoading && places.length > 0 && (
                <div className="hidden sm:block text-sm text-gray-300">
                  <span className="font-medium text-white">{totalCount}</span>{' '}
                  {totalCount === 1 ? 'restaurant' : 'restaurants'} found
                </div>
              )}
              <div className="flex items-center gap-1 glass-card p-1 rounded-lg">
                <button
                  onClick={() => setViewMode('grid')}
                  className={`p-2 rounded transition-colors ${
                    viewMode === 'grid'
                      ? 'bg-indigo-500 text-white'
                      : 'text-gray-300 hover:text-white'
                  }`}
                >
                  <Grid className="w-4 h-4" />
                </button>
                <button
                  onClick={() => setViewMode('list')}
                  className={`p-2 rounded transition-colors ${
                    viewMode === 'list'
                      ? 'bg-indigo-500 text-white'
                      : 'text-gray-300 hover:text-white'
                  }`}
                >
                  <List className="w-4 h-4" />
                </button>
              </div>
            </div>
          </div>
        </GlassCard>

        {/* Filter Panel */}
        {showFilters && (
          <div className="mb-6">
            <FilterPanel
              filters={[
                {
                  title: 'Cuisine Types',
                  options: [
                    { label: 'Middle Eastern', value: 'middle-eastern' },
                    { label: 'Asian', value: 'asian' },
                    { label: 'Mediterranean', value: 'mediterranean' },
                  ],
                  selected: filters.cuisineTypes || [],
                  onSelectionChange: (values) => {
                    handleFiltersChange({ ...filters, cuisineTypes: values });
                  },
                },
                {
                  title: 'Dietary Tags',
                  options: [
                    { label: 'Halal', value: 'Halal' },
                    { label: 'Vegan', value: 'Vegan' },
                    { label: 'Vegetarian', value: 'Vegetarian' },
                  ],
                  selected: filters.dietaryTags || [],
                  onSelectionChange: (values) => {
                    handleFiltersChange({ ...filters, dietaryTags: values });
                  },
                },
              ]}
              onClose={() => setShowFilters(false)}
              onApply={() => setShowFilters(false)}
            />
          </div>
        )}

        {/* Results Section */}
        <div className="space-y-6">
          {isLoading ? (
            <div className={`grid gap-6 ${
              viewMode === 'grid' 
                ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4' 
                : 'grid-cols-1 max-w-4xl'
            }`}>
              {[...Array(8)].map((_, i) => (
                <PlaceCardSkeleton key={i} />
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
              <StaggerChildren
                className={`grid gap-6 ${
                  viewMode === 'grid'
                    ? 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4'
                    : 'grid-cols-1 max-w-4xl'
                }`}
              >
                {places.map((place) => (
                  <PlaceCard3D
                    key={place.id}
                    place={mapPlaceToCard3D(place)}
                    onClick={() => router.push(`/places/${place.id}`)}
                  />
                ))}
              </StaggerChildren>

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
      <div className="min-h-screen bg-[#0f172a] flex items-center justify-center">
        <LoadingSpinner />
      </div>
    }>
      <PlacesPageContent />
    </Suspense>
  );
}

