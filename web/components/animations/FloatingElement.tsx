'use client';

import { ReactNode } from 'react';
import { cn } from '@/lib/utils/cn';

interface FloatingElementProps {
  children: ReactNode;
  className?: string;
  duration?: number;
}

export function FloatingElement({
  children,
  className = '',
  duration = 3,
}: FloatingElementProps) {
  return (
    <div
      className={cn('floating', className)}
      style={{
        animationDuration: `${duration}s`,
      }}
    >
      {children}
    </div>
  );
}

