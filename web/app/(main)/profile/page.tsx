'use client'

import { useState } from 'react';
import { User, Star, Calendar, Award, MapPin, Edit } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useAuth } from '@/hooks/useAuth';
import { useReviews } from '@/hooks/useReviews';
import { ReviewActions } from '@/components/features/reviews/review-actions';
import { Pagination } from '@/components/ui/pagination';
import { EmptyState } from '@/components/ui/empty-state';
import Link from 'next/link';
import { useRouter } from 'next/navigation';

export default function ProfilePage() {
  const { user, isAuthenticated, isLoading } = useAuth();
  const { useUserReviews } = useReviews();
  const router = useRouter();
  const [currentPage, setCurrentPage] = useState(1);

  // Get user's reviews
  const { data: reviewsResponse, isLoading: reviewsLoading } = useUserReviews(
    user?.id || '',
    currentPage,
    10
  );

  if (!isAuthenticated) {
    router.push('/login');
    return null;
  }

  if (isLoading) {
    return (
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="animate-pulse">
          <div className="h-8 bg-gray-200 rounded w-1/3 mb-6"></div>
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <div className="h-64 bg-gray-200 rounded"></div>
            <div className="lg:col-span-2 h-64 bg-gray-200 rounded"></div>
          </div>
        </div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900 mb-4">User Not Found</h1>
          <Button onClick={() => router.push('/')}>Go Home</Button>
        </div>
      </div>
    );
  }

  const reviews = reviewsResponse?.success && reviewsResponse.data ? reviewsResponse.data.reviews : [];
  const totalReviews = reviewsResponse?.success && reviewsResponse.data ? reviewsResponse.data.totalCount : 0;

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">My Profile</h1>
        <p className="text-gray-600">Manage your account and reviews</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Profile Info Sidebar */}
        <div className="space-y-6">
          {/* User Info Card */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <User className="w-5 h-5" />
                <span>Profile</span>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="text-center">
                <div className="w-20 h-20 bg-gray-200 rounded-full flex items-center justify-center mx-auto mb-4">
                  <User className="w-10 h-10 text-gray-500" />
                </div>
                <h3 className="font-semibold text-lg">
                  {user.firstName} {user.lastName}
                </h3>
                <p className="text-gray-600 text-sm">{user.email}</p>
                {user.isVerified && (
                  <span className="inline-block bg-blue-100 text-blue-700 text-xs px-2 py-1 rounded-full mt-2">
                    Verified User
                  </span>
                )}
              </div>

              <div className="space-y-3 text-sm">
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">Member since</span>
                  <span className="font-medium">
                    {new Date(user.joinDate).toLocaleDateString()}
                  </span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">Total reviews</span>
                  <span className="font-medium">{totalReviews}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-gray-600">Reputation</span>
                  <span className="font-medium">{user.reputation} points</span>
                </div>
              </div>

              <Button variant="outline" className="w-full">
                <Edit className="w-4 h-4 mr-2" />
                Edit Profile
              </Button>
            </CardContent>
          </Card>

          {/* Stats Card */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <Award className="w-5 h-5" />
                <span>Statistics</span>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-600">Reviews this month</span>
                <span className="font-medium">
                  {/* This would be calculated from actual data */}
                  {Math.floor(totalReviews / 6)}
                </span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-600">Average rating given</span>
                <span className="font-medium flex items-center">
                  4.2
                  <Star className="w-3 h-3 text-yellow-400 fill-current ml-1" />
                </span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-600">Helpful votes received</span>
                <span className="font-medium">
                  {/* This would be calculated from actual data */}
                  {Math.floor(totalReviews * 2.3)}
                </span>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Reviews Section */}
        <div className="lg:col-span-2 space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>My Reviews</CardTitle>
            </CardHeader>
            <CardContent>
              {reviewsLoading ? (
                <div className="space-y-4">
                  {[...Array(3)].map((_, i) => (
                    <div key={i} className="animate-pulse">
                      <div className="flex items-center space-x-2 mb-2">
                        <div className="h-4 bg-gray-200 rounded w-32"></div>
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
                          <Link 
                            href={`/places/${review.placeId}`}
                            className="font-medium text-blue-600 hover:text-blue-800"
                          >
                            Place Name {/* This would come from place data */}
                          </Link>
                          {review.isGpsVerified && (
                            <span className="bg-blue-100 text-blue-700 text-xs px-2 py-1 rounded-full">
                              GPS Verified
                            </span>
                          )}
                        </div>
                        <div className="text-sm text-gray-500">
                          {new Date(review.createdAt).toLocaleDateString()}
                        </div>
                      </div>

                      <div className="flex items-center mb-2">
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
                        <span className="ml-2 text-sm text-gray-600">
                          {review.rating} star{review.rating !== 1 ? 's' : ''}
                        </span>
                      </div>

                      <p className="text-gray-800 mb-3">{review.text}</p>

                      {review.dietaryAccuracy && (
                        <div className="mb-3">
                          <span className="text-sm text-gray-600">
                            Dietary accuracy: {review.dietaryAccuracy}
                          </span>
                        </div>
                      )}

                      <div className="flex items-center justify-between text-sm">
                        <div className="text-gray-500">
                          {review.helpfulCount} people found this helpful
                        </div>
                        <ReviewActions review={review} />
                      </div>
                    </div>
                  ))}

                  {/* Pagination */}
                  {reviewsResponse?.success && reviewsResponse.data && reviewsResponse.data.totalPages > 1 && (
                    <div className="mt-6">
                      <Pagination
                        currentPage={currentPage}
                        totalPages={reviewsResponse.data.totalPages}
                        onPageChange={setCurrentPage}
                      />
                    </div>
                  )}
                </div>
              ) : (
                <EmptyState
                  icon={<Star className="w-12 h-12" />}
                  title="No reviews yet"
                  description="Start exploring restaurants and share your experiences!"
                  action={{
                    label: 'Discover Restaurants',
                    onClick: () => router.push('/'),
                  }}
                />
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
