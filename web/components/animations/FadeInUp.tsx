'use client';

import { ReactNode } from 'react';
import { cn } from '@/lib/utils/cn';

interface FadeInUpProps {
  children: ReactNode;
  className?: string;
  delay?: number;
  duration?: number;
}

export function FadeInUp({
  children,
  className = '',
  delay = 0,
  duration = 0.6,
}: FadeInUpProps) {
  return (
    <div
      className={cn('animate-fade-in-up', className)}
      style={{
        animationDelay: `${delay}s`,
        animationDuration: `${duration}s`,
      }}
    >
      {children}
    </div>
  );
}

