'use client'

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { usePlaces } from '@/hooks/usePlaces';
import { useAuth } from '@/hooks/useAuth';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { MapPin, Loader2, ArrowLeft } from 'lucide-react';
import toast from 'react-hot-toast';
import Link from 'next/link';

const createPlaceSchema = z.object({
  name: z.string().min(1, 'Name is required').max(255, 'Name too long'),
  address: z.string().min(1, 'Address is required'),
  latitude: z.number().min(-90).max(90),
  longitude: z.number().min(-180).max(180),
  phone: z.string().optional(),
  website: z.string().url('Invalid URL').optional().or(z.literal('')),
  email: z.string().email('Invalid email').optional().or(z.literal('')),
  priceRange: z.number().min(1).max(4),
  cuisineTypes: z.string().min(1, 'At least one cuisine type is required'),
});

type CreatePlaceFormData = z.infer<typeof createPlaceSchema>;

export default function CreatePlacePage() {
  const router = useRouter();
  const { isAuthenticated } = useAuth();
  const { useCreatePlace } = usePlaces();
  const createPlaceMutation = useCreatePlace();
  const [locationLoading, setLocationLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm<CreatePlaceFormData>({
    resolver: zodResolver(createPlaceSchema),
    defaultValues: {
      priceRange: 2,
      cuisineTypes: '',
    },
  });

  // Check authentication
  useEffect(() => {
    if (!isAuthenticated) {
      router.push('/login?redirect=/places/new');
    }
  }, [isAuthenticated, router]);

  if (!isAuthenticated) {
    return null;
  }

  const getCurrentLocation = () => {
    setLocationLoading(true);
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setValue('latitude', position.coords.latitude);
          setValue('longitude', position.coords.longitude);
          setLocationLoading(false);
          toast.success('Location detected!');
        },
        (error) => {
          setLocationLoading(false);
          toast.error('Could not get your location. Please enter manually.');
        }
      );
    } else {
      setLocationLoading(false);
      toast.error('Geolocation is not supported by your browser.');
    }
  };

  const onSubmit = async (data: CreatePlaceFormData) => {
    try {
      const placeData = {
        name: data.name,
        address: data.address,
        latitude: data.latitude,
        longitude: data.longitude,
        phone: data.phone || undefined,
        website: data.website || undefined,
        email: data.email || undefined,
        priceRange: data.priceRange,
        cuisineTypes: data.cuisineTypes.split(',').map(s => s.trim()).filter(Boolean),
      };

      const response = await createPlaceMutation.mutateAsync(placeData);
      
      // The API returns ApiResponse<Place>, so response is the Place object directly
      // Check if response exists (it's the data from the API response)
      if (response && typeof response === 'object' && 'id' in response) {
        toast.success('Restaurant created successfully!');
        router.push(`/places/${(response as any).id}`);
      } else if (response && typeof response === 'object' && 'success' in response) {
        // Handle ApiResponse format
        const apiResponse = response as any;
        if (apiResponse.success && apiResponse.data) {
          toast.success('Restaurant created successfully!');
          router.push(`/places/${apiResponse.data.id}`);
        } else {
          toast.error(apiResponse.message || 'Failed to create restaurant');
        }
      } else {
        toast.error('Failed to create restaurant');
      }
    } catch (error: any) {
      toast.error(error.message || 'Failed to create restaurant');
    }
  };

  return (
    <div className="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-6">
        <Link href="/" className="inline-flex items-center text-gray-600 hover:text-gray-900">
          <ArrowLeft className="w-4 h-4 mr-2" />
          Back to Home
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-2xl">Add a New Restaurant</CardTitle>
          <p className="text-gray-600 mt-2">
            Help others discover great places by adding a restaurant to The Dish.
          </p>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {/* Name */}
            <div>
              <Label htmlFor="name">Restaurant Name *</Label>
              <Input
                id="name"
                {...register('name')}
                placeholder="e.g., Joe's Pizza"
                className="mt-1"
              />
              {errors.name && (
                <p className="text-sm text-red-600 mt-1">{errors.name.message}</p>
              )}
            </div>

            {/* Address */}
            <div>
              <Label htmlFor="address">Address *</Label>
              <Input
                id="address"
                {...register('address')}
                placeholder="e.g., 123 Main St, New York, NY 10001"
                className="mt-1"
              />
              {errors.address && (
                <p className="text-sm text-red-600 mt-1">{errors.address.message}</p>
              )}
            </div>

            {/* Location */}
            <div>
              <Label>Location Coordinates *</Label>
              <div className="grid grid-cols-2 gap-4 mt-1">
                <div>
                  <Input
                    id="latitude"
                    type="number"
                    step="any"
                    {...register('latitude', { valueAsNumber: true })}
                    placeholder="Latitude (e.g., 40.7128)"
                  />
                  {errors.latitude && (
                    <p className="text-sm text-red-600 mt-1">{errors.latitude.message}</p>
                  )}
                </div>
                <div>
                  <Input
                    id="longitude"
                    type="number"
                    step="any"
                    {...register('longitude', { valueAsNumber: true })}
                    placeholder="Longitude (e.g., -74.0060)"
                  />
                  {errors.longitude && (
                    <p className="text-sm text-red-600 mt-1">{errors.longitude.message}</p>
                  )}
                </div>
              </div>
              <Button
                type="button"
                variant="outline"
                onClick={getCurrentLocation}
                disabled={locationLoading}
                className="mt-2"
              >
                <MapPin className="w-4 h-4 mr-2" />
                {locationLoading ? 'Detecting...' : 'Use My Current Location'}
              </Button>
            </div>

            {/* Contact Info */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <Label htmlFor="phone">Phone</Label>
                <Input
                  id="phone"
                  {...register('phone')}
                  placeholder="(555) 123-4567"
                  className="mt-1"
                />
              </div>
              <div>
                <Label htmlFor="website">Website</Label>
                <Input
                  id="website"
                  {...register('website')}
                  placeholder="https://example.com"
                  className="mt-1"
                />
                {errors.website && (
                  <p className="text-sm text-red-600 mt-1">{errors.website.message}</p>
                )}
              </div>
              <div>
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  {...register('email')}
                  placeholder="info@example.com"
                  className="mt-1"
                />
                {errors.email && (
                  <p className="text-sm text-red-600 mt-1">{errors.email.message}</p>
                )}
              </div>
            </div>

            {/* Price Range */}
            <div>
              <Label htmlFor="priceRange">Price Range *</Label>
              <select
                id="priceRange"
                {...register('priceRange', { valueAsNumber: true })}
                className="w-full px-3 py-2 border rounded-md mt-1"
              >
                <option value={1}>$ - Budget Friendly</option>
                <option value={2}>$$ - Moderate</option>
                <option value={3}>$$$ - Expensive</option>
                <option value={4}>$$$$ - Very Expensive</option>
              </select>
            </div>

            {/* Cuisine Types */}
            <div>
              <Label htmlFor="cuisineTypes">Cuisine Types * (comma-separated)</Label>
              <Input
                id="cuisineTypes"
                {...register('cuisineTypes')}
                placeholder="e.g., Italian, Pizza, Mediterranean"
                className="mt-1"
              />
              <p className="text-sm text-gray-500 mt-1">
                Separate multiple cuisines with commas
              </p>
              {errors.cuisineTypes && (
                <p className="text-sm text-red-600 mt-1">{errors.cuisineTypes.message}</p>
              )}
            </div>

            {/* Submit */}
            <div className="flex gap-4 pt-4">
              <Button
                type="submit"
                disabled={createPlaceMutation.isPending}
                className="flex-1"
                size="lg"
              >
                {createPlaceMutation.isPending ? (
                  <>
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    Creating...
                  </>
                ) : (
                  'Create Restaurant'
                )}
              </Button>
              <Button
                type="button"
                variant="outline"
                onClick={() => router.back()}
                size="lg"
              >
                Cancel
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}

