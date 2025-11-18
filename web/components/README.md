# Modern 3D UI Component Library

A comprehensive collection of modern, interactive 3D components built with Three.js, React, and Tailwind CSS following the design system standards.

## 📁 Component Structure

```
components/
├── 3d/                    # Three.js 3D components
│   ├── ParticleBackground.tsx
│   ├── TrustScore3D.tsx
│   ├── InteractiveMesh.tsx
│   └── HeroWith3D.tsx
├── cards/                 # Interactive card components
│   ├── TiltCard.tsx
│   ├── GlassCard.tsx
│   └── PlaceCard3D.tsx
├── animations/            # Animation wrappers
│   ├── FadeInUp.tsx
│   ├── StaggerChildren.tsx
│   └── FloatingElement.tsx
├── search/                # Search components
│   ├── AnimatedSearchBar.tsx
│   └── FilterPanel.tsx
└── ui/                    # UI components
    ├── Button3D.tsx
    ├── Badge3D.tsx
    └── AnimatedIcon.tsx
```

## 🚀 Quick Start

### Import Components

```tsx
// Individual imports
import { ParticleBackground } from '@/components/3d';
import { PlaceCard3D } from '@/components/cards';
import { AnimatedSearchBar } from '@/components/search';
import { Button3D } from '@/components/ui';

// Or from index files
import { ParticleBackground, TrustScore3D } from '@/components/3d';
```

## 📦 Components

### 3D Components

#### ParticleBackground
Animated particle system background using Three.js.

```tsx
<ParticleBackground
  particleCount={5000}
  color="#6366f1"
  speed={0.001}
/>
```

#### TrustScore3D
3D rotating ring visualization for trust scores.

```tsx
<TrustScore3D score={85} size={200} />
```

#### InteractiveMesh
Interactive 3D mesh that responds to mouse movement.

```tsx
<InteractiveMesh
  geometry="box"
  color="#6366f1"
  onHover={() => console.log('Hovered')}
  onClick={() => console.log('Clicked')}
/>
```

#### HeroWith3D
Complete hero section with particle background.

```tsx
<HeroWith3D
  title="Welcome to The Dish"
  subtitle="Find halal restaurants near you"
  ctaText="Get Started"
  particleColor="#10b981"
/>
```

### Card Components

#### TiltCard
Card with 3D tilt effect on mouse move.

```tsx
<TiltCard intensity={10} perspective={1000}>
  <div>Your content here</div>
</TiltCard>
```

#### GlassCard
Glass morphism card with backdrop blur.

```tsx
<GlassCard dark hover>
  <div>Your content here</div>
</GlassCard>
```

#### PlaceCard3D
Complete restaurant place card with 3D effects.

```tsx
<PlaceCard3D
  place={{
    id: '1',
    name: 'Restaurant Name',
    cuisine: 'Middle Eastern',
    image: '/restaurant.jpg',
    rating: 4.5,
    isHalal: true,
    trustScore: 85
  }}
/>
```

### Animation Components

#### FadeInUp
Fade in with upward motion animation.

```tsx
<FadeInUp delay={0.2} duration={0.6}>
  <div>Content</div>
</FadeInUp>
```

#### StaggerChildren
Stagger animation for multiple children.

```tsx
<StaggerChildren staggerDelay={0.1}>
  <div>Item 1</div>
  <div>Item 2</div>
  <div>Item 3</div>
</StaggerChildren>
```

#### FloatingElement
Floating animation effect.

```tsx
<FloatingElement duration={3}>
  <div>Floating content</div>
</FloatingElement>
```

### Search Components

#### AnimatedSearchBar
Search bar with particle effects on focus.

```tsx
<AnimatedSearchBar
  placeholder="Search restaurants..."
  onSearch={(value) => console.log(value)}
  value={searchValue}
  onChange={setSearchValue}
/>
```

#### FilterPanel
Glass morphism filter panel.

```tsx
<FilterPanel
  filters={[
    {
      title: 'Cuisine',
      options: [
        { label: 'Middle Eastern', value: 'middle-eastern' },
        { label: 'Asian', value: 'asian' }
      ],
      selected: [],
      onSelectionChange: (values) => console.log(values)
    }
  ]}
  onApply={() => console.log('Applied')}
  onReset={() => console.log('Reset')}
/>
```

### UI Components

#### Button3D
3D-styled button with hover effects.

```tsx
<Button3D
  variant="primary"
  size="md"
  glow
  onClick={() => console.log('Clicked')}
>
  Click Me
</Button3D>
```

#### Badge3D
Animated badge component.

```tsx
<Badge3D variant="halal" />
<Badge3D variant="verified" />
<Badge3D variant="custom">Custom Badge</Badge3D>
```

#### AnimatedIcon
Animated icon component.

```tsx
<AnimatedIcon
  name="rocket"
  size={24}
  animated
  color="#6366f1"
/>
```

## 🎨 Design System

All components follow the modern design system defined in `globals.css`:

- **Colors**: Primary (Indigo), Accent (Amber), Neutrals
- **Glass Morphism**: Backdrop blur effects
- **Animations**: Smooth transitions and micro-interactions
- **Accessibility**: ARIA labels and reduced motion support

## ⚡ Performance

- All Three.js components include proper cleanup
- Performance optimizations (pixel ratio limiting, FPS control)
- Lazy loading support
- Reduced motion support for accessibility

## 🔧 Customization

All components accept `className` props for additional styling:

```tsx
<ParticleBackground
  className="custom-class"
  particleCount={3000}
/>
```

## 📝 Examples

See the design system document for complete usage examples and patterns.

## 🐛 Troubleshooting

### Three.js not rendering
- Ensure WebGL is supported in the browser
- Check browser console for errors
- Verify Three.js dependencies are installed

### Performance issues
- Reduce particle count
- Lower pixel ratio
- Check for memory leaks (proper cleanup)

### Styling issues
- Ensure Tailwind CSS is properly configured
- Check that `globals.css` is imported
- Verify CSS variables are defined

