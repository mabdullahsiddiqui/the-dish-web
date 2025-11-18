'use client';

import { GlassCard } from '@/components/cards/GlassCard';

export function PlaceCardSkeleton() {
  return (
    <GlassCard className="p-0 h-full overflow-hidden animate-pulse">
      {/* Image skeleton */}
      <div className="h-48 bg-gray-700/50 rounded-t-lg" />
      
      {/* Content skeleton */}
      <div className="p-6 space-y-4">
        {/* Title */}
        <div className="h-6 bg-gray-700/50 rounded w-3/4" />
        
        {/* Address */}
        <div className="h-4 bg-gray-700/50 rounded w-full" />
        <div className="h-4 bg-gray-700/50 rounded w-2/3" />
        
        {/* Rating */}
        <div className="flex items-center gap-2">
          <div className="h-5 bg-gray-700/50 rounded w-16" />
          <div className="h-4 bg-gray-700/50 rounded w-24" />
        </div>
        
        {/* Tags */}
        <div className="flex gap-2">
          <div className="h-6 bg-gray-700/50 rounded-full w-20" />
          <div className="h-6 bg-gray-700/50 rounded-full w-16" />
        </div>
      </div>
    </GlassCard>
  );
}

