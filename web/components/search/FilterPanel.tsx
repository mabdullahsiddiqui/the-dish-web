'use client';

import { ReactNode, useState } from 'react';
import { cn } from '@/lib/utils/cn';
import { GlassCard } from '../cards/GlassCard';
import { X, Filter } from 'lucide-react';

interface FilterOption {
  label: string;
  value: string;
  count?: number;
}

interface FilterGroup {
  title: string;
  options: FilterOption[];
  selected?: string[];
  onSelectionChange?: (values: string[]) => void;
}

interface FilterPanelProps {
  filters: FilterGroup[];
  className?: string;
  onClose?: () => void;
  onApply?: () => void;
  onReset?: () => void;
}

export function FilterPanel({
  filters,
  className = '',
  onClose,
  onApply,
  onReset,
}: FilterPanelProps) {
  const [localFilters, setLocalFilters] = useState<FilterGroup[]>(filters);

  const handleToggleOption = (groupIndex: number, value: string) => {
    const updatedFilters = [...localFilters];
    const group = updatedFilters[groupIndex];
    const selected = group.selected || [];

    const newSelected = selected.includes(value)
      ? selected.filter((v) => v !== value)
      : [...selected, value];

    updatedFilters[groupIndex] = {
      ...group,
      selected: newSelected,
    };

    setLocalFilters(updatedFilters);
    group.onSelectionChange?.(newSelected);
  };

  const handleReset = () => {
    const resetFilters = localFilters.map((group) => ({
      ...group,
      selected: [],
    }));
    setLocalFilters(resetFilters);
    onReset?.();
  };

  return (
    <GlassCard className={cn('p-6', className)}>
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-2">
          <Filter className="w-5 h-5 text-white" />
          <h3 className="text-xl font-bold text-white">Filters</h3>
        </div>
        {onClose && (
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-white transition-colors"
            aria-label="Close filters"
          >
            <X className="w-5 h-5" />
          </button>
        )}
      </div>

      <div className="space-y-6">
        {localFilters.map((group, groupIndex) => (
          <div key={groupIndex}>
            <h4 className="text-sm font-semibold text-gray-300 mb-3">
              {group.title}
            </h4>
            <div className="space-y-2">
              {group.options.map((option) => {
                const isSelected = group.selected?.includes(option.value);
                return (
                  <label
                    key={option.value}
                    className={cn(
                      'flex items-center justify-between p-3 rounded-lg cursor-pointer transition-all',
                      isSelected
                        ? 'bg-indigo-500/20 border border-indigo-500'
                        : 'bg-white/5 border border-transparent hover:bg-white/10'
                    )}
                  >
                    <div className="flex items-center gap-3">
                      <input
                        type="checkbox"
                        checked={isSelected}
                        onChange={() =>
                          handleToggleOption(groupIndex, option.value)
                        }
                        className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500"
                      />
                      <span className="text-white">{option.label}</span>
                    </div>
                    {option.count !== undefined && (
                      <span className="text-sm text-gray-400">
                        {option.count}
                      </span>
                    )}
                  </label>
                );
              })}
            </div>
          </div>
        ))}
      </div>

      <div className="flex gap-3 mt-6 pt-6 border-t border-white/10">
        <button
          onClick={handleReset}
          className="flex-1 px-4 py-2 bg-white/10 hover:bg-white/20 text-white rounded-lg transition-colors"
        >
          Reset
        </button>
        {onApply && (
          <button
            onClick={onApply}
            className="flex-1 px-4 py-2 bg-gradient-to-r from-indigo-500 to-purple-600 hover:from-indigo-600 hover:to-purple-700 text-white rounded-lg transition-all"
          >
            Apply Filters
          </button>
        )}
      </div>
    </GlassCard>
  );
}

