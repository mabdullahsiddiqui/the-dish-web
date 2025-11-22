'use client'

import { useState } from 'react';
import { User, Star, Calendar, Award, MapPin, Edit } from 'lucide-react';
import { Button3D } from '@/components/ui/Button3D';
import { GlassCard } from '@/components/cards/GlassCard';
import { Badge3D } from '@/components/ui/Badge3D';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { StaggerChildren } from '@/components/animations/StaggerChildren';
import { useAuth } from '@/hooks/useAuth';
import { useReviews } from '@/hooks/useReviews';
import { ReviewActions } from '@/components/features/reviews/review-actions';
import { Pagination } from '@/components/ui/pagination';
import { EmptyState } from '@/components/ui/empty-state';
import { getReputationLevelColor, getReputationLevelBgColor, getReputationLevelDescription } from '@/lib/utils/reputation';
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
      <div className="min-h-screen bg-[#0f172a]">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-700 rounded w-1/3 mb-6"></div>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              <div className="h-64 bg-gray-700 rounded"></div>
              <div className="lg:col-span-2 h-64 bg-gray-700 rounded"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="min-h-screen bg-[#0f172a]">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="text-center">
            <h1 className="text-2xl font-bold text-white mb-4">User Not Found</h1>
            <Button3D variant="primary" onClick={() => router.push('/')}>Go Home</Button3D>
          </div>
        </div>
      </div>
    );
  }

  const reviews = reviewsResponse?.success && reviewsResponse.data ? reviewsResponse.data.reviews : [];
  const totalReviews = reviewsResponse?.success && reviewsResponse.data ? reviewsResponse.data.totalCount : 0;

  return (
    <div className="min-h-screen bg-[#0f172a]">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <FadeInUp>
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-white mb-2">My Profile</h1>
            <p className="text-gray-300">Manage your account and reviews</p>
          </div>
        </FadeInUp>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Profile Info Sidebar */}
          <div className="space-y-6">
            {/* User Info Card */}
            <FadeInUp delay={0.1}>
              <GlassCard className="p-6">
                <h2 className="text-xl font-bold text-white mb-4 flex items-center space-x-2">
                  <User className="w-5 h-5" />
                  <span>Profile</span>
                </h2>
                <div className="space-y-4">
                  <div className="text-center">
                    <div className="w-20 h-20 bg-gray-700 rounded-full flex items-center justify-center mx-auto mb-4">
                      <User className="w-10 h-10 text-gray-400" />
                    </div>
                    <h3 className="font-semibold text-lg text-white">
                      {user.firstName} {user.lastName}
                    </h3>
                    <p className="text-gray-300 text-sm">{user.email}</p>
                    {user.isVerified && (
                      <div className="mt-2">
                        <Badge3D variant="verified" />
                      </div>
                    )}
                  </div>

                  <div className="space-y-3 text-sm">
                    <div className="flex items-center justify-between">
                      <span className="text-gray-300">Member since</span>
                      <span className="font-medium text-white">
                        {new Date(user.joinDate).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-gray-300">Total reviews</span>
                      <span className="font-medium text-white">{totalReviews}</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-gray-300">Reputation</span>
                      <span className="font-medium text-white">{user.reputation} points</span>
                    </div>
                    <div className="pt-2 border-t border-gray-700">
                      <div className={`inline-flex items-center px-3 py-1 rounded-full border ${getReputationLevelBgColor(user.reputationLevel || 'Bronze')}`}>
                        <span className={`text-sm font-semibold ${getReputationLevelColor(user.reputationLevel || 'Bronze')}`}>
                          {user.reputationLevel || 'Bronze'}
                        </span>
                      </div>
                      <p className="text-xs text-gray-400 mt-1">
                        {getReputationLevelDescription(user.reputationLevel || 'Bronze')}
                      </p>
                    </div>
                  </div>

                  <Button3D variant="outline" className="w-full">
                    <Edit className="w-4 h-4 mr-2" />
                    Edit Profile
                  </Button3D>
                </div>
              </GlassCard>
            </FadeInUp>

            {/* Stats Card */}
            <FadeInUp delay={0.2}>
              <GlassCard className="p-6">
                <h2 className="text-xl font-bold text-white mb-4 flex items-center space-x-2">
                  <Award className="w-5 h-5" />
                  <span>Statistics</span>
                </h2>
                <div className="space-y-3">
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-300">Reviews this month</span>
                    <span className="font-medium text-white">
                      {/* This would be calculated from actual data */}
                      {Math.floor(totalReviews / 6)}
                    </span>
                  </div>
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-300">Average rating given</span>
                    <span className="font-medium flex items-center text-white">
                      4.2
                      <Star className="w-3 h-3 text-yellow-400 fill-current ml-1" />
                    </span>
                  </div>
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-300">Helpful votes received</span>
                    <span className="font-medium text-white">
                      {/* This would be calculated from actual data */}
                      {Math.floor(totalReviews * 2.3)}
                    </span>
                  </div>
                </div>
              </GlassCard>
            </FadeInUp>
          </div>

          {/* Reviews Section */}
          <div className="lg:col-span-2 space-y-6">
            <FadeInUp delay={0.3}>
              <GlassCard className="p-6">
                <h2 className="text-xl font-bold text-white mb-4">My Reviews</h2>
                {reviewsLoading ? (
                  <div className="space-y-4">
                    {[...Array(3)].map((_, i) => (
                      <div key={i} className="animate-pulse">
                        <div className="flex items-center space-x-2 mb-2">
                          <div className="h-4 bg-gray-700 rounded w-32"></div>
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
                            <Link 
                              href={`/places/${review.placeId}`}
                              className="font-medium text-indigo-400 hover:text-indigo-300"
                            >
                              Place Name {/* This would come from place data */}
                            </Link>
                            {review.isGpsVerified && (
                              <Badge3D variant="custom" className="text-xs px-2 py-0.5">
                                GPS Verified
                              </Badge3D>
                            )}
                          </div>
                          <div className="text-sm text-gray-400">
                            {new Date(review.createdAt).toLocaleDateString()}
                          </div>
                        </div>

                        <div className="flex items-center mb-2">
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
                          <span className="ml-2 text-sm text-gray-300">
                            {review.rating} star{review.rating !== 1 ? 's' : ''}
                          </span>
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

                        <div className="flex items-center justify-between text-sm">
                          <div className="text-gray-400">
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
                  </StaggerChildren>
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
              </GlassCard>
            </FadeInUp>
          </div>
        </div>
      </div>
    </div>
  );
}
