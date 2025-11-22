export interface Review {
  id: string;
  userId: string;
  placeId: string;
  rating: number; // 1-5
  text: string;
  photos?: ReviewPhoto[];
  isGpsVerified: boolean;
  dietaryAccuracy?: DietaryAccuracy;
  helpfulCount: number;
  notHelpfulCount: number;
  userHelpfulVote?: boolean; // true = helpful, false = not helpful, undefined = no vote
  createdAt: string;
  updatedAt: string;
  // User info for display
  user?: {
    firstName: string;
    lastName: string;
    reviewCount: number;
    isVerified: boolean;
    reputation?: number;
    reputationLevel?: 'Bronze' | 'Silver' | 'Gold' | 'Platinum' | 'Diamond';
  };
}

export interface ReviewPhoto {
  id: string;
  url: string;
  caption?: string;
  uploadedAt: string;
}

export interface CreateReviewRequest {
  placeId: string;
  rating: number;
  text: string;
  checkInLatitude?: number;
  checkInLongitude?: number;
  placeLatitude: number;
  placeLongitude: number;
  dietaryAccuracy?: DietaryAccuracy;
}

export interface UpdateReviewRequest {
  rating: number;
  text: string;
  dietaryAccuracy?: DietaryAccuracy;
}

export interface ReviewListResponse {
  reviews: Review[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export enum DietaryAccuracy {
  VeryAccurate = 'VeryAccurate',
  SomewhatAccurate = 'SomewhatAccurate',
  NotAccurate = 'NotAccurate',
  NotSure = 'NotSure'
}

