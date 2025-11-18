'use client'

import { useState } from 'react';
import dynamic from 'next/dynamic';
import { MapPin, ExternalLink } from 'lucide-react';
import { Place } from '@/types/place';
import { Button } from '@/components/ui/button';

// Dynamically import map components to avoid SSR issues
const MapContainer = dynamic(
  () => import('react-leaflet').then((mod) => mod.MapContainer),
  { 
    ssr: false,
    loading: () => (
      <div className="h-48 bg-gray-100 rounded flex items-center justify-center">
        <div className="flex items-center space-x-2 text-gray-500">
          <MapPin className="w-5 h-5" />
          <span>Loading map...</span>
        </div>
      </div>
    )
  }
);

const TileLayer = dynamic(
  () => import('react-leaflet').then((mod) => mod.TileLayer),
  { ssr: false }
);

const Marker = dynamic(
  () => import('react-leaflet').then((mod) => mod.Marker),
  { ssr: false }
);

const Popup = dynamic(
  () => import('react-leaflet').then((mod) => mod.Popup),
  { ssr: false }
);

interface PlaceMapProps {
  place: Place;
  height?: string;
}

export function PlaceMap({ place, height = 'h-48' }: PlaceMapProps) {
  const [mapError, setMapError] = useState(false);

  if (mapError) {
    return (
      <div className={`${height} bg-gray-100 rounded flex flex-col items-center justify-center space-y-3`}>
        <div className="flex items-center space-x-2 text-gray-500">
          <MapPin className="w-5 h-5" />
          <span>Map unavailable</span>
        </div>
        <Button
          variant="outline"
          size="sm"
          onClick={() => {
            const mapsUrl = `https://maps.google.com/maps?q=${place.latitude},${place.longitude}`;
            window.open(mapsUrl, '_blank');
          }}
          className="flex items-center space-x-2"
        >
          <ExternalLink className="w-4 h-4" />
          <span>View on Google Maps</span>
        </Button>
      </div>
    );
  }

  if (typeof window === 'undefined') {
    return null; // Let the loading component handle SSR
  }

  try {
    return (
      <div className={height}>
        <MapContainer
          center={[place.latitude, place.longitude]}
          zoom={15}
          style={{ height: '100%', width: '100%' }}
          className="rounded"
        >
          <TileLayer
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          />
          <Marker position={[place.latitude, place.longitude]}>
            <Popup>
              <div className="p-2">
                <h3 className="font-semibold text-sm">{place.name}</h3>
                <p className="text-xs text-gray-600">{place.address}</p>
                {place.phone && (
                  <p className="text-xs text-gray-600 mt-1">{place.phone}</p>
                )}
                <Button
                  variant="outline"
                  size="sm"
                  className="mt-2 text-xs"
                  onClick={() => {
                    const mapsUrl = `https://maps.google.com/maps?q=${place.latitude},${place.longitude}`;
                    window.open(mapsUrl, '_blank');
                  }}
                >
                  <ExternalLink className="w-3 h-3 mr-1" />
                  Directions
                </Button>
              </div>
            </Popup>
          </Marker>
        </MapContainer>
      </div>
    );
  } catch (error) {
    console.error('Map error:', error);
    return (
      <div className={`${height} bg-gray-100 rounded flex flex-col items-center justify-center space-y-3`}>
        <div className="flex items-center space-x-2 text-gray-500">
          <MapPin className="w-5 h-5" />
          <span>Map unavailable</span>
        </div>
        <Button
          variant="outline"
          size="sm"
          onClick={() => {
            const mapsUrl = `https://maps.google.com/maps?q=${place.latitude},${place.longitude}`;
            window.open(mapsUrl, '_blank');
          }}
          className="flex items-center space-x-2"
        >
          <ExternalLink className="w-4 h-4" />
          <span>View on Google Maps</span>
        </Button>
      </div>
    );
  }
}
