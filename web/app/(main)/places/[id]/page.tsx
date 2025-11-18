'use client'

import { use } from 'react';
import { notFound } from 'next/navigation';
import { MapPin, Phone, Globe, Clock, Star, User } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { usePlaces } from '@/hooks/usePlaces';
import { useReviews } from '@/hooks/useReviews';
import { useAuth } from '@/hooks/useAuth';
import { ReviewHelpfulButtons } from '@/components/features/reviews/review-helpful-buttons';
import { PlaceMap } from '@/components/features/places/place-map';
import Link from 'next/link';

interface PlaceDetailPageProps {
  params: Promise<{ id: string }>;
}

export default function PlaceDetailPage({ params }: PlaceDetailPageProps) {
  const { id } = use(params);
  const { usePlace } = usePlaces();
  const { usePlaceReviews } = useReviews();
  const { isAuthenticated } = useAuth();

  const { data: placeResponse, isLoading: placeLoading, error: placeError } = usePlace(id);
  const { data: reviewsResponse, isLoading: reviewsLoading } = usePlaceReviews(id, 1, 10);

  if (placeLoading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="animate-pulse">
          <div className="h-8 bg-gray-200 rounded w-1/3 mb-4"></div>
          <div className="h-64 bg-gray-200 rounded mb-6"></div>
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <div className="lg:col-span-2 space-y-4">
              <div className="h-4 bg-gray-200 rounded w-3/4"></div>
              <div className="h-4 bg-gray-200 rounded w-1/2"></div>
            </div>
            <div className="h-32 bg-gray-200 rounded"></div>
          </div>
        </div>
      </div>
    );
  }

  if (placeError || !placeResponse?.success || !placeResponse.data) {
    return notFound();
  }

  const place = placeResponse.data;
  const reviews = reviewsResponse?.success && reviewsResponse.data ? reviewsResponse.data.reviews : [];

  const getPriceRangeText = (priceRange: number) => {
    return '$'.repeat(priceRange);
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header Section */}
      <div className="mb-8">
        <div className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-4">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">{place.name}</h1>
            <div className="flex items-center text-gray-600 mb-2">
              <MapPin className="w-4 h-4 mr-1" />
              <span>{place.address}</span>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex items-center">
                <Star className="w-5 h-5 text-yellow-400 fill-current mr-1" />
                <span className="font-semibold">{place.averageRating.toFixed(1)}</span>
                <span className="text-gray-600 ml-1">({place.reviewCount} reviews)</span>
              </div>
              <span className="text-gray-600">{getPriceRangeText(place.priceRange)}</span>
              {place.isVerified && (
                <span className="bg-green-100 text-green-700 px-2 py-1 rounded-full text-sm font-medium">
                  Verified
                </span>
              )}
            </div>
          </div>
          
          <div className="flex flex-col sm:flex-row gap-2">
            {isAuthenticated && (
              <Link href={`/places/${id}/review`}>
                <Button>Write Review</Button>
              </Link>
            )}
            <Button variant="outline">Share</Button>
            {!place.isClaimed && (
              <Button variant="outline">Claim Business</Button>
            )}
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left Column - Main Info */}
        <div className="lg:col-span-2 space-y-6">
          {/* Photos */}
          <Card>
            <CardContent className="p-0">
              <div className="h-64 bg-gray-200 rounded-lg overflow-hidden">
                {place.photos && place.photos.length > 0 ? (
                  <img
                    src={place.photos[0].url}
                    alt={place.name}
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <div className="w-full h-full bg-gradient-to-br from-gray-200 to-gray-300 flex items-center justify-center">
                    <span className="text-gray-500">No Photos Available</span>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Cuisine Types and Dietary Tags */}
          <Card>
            <CardHeader>
              <CardTitle>Cuisine & Dietary Information</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div>
                  <h4 className="font-medium text-sm text-gray-700 mb-2">Cuisine Types</h4>
                  <div className="flex flex-wrap gap-2">
                    {place.cuisineTypes.map((cuisine) => (
                      <span
                        key={cuisine}
                        className="inline-block bg-blue-100 text-blue-700 text-sm px-3 py-1 rounded-full"
                      >
                        {cuisine}
                      </span>
                    ))}
                  </div>
                </div>
                
                {place.dietaryTags && place.dietaryTags.length > 0 && (
                  <div>
                    <h4 className="font-medium text-sm text-gray-700 mb-2">Dietary Options</h4>
                    <div className="flex flex-wrap gap-2">
                      {place.dietaryTags.map((tag) => (
                        <span
                          key={tag}
                          className="inline-block bg-green-100 text-green-700 text-sm px-3 py-1 rounded-full"
                        >
                          {tag}
                        </span>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Reviews Section */}
          <Card>
            <CardHeader>
              <CardTitle>Reviews</CardTitle>
            </CardHeader>
            <CardContent>
              {reviewsLoading ? (
                <div className="space-y-4">
                  {[...Array(3)].map((_, i) => (
                    <div key={i} className="animate-pulse">
                      <div className="flex items-center space-x-2 mb-2">
                        <div className="h-4 bg-gray-200 rounded w-20"></div>
                        <div className="h-4 bg-gray-200 rounded w-16"></div>
                      </div>
                      <div className="h-4 bg-gray-200 rounded w-full mb-2"></div>
                      <div className="h-4 bg-gray-200 rounded w-3/4"></div>
                    </div>
                  ))}
                </div>
              ) : reviews.length > 0 ? (
                <div className="space-y-6">
                  {reviews.map((review) => (
                    <div key={review.id} className="border-b pb-6 last:border-b-0">
                      <div className="flex items-center justify-between mb-3">
                        <div className="flex items-center space-x-2">
                          <User className="w-8 h-8 bg-gray-200 rounded-full p-1" />
                          <div>
                            <div className="font-medium text-sm">
                              {review.user ? `${review.user.firstName} ${review.user.lastName}` : 'Anonymous'}
                            </div>
                            <div className="text-xs text-gray-500">
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
                                  i < review.rating ? 'text-yellow-400 fill-current' : 'text-gray-300'
                                }`}
                              />
                            ))}
                          </div>
                          {review.isGpsVerified && (
                            <span className="bg-blue-100 text-blue-700 text-xs px-2 py-1 rounded-full">
                              GPS Verified
                            </span>
                          )}
                        </div>
                      </div>
                      
                      <p className="text-gray-800 mb-3">{review.text}</p>
                      
                      {review.dietaryAccuracy && (
                        <div className="mb-3">
                          <span className="text-sm text-gray-600">
                            Dietary accuracy: {review.dietaryAccuracy}
                          </span>
                        </div>
                      )}
                      
                      <ReviewHelpfulButtons review={review} />
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-center text-gray-500 py-8">
                  <p className="mb-4">No reviews yet</p>
                  {isAuthenticated && (
                    <Link href={`/places/${id}/review`}>
                      <Button>Be the first to review</Button>
                    </Link>
                  )}
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Right Column - Info Panel */}
        <div className="space-y-6">
          {/* Contact Info */}
          <Card>
            <CardHeader>
              <CardTitle>Contact Information</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              {place.phone && (
                <div className="flex items-center space-x-2">
                  <Phone className="w-4 h-4 text-gray-600" />
                  <span>{place.phone}</span>
                </div>
              )}
              
              {place.website && (
                <div className="flex items-center space-x-2">
                  <Globe className="w-4 h-4 text-gray-600" />
                  <a
                    href={place.website}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-blue-600 hover:underline"
                  >
                    Visit Website
                  </a>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Hours */}
          {place.hoursOfOperation && (
            <Card>
              <CardHeader>
                <CardTitle>Hours</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-1 text-sm">
                  {Object.entries(place.hoursOfOperation).map(([day, hours]) => (
                    <div key={day} className="flex justify-between">
                      <span className="capitalize">{day}</span>
                      <span>{hours}</span>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Amenities */}
          {place.amenities && place.amenities.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Amenities</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex flex-wrap gap-2">
                  {place.amenities.map((amenity) => (
                    <span
                      key={amenity}
                      className="inline-block bg-gray-100 text-gray-700 text-sm px-2 py-1 rounded"
                    >
                      {amenity}
                    </span>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Map */}
          <Card>
            <CardHeader>
              <CardTitle>Location</CardTitle>
            </CardHeader>
            <CardContent>
              <PlaceMap place={place} />
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
