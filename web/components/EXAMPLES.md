# Component Usage Examples

## Complete Hero Section

```tsx
import { HeroWith3D } from '@/components/3d/HeroWith3D';

export default function HomePage() {
  return (
    <HeroWith3D
      title="Discover Halal Restaurants"
      subtitle="Find verified halal restaurants in your area"
      ctaText="Start Exploring"
      particleColor="#10b981"
      onCtaClick={() => router.push('/places')}
    />
  );
}
```

## Restaurant Listing with 3D Cards

```tsx
import { PlaceCard3D } from '@/components/cards';
import { StaggerChildren } from '@/components/animations';

export function RestaurantList({ places }) {
  return (
    <StaggerChildren>
      {places.map((place) => (
        <PlaceCard3D
          key={place.id}
          place={place}
          onClick={() => router.push(`/places/${place.id}`)}
        />
      ))}
    </StaggerChildren>
  );
}
```

## Search Page with Filters

```tsx
import { AnimatedSearchBar } from '@/components/search/AnimatedSearchBar';
import { FilterPanel } from '@/components/search/FilterPanel';
import { useState } from 'react';

export function SearchPage() {
  const [searchValue, setSearchValue] = useState('');
  const [showFilters, setShowFilters] = useState(false);

  return (
    <div className="container mx-auto p-6">
      <AnimatedSearchBar
        placeholder="Search halal restaurants..."
        value={searchValue}
        onChange={setSearchValue}
        onSearch={(value) => console.log('Search:', value)}
      />

      {showFilters && (
        <FilterPanel
          filters={[
            {
              title: 'Cuisine Type',
              options: [
                { label: 'Middle Eastern', value: 'middle-eastern', count: 45 },
                { label: 'Asian', value: 'asian', count: 32 },
                { label: 'Mediterranean', value: 'mediterranean', count: 28 },
              ],
              selected: [],
              onSelectionChange: (values) => console.log(values),
            },
          ]}
          onClose={() => setShowFilters(false)}
          onApply={() => setShowFilters(false)}
        />
      )}
    </div>
  );
}
```

## Trust Score Display

```tsx
import { TrustScore3D } from '@/components/3d';
import { GlassCard } from '@/components/cards';

export function RestaurantDetail({ restaurant }) {
  return (
    <GlassCard className="p-6">
      <div className="flex items-center gap-6">
        <TrustScore3D score={restaurant.trustScore} size={150} />
        <div>
          <h3 className="text-2xl font-bold text-white mb-2">
            {restaurant.name}
          </h3>
          <p className="text-gray-300">
            Trust Score: {restaurant.trustScore}%
          </p>
        </div>
      </div>
    </GlassCard>
  );
}
```

## Custom Glass Card Layout

```tsx
import { TiltCard } from '@/components/cards/TiltCard';
import { GlassCard } from '@/components/cards/GlassCard';
import { Button3D } from '@/components/ui/Button3D';
import { Badge3D } from '@/components/ui/Badge3D';

export function CustomCard() {
  return (
    <TiltCard>
      <GlassCard hover className="p-8">
        <div className="flex items-start justify-between mb-4">
          <h2 className="text-3xl font-bold text-white">Card Title</h2>
          <Badge3D variant="verified" />
        </div>
        <p className="text-gray-300 mb-6">
          Your card content goes here with beautiful glass morphism effects.
        </p>
        <Button3D variant="primary" size="md">
          Action Button
        </Button3D>
      </GlassCard>
    </TiltCard>
  );
}
```

## Animated Icon Grid

```tsx
import { AnimatedIcon } from '@/components/ui/AnimatedIcon';
import { StaggerChildren } from '@/components/animations';

export function IconGrid() {
  const icons = ['rocket', 'star', 'heart', 'award', 'trendingUp', 'sparkles'];

  return (
    <StaggerChildren className="grid grid-cols-3 gap-6">
      {icons.map((icon) => (
        <div key={icon} className="flex flex-col items-center gap-2">
          <AnimatedIcon name={icon as any} size={48} />
          <span className="text-white capitalize">{icon}</span>
        </div>
      ))}
    </StaggerChildren>
  );
}
```

## Interactive 3D Mesh Showcase

```tsx
import { InteractiveMesh } from '@/components/3d';
import { GlassCard } from '@/components/cards';

export function MeshShowcase() {
  return (
    <div className="grid grid-cols-3 gap-6">
      <GlassCard className="p-6">
        <h3 className="text-white mb-4">Box</h3>
        <InteractiveMesh geometry="box" color="#6366f1" />
      </GlassCard>
      <GlassCard className="p-6">
        <h3 className="text-white mb-4">Sphere</h3>
        <InteractiveMesh geometry="sphere" color="#10b981" />
      </GlassCard>
      <GlassCard className="p-6">
        <h3 className="text-white mb-4">Torus</h3>
        <InteractiveMesh geometry="torus" color="#f59e0b" />
      </GlassCard>
    </div>
  );
}
```

## Complete Page Example

```tsx
'use client';

import { HeroWith3D } from '@/components/3d/HeroWith3D';
import { AnimatedSearchBar } from '@/components/search/AnimatedSearchBar';
import { PlaceCard3D } from '@/components/cards/PlaceCard3D';
import { StaggerChildren } from '@/components/animations/StaggerChildren';
import { Button3D } from '@/components/ui/Button3D';
import { useState } from 'react';

export default function HomePage() {
  const [searchValue, setSearchValue] = useState('');

  const featuredPlaces = [
    {
      id: '1',
      name: 'Al-Amin Restaurant',
      cuisine: 'Middle Eastern',
      image: '/restaurant1.jpg',
      rating: 4.8,
      isHalal: true,
      trustScore: 95,
    },
    {
      id: '2',
      name: 'Halal Kitchen',
      cuisine: 'Asian Fusion',
      image: '/restaurant2.jpg',
      rating: 4.6,
      isHalal: true,
      trustScore: 88,
    },
  ];

  return (
    <div>
      <HeroWith3D
        title="Discover Halal Restaurants"
        subtitle="Find verified halal restaurants in your area"
        ctaText="Start Exploring"
        particleColor="#10b981"
      />

      <div className="container mx-auto px-6 py-12">
        <div className="mb-8">
          <AnimatedSearchBar
            placeholder="Search halal restaurants..."
            value={searchValue}
            onChange={setSearchValue}
          />
        </div>

        <h2 className="text-3xl font-bold text-white mb-6">
          Featured Restaurants
        </h2>

        <StaggerChildren className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {featuredPlaces.map((place) => (
            <PlaceCard3D key={place.id} place={place} />
          ))}
        </StaggerChildren>
      </div>
    </div>
  );
}
```

