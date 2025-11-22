'use client'

import { useState, useMemo } from 'react';
import { notFound, useParams } from 'next/navigation';
import { MapPin, Phone, Globe, Clock, Star, User, Filter, ArrowUpDown } from 'lucide-react';
import { Button3D } from '@/components/ui/Button3D';
import { GlassCard } from '@/components/cards/GlassCard';
import { Badge3D } from '@/components/ui/Badge3D';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { StaggerChildren } from '@/components/animations/StaggerChildren';
import { usePlaces } from '@/hooks/usePlaces';
import { useReviews } from '@/hooks/useReviews';
import { useAuth } from '@/hooks/useAuth';
import { ReviewHelpfulButtons } from '@/components/features/reviews/review-helpful-buttons';
import { PlaceMap } from '@/components/features/places/place-map';
import { Review } from '@/types/review';
import { getReputationLevelColor, getReputationLevelBgColor } from '@/lib/utils/reputation';
import toast from 'react-hot-toast';
import Link from 'next/link';

type SortOption = 'recent' | 'helpful' | 'highest' | 'lowest';
type FilterOption = 'all' | '5' | '4' | '3' | '2' | '1';

export default function PlaceDetailPage() {
  const params = useParams();
  const id = params.id as string;
  const { usePlace } = usePlaces();
  const { usePlaceReviews } = useReviews();
  const { isAuthenticated } = useAuth();

  const [sortBy, setSortBy] = useState<SortOption>('recent');
  const [filterBy, setFilterBy] = useState<FilterOption>('all');

  const { data: placeResponse, isLoading: placeLoading, error: placeError } = usePlace(id);
  const { data: reviewsResponse, isLoading: reviewsLoading } = usePlaceReviews(id, 1, 100);

  // Get place and reviews data (safe defaults)
  const place = placeResponse?.success && placeResponse.data ? placeResponse.data : null;
  const allReviews = reviewsResponse?.success && reviewsResponse.data ? reviewsResponse.data.reviews : [];

  // Filter and sort reviews - MUST be called before any conditional returns
  const reviews = useMemo(() => {
    let filtered = [...allReviews];

    // Filter by rating
    if (filterBy !== 'all') {
      const rating = parseInt(filterBy);
      filtered = filtered.filter((review) => review.rating === rating);
    }

    // Sort reviews
    filtered.sort((a, b) => {
      switch (sortBy) {
        case 'recent':
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        case 'helpful':
          const aRatio = a.helpfulCount / (a.helpfulCount + a.notHelpfulCount || 1);
          const bRatio = b.helpfulCount / (b.helpfulCount + b.notHelpfulCount || 1);
          if (aRatio !== bRatio) return bRatio - aRatio;
          return b.helpfulCount - a.helpfulCount;
        case 'highest':
          return b.rating - a.rating;
        case 'lowest':
          return a.rating - b.rating;
        default:
          return 0;
      }
    });

    return filtered;
  }, [allReviews, sortBy, filterBy]);

  // Now we can do conditional returns AFTER all hooks
  if (placeLoading) {
    return (
      <div className="min-h-screen bg-[#0f172a]">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-700 rounded w-1/3 mb-4"></div>
            <div className="h-64 bg-gray-700 rounded mb-6"></div>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
              <div className="lg:col-span-2 space-y-4">
                <div className="h-4 bg-gray-700 rounded w-3/4"></div>
                <div className="h-4 bg-gray-700 rounded w-1/2"></div>
              </div>
              <div className="h-32 bg-gray-700 rounded"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (placeError || !placeResponse?.success || !place) {
    return notFound();
  }

  const getPriceRangeText = (priceRange: number) => {
    return '$'.repeat(priceRange);
  };

  return (
    <div className="min-h-screen bg-[#0f172a]">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header Section */}
        <FadeInUp>
          <div className="mb-8">
            <div className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-4">
              <div>
                <h1 className="text-3xl font-bold text-white mb-2">{place.name}</h1>
                <div className="flex items-center text-gray-300 mb-2">
                  <MapPin className="w-4 h-4 mr-1" />
                  <span>{place.address}</span>
                </div>
                <div className="flex items-center space-x-4 flex-wrap">
                  <div className="flex items-center">
                    <Star className="w-5 h-5 text-yellow-400 fill-current mr-1" />
                    <span className="font-semibold text-white">{place.averageRating.toFixed(1)}</span>
                    <span className="text-gray-300 ml-1">({place.reviewCount} reviews)</span>
                  </div>
                  <span className="text-gray-300">{getPriceRangeText(place.priceRange)}</span>
                  {place.isVerified && (
                    <Badge3D variant="verified" />
                  )}
                </div>
              </div>
              
              <div className="flex flex-col sm:flex-row gap-2">
                {isAuthenticated && (
                  <Link href={`/places/${id}/review`}>
                    <Button3D variant="primary" size="md">Write Review</Button3D>
                  </Link>
                )}
                <Button3D variant="outline" size="md">Share</Button3D>
                {!place.isClaimed && (
                  <Button3D variant="outline" size="md">Claim Business</Button3D>
                )}
              </div>
            </div>
          </div>
        </FadeInUp>

        {/* Main Content */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Left Column - Main Info */}
          <div className="lg:col-span-2 space-y-6">
            {/* Photos */}
            <FadeInUp delay={0.1}>
              <GlassCard className="p-0 overflow-hidden">
                <div className="h-64 bg-gradient-to-br from-gray-700 to-gray-800 rounded-lg overflow-hidden">
                  {place.photos && place.photos.length > 0 ? (
                    <img
                      src={place.photos[0].url}
                      alt={place.name}
                      className="w-full h-full object-cover"
                    />
                  ) : (
                    <div className="w-full h-full bg-gradient-to-br from-gray-700 via-gray-800 to-gray-900 flex items-center justify-center">
                      <span className="text-gray-400">No Photos Available</span>
                    </div>
                  )}
                </div>
              </GlassCard>
            </FadeInUp>

            {/* Cuisine Types and Dietary Tags */}
            <FadeInUp delay={0.2}>
              <GlassCard className="p-6">
                <h2 className="text-xl font-bold text-white mb-4">Cuisine & Dietary Information</h2>
                <div className="space-y-4">
                  <div>
                    <h4 className="font-medium text-sm text-gray-300 mb-2">Cuisine Types</h4>
                    <div className="flex flex-wrap gap-2">
                      {place.cuisineTypes.map((cuisine) => (
                        <span
                          key={cuisine}
                          className="inline-block bg-indigo-500/20 text-indigo-300 border border-indigo-500/30 text-sm px-3 py-1 rounded-full"
                        >
                          {cuisine}
                        </span>
                      ))}
                    </div>
                  </div>
                  
                  {place.dietaryTags && place.dietaryTags.length > 0 && (
                    <div>
                      <h4 className="font-medium text-sm text-gray-300 mb-2">Dietary Options</h4>
                      <div className="flex flex-wrap gap-2">
                        {place.dietaryTags.map((tag) => (
                          <span
                            key={tag}
                            className="inline-block bg-green-500/20 text-green-300 border border-green-500/30 text-sm px-3 py-1 rounded-full"
                          >
                            {tag}
                          </span>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </GlassCard>
            </FadeInUp>

            {/* Reviews Section */}
            <FadeInUp delay={0.3}>
              <GlassCard className="p-6">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-4">
                  <h2 className="text-xl font-bold text-white">Reviews</h2>
                  
                  {/* Sort and Filter Controls */}
                  <div className="flex flex-wrap items-center gap-2">
                    {/* Sort Dropdown */}
                    <div className="relative">
                      <select
                        value={sortBy}
                        onChange={(e) => setSortBy(e.target.value as SortOption)}
                        className="bg-gray-800/50 border border-gray-700 text-white text-sm rounded-md px-3 py-2 pr-8 focus:ring-2 focus:ring-indigo-500 focus:border-transparent appearance-none cursor-pointer"
                      >
                        <option value="recent">Most Recent</option>
                        <option value="helpful">Most Helpful</option>
                        <option value="highest">Highest Rating</option>
                        <option value="lowest">Lowest Rating</option>
                      </select>
                      <ArrowUpDown className="absolute right-2 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" />
                    </div>

                    {/* Filter Dropdown */}
                    <div className="relative">
                      <select
                        value={filterBy}
                        onChange={(e) => setFilterBy(e.target.value as FilterOption)}
                        className="bg-gray-800/50 border border-gray-700 text-white text-sm rounded-md px-3 py-2 pr-8 focus:ring-2 focus:ring-indigo-500 focus:border-transparent appearance-none cursor-pointer"
                      >
                        <option value="all">All Ratings</option>
                        <option value="5">5 Stars</option>
                        <option value="4">4 Stars</option>
                        <option value="3">3 Stars</option>
                        <option value="2">2 Stars</option>
                        <option value="1">1 Star</option>
                      </select>
                      <Filter className="absolute right-2 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" />
                    </div>
                  </div>
                </div>

                {reviewsLoading ? (
                  <div className="space-y-4">
                    {[...Array(3)].map((_, i) => (
                      <div key={i} className="animate-pulse">
                        <div className="flex items-center space-x-2 mb-2">
                          <div className="h-4 bg-gray-700 rounded w-20"></div>
                          <div className="h-4 bg-gray-700 rounded w-16"></div>
                        </div>
                        <div className="h-4 bg-gray-700 rounded w-full mb-2"></div>
                        <div className="h-4 bg-gray-700 rounded w-3/4"></div>
                      </div>
                    ))}
                  </div>
                ) : reviews.length > 0 ? (
                  <StaggerChildren className="space-y-6">
                    {reviews.map((review) => (
                      <div key={review.id} className="border-b border-gray-700 pb-6 last:border-b-0">
                        <div className="flex items-center justify-between mb-3">
                          <div className="flex items-center space-x-2">
                            <User className="w-8 h-8 bg-gray-700 rounded-full p-1 text-gray-400" />
                            <div>
                              <div className="flex items-center space-x-2">
                                <span className="font-medium text-sm text-white">
                                  {review.user ? `${review.user.firstName} ${review.user.lastName}` : 'Anonymous'}
                                </span>
                                {review.user?.reputationLevel && (
                                  <span className={`text-xs px-2 py-0.5 rounded-full border ${getReputationLevelBgColor(review.user.reputationLevel)} ${getReputationLevelColor(review.user.reputationLevel)}`}>
                                    {review.user.reputationLevel}
                                  </span>
                                )}
                              </div>
                              <div className="text-xs text-gray-400">
                                {new Date(review.createdAt).toLocaleDateString()}
                              </div>
                            </div>
                          </div>
                          <div className="flex items-center space-x-2">
                            <div className="flex">
                              {[...Array(5)].map((_, i) => (
                                <Star
                                  key={i}
                                  className={`w-4 h-4 ${
                                    i < review.rating ? 'text-yellow-400 fill-current' : 'text-gray-600'
                                  }`}
                                />
                              ))}
                            </div>
                            {review.isGpsVerified && (
                              <Badge3D variant="custom" className="text-xs px-2 py-0.5">
                                GPS Verified
                              </Badge3D>
                            )}
                          </div>
                        </div>
                        
                        <p className="text-gray-200 mb-3">{review.text}</p>
                        
                        {(() => {
                          const dietaryAcc = review.dietaryAccuracy;
                          if (!dietaryAcc) return null;
                          if (typeof dietaryAcc === 'string' && dietaryAcc.trim()) {
                            return (
                              <div className="mb-3">
                                <span className="text-sm text-gray-400">
                                  Dietary accuracy: {dietaryAcc}
                                </span>
                              </div>
                            );
                          }
                          if (typeof dietaryAcc === 'object' && dietaryAcc !== null) {
                            const entries = Object.entries(dietaryAcc as Record<string, string>);
                            if (entries.length > 0) {
                              return (
                                <div className="mb-3">
                                  <span className="text-sm text-gray-400">
                                    Dietary accuracy: {entries.map(([key, value]) => `${key}: ${value}`).join(', ')}
                                  </span>
                                </div>
                              );
                            }
                          }
                          return null;
                        })()}
                        
                        <div className="flex items-center justify-between">
                          <ReviewHelpfulButtons review={review} />
                          {isAuthenticated && (
                            <button
                              onClick={() => {
                                // TODO: Implement report functionality
                                toast.success('Report submitted. Thank you for helping keep our community safe.');
                              }}
                              className="text-xs text-gray-400 hover:text-red-400 transition-colors"
                            >
                              Report
                            </button>
                          )}
                        </div>
                      </div>
                    ))}
                  </StaggerChildren>
                ) : (
                  <div className="text-center text-gray-400 py-8">
                    <p className="mb-4">No reviews yet</p>
                    {isAuthenticated && (
                      <Link href={`/places/${id}/review`}>
                        <Button3D variant="primary" size="md">Be the first to review</Button3D>
                      </Link>
                    )}
                  </div>
                )}
              </GlassCard>
            </FadeInUp>
          </div>

          {/* Right Column - Info Panel */}
          <div className="space-y-6">
            {/* Contact Info */}
            <FadeInUp delay={0.1}>
              <GlassCard className="p-6">
                <h2 className="text-xl font-bold text-white mb-4">Contact Information</h2>
                <div className="space-y-3">
                  {place.phone && (
                    <div className="flex items-center space-x-2">
                      <Phone className="w-4 h-4 text-gray-300" />
                      <span className="text-gray-200">{place.phone}</span>
                    </div>
                  )}
                  
                  {place.website && (
                    <div className="flex items-center space-x-2">
                      <Globe className="w-4 h-4 text-gray-300" />
                      <a
                        href={place.website}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-indigo-400 hover:text-indigo-300 hover:underline"
                      >
                        Visit Website
                      </a>
                    </div>
                  )}
                </div>
              </GlassCard>
            </FadeInUp>

            {/* Hours */}
            {place.hoursOfOperation && (
              <FadeInUp delay={0.2}>
                <GlassCard className="p-6">
                  <h2 className="text-xl font-bold text-white mb-4">Hours</h2>
                  <div className="space-y-1 text-sm">
                    {Object.entries(place.hoursOfOperation).map(([day, hours]) => (
                      <div key={day} className="flex justify-between text-gray-200">
                        <span className="capitalize">{day}</span>
                        <span>{hours}</span>
                      </div>
                    ))}
                  </div>
                </GlassCard>
              </FadeInUp>
            )}

            {/* Amenities */}
            {place.amenities && place.amenities.length > 0 && (
              <FadeInUp delay={0.3}>
                <GlassCard className="p-6">
                  <h2 className="text-xl font-bold text-white mb-4">Amenities</h2>
                  <div className="flex flex-wrap gap-2">
                    {place.amenities.map((amenity) => (
                      <span
                        key={amenity}
                        className="inline-block bg-gray-700/50 text-gray-200 border border-gray-600 text-sm px-2 py-1 rounded"
                      >
                        {amenity}
                      </span>
                    ))}
                  </div>
                </GlassCard>
              </FadeInUp>
            )}

            {/* Map */}
            <FadeInUp delay={0.4}>
              <GlassCard className="p-6">
                <h2 className="text-xl font-bold text-white mb-4">Location</h2>
                <PlaceMap place={place} />
              </GlassCard>
            </FadeInUp>
          </div>
        </div>
      </div>
    </div>
  );
}
