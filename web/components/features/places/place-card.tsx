import Link from 'next/link';
import { Star, MapPin, DollarSign, CheckCircle2 } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Place } from '@/types/place';

interface PlaceCardProps {
  place: Place;
}

export function PlaceCard({ place }: PlaceCardProps) {
  const getPriceRangeText = (priceRange: number) => {
    return '$'.repeat(priceRange);
  };

  return (
    <Link href={`/places/${place.id}`} className="block group">
      <Card className="h-full overflow-hidden transition-all duration-300 hover:shadow-xl hover:-translate-y-1 border-gray-200 hover:border-blue-300">
        <CardContent className="p-0">
          {/* Image section with overlay on hover */}
          <div className="h-56 bg-gray-200 rounded-t-lg relative overflow-hidden">
            {place.photos && place.photos.length > 0 ? (
              <>
                <img
                  src={place.photos[0].url}
                  alt={place.name}
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
              </>
            ) : (
              <div className="w-full h-full bg-gradient-to-br from-blue-100 via-purple-100 to-pink-100 flex items-center justify-center group-hover:from-blue-200 group-hover:via-purple-200 group-hover:to-pink-200 transition-all duration-300">
                <div className="text-center">
                  <MapPin className="w-12 h-12 text-gray-400 mx-auto mb-2" />
                  <span className="text-gray-500 text-sm font-medium">No Photo</span>
                </div>
              </div>
            )}
            
            {/* Verified badge */}
            {place.isVerified && (
              <div className="absolute top-3 right-3 bg-green-500 text-white px-2.5 py-1 rounded-full text-xs font-semibold flex items-center gap-1 shadow-lg backdrop-blur-sm">
                <CheckCircle2 className="w-3 h-3" />
                <span>Verified</span>
              </div>
            )}

            {/* Price range badge */}
            <div className="absolute top-3 left-3 bg-white/95 backdrop-blur-sm text-gray-900 px-2.5 py-1 rounded-full text-xs font-semibold flex items-center gap-1 shadow-md">
              <DollarSign className="w-3 h-3" />
              <span>{getPriceRangeText(place.priceRange)}</span>
            </div>
          </div>

          {/* Content section */}
          <div className="p-5">
            {/* Name */}
            <h3 className="font-bold text-lg text-gray-900 line-clamp-1 mb-2 group-hover:text-blue-600 transition-colors">
              {place.name}
            </h3>

            {/* Address */}
            <div className="flex items-start text-sm text-gray-600 mb-3">
              <MapPin className="w-4 h-4 mr-1.5 mt-0.5 flex-shrink-0" />
              <span className="line-clamp-2">{place.address}</span>
            </div>

            {/* Rating */}
            <div className="flex items-center gap-2 mb-4">
              <div className="flex items-center bg-yellow-50 px-2.5 py-1 rounded-full">
                <Star className="w-4 h-4 text-yellow-500 fill-current" />
                <span className="ml-1.5 text-sm font-bold text-gray-900">
                  {place.averageRating.toFixed(1)}
                </span>
              </div>
              <span className="text-sm text-gray-500">
                ({place.reviewCount} {place.reviewCount === 1 ? 'review' : 'reviews'})
              </span>
            </div>

            {/* Cuisine types */}
            {place.cuisineTypes && place.cuisineTypes.length > 0 && (
              <div className="flex flex-wrap gap-1.5 mb-3">
                {place.cuisineTypes.slice(0, 3).map((cuisine) => (
                  <span
                    key={cuisine}
                    className="inline-block bg-blue-50 text-blue-700 text-xs px-2.5 py-1 rounded-full font-medium border border-blue-100"
                  >
                    {cuisine}
                  </span>
                ))}
                {place.cuisineTypes.length > 3 && (
                  <span className="inline-block text-gray-500 text-xs px-2.5 py-1">
                    +{place.cuisineTypes.length - 3}
                  </span>
                )}
              </div>
            )}

            {/* Dietary tags */}
            {place.dietaryTags && place.dietaryTags.length > 0 && (
              <div className="flex flex-wrap gap-1.5">
                {place.dietaryTags.slice(0, 2).map((tag) => (
                  <span
                    key={tag}
                    className="inline-block bg-green-50 text-green-700 text-xs px-2.5 py-1 rounded-full font-medium border border-green-100"
                  >
                    {tag}
                  </span>
                ))}
                {place.dietaryTags.length > 2 && (
                  <span className="inline-block text-green-600 text-xs px-2.5 py-1 font-medium">
                    +{place.dietaryTags.length - 2} more
                  </span>
                )}
              </div>
            )}
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}

