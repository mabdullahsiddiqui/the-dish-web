'use client'

import { useState, useEffect, Suspense } from 'react';
import { useRouter } from 'next/navigation';
import { MapPin, Star, Utensils, TrendingUp, Navigation } from 'lucide-react';
import { HeroWith3D } from '@/components/3d/HeroWith3D';
import { AnimatedSearchBar } from '@/components/search/AnimatedSearchBar';
import { TiltCard } from '@/components/cards/TiltCard';
import { GlassCard } from '@/components/cards/GlassCard';
import { Button3D } from '@/components/ui/Button3D';
import { Badge3D } from '@/components/ui/Badge3D';
import { AnimatedIcon } from '@/components/ui/AnimatedIcon';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { StaggerChildren } from '@/components/animations/StaggerChildren';
import { PlaceCard3D } from '@/components/cards/PlaceCard3D';
import { usePlaces } from '@/hooks/usePlaces';
import { useReviews } from '@/hooks/useReviews';
import { useAuth } from '@/hooks/useAuth';
import { mapPlaceToCard3D } from '@/lib/utils/place-mapper';
import { LoadingSpinner } from '@/components/ui/loading';
import { PlaceCardSkeleton } from '@/components/ui/PlaceCardSkeleton';
import Link from 'next/link';

function HomePageContent() {
  const [searchQuery, setSearchQuery] = useState('');
  const [userLocation, setUserLocation] = useState<{ latitude: number; longitude: number } | null>(null);
  const router = useRouter();
  const { useSearchPlaces, useNearbyPlaces } = usePlaces();
  const { useRecentReviews } = useReviews();
  const { isAuthenticated } = useAuth();

  // Get user location
  useEffect(() => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setUserLocation({
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
          });
        },
        () => {
          // Location access denied or unavailable
        }
      );
    }
  }, []);

  // Fetch trending places (highly rated with many reviews)
  const { data: trendingResponse, isLoading: trendingLoading } = useSearchPlaces({
    minRating: 4.0,
    page: 1,
    pageSize: 6,
  });

  // Fetch nearby places if location available
  const { data: nearbyResponse, isLoading: nearbyLoading } = useNearbyPlaces({
    latitude: userLocation?.latitude || 0,
    longitude: userLocation?.longitude || 0,
    radiusKm: 5,
  });

  // Fetch recent reviews
  const { data: recentReviewsResponse, isLoading: reviewsLoading } = useRecentReviews(1, 6);

  const handleSearch = (value: string) => {
    if (value.trim()) {
      router.push(`/search?q=${encodeURIComponent(value.trim())}`);
    }
  };

  const trendingPlaces = trendingResponse?.success && trendingResponse.data ? trendingResponse.data.places : [];
  const nearbyPlaces = nearbyResponse?.success && nearbyResponse.data ? nearbyResponse.data : [];
  const recentReviews = recentReviewsResponse?.success && recentReviewsResponse.data ? recentReviewsResponse.data.reviews : [];

  return (
    <div className="min-h-screen bg-[#0f172a]">
      {/* Hero Section */}
      <HeroWith3D
        title="Discover Your Next Favorite Dish"
        subtitle="Find halal restaurants that match your dietary preferences and taste buds"
        ctaText="Start Exploring"
        particleColor="#10b981"
        onCtaClick={() => router.push('/places')}
      />

      {/* Search Bar Section */}
      <section className="py-8 bg-[#0f172a]">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <AnimatedSearchBar
            placeholder="Search restaurants, cuisines, or dishes..."
            value={searchQuery}
            onChange={setSearchQuery}
            onSearch={handleSearch}
          />
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16 bg-[#0f172a]">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <FadeInUp>
            <div className="text-center mb-12">
              <h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
                Why Choose The Dish?
              </h2>
              <p className="text-xl text-gray-300">
                More than just reviews - find places that truly match your preferences
              </p>
            </div>
          </FadeInUp>

          <StaggerChildren className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <TiltCard>
              <GlassCard hover className="p-6 text-center h-full">
                <div className="w-16 h-16 bg-indigo-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                  <AnimatedIcon name="utensils" size={32} color="#6366f1" />
                </div>
                <h3 className="text-xl font-semibold mb-2 text-white">Dietary Preferences</h3>
                <p className="text-gray-300">
                  Filter by halal, kosher, vegan, gluten-free, and more. Find restaurants that cater to your specific needs.
                </p>
              </GlassCard>
            </TiltCard>

            <TiltCard>
              <GlassCard hover className="p-6 text-center h-full">
                <div className="w-16 h-16 bg-green-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                  <AnimatedIcon name="mapPin" size={32} color="#10b981" />
                </div>
                <h3 className="text-xl font-semibold mb-2 text-white">GPS Verified</h3>
                <p className="text-gray-300">
                  All reviews are GPS-verified to ensure authenticity. Real reviews from real visitors.
                </p>
              </GlassCard>
            </TiltCard>

            <TiltCard>
              <GlassCard hover className="p-6 text-center h-full">
                <div className="w-16 h-16 bg-amber-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                  <AnimatedIcon name="star" size={32} color="#f59e0b" />
                </div>
                <h3 className="text-xl font-semibold mb-2 text-white">Community Driven</h3>
                <p className="text-gray-300">
                  Join a community of food lovers sharing honest reviews and recommendations.
                </p>
              </GlassCard>
            </TiltCard>
          </StaggerChildren>
        </div>
      </section>

      {/* Trending Places Section */}
      {trendingPlaces.length > 0 && (
        <section className="py-16 bg-[#1e293b]">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <FadeInUp>
              <div className="flex items-center justify-between mb-12">
                <div>
                  <h2 className="text-3xl md:text-4xl font-bold text-white mb-4 flex items-center space-x-2">
                    <TrendingUp className="w-8 h-8 text-indigo-400" />
                    <span>Trending Places</span>
                  </h2>
                  <p className="text-xl text-gray-300">
                    Highly rated restaurants loved by our community
                  </p>
                </div>
                <Button3D variant="outline" onClick={() => router.push('/places')}>
                  View All
                </Button3D>
              </div>
            </FadeInUp>

            {trendingLoading ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {[...Array(6)].map((_, i) => (
                  <PlaceCardSkeleton key={i} />
                ))}
              </div>
            ) : (
              <StaggerChildren className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {trendingPlaces.slice(0, 6).map((place) => (
                  <PlaceCard3D
                    key={place.id}
                    place={mapPlaceToCard3D(place)}
                    onClick={() => router.push(`/places/${place.id}`)}
                  />
                ))}
              </StaggerChildren>
            )}
          </div>
        </section>
      )}

      {/* Nearby Places Section */}
      {userLocation && nearbyPlaces.length > 0 && (
        <section className="py-16 bg-[#0f172a]">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <FadeInUp>
              <div className="flex items-center justify-between mb-12">
                <div>
                  <h2 className="text-3xl md:text-4xl font-bold text-white mb-4 flex items-center space-x-2">
                    <Navigation className="w-8 h-8 text-green-400" />
                    <span>Nearby Places</span>
                  </h2>
                  <p className="text-xl text-gray-300">
                    Great restaurants close to you
                  </p>
                </div>
                <Button3D variant="outline" onClick={() => router.push('/places')}>
                  View All
                </Button3D>
              </div>
            </FadeInUp>

            {nearbyLoading ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {[...Array(6)].map((_, i) => (
                  <PlaceCardSkeleton key={i} />
                ))}
              </div>
            ) : (
              <StaggerChildren className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {nearbyPlaces.slice(0, 6).map((place) => (
                  <PlaceCard3D
                    key={place.id}
                    place={mapPlaceToCard3D(place)}
                    onClick={() => router.push(`/places/${place.id}`)}
                  />
                ))}
              </StaggerChildren>
            )}
          </div>
        </section>
      )}

      {/* Recent Reviews Section */}
      <section className={`py-16 ${userLocation && nearbyPlaces.length > 0 ? 'bg-[#1e293b]' : 'bg-[#1e293b]'}`}>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <FadeInUp>
            <div className="text-center mb-12">
              <h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
                Recent Reviews
              </h2>
              <p className="text-xl text-gray-300">
                See what our community is discovering
              </p>
            </div>
          </FadeInUp>

          {reviewsLoading ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {[...Array(6)].map((_, i) => (
                <div key={i} className="animate-pulse">
                  <div className="h-32 bg-gray-700 rounded"></div>
                </div>
              ))}
            </div>
          ) : recentReviews.length > 0 ? (
            <StaggerChildren className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {recentReviews.map((review) => (
                <TiltCard key={review.id}>
                  <GlassCard hover className="p-6 h-full">
                    <div className="flex items-center mb-3">
                      <div className="flex">
                        {[...Array(5)].map((_, i) => (
                          <Star
                            key={i}
                            className={`w-4 h-4 ${
                              i < review.rating ? 'text-yellow-400 fill-current' : 'text-gray-500'
                            }`}
                          />
                        ))}
                      </div>
                      <span className="ml-2 text-sm text-gray-400">
                        {new Date(review.createdAt).toLocaleDateString()}
                      </span>
                    </div>
                    <p className="text-gray-200 mb-3 line-clamp-3">
                      {review.text}
                    </p>
                    <div className="text-sm text-gray-400 mb-3">
                      <span className="font-medium text-white">
                        {review.user ? `${review.user.firstName} ${review.user.lastName}` : 'Anonymous'}
                      </span>
                      {' reviewed '}
                      <Link
                        href={`/places/${review.placeId}`}
                        className="font-medium text-indigo-400 hover:text-indigo-300"
                      >
                        Place
                      </Link>
                    </div>
                    <div className="flex items-center justify-between">
                      <div className="flex gap-2">
                        {review.isGpsVerified && (
                          <Badge3D variant="custom" className="text-xs">
                            GPS Verified
                          </Badge3D>
                        )}
                      </div>
                      <div className="text-xs text-gray-400">
                        {review.helpfulCount} helpful
                      </div>
                    </div>
                  </GlassCard>
                </TiltCard>
              ))}
            </StaggerChildren>
          ) : (
            <div className="text-center text-gray-400 py-8">
              <p>No recent reviews yet</p>
            </div>
          )}

          <FadeInUp delay={0.4}>
            <div className="text-center mt-8">
              <Button3D variant="outline" onClick={() => router.push('/places')}>
                View All Places
              </Button3D>
            </div>
          </FadeInUp>
        </div>
      </section>

      {/* Call to Action */}
      <section className="py-16 bg-[#0f172a]">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <FadeInUp>
            <h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
              Ready to Discover Great Food?
            </h2>
            <p className="text-xl text-gray-300 mb-8">
              Join our community and start exploring restaurants near you
            </p>
          </FadeInUp>
          <FadeInUp delay={0.2}>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button3D size="lg" onClick={() => router.push('/register')} glow>
                Sign Up Now
              </Button3D>
              <Button3D variant="outline" size="lg" onClick={() => router.push('/places')}>
                Explore Restaurants
              </Button3D>
            </div>
          </FadeInUp>
        </div>
      </section>
    </div>
  );
}

export default function HomePage() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <HomePageContent />
    </Suspense>
  );
}
