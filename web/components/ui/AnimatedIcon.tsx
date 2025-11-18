'use client';

import { ReactNode } from 'react';
import { cn } from '@/lib/utils/cn';
import {
  Rocket,
  Star,
  Heart,
  MapPin,
  Utensils,
  Award,
  TrendingUp,
  Sparkles,
} from 'lucide-react';

type IconName =
  | 'rocket'
  | 'star'
  | 'heart'
  | 'mapPin'
  | 'utensils'
  | 'award'
  | 'trendingUp'
  | 'sparkles';

interface AnimatedIconProps {
  name: IconName;
  className?: string;
  size?: number;
  animated?: boolean;
  color?: string;
}

const iconMap: Record<IconName, React.ComponentType<any>> = {
  rocket: Rocket,
  star: Star,
  heart: Heart,
  mapPin: MapPin,
  utensils: Utensils,
  award: Award,
  trendingUp: TrendingUp,
  sparkles: Sparkles,
};

export function AnimatedIcon({
  name,
  className = '',
  size = 24,
  animated = true,
  color,
}: AnimatedIconProps) {
  const IconComponent = iconMap[name];

  if (!IconComponent) {
    console.warn(`Icon "${name}" not found`);
    return null;
  }

  return (
    <IconComponent
      className={cn(
        'transition-all duration-300',
        animated && 'hover:scale-110 hover:rotate-12',
        className
      )}
      size={size}
      color={color}
    />
  );
}

