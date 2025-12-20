'use client';

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Star, Upload, MapPin, Check, AlertCircle } from 'lucide-react';
import { Button3D } from '@/components/ui/Button3D';
import { GlassCard } from '@/components/cards/GlassCard';
import { useReviews } from '@/hooks/useReviews';
import { DietaryAccuracy } from '@/types/review';
import { LoadingSpinner } from '@/components/ui/loading';
import { cn } from '@/lib/utils/cn';
import { PhotoUpload } from '@/components/features/reviews/photo-upload';
import { usePhotos } from '@/hooks/usePhotos';

const reviewSchema = z.object({
  rating: z.number().min(1, 'Please select a rating'),
  text: z.string().min(10, 'Review must be at least 10 characters').max(1000, 'Review must be less than 1000 characters'),
  dietaryAccuracy: z.nativeEnum(DietaryAccuracy).optional(),
});

type ReviewFormData = z.infer<typeof reviewSchema>;

interface ReviewSubmissionFormProps {
  placeId: string;
  placeName: string;
  placeLatitude?: number;
  placeLongitude?: number;
  onSuccess?: () => void;
  onCancel?: () => void;
}

export function ReviewSubmissionForm({
  placeId,
  placeName,
  placeLatitude,
  placeLongitude,
  onSuccess,
  onCancel,
}: ReviewSubmissionFormProps) {
  const [photos, setPhotos] = useState<File[]>([]);
  const [location, setLocation] = useState<{ lat: number; lng: number } | null>(null);
  const [locationError, setLocationError] = useState<string | null>(null);
  const [isGettingLocation, setIsGettingLocation] = useState(false);

  const { useCreateReview } = useReviews();
  const createReview = useCreateReview();
  const { useUploadPhoto } = usePhotos();
  const uploadPhoto = useUploadPhoto();

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<ReviewFormData>({
    resolver: zodResolver(reviewSchema),
    defaultValues: {
      rating: 0,
      text: '',
      dietaryAccuracy: undefined,
    },
  });

  const rating = watch('rating');

  const handlePhotosChange = (newPhotos: File[]) => {
    setPhotos(newPhotos);
  };

  const getLocation = () => {
    setIsGettingLocation(true);
    setLocationError(null);

    if (!navigator.geolocation) {
      setLocationError('Geolocation is not supported by your browser');
      setIsGettingLocation(false);
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (position) => {
        setLocation({
          lat: position.coords.latitude,
          lng: position.coords.longitude,
        });
        setIsGettingLocation(false);
      },
      (error) => {
        setLocationError('Unable to retrieve your location');
        setIsGettingLocation(false);
      }
    );
  };

  const onSubmit = async (data: ReviewFormData) => {
    try {
      // Upload photos first
      const uploadedUrls: string[] = [];

      if (photos.length > 0) {
        const uploadPromises = photos.map(photo => uploadPhoto.mutateAsync(photo));
        const results = await Promise.all(uploadPromises);
        results.forEach(res => {
          if (res.success && res.data?.url) uploadedUrls.push(res.data.url);
        });
      }

      await createReview.mutateAsync({
        placeId,
        rating: data.rating,
        text: data.text,
        photoUrls: uploadedUrls,
        checkInLatitude: location?.lat,
        checkInLongitude: location?.lng,
        placeLatitude: placeLatitude || 0,
        placeLongitude: placeLongitude || 0,
        dietaryAccuracy: data.dietaryAccuracy,
      });

      onSuccess?.();
    } catch (error) {
      console.error('Failed to submit review:', error);
    }
  };

  return (
    <GlassCard className="p-6 w-full max-w-2xl mx-auto">
      <h2 className="text-2xl font-bold text-white mb-6">Write a Review for {placeName}</h2>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        {/* Rating */}
        <div className="space-y-2">
          <label className="block text-sm font-medium text-gray-300">Rating</label>
          <div className="flex gap-2">
            {[1, 2, 3, 4, 5].map((star) => (
              <button
                key={star}
                type="button"
                onClick={() => setValue('rating', star, { shouldValidate: true })}
                className="focus:outline-none transition-transform hover:scale-110"
              >
                <Star
                  className={cn(
                    'w-8 h-8 transition-colors',
                    star <= rating ? 'text-yellow-400 fill-yellow-400' : 'text-gray-600'
                  )}
                />
              </button>
            ))}
          </div>
          {errors.rating && (
            <p className="text-red-400 text-sm">{errors.rating.message}</p>
          )}
        </div>

        {/* Review Text */}
        <div className="space-y-2">
          <label className="block text-sm font-medium text-gray-300">Your Review</label>
          <textarea
            {...register('text')}
            rows={4}
            className="w-full bg-white/5 border border-white/10 rounded-lg p-3 text-white placeholder:text-gray-500 focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none transition-all"
            placeholder="Share your experience..."
          />
          {errors.text && (
            <p className="text-red-400 text-sm">{errors.text.message}</p>
          )}
        </div>

        {/* Dietary Accuracy */}
        <div className="space-y-2">
          <label className="block text-sm font-medium text-gray-300">Dietary Accuracy</label>
          <p className="text-xs text-gray-400 mb-2">How accurate were the dietary labels?</p>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            {Object.values(DietaryAccuracy).map((value) => (
              <label
                key={value}
                className={cn(
                  'flex items-center p-3 rounded-lg border cursor-pointer transition-all',
                  watch('dietaryAccuracy') === value
                    ? 'bg-indigo-500/20 border-indigo-500'
                    : 'bg-white/5 border-transparent hover:bg-white/10'
                )}
              >
                <input
                  type="radio"
                  value={value}
                  {...register('dietaryAccuracy')}
                  className="sr-only"
                />
                <span className="text-sm text-white">
                  {value.replace(/([A-Z])/g, ' $1').trim()}
                </span>
                {watch('dietaryAccuracy') === value && (
                  <Check className="w-4 h-4 text-indigo-400 ml-auto" />
                )}
              </label>
            ))}
          </div>
        </div>

        {/* Photos */}
        <div className="space-y-2">
          <label className="block text-sm font-medium text-gray-300">Photos</label>
          <PhotoUpload
            onPhotosChange={handlePhotosChange}
            maxPhotos={5}
            maxSize={10}
          />
        </div>

        {/* GPS Verification */}
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <label className="block text-sm font-medium text-gray-300">GPS Verification</label>
            {location && (
              <span className="text-xs text-green-400 flex items-center gap-1">
                <Check className="w-3 h-3" /> Location acquired
              </span>
            )}
          </div>
          <button
            type="button"
            onClick={getLocation}
            disabled={isGettingLocation || !!location}
            className={cn(
              'w-full flex items-center justify-center gap-2 p-3 rounded-lg border transition-all',
              location
                ? 'bg-green-500/10 border-green-500/50 text-green-400'
                : 'bg-white/5 border-white/10 text-gray-300 hover:bg-white/10'
            )}
          >
            {isGettingLocation ? (
              <LoadingSpinner size="sm" />
            ) : (
              <MapPin className="w-4 h-4" />
            )}
            {location ? 'GPS Verified' : 'Verify Location (Optional)'}
          </button>
          {locationError && (
            <p className="text-red-400 text-xs flex items-center gap-1">
              <AlertCircle className="w-3 h-3" /> {locationError}
            </p>
          )}
          <p className="text-xs text-gray-500">
            Verifying your location helps build trust in the community.
          </p>
        </div>

        {/* Actions */}
        <div className="flex gap-4 pt-4">
          <Button3D
            type="button"
            variant="outline"
            onClick={onCancel}
            className="flex-1"
          >
            Cancel
          </Button3D>
          <Button3D
            type="submit"
            disabled={createReview.isPending || uploadPhoto.isPending}
            className="flex-1"
            glow
          >
            {createReview.isPending || uploadPhoto.isPending ? <LoadingSpinner size="sm" /> : 'Submit Review'}
          </Button3D>
        </div>
      </form>
    </GlassCard>
  );
}
