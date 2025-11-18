'use client'

import { use, useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Star, Save, X } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useReviews } from '@/hooks/useReviews';
import { useAuth } from '@/hooks/useAuth';
import { DietaryAccuracy } from '@/types/review';

interface EditReviewPageProps {
  params: Promise<{ id: string }>;
}

const reviewSchema = z.object({
  rating: z.number().min(1, 'Please select a rating').max(5),
  text: z.string().min(10, 'Review must be at least 10 characters'),
  dietaryAccuracy: z.nativeEnum(DietaryAccuracy).optional(),
});

type ReviewFormData = z.infer<typeof reviewSchema>;

export default function EditReviewPage({ params }: EditReviewPageProps) {
  const { id } = use(params);
  const router = useRouter();
  const { useReview, useUpdateReview } = useReviews();
  const { user, isAuthenticated } = useAuth();

  // States
  const [selectedRating, setSelectedRating] = useState(0);
  const [hoverRating, setHoverRating] = useState(0);

  // Hooks
  const { data: reviewResponse, isLoading: reviewLoading } = useReview(id);
  const updateReviewMutation = useUpdateReview();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = useForm<ReviewFormData>({
    resolver: zodResolver(reviewSchema),
  });

  // Check authentication and ownership
  useEffect(() => {
    if (!isAuthenticated) {
      router.push('/login');
      return;
    }

    if (reviewResponse?.success && reviewResponse.data && user) {
      const review = reviewResponse.data;
      
      // Check if user owns this review
      if (review.userId !== user.id) {
        router.push('/profile');
        return;
      }

      // Populate form with existing data
      setSelectedRating(review.rating);
      setValue('rating', review.rating);
      setValue('text', review.text);
      if (review.dietaryAccuracy) {
        setValue('dietaryAccuracy', review.dietaryAccuracy);
      }
    }
  }, [reviewResponse, isAuthenticated, user, router, setValue]);

  const onSubmit = async (data: ReviewFormData) => {
    try {
      await updateReviewMutation.mutateAsync({
        id,
        data: {
          rating: data.rating,
          text: data.text,
          dietaryAccuracy: data.dietaryAccuracy,
        },
      });

      // Success - redirect back to profile
      router.push('/profile');
    } catch (error) {
      console.error('Failed to update review:', error);
    }
  };

  if (!isAuthenticated) {
    return null; // Will redirect to login
  }

  if (reviewLoading) {
    return (
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="animate-pulse">
          <div className="h-8 bg-gray-200 rounded w-1/3 mb-4"></div>
          <div className="h-64 bg-gray-200 rounded"></div>
        </div>
      </div>
    );
  }

  if (!reviewResponse?.success || !reviewResponse.data) {
    return (
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900 mb-4">Review Not Found</h1>
          <Button onClick={() => router.back()}>Go Back</Button>
        </div>
      </div>
    );
  }

  const review = reviewResponse.data;

  return (
    <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Edit Review</h1>
        <p className="text-lg text-gray-600">Update your review</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        {/* Rating Section */}
        <Card>
          <CardHeader>
            <CardTitle>Overall Rating</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex items-center space-x-2">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  className="focus:outline-none"
                  onMouseEnter={() => setHoverRating(star)}
                  onMouseLeave={() => setHoverRating(0)}
                  onClick={() => {
                    setSelectedRating(star);
                    setValue('rating', star);
                  }}
                >
                  <Star
                    className={`w-8 h-8 transition-colors ${
                      star <= (hoverRating || selectedRating)
                        ? 'text-yellow-400 fill-current'
                        : 'text-gray-300'
                    }`}
                  />
                </button>
              ))}
              {selectedRating > 0 && (
                <span className="ml-2 text-sm text-gray-600">
                  {selectedRating} star{selectedRating !== 1 ? 's' : ''}
                </span>
              )}
            </div>
            {errors.rating && (
              <p className="text-red-500 text-sm mt-2">{errors.rating.message}</p>
            )}
          </CardContent>
        </Card>

        {/* Review Text */}
        <Card>
          <CardHeader>
            <CardTitle>Your Review</CardTitle>
          </CardHeader>
          <CardContent>
            <textarea
              {...register('text')}
              placeholder="Share details about your experience..."
              className="w-full h-32 p-3 border border-gray-300 rounded-md resize-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              defaultValue={review.text}
            />
            {errors.text && (
              <p className="text-red-500 text-sm mt-2">{errors.text.message}</p>
            )}
          </CardContent>
        </Card>

        {/* Dietary Accuracy */}
        <Card>
          <CardHeader>
            <CardTitle>Dietary Accuracy</CardTitle>
            <p className="text-sm text-gray-600">
              How accurate were the dietary labels for this restaurant?
            </p>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              {Object.values(DietaryAccuracy).map((accuracy) => (
                <label key={accuracy} className="flex items-center space-x-2">
                  <input
                    type="radio"
                    value={accuracy}
                    {...register('dietaryAccuracy')}
                    className="text-blue-600"
                    defaultChecked={review.dietaryAccuracy === accuracy}
                  />
                  <span className="text-sm">{accuracy.replace(/([A-Z])/g, ' $1').trim()}</span>
                </label>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Action Buttons */}
        <div className="flex space-x-4">
          <Button
            type="submit"
            disabled={updateReviewMutation.isPending || selectedRating === 0}
            className="flex-1 flex items-center justify-center space-x-2"
          >
            <Save className="w-4 h-4" />
            <span>
              {updateReviewMutation.isPending ? 'Saving...' : 'Save Changes'}
            </span>
          </Button>
          <Button 
            type="button" 
            variant="outline" 
            onClick={() => router.back()}
            className="flex items-center space-x-2"
          >
            <X className="w-4 h-4" />
            <span>Cancel</span>
          </Button>
        </div>
      </form>
    </div>
  );
}

