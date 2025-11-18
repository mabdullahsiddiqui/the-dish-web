'use client'

import { useState } from 'react';
import { Filter, ChevronDown, X } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent } from '@/components/ui/card';

interface SearchFiltersProps {
  onFiltersChange: (filters: SearchFilters) => void;
  initialFilters?: SearchFilters;
}

export interface SearchFilters {
  cuisineTypes: string[];
  dietaryTags: string[];
  priceRange: number[];
  minRating: number;
  radiusKm: number;
}

const CUISINE_OPTIONS = [
  'Italian', 'Chinese', 'Japanese', 'Mexican', 'Indian', 'Thai', 'French',
  'Mediterranean', 'American', 'Korean', 'Vietnamese', 'Lebanese', 'Greek',
  'Turkish', 'Spanish', 'Ethiopian', 'Moroccan', 'Brazilian'
];

const DIETARY_OPTIONS = [
  'Halal', 'Kosher', 'Vegan', 'Vegetarian', 'Gluten-Free', 'Dairy-Free',
  'Nut-Free', 'Keto', 'Paleo', 'Organic', 'Farm-to-Table'
];

const PRICE_RANGES = [
  { value: 1, label: '$', description: 'Budget-friendly' },
  { value: 2, label: '$$', description: 'Moderate' },
  { value: 3, label: '$$$', description: 'Expensive' },
  { value: 4, label: '$$$$', description: 'Very Expensive' },
];

export function SearchFilters({ onFiltersChange, initialFilters }: SearchFiltersProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [filters, setFilters] = useState<SearchFilters>(
    initialFilters || {
      cuisineTypes: [],
      dietaryTags: [],
      priceRange: [1, 4],
      minRating: 0,
      radiusKm: 25,
    }
  );

  const updateFilters = (newFilters: Partial<SearchFilters>) => {
    const updatedFilters = { ...filters, ...newFilters };
    setFilters(updatedFilters);
    onFiltersChange(updatedFilters);
  };

  const toggleArrayFilter = (array: string[], item: string) => {
    return array.includes(item)
      ? array.filter((i) => i !== item)
      : [...array, item];
  };

  const clearFilters = () => {
    const clearedFilters: SearchFilters = {
      cuisineTypes: [],
      dietaryTags: [],
      priceRange: [1, 4],
      minRating: 0,
      radiusKm: 25,
    };
    setFilters(clearedFilters);
    onFiltersChange(clearedFilters);
  };

  const activeFiltersCount = 
    filters.cuisineTypes.length + 
    filters.dietaryTags.length + 
    (filters.minRating > 0 ? 1 : 0) + 
    (filters.priceRange[0] !== 1 || filters.priceRange[1] !== 4 ? 1 : 0) +
    (filters.radiusKm !== 25 ? 1 : 0);

  return (
    <div className="relative">
      <Button
        variant="outline"
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center space-x-2"
      >
        <Filter className="w-4 h-4" />
        <span>Filters</span>
        {activeFiltersCount > 0 && (
          <span className="bg-blue-500 text-white rounded-full text-xs px-2 py-1 min-w-5 h-5 flex items-center justify-center">
            {activeFiltersCount}
          </span>
        )}
        <ChevronDown className={`w-4 h-4 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
      </Button>

      {isOpen && (
        <Card className="absolute top-12 left-0 z-10 w-96 max-w-screen-sm">
          <CardContent className="p-4">
            <div className="flex justify-between items-center mb-4">
              <h3 className="font-semibold">Search Filters</h3>
              <div className="flex items-center space-x-2">
                <Button variant="ghost" size="sm" onClick={clearFilters}>
                  Clear All
                </Button>
                <Button variant="ghost" size="sm" onClick={() => setIsOpen(false)}>
                  <X className="w-4 h-4" />
                </Button>
              </div>
            </div>

            <div className="space-y-6">
              {/* Cuisine Types */}
              <div>
                <Label className="text-sm font-medium mb-2 block">Cuisine Types</Label>
                <div className="grid grid-cols-2 gap-2 max-h-32 overflow-y-auto">
                  {CUISINE_OPTIONS.map((cuisine) => (
                    <label key={cuisine} className="flex items-center space-x-2 text-sm">
                      <input
                        type="checkbox"
                        checked={filters.cuisineTypes.includes(cuisine)}
                        onChange={() =>
                          updateFilters({
                            cuisineTypes: toggleArrayFilter(filters.cuisineTypes, cuisine),
                          })
                        }
                        className="rounded border-gray-300"
                      />
                      <span>{cuisine}</span>
                    </label>
                  ))}
                </div>
              </div>

              {/* Dietary Tags */}
              <div>
                <Label className="text-sm font-medium mb-2 block">Dietary Preferences</Label>
                <div className="grid grid-cols-2 gap-2 max-h-32 overflow-y-auto">
                  {DIETARY_OPTIONS.map((dietary) => (
                    <label key={dietary} className="flex items-center space-x-2 text-sm">
                      <input
                        type="checkbox"
                        checked={filters.dietaryTags.includes(dietary)}
                        onChange={() =>
                          updateFilters({
                            dietaryTags: toggleArrayFilter(filters.dietaryTags, dietary),
                          })
                        }
                        className="rounded border-gray-300"
                      />
                      <span>{dietary}</span>
                    </label>
                  ))}
                </div>
              </div>

              {/* Price Range */}
              <div>
                <Label className="text-sm font-medium mb-2 block">Price Range</Label>
                <div className="grid grid-cols-4 gap-2">
                  {PRICE_RANGES.map((price) => (
                    <label key={price.value} className="flex flex-col items-center p-2 border rounded cursor-pointer">
                      <input
                        type="checkbox"
                        checked={
                          filters.priceRange[0] <= price.value && filters.priceRange[1] >= price.value
                        }
                        onChange={(e) => {
                          if (e.target.checked) {
                            updateFilters({
                              priceRange: [
                                Math.min(filters.priceRange[0], price.value),
                                Math.max(filters.priceRange[1], price.value),
                              ],
                            });
                          }
                        }}
                        className="mb-1"
                      />
                      <span className="font-medium">{price.label}</span>
                      <span className="text-xs text-gray-500">{price.description}</span>
                    </label>
                  ))}
                </div>
              </div>

              {/* Minimum Rating */}
              <div>
                <Label className="text-sm font-medium mb-2 block">
                  Minimum Rating: {filters.minRating > 0 ? `${filters.minRating}+ stars` : 'Any'}
                </Label>
                <input
                  type="range"
                  min="0"
                  max="5"
                  step="0.5"
                  value={filters.minRating}
                  onChange={(e) => updateFilters({ minRating: parseFloat(e.target.value) })}
                  className="w-full"
                />
              </div>

              {/* Radius */}
              <div>
                <Label className="text-sm font-medium mb-2 block">
                  Distance: {filters.radiusKm} km
                </Label>
                <input
                  type="range"
                  min="1"
                  max="50"
                  step="1"
                  value={filters.radiusKm}
                  onChange={(e) => updateFilters({ radiusKm: parseInt(e.target.value) })}
                  className="w-full"
                />
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}

