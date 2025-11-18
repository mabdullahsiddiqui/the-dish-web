import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { reviewsApi } from '@/lib/api/reviews';
import { CreateReviewRequest, UpdateReviewRequest } from '@/types/review';
import toast from 'react-hot-toast';

export function useReviews() {
  const queryClient = useQueryClient();

  // Get review by ID
  const useReview = (id: string) => {
    return useQuery({
      queryKey: ['review', id],
      queryFn: () => reviewsApi.getById(id),
      enabled: !!id,
    });
  };

  // Get reviews for a place
  const usePlaceReviews = (placeId: string, page: number = 1, pageSize: number = 20) => {
    return useQuery({
      queryKey: ['reviews', 'place', placeId, page, pageSize],
      queryFn: () => reviewsApi.getByPlace(placeId, page, pageSize),
      enabled: !!placeId,
    });
  };

  // Get reviews by user
  const useUserReviews = (userId: string, page: number = 1, pageSize: number = 20) => {
    return useQuery({
      queryKey: ['reviews', 'user', userId, page, pageSize],
      queryFn: () => reviewsApi.getByUser(userId, page, pageSize),
      enabled: !!userId,
    });
  };

  // Get recent reviews
  const useRecentReviews = (page: number = 1, pageSize: number = 20) => {
    return useQuery({
      queryKey: ['reviews', 'recent', page, pageSize],
      queryFn: () => reviewsApi.getRecent(page, pageSize),
    });
  };

  // Create review mutation
  const useCreateReview = () => {
    return useMutation({
      mutationFn: reviewsApi.create,
      onSuccess: (response, variables) => {
        if (response.success) {
          toast.success('Review submitted successfully!');
          // Invalidate place reviews and place data (to update review count/rating)
          queryClient.invalidateQueries({ queryKey: ['reviews', 'place', variables.placeId] });
          queryClient.invalidateQueries({ queryKey: ['place', variables.placeId] });
          queryClient.invalidateQueries({ queryKey: ['reviews', 'recent'] });
        } else {
          toast.error(response.message || 'Failed to submit review');
        }
      },
      onError: (error: any) => {
        toast.error(error.message || 'Failed to submit review. Please try again.');
      },
    });
  };

  // Update review mutation
  const useUpdateReview = () => {
    return useMutation({
      mutationFn: ({ id, data }: { id: string; data: UpdateReviewRequest }) =>
        reviewsApi.update(id, data),
      onSuccess: (response, variables) => {
        if (response.success && response.data) {
          toast.success('Review updated successfully!');
          queryClient.invalidateQueries({ queryKey: ['review', variables.id] });
          queryClient.invalidateQueries({ queryKey: ['reviews', 'place', response.data.placeId] });
          queryClient.invalidateQueries({ queryKey: ['place', response.data.placeId] });
          queryClient.invalidateQueries({ queryKey: ['reviews', 'user'] });
        } else {
          toast.error(response.message || 'Failed to update review');
        }
      },
      onError: (error: any) => {
        toast.error(error.message || 'Failed to update review. Please try again.');
      },
    });
  };

  // Delete review mutation
  const useDeleteReview = () => {
    return useMutation({
      mutationFn: reviewsApi.delete,
      onSuccess: (response) => {
        if (response.success) {
          toast.success('Review deleted successfully');
          queryClient.invalidateQueries({ queryKey: ['reviews'] });
          queryClient.invalidateQueries({ queryKey: ['places'] });
        } else {
          toast.error(response.message || 'Failed to delete review');
        }
      },
      onError: (error: any) => {
        toast.error(error.message || 'Failed to delete review. Please try again.');
      },
    });
  };

  // Mark review helpful mutation
  const useMarkReviewHelpful = () => {
    return useMutation({
      mutationFn: ({ id, isHelpful }: { id: string; isHelpful: boolean }) =>
        reviewsApi.markHelpful(id, isHelpful),
      onSuccess: (response, variables) => {
        if (response.success && response.data) {
          toast.success(variables.isHelpful ? 'Marked as helpful' : 'Marked as not helpful');
          queryClient.invalidateQueries({ queryKey: ['review', variables.id] });
          queryClient.invalidateQueries({ queryKey: ['reviews', 'place', response.data.placeId] });
        } else {
          toast.error(response.message || 'Failed to record vote');
        }
      },
      onError: (error: any) => {
        toast.error(error.message || 'Failed to record vote. Please try again.');
      },
    });
  };

  return {
    useReview,
    usePlaceReviews,
    useUserReviews,
    useRecentReviews,
    useCreateReview,
    useUpdateReview,
    useDeleteReview,
    useMarkReviewHelpful,
  };
}
