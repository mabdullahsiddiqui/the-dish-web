'use client';

import { ReactNode } from 'react';
import { cn } from '@/lib/utils/cn';
import { Check } from 'lucide-react';

interface Badge3DProps {
  children?: ReactNode;
  variant?: 'halal' | 'verified' | 'popular' | 'new' | 'custom';
  className?: string;
  icon?: ReactNode;
}

export function Badge3D({
  children,
  variant = 'custom',
  className = '',
  icon,
}: Badge3DProps) {
  const variants = {
    halal: {
      bg: 'bg-green-500',
      text: 'text-white',
      content: (
        <>
          <Check className="w-3 h-3" />
          <span>Halal Verified</span>
        </>
      ),
    },
    verified: {
      bg: 'bg-blue-500',
      text: 'text-white',
      content: (
        <>
          <Check className="w-3 h-3" />
          <span>Verified</span>
        </>
      ),
    },
    popular: {
      bg: 'bg-purple-500',
      text: 'text-white',
      content: <span>Popular</span>,
    },
    new: {
      bg: 'bg-orange-500',
      text: 'text-white',
      content: <span>New</span>,
    },
    custom: {
      bg: 'bg-indigo-500',
      text: 'text-white',
      content: children || <span>Badge</span>,
    },
  };

  const variantConfig = variants[variant];

  return (
    <div
      className={cn(
        'inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-sm font-semibold shadow-lg',
        variantConfig.bg,
        variantConfig.text,
        className
      )}
    >
      {icon || variantConfig.content}
    </div>
  );
}

