'use client';

import { useState, useRef, useEffect } from 'react';
import { cn } from '@/lib/utils/cn';
import { Search } from 'lucide-react';

interface AnimatedSearchBarProps {
  placeholder?: string;
  onSearch?: (value: string) => void;
  className?: string;
  value?: string;
  onChange?: (value: string) => void;
}

export function AnimatedSearchBar({
  placeholder = 'Search halal restaurants...',
  onSearch,
  className = '',
  value: controlledValue,
  onChange,
}: AnimatedSearchBarProps) {
  const [focused, setFocused] = useState(false);
  const [internalValue, setInternalValue] = useState('');
  const inputRef = useRef<HTMLInputElement>(null);

  const isControlled = controlledValue !== undefined;
  const value = isControlled ? controlledValue : internalValue;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    if (!isControlled) {
      setInternalValue(newValue);
    }
    onChange?.(newValue);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      onSearch?.(value);
    }
  };

  return (
    <div className={cn('relative', className)}>
      {/* Particle effect on focus */}
      {focused && (
        <div className="absolute inset-0 -z-10 pointer-events-none">
          {[...Array(20)].map((_, i) => (
            <div
              key={i}
              className="absolute w-1 h-1 bg-indigo-500 rounded-full animate-ping"
              style={{
                top: `${Math.random() * 100}%`,
                left: `${Math.random() * 100}%`,
                animationDelay: `${Math.random() * 2}s`,
                animationDuration: `${1 + Math.random()}s`,
              }}
              aria-hidden="true"
            />
          ))}
        </div>
      )}

      <div
        className={cn(
          'glass-card p-4 rounded-full transition-all duration-300 flex items-center gap-3',
          focused && 'ring-4 ring-indigo-500 scale-105'
        )}
      >
        <Search className="w-5 h-5 text-gray-400 flex-shrink-0" />
        <input
          ref={inputRef}
          type="text"
          placeholder={placeholder}
          value={value}
          onChange={handleChange}
          onKeyDown={handleKeyDown}
          onFocus={() => setFocused(true)}
          onBlur={() => setFocused(false)}
          className="bg-transparent text-white w-full outline-none placeholder:text-gray-400"
          aria-label={placeholder}
        />
      </div>
    </div>
  );
}

