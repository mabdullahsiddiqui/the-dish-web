'use client'

import { useState, Suspense } from 'react';
import { useRouter } from 'next/navigation';
import { MapPin, Star, Utensils } from 'lucide-react';
import { HeroWith3D } from '@/components/3d/HeroWith3D';
import { AnimatedSearchBar } from '@/components/search/AnimatedSearchBar';
import { TiltCard } from '@/components/cards/TiltCard';
import { GlassCard } from '@/components/cards/GlassCard';
import { Button3D } from '@/components/ui/Button3D';
import { Badge3D } from '@/components/ui/Badge3D';
import { AnimatedIcon } from '@/components/ui/AnimatedIcon';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { StaggerChildren } from '@/components/animations/StaggerChildren';

function HomePageContent() {
  const [searchQuery, setSearchQuery] = useState('');
  const router = useRouter();
  
  const handleSearch = (value: string) => {
    if (value.trim()) {
      router.push(`/search?q=${encodeURIComponent(value.trim())}`);
    }
  };

  return (
    <div className="min-h-screen bg-[#0f172a]">
      {/* Hero Section */}
      <HeroWith3D
        title="Discover Your Next Favorite Dish"
        subtitle="Find halal restaurants that match your dietary preferences and taste buds"
        ctaText="Start Exploring"
        particleColor="#10b981"
        onCtaClick={() => router.push('/places')}
      />

      {/* Search Bar Section */}
      <section className="py-8 bg-[#0f172a]">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <AnimatedSearchBar
            placeholder="Search restaurants, cuisines, or dishes..."
            value={searchQuery}
            onChange={setSearchQuery}
            onSearch={handleSearch}
          />
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16 bg-[#0f172a]">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <FadeInUp>
            <div className="text-center mb-12">
              <h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
                Why Choose The Dish?
              </h2>
              <p className="text-xl text-gray-300">
                More than just reviews - find places that truly match your preferences
              </p>
            </div>
          </FadeInUp>

          <StaggerChildren className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <TiltCard>
              <GlassCard hover className="p-6 text-center h-full">
                <div className="w-16 h-16 bg-indigo-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                  <AnimatedIcon name="utensils" size={32} color="#6366f1" />
                </div>
                <h3 className="text-xl font-semibold mb-2 text-white">Dietary Preferences</h3>
                <p className="text-gray-300">
                  Filter by halal, kosher, vegan, gluten-free, and more. Find restaurants that cater to your specific needs.
                </p>
              </GlassCard>
            </TiltCard>

            <TiltCard>
              <GlassCard hover className="p-6 text-center h-full">
                <div className="w-16 h-16 bg-green-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                  <AnimatedIcon name="mapPin" size={32} color="#10b981" />
                </div>
                <h3 className="text-xl font-semibold mb-2 text-white">GPS Verified</h3>
                <p className="text-gray-300">
                  All reviews are GPS-verified to ensure authenticity. Real reviews from real visitors.
                </p>
              </GlassCard>
            </TiltCard>

            <TiltCard>
              <GlassCard hover className="p-6 text-center h-full">
                <div className="w-16 h-16 bg-amber-500/20 rounded-full flex items-center justify-center mx-auto mb-4">
                  <AnimatedIcon name="star" size={32} color="#f59e0b" />
                </div>
                <h3 className="text-xl font-semibold mb-2 text-white">Community Driven</h3>
                <p className="text-gray-300">
                  Join a community of food lovers sharing honest reviews and recommendations.
                </p>
              </GlassCard>
            </TiltCard>
          </StaggerChildren>
        </div>
      </section>

      {/* Recent Reviews Section */}
      <section className="py-16 bg-[#1e293b]">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <FadeInUp>
            <div className="text-center mb-12">
              <h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
                Recent Reviews
              </h2>
              <p className="text-xl text-gray-300">
                See what our community is discovering
              </p>
            </div>
          </FadeInUp>

          <StaggerChildren className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {/* Sample review cards - these would come from actual API data */}
            <TiltCard>
              <GlassCard hover className="p-6 h-full">
                <div className="flex items-center mb-3">
                  <div className="flex">
                    {[...Array(5)].map((_, i) => (
                      <Star key={i} className="w-4 h-4 text-yellow-400 fill-current" />
                    ))}
                  </div>
                  <span className="ml-2 text-sm text-gray-400">2 hours ago</span>
                </div>
                <p className="text-gray-200 mb-3">
                  "Amazing halal options and the service was excellent. The lamb biryani was perfectly spiced!"
                </p>
                <div className="text-sm text-gray-400 mb-3">
                  <span className="font-medium text-white">Sarah M.</span> reviewed{' '}
                  <span className="font-medium text-white">Spice Garden</span>
                </div>
                <div className="mt-2">
                  <Badge3D variant="halal" />
                </div>
              </GlassCard>
            </TiltCard>

            <TiltCard>
              <GlassCard hover className="p-6 h-full">
                <div className="flex items-center mb-3">
                  <div className="flex">
                    {[...Array(4)].map((_, i) => (
                      <Star key={i} className="w-4 h-4 text-yellow-400 fill-current" />
                    ))}
                    <Star className="w-4 h-4 text-gray-500" />
                  </div>
                  <span className="ml-2 text-sm text-gray-400">5 hours ago</span>
                </div>
                <p className="text-gray-200 mb-3">
                  "Great vegan options! The mushroom risotto was creamy and flavorful. Will definitely come back."
                </p>
                <div className="text-sm text-gray-400 mb-3">
                  <span className="font-medium text-white">Alex K.</span> reviewed{' '}
                  <span className="font-medium text-white">Green Kitchen</span>
                </div>
                <div className="mt-2">
                  <Badge3D variant="custom">
                    <span>Vegan</span>
                  </Badge3D>
                </div>
              </GlassCard>
            </TiltCard>

            <TiltCard>
              <GlassCard hover className="p-6 h-full">
                <div className="flex items-center mb-3">
                  <div className="flex">
                    {[...Array(5)].map((_, i) => (
                      <Star key={i} className="w-4 h-4 text-yellow-400 fill-current" />
                    ))}
                  </div>
                  <span className="ml-2 text-sm text-gray-400">1 day ago</span>
                </div>
                <p className="text-gray-200 mb-3">
                  "Best Korean BBQ in the city! Authentic flavors and great atmosphere. The kimchi was perfect."
                </p>
                <div className="text-sm text-gray-400 mb-3">
                  <span className="font-medium text-white">Mike L.</span> reviewed{' '}
                  <span className="font-medium text-white">Seoul Kitchen</span>
                </div>
                <div className="mt-2">
                  <Badge3D variant="custom">
                    <span>Korean</span>
                  </Badge3D>
                </div>
              </GlassCard>
            </TiltCard>
          </StaggerChildren>

          <FadeInUp delay={0.4}>
            <div className="text-center mt-8">
              <Button3D variant="outline" onClick={() => router.push('/places')}>
                View All Reviews
              </Button3D>
            </div>
          </FadeInUp>
        </div>
      </section>

      {/* Call to Action */}
      <section className="py-16 bg-[#0f172a]">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <FadeInUp>
            <h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
              Ready to Discover Great Food?
            </h2>
            <p className="text-xl text-gray-300 mb-8">
              Join our community and start exploring restaurants near you
            </p>
          </FadeInUp>
          <FadeInUp delay={0.2}>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button3D size="lg" onClick={() => router.push('/register')} glow>
                Sign Up Now
              </Button3D>
              <Button3D variant="outline" size="lg" onClick={() => router.push('/places')}>
                Explore Restaurants
              </Button3D>
            </div>
          </FadeInUp>
        </div>
      </section>
    </div>
  );
}

export default function HomePage() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <HomePageContent />
    </Suspense>
  );
}
