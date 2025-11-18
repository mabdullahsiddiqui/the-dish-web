import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { placesApi } from '@/lib/api/places';
import { SearchPlacesRequest, CreatePlaceRequest, UpdatePlaceRequest } from '@/types/place';

export function usePlaces() {
  const queryClient = useQueryClient();

  // Get place by ID
  const usePlace = (id: string) => {
    return useQuery({
      queryKey: ['place', id],
      queryFn: () => placesApi.getById(id),
      enabled: !!id,
    });
  };

  // Search places
  const useSearchPlaces = (params: SearchPlacesRequest) => {
    return useQuery({
      queryKey: ['places', 'search', params],
      queryFn: () => placesApi.search(params),
    });
  };

  // Get nearby places
  const useNearbyPlaces = (params: {
    latitude: number;
    longitude: number;
    radiusKm?: number;
    dietaryFilters?: string[];
    cuisineFilters?: string[];
    priceRange?: number;
  }) => {
    return useQuery({
      queryKey: ['places', 'nearby', params],
      queryFn: () => placesApi.getNearby(params),
      enabled: !!(params.latitude && params.longitude),
    });
  };

  // Create place mutation
  const useCreatePlace = () => {
    return useMutation({
      mutationFn: placesApi.create,
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['places'] });
      },
    });
  };

  // Update place mutation
  const useUpdatePlace = () => {
    return useMutation({
      mutationFn: ({ id, data }: { id: string; data: UpdatePlaceRequest }) =>
        placesApi.update(id, data),
      onSuccess: (_, variables) => {
        queryClient.invalidateQueries({ queryKey: ['place', variables.id] });
        queryClient.invalidateQueries({ queryKey: ['places'] });
      },
    });
  };

  // Claim place mutation
  const useClaimPlace = () => {
    return useMutation({
      mutationFn: placesApi.claim,
      onSuccess: (_, placeId) => {
        queryClient.invalidateQueries({ queryKey: ['place', placeId] });
      },
    });
  };

  // Upload photo mutation
  const useUploadPlacePhoto = () => {
    return useMutation({
      mutationFn: ({ 
        placeId, 
        file, 
        caption, 
        isFeatured 
      }: { 
        placeId: string; 
        file: File; 
        caption?: string; 
        isFeatured?: boolean;
      }) => placesApi.uploadPhoto(placeId, file, caption, isFeatured),
      onSuccess: (_, variables) => {
        queryClient.invalidateQueries({ queryKey: ['place', variables.placeId] });
      },
    });
  };

  return {
    usePlace,
    useSearchPlaces,
    useNearbyPlaces,
    useCreatePlace,
    useUpdatePlace,
    useClaimPlace,
    useUploadPlacePhoto,
  };
}

