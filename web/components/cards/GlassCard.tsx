'use client';

import { ReactNode } from 'react';
import { cn } from '@/lib/utils/cn';

interface GlassCardProps {
  children: ReactNode;
  className?: string;
  dark?: boolean;
  hover?: boolean;
}

export function GlassCard({
  children,
  className = '',
  dark = false,
  hover = false,
}: GlassCardProps) {
  return (
    <div
      className={cn(
        dark ? 'glass-card-dark' : 'glass-card',
        hover && 'interactive-element',
        className
      )}
    >
      {children}
    </div>
  );
}

