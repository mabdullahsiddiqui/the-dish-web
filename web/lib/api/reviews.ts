import { api } from './client';
import { 
  Review, 
  CreateReviewRequest, 
  UpdateReviewRequest, 
  ReviewListResponse 
} from '@/types/review';

export const reviewsApi = {
  // Get review by ID
  getById: async (id: string) => {
    const response = await api.get<Review>(`/reviews/${id}`);
    return response.data;
  },

  // Get reviews for a place
  getByPlace: async (placeId: string, page: number = 1, pageSize: number = 20) => {
    const response = await api.get<ReviewListResponse>(
      `/reviews/place/${placeId}?page=${page}&pageSize=${pageSize}`
    );
    return response.data;
  },

  // Get reviews by user
  getByUser: async (userId: string, page: number = 1, pageSize: number = 20) => {
    const response = await api.get<ReviewListResponse>(
      `/reviews/user/${userId}?page=${page}&pageSize=${pageSize}`
    );
    return response.data;
  },

  // Get recent reviews
  getRecent: async (page: number = 1, pageSize: number = 20) => {
    const response = await api.get<ReviewListResponse>(
      `/reviews/recent?page=${page}&pageSize=${pageSize}`
    );
    return response.data;
  },

  // Create new review
  create: async (data: CreateReviewRequest) => {
    const response = await api.post<Review>('/reviews', data);
    return response.data;
  },

  // Update review
  update: async (id: string, data: UpdateReviewRequest) => {
    const response = await api.put<Review>(`/reviews/${id}`, data);
    return response.data;
  },

  // Delete review
  delete: async (id: string) => {
    const response = await api.delete<boolean>(`/reviews/${id}`);
    return response.data;
  },

  // Mark review as helpful/not helpful
  markHelpful: async (id: string, isHelpful: boolean) => {
    const response = await api.post<Review>(`/reviews/${id}/helpful`, isHelpful);
    return response.data;
  },
};

