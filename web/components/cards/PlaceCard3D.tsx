'use client';

import { TiltCard } from './TiltCard';
import { GlassCard } from './GlassCard';
import { Badge3D } from '../ui/Badge3D';
import Image from 'next/image';

interface Place {
  id: string;
  name: string;
  cuisine: string;
  image: string;
  rating: number;
  isHalal?: boolean;
  trustScore?: number;
}

interface PlaceCard3DProps {
  place: Place;
  onClick?: () => void;
  className?: string;
}

export function PlaceCard3D({
  place,
  onClick,
  className = '',
}: PlaceCard3DProps) {
  return (
    <TiltCard className={className}>
      <GlassCard hover>
        <div
          className="group cursor-pointer p-6 h-full relative overflow-hidden"
          onClick={onClick}
          role="button"
          tabIndex={0}
          onKeyDown={(e) => {
            if (e.key === 'Enter' || e.key === ' ') {
              onClick?.();
            }
          }}
        >
          {/* Animated gradient background on hover */}
          <div className="absolute inset-0 bg-gradient-to-br from-indigo-500/20 to-purple-500/20 opacity-0 group-hover:opacity-100 transition-opacity duration-500" />

          {/* Floating badge */}
          {place.isHalal && (
            <div className="absolute top-4 right-4 z-20">
              <Badge3D variant="halal" className="floating" />
            </div>
          )}

          {/* Content */}
          <div className="relative z-10">
            <div className="relative w-full h-48 mb-4 rounded-lg overflow-hidden">
              <Image
                src={place.image || '/placeholder-restaurant.jpg'}
                alt={place.name}
                fill
                className="object-cover group-hover:scale-105 transition-transform duration-300"
                sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
              />
            </div>

            <h3 className="text-2xl font-bold text-white mb-2">
              {place.name}
            </h3>
            <p className="text-gray-300 mb-4">{place.cuisine}</p>

            {/* Animated rating */}
            <div className="flex items-center gap-2">
              <div className="flex">
                {[...Array(5)].map((_, i) => (
                  <svg
                    key={i}
                    className="w-5 h-5 text-yellow-400 transition-all duration-200"
                    style={{
                      transform: `scale(${i < Math.floor(place.rating) ? 1 : 0.8})`,
                      transitionDelay: `${i * 50}ms`,
                    }}
                    fill="currentColor"
                    viewBox="0 0 20 20"
                    aria-hidden="true"
                  >
                    <path d="M10 15l-5.878 3.09 1.123-6.545L.489 6.91l6.572-.955L10 0l2.939 5.955 6.572.955-4.756 4.635 1.123 6.545z" />
                  </svg>
                ))}
              </div>
              <span className="text-white font-semibold">{place.rating}</span>
            </div>

            {/* Trust Score */}
            {place.trustScore !== undefined && (
              <div className="mt-4 pt-4 border-t border-white/10">
                <div className="flex items-center justify-between">
                  <span className="text-sm text-gray-300">Trust Score</span>
                  <span className="text-lg font-bold text-white">
                    {place.trustScore}%
                  </span>
                </div>
              </div>
            )}
          </div>
        </div>
      </GlassCard>
    </TiltCard>
  );
}

