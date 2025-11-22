'use client'

import { useState, useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { MapPin, Star, Camera, CheckCircle, AlertCircle } from 'lucide-react';
import { Button3D } from '@/components/ui/Button3D';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { GlassCard } from '@/components/cards/GlassCard';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { usePlaces } from '@/hooks/usePlaces';
import { useReviews } from '@/hooks/useReviews';
import { useAuth } from '@/hooks/useAuth';
import { DietaryAccuracy } from '@/types/review';
import { PhotoUpload } from '@/components/features/reviews/photo-upload';
import toast from 'react-hot-toast';

const reviewSchema = z.object({
  rating: z.number().min(1, 'Please select a rating').max(5),
  text: z.string().min(10, 'Review must be at least 10 characters'),
  dietaryAccuracy: z.nativeEnum(DietaryAccuracy).optional(),
});

type ReviewFormData = z.infer<typeof reviewSchema>;

interface GpsLocation {
  latitude: number;
  longitude: number;
  accuracy: number;
}

export default function ReviewPage() {
  const params = useParams();
  const id = params.id as string;
  const router = useRouter();
  const { usePlace } = usePlaces();
  const { useCreateReview } = useReviews();
  const { isAuthenticated } = useAuth();

  // States
  const [selectedRating, setSelectedRating] = useState(0);
  const [hoverRating, setHoverRating] = useState(0);
  const [userLocation, setUserLocation] = useState<GpsLocation | null>(null);
  const [locationLoading, setLocationLoading] = useState(false);
  const [locationError, setLocationError] = useState<string | null>(null);
  const [isVerified, setIsVerified] = useState(false);
  const [distance, setDistance] = useState<number | null>(null);
  const [selectedPhotos, setSelectedPhotos] = useState<File[]>([]);

  // Hooks
  const { data: placeResponse, isLoading: placeLoading } = usePlace(id);
  const createReviewMutation = useCreateReview();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
  } = useForm<ReviewFormData>({
    resolver: zodResolver(reviewSchema),
  });

  // Check authentication
  useEffect(() => {
    if (!isAuthenticated) {
      router.push(`/login?redirect=/places/${id}/review`);
    }
  }, [isAuthenticated, router, id]);

  // Get user location on mount
  useEffect(() => {
    requestLocation();
  }, []);

  const requestLocation = () => {
    if (!navigator.geolocation) {
      setLocationError('Geolocation is not supported by this browser');
      return;
    }

    setLocationLoading(true);
    setLocationError(null);

    navigator.geolocation.getCurrentPosition(
      (position) => {
        const location: GpsLocation = {
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
          accuracy: position.coords.accuracy,
        };
        setUserLocation(location);
        setLocationLoading(false);

        // Calculate distance if we have place data
        if (placeResponse?.success && placeResponse.data) {
          const dist = calculateDistance(
            location.latitude,
            location.longitude,
            placeResponse.data.latitude,
            placeResponse.data.longitude
          );
          setDistance(dist);
          setIsVerified(dist <= 200); // Verify if within 200m
        }
      },
      (error) => {
        setLocationLoading(false);
        switch (error.code) {
          case error.PERMISSION_DENIED:
            setLocationError('Location access denied. Please enable location services.');
            break;
          case error.POSITION_UNAVAILABLE:
            setLocationError('Location information is unavailable.');
            break;
          case error.TIMEOUT:
            setLocationError('Location request timed out.');
            break;
          default:
            setLocationError('An unknown error occurred.');
            break;
        }
      },
      {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 300000, // 5 minutes
      }
    );
  };

  const calculateDistance = (lat1: number, lon1: number, lat2: number, lon2: number): number => {
    const R = 6371e3; // Earth's radius in meters
    const φ1 = (lat1 * Math.PI) / 180;
    const φ2 = (lat2 * Math.PI) / 180;
    const Δφ = ((lat2 - lat1) * Math.PI) / 180;
    const Δλ = ((lon2 - lon1) * Math.PI) / 180;

    const a =
      Math.sin(Δφ / 2) * Math.sin(Δφ / 2) +
      Math.cos(φ1) * Math.cos(φ2) * Math.sin(Δλ / 2) * Math.sin(Δλ / 2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

    return R * c; // Distance in meters
  };

  const onSubmit = async (data: ReviewFormData) => {
    if (!placeResponse?.success || !placeResponse.data) return;

    // Verify token exists before submitting
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('auth_token');
      if (!token) {
        toast.error('Your session has expired. Please log in again.');
        router.push(`/login?redirect=/places/${id}/review`);
        return;
      }
    }

    try {
      // Create the review first
      const reviewResponse = await createReviewMutation.mutateAsync({
        placeId: id,
        rating: data.rating,
        text: data.text,
        checkInLatitude: userLocation?.latitude,
        checkInLongitude: userLocation?.longitude,
        placeLatitude: placeResponse.data.latitude,
        placeLongitude: placeResponse.data.longitude,
        dietaryAccuracy: data.dietaryAccuracy,
      });

      // TODO: Upload photos if any are selected
      // Note: Photo upload for reviews would require a separate endpoint
      // This could be implemented as a separate API call after review creation
      if (selectedPhotos.length > 0 && reviewResponse.success && reviewResponse.data) {
        console.log('Photos to upload:', selectedPhotos.length);
        // Future implementation: Upload photos to review
        toast.success(`Review submitted with ${selectedPhotos.length} photo${selectedPhotos.length !== 1 ? 's' : ''}!`);
      }

      // Success - redirect to place page
      router.push(`/places/${id}`);
    } catch (error) {
      console.error('Failed to create review:', error);
    }
  };

  if (!isAuthenticated) {
    return null; // Will redirect to login
  }

  if (placeLoading) {
    return (
      <div className="min-h-screen bg-[#0f172a]">
        <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-700 rounded w-1/3 mb-4"></div>
            <div className="h-64 bg-gray-700 rounded"></div>
          </div>
        </div>
      </div>
    );
  }

  if (!placeResponse?.success || !placeResponse.data) {
    return (
      <div className="min-h-screen bg-[#0f172a]">
        <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="text-center">
            <h1 className="text-2xl font-bold text-white mb-4">Place Not Found</h1>
            <Button3D variant="primary" onClick={() => router.back()}>Go Back</Button3D>
          </div>
        </div>
      </div>
    );
  }

  const place = placeResponse.data;

  return (
    <div className="min-h-screen bg-[#0f172a]">
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <FadeInUp>
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-white mb-2">Write a Review</h1>
            <p className="text-lg text-gray-300">Share your experience at {place.name}</p>
          </div>
        </FadeInUp>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* GPS Verification Section */}
          <FadeInUp delay={0.1}>
            <GlassCard className="p-6">
              <h2 className="text-xl font-bold text-white mb-4 flex items-center space-x-2">
                <MapPin className="w-5 h-5" />
                <span>Location Verification</span>
              </h2>
              <div>
                {locationLoading ? (
                  <div className="flex items-center space-x-2 text-indigo-400">
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-indigo-400"></div>
                    <span>Getting your location...</span>
                  </div>
                ) : locationError ? (
                  <div className="space-y-3">
                    <div className="flex items-center space-x-2 text-red-400">
                      <AlertCircle className="w-5 h-5" />
                      <span>{locationError}</span>
                    </div>
                    <Button3D
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={requestLocation}
                    >
                      Try Again
                    </Button3D>
                  </div>
                ) : isVerified ? (
                  <div className="flex items-center space-x-2 text-green-400">
                    <CheckCircle className="w-5 h-5" />
                    <span>
                      Location verified! You are {distance?.toFixed(0)}m from this place.
                    </span>
                  </div>
                ) : distance !== null ? (
                  <div className="flex items-center space-x-2 text-orange-400">
                    <AlertCircle className="w-5 h-5" />
                    <span>
                      You are {distance.toFixed(0)}m from this place. GPS verification requires being within 200m.
                    </span>
                  </div>
                ) : (
                  <div className="text-gray-300">
                    Location not available. Your review will be submitted without GPS verification.
                  </div>
                )}
              </div>
            </GlassCard>
          </FadeInUp>

          {/* Rating Section */}
          <FadeInUp delay={0.2}>
            <GlassCard className="p-6">
              <h2 className="text-xl font-bold text-white mb-4">Overall Rating</h2>
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
                          : 'text-gray-600'
                      }`}
                    />
                  </button>
                ))}
                {selectedRating > 0 && (
                  <span className="ml-2 text-sm text-gray-300">
                    {selectedRating} star{selectedRating !== 1 ? 's' : ''}
                  </span>
                )}
              </div>
              {errors.rating && (
                <p className="text-red-400 text-sm mt-2">{errors.rating.message}</p>
              )}
            </GlassCard>
          </FadeInUp>

          {/* Review Text */}
          <FadeInUp delay={0.3}>
            <GlassCard className="p-6">
              <h2 className="text-xl font-bold text-white mb-4">Your Review</h2>
              <textarea
                {...register('text')}
                placeholder="Share details about your experience at this restaurant..."
                className="w-full h-32 p-3 bg-gray-800/50 border border-gray-700 rounded-md resize-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent text-white placeholder-gray-400"
              />
              {errors.text && (
                <p className="text-red-400 text-sm mt-2">{errors.text.message}</p>
              )}
            </GlassCard>
          </FadeInUp>

          {/* Dietary Accuracy */}
          {place.dietaryTags && place.dietaryTags.length > 0 && (
            <FadeInUp delay={0.4}>
              <GlassCard className="p-6">
                <h2 className="text-xl font-bold text-white mb-2">Dietary Accuracy</h2>
                <p className="text-sm text-gray-300 mb-4">
                  How accurate were the dietary labels for this restaurant?
                </p>
                <div className="space-y-2">
                  {Object.values(DietaryAccuracy).map((accuracy) => (
                    <label key={accuracy} className="flex items-center space-x-2 cursor-pointer">
                      <input
                        type="radio"
                        value={accuracy}
                        {...register('dietaryAccuracy')}
                        className="text-indigo-500 accent-indigo-500"
                      />
                      <span className="text-sm text-gray-200">{accuracy.replace(/([A-Z])/g, ' $1').trim()}</span>
                    </label>
                  ))}
                </div>
              </GlassCard>
            </FadeInUp>
          )}

          {/* Photo Upload */}
          <FadeInUp delay={0.5}>
            <GlassCard className="p-6">
              <h2 className="text-xl font-bold text-white mb-4 flex items-center space-x-2">
                <Camera className="w-5 h-5" />
                <span>Add Photos (Optional)</span>
              </h2>
              <PhotoUpload
                onPhotosChange={setSelectedPhotos}
                maxPhotos={5}
                maxSize={10}
              />
            </GlassCard>
          </FadeInUp>

          {/* Submit Button */}
          <FadeInUp delay={0.6}>
            <div className="flex space-x-4">
              <Button3D
                type="submit"
                variant="primary"
                disabled={createReviewMutation.isPending || selectedRating === 0}
                className="flex-1"
              >
                {createReviewMutation.isPending ? 'Submitting...' : 'Submit Review'}
              </Button3D>
              <Button3D type="button" variant="outline" onClick={() => router.back()}>
                Cancel
              </Button3D>
            </div>
          </FadeInUp>
        </form>
      </div>
    </div>
  );
}
