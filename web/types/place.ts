import { Location } from './api';

export interface Place {
  id: string;
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  phone?: string;
  website?: string;
  email?: string;
  priceRange: number; // 1-4
  cuisineTypes: string[];
  averageRating: number;
  reviewCount: number;
  hoursOfOperation?: Record<string, string>;
  amenities?: string[];
  dietaryTags?: string[];
  parkingInfo?: string;
  photos?: PlacePhoto[];
  isVerified: boolean;
  isClaimed: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface PlacePhoto {
  id: string;
  url: string;
  caption?: string;
  isFeatured: boolean;
  uploadedAt: string;
}

export interface CreatePlaceRequest {
  name: string;
  address: string;
  latitude: number;
  longitude: number;
  phone?: string;
  website?: string;
  email?: string;
  priceRange: number;
  cuisineTypes: string[];
  hoursOfOperation?: Record<string, string>;
  amenities?: string[];
  parkingInfo?: string;
}

export interface UpdatePlaceRequest extends CreatePlaceRequest {}

export interface SearchPlacesRequest {
  searchTerm?: string;
  cuisineTypes?: string[];
  dietaryTags?: string[];
  minPriceRange?: number;
  maxPriceRange?: number;
  minRating?: number;
  latitude?: number;
  longitude?: number;
  radiusKm?: number;
  page?: number;
  pageSize?: number;
}

export interface SearchPlacesResponse {
  places: Place[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

