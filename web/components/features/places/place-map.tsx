'use client'

import { MapPin, ExternalLink } from 'lucide-react';
import { Place } from '@/types/place';
import { Button } from '@/components/ui/button';

interface PlaceMapProps {
  place: Place;
  height?: string;
}

export function PlaceMap({ place, height = 'h-48' }: PlaceMapProps) {
  // OpenStreetMap embed URL - no API key required
  const mapEmbedUrl = `https://www.openstreetmap.org/export/embed.html?bbox=${place.longitude - 0.01},${place.latitude - 0.01},${place.longitude + 0.01},${place.latitude + 0.01}&layer=mapnik&marker=${place.latitude},${place.longitude}`;
  const mapsUrl = `https://maps.google.com/maps?q=${place.latitude},${place.longitude}`;

  return (
    <div className={`${height} rounded overflow-hidden relative bg-gray-100`}>
      <iframe
        width="100%"
        height="100%"
        style={{ border: 0 }}
        loading="lazy"
        allowFullScreen
        referrerPolicy="no-referrer-when-downgrade"
        src={mapEmbedUrl}
        className="rounded"
        title={`Map showing ${place.name}`}
      />
      <div className="absolute bottom-2 right-2 z-10">
        <Button
          variant="outline"
          size="sm"
          onClick={() => window.open(mapsUrl, '_blank')}
          className="flex items-center space-x-2 bg-white/90 hover:bg-white backdrop-blur-sm"
        >
          <ExternalLink className="w-4 h-4" />
          <span>Open in Maps</span>
        </Button>
      </div>
    </div>
  );
}
