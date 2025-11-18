import { Place } from '@/types/place';

export interface PlaceCard3DData {
  id: string;
  name: string;
  cuisine: string;
  image: string;
  rating: number;
  isHalal?: boolean;
  trustScore?: number;
}

/**
 * Maps a Place object to PlaceCard3D interface
 */
export function mapPlaceToCard3D(place: Place): PlaceCard3DData {
  // Safely check if dietaryTags is an array and includes 'Halal'
  const isHalal = Array.isArray(place.dietaryTags) 
    ? place.dietaryTags.includes('Halal')
    : false;

  return {
    id: place.id,
    name: place.name,
    cuisine: Array.isArray(place.cuisineTypes) && place.cuisineTypes.length > 0
      ? place.cuisineTypes[0]
      : 'Restaurant',
    image: place.photos?.[0]?.url || '/placeholder-restaurant.jpg',
    rating: place.averageRating || 0,
    isHalal,
    trustScore: place.isVerified 
      ? Math.min(95, Math.max(60, Math.round((place.averageRating || 0) * 20))) 
      : undefined,
  };
}

