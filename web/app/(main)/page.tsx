'use client'

import { useState, Suspense } from 'react';
import { useRouter } from 'next/navigation';
import { Search, MapPin, Star, Utensils } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent } from '@/components/ui/card';

function HomePageContent() {
  const [searchQuery, setSearchQuery] = useState('');
  const router = useRouter();
  
  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      router.push(`/search?q=${encodeURIComponent(searchQuery.trim())}`);
    }
  };

  return (
    <div>
      {/* Hero Section */}
      <section className="bg-gradient-to-r from-blue-600 to-purple-600 text-white py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h1 className="text-4xl md:text-6xl font-bold mb-4">
              Discover Your Next
              <br />
              <span className="text-yellow-300">Favorite Dish</span>
            </h1>
            <p className="text-xl md:text-2xl mb-8 text-blue-100">
              Find restaurants that match your dietary preferences and taste buds
            </p>
            
            {/* Hero Search */}
            <form onSubmit={handleSearch} className="max-w-2xl mx-auto">
              <div className="flex flex-col sm:flex-row gap-4">
                <div className="flex-1 relative">
                  <Input
                    type="text"
                    placeholder="Search restaurants, cuisines, or dishes..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="pl-12 h-14 text-lg bg-white text-gray-900"
                  />
                  <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                </div>
                <Button type="submit" size="lg" className="h-14 px-8 bg-yellow-500 hover:bg-yellow-600 text-gray-900 font-semibold">
                  Search
                </Button>
              </div>
            </form>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-4">
              Why Choose The Dish?
            </h2>
            <p className="text-xl text-gray-600">
              More than just reviews - find places that truly match your preferences
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <Card>
              <CardContent className="p-6 text-center">
                <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <Utensils className="w-6 h-6 text-blue-600" />
                </div>
                <h3 className="text-xl font-semibold mb-2">Dietary Preferences</h3>
                <p className="text-gray-600">
                  Filter by halal, kosher, vegan, gluten-free, and more. Find restaurants that cater to your specific needs.
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6 text-center">
                <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <MapPin className="w-6 h-6 text-green-600" />
                </div>
                <h3 className="text-xl font-semibold mb-2">GPS Verified</h3>
                <p className="text-gray-600">
                  All reviews are GPS-verified to ensure authenticity. Real reviews from real visitors.
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6 text-center">
                <div className="w-12 h-12 bg-yellow-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <Star className="w-6 h-6 text-yellow-600" />
                </div>
                <h3 className="text-xl font-semibold mb-2">Community Driven</h3>
                <p className="text-gray-600">
                  Join a community of food lovers sharing honest reviews and recommendations.
                </p>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>

      {/* Recent Reviews Section */}
      <section className="py-16 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-4">
              Recent Reviews
            </h2>
            <p className="text-xl text-gray-600">
              See what our community is discovering
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {/* Sample review cards - these would come from actual API data */}
            <Card>
              <CardContent className="p-6">
                <div className="flex items-center mb-3">
                  <div className="flex">
                    {[...Array(5)].map((_, i) => (
                      <Star key={i} className="w-4 h-4 text-yellow-400 fill-current" />
                    ))}
                  </div>
                  <span className="ml-2 text-sm text-gray-600">2 hours ago</span>
                </div>
                <p className="text-gray-800 mb-3">
                  "Amazing halal options and the service was excellent. The lamb biryani was perfectly spiced!"
                </p>
                <div className="text-sm text-gray-600">
                  <span className="font-medium">Sarah M.</span> reviewed{' '}
                  <span className="font-medium text-gray-900">Spice Garden</span>
                </div>
                <div className="mt-2">
                  <span className="inline-block bg-green-100 text-green-700 text-xs px-2 py-1 rounded">
                    Halal
                  </span>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6">
                <div className="flex items-center mb-3">
                  <div className="flex">
                    {[...Array(4)].map((_, i) => (
                      <Star key={i} className="w-4 h-4 text-yellow-400 fill-current" />
                    ))}
                    <Star className="w-4 h-4 text-gray-300" />
                  </div>
                  <span className="ml-2 text-sm text-gray-600">5 hours ago</span>
                </div>
                <p className="text-gray-800 mb-3">
                  "Great vegan options! The mushroom risotto was creamy and flavorful. Will definitely come back."
                </p>
                <div className="text-sm text-gray-600">
                  <span className="font-medium">Alex K.</span> reviewed{' '}
                  <span className="font-medium text-gray-900">Green Kitchen</span>
                </div>
                <div className="mt-2">
                  <span className="inline-block bg-green-100 text-green-700 text-xs px-2 py-1 rounded">
                    Vegan
                  </span>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6">
                <div className="flex items-center mb-3">
                  <div className="flex">
                    {[...Array(5)].map((_, i) => (
                      <Star key={i} className="w-4 h-4 text-yellow-400 fill-current" />
                    ))}
                  </div>
                  <span className="ml-2 text-sm text-gray-600">1 day ago</span>
                </div>
                <p className="text-gray-800 mb-3">
                  "Best Korean BBQ in the city! Authentic flavors and great atmosphere. The kimchi was perfect."
                </p>
                <div className="text-sm text-gray-600">
                  <span className="font-medium">Mike L.</span> reviewed{' '}
                  <span className="font-medium text-gray-900">Seoul Kitchen</span>
                </div>
                <div className="mt-2">
                  <span className="inline-block bg-blue-100 text-blue-700 text-xs px-2 py-1 rounded">
                    Korean
                  </span>
                </div>
              </CardContent>
            </Card>
          </div>

          <div className="text-center mt-8">
            <Button variant="outline" onClick={() => router.push('/places')}>
              View All Reviews
            </Button>
          </div>
        </div>
      </section>

      {/* Call to Action */}
      <section className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-3xl font-bold text-gray-900 mb-4">
            Ready to Discover Great Food?
          </h2>
          <p className="text-xl text-gray-600 mb-8">
            Join our community and start exploring restaurants near you
          </p>
          <div className="space-x-4">
            <Button size="lg" onClick={() => router.push('/register')}>
              Sign Up Now
            </Button>
            <Button variant="outline" size="lg" onClick={() => router.push('/places')}>
              Explore Restaurants
            </Button>
          </div>
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
