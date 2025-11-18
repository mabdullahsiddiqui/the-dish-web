'use client';

import { ReactNode } from 'react';
import { cn } from '@/lib/utils/cn';

interface StaggerChildrenProps {
  children: ReactNode;
  className?: string;
  staggerDelay?: number;
}

export function StaggerChildren({
  children,
  className = '',
  staggerDelay = 0.1,
}: StaggerChildrenProps) {
  const childrenArray = Array.isArray(children) ? children : [children];

  return (
    <div className={cn('', className)}>
      {childrenArray.map((child, index) => (
        <div
          key={index}
          className="stagger-item"
          style={{
            animationDelay: `${index * staggerDelay}s`,
          }}
        >
          {child}
        </div>
      ))}
    </div>
  );
}

