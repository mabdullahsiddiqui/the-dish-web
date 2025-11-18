import { api } from './client';
import { 
  Place, 
  CreatePlaceRequest, 
  UpdatePlaceRequest, 
  SearchPlacesRequest, 
  SearchPlacesResponse,
  PlacePhoto 
} from '@/types/place';

export const placesApi = {
  // Get place by ID
  getById: async (id: string) => {
    const response = await api.get<Place>(`/places/${id}`);
    return response.data;
  },

  // Get nearby places
  getNearby: async (params: {
    latitude: number;
    longitude: number;
    radiusKm?: number;
    dietaryFilters?: string[];
    cuisineFilters?: string[];
    priceRange?: number;
  }) => {
    const searchParams = new URLSearchParams();
    searchParams.append('latitude', params.latitude.toString());
    searchParams.append('longitude', params.longitude.toString());
    
    if (params.radiusKm) searchParams.append('radiusKm', params.radiusKm.toString());
    if (params.dietaryFilters?.length) {
      params.dietaryFilters.forEach(filter => searchParams.append('dietaryFilters', filter));
    }
    if (params.cuisineFilters?.length) {
      params.cuisineFilters.forEach(filter => searchParams.append('cuisineFilters', filter));
    }
    if (params.priceRange) searchParams.append('priceRange', params.priceRange.toString());

    const response = await api.get<Place[]>(`/places/nearby?${searchParams.toString()}`);
    return response.data;
  },

  // Search places
  search: async (params: SearchPlacesRequest) => {
    const searchParams = new URLSearchParams();
    
    if (params.searchTerm) searchParams.append('searchTerm', params.searchTerm);
    if (params.cuisineTypes?.length) {
      params.cuisineTypes.forEach(type => searchParams.append('cuisineTypes', type));
    }
    if (params.dietaryTags?.length) {
      params.dietaryTags.forEach(tag => searchParams.append('dietaryTags', tag));
    }
    if (params.minPriceRange) searchParams.append('minPriceRange', params.minPriceRange.toString());
    if (params.maxPriceRange) searchParams.append('maxPriceRange', params.maxPriceRange.toString());
    if (params.minRating) searchParams.append('minRating', params.minRating.toString());
    if (params.latitude) searchParams.append('latitude', params.latitude.toString());
    if (params.longitude) searchParams.append('longitude', params.longitude.toString());
    if (params.radiusKm) searchParams.append('radiusKm', params.radiusKm.toString());
    if (params.page) searchParams.append('page', params.page.toString());
    if (params.pageSize) searchParams.append('pageSize', params.pageSize.toString());

    const response = await api.get<SearchPlacesResponse>(`/places/search?${searchParams.toString()}`);
    return response.data;
  },

  // Create new place
  create: async (data: CreatePlaceRequest) => {
    const response = await api.post<Place>('/places', data);
    return response.data;
  },

  // Update place
  update: async (id: string, data: UpdatePlaceRequest) => {
    const response = await api.put<Place>(`/places/${id}`, data);
    return response.data;
  },

  // Claim place
  claim: async (id: string) => {
    const response = await api.post<Place>(`/places/${id}/claim`);
    return response.data;
  },

  // Upload place photo
  uploadPhoto: async (
    placeId: string, 
    file: File, 
    caption?: string, 
    isFeatured?: boolean
  ) => {
    const formData = new FormData();
    formData.append('file', file);
    if (caption) formData.append('caption', caption);
    if (isFeatured !== undefined) formData.append('isFeatured', isFeatured.toString());

    const response = await api.post<PlacePhoto>(
      `/places/${placeId}/photos`, 
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },
};

