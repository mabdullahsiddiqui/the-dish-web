'use client'

import Link from 'next/link';
import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Search, Menu, X, User, LogOut } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { useAuth } from '@/hooks/useAuth';

export function Header() {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const { user, isAuthenticated, logout } = useAuth();
  const router = useRouter();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      router.push(`/search?q=${encodeURIComponent(searchQuery.trim())}`);
      setSearchQuery('');
      setIsMobileMenuOpen(false);
    }
  };

  return (
    <header className="bg-white shadow-sm border-b sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <div className="flex-shrink-0">
            <Link href="/" className="flex items-center">
              <span className="text-2xl font-bold text-gray-900">The Dish</span>
            </Link>
          </div>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center space-x-8">
            <Link href="/" className="text-gray-700 hover:text-gray-900 font-medium">
              Home
            </Link>
            <Link href="/places" className="text-gray-700 hover:text-gray-900 font-medium">
              Restaurants
            </Link>
            {isAuthenticated && (
              <Link href="/places/new">
                <Button variant="outline" size="sm">
                  Add Restaurant
                </Button>
              </Link>
            )}
          </nav>

          {/* Search Bar */}
          <form onSubmit={handleSearch} className="hidden md:flex flex-1 max-w-md mx-8">
            <div className="relative w-full">
              <Input
                type="text"
                placeholder="Search restaurants..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-10"
              />
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
            </div>
          </form>

          {/* Desktop Auth Section */}
          <div className="hidden md:flex items-center space-x-4">
            {isAuthenticated && user ? (
              <div className="flex items-center space-x-4">
                <Link href="/profile" className="flex items-center space-x-2 text-gray-700 hover:text-gray-900">
                  <User className="w-4 h-4" />
                  <span>{user.firstName}</span>
                </Link>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={logout}
                  className="flex items-center space-x-2"
                >
                  <LogOut className="w-4 h-4" />
                  <span>Logout</span>
                </Button>
              </div>
            ) : (
              <div className="flex items-center space-x-4">
                <Link href="/login">
                  <Button variant="ghost">Sign In</Button>
                </Link>
                <Link href="/register">
                  <Button>Sign Up</Button>
                </Link>
              </div>
            )}
          </div>

          {/* Mobile menu button */}
          <div className="md:hidden">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            >
              {isMobileMenuOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
            </Button>
          </div>
        </div>
      </div>

      {/* Mobile Menu */}
      {isMobileMenuOpen && (
        <div className="md:hidden border-t bg-white">
          <div className="px-4 py-4 space-y-4">
            {/* Mobile Search */}
            <form onSubmit={handleSearch} className="w-full">
              <div className="relative">
                <Input
                  type="text"
                  placeholder="Search restaurants..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="pl-10"
                />
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
              </div>
            </form>

            {/* Mobile Navigation */}
            <nav className="space-y-2">
              <Link
                href="/"
                className="block px-3 py-2 text-gray-700 hover:bg-gray-100 rounded-md"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                Home
              </Link>
              <Link
                href="/places"
                className="block px-3 py-2 text-gray-700 hover:bg-gray-100 rounded-md"
                onClick={() => setIsMobileMenuOpen(false)}
              >
                Restaurants
              </Link>
              {isAuthenticated && (
                <Link
                  href="/places/new"
                  className="block px-3 py-2 text-gray-700 hover:bg-gray-100 rounded-md"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Add Restaurant
                </Link>
              )}
            </nav>

            {/* Mobile Auth Section */}
            <div className="pt-4 border-t">
              {isAuthenticated && user ? (
                <div className="space-y-2">
                  <Link
                    href="/profile"
                    className="flex items-center space-x-2 px-3 py-2 text-gray-700 hover:bg-gray-100 rounded-md"
                    onClick={() => setIsMobileMenuOpen(false)}
                  >
                    <User className="w-4 h-4" />
                    <span>{user.firstName} {user.lastName}</span>
                  </Link>
                  <button
                    onClick={() => {
                      logout();
                      setIsMobileMenuOpen(false);
                    }}
                    className="flex items-center space-x-2 w-full px-3 py-2 text-gray-700 hover:bg-gray-100 rounded-md text-left"
                  >
                    <LogOut className="w-4 h-4" />
                    <span>Logout</span>
                  </button>
                </div>
              ) : (
                <div className="space-y-2">
                  <Link href="/login" onClick={() => setIsMobileMenuOpen(false)}>
                    <Button variant="ghost" className="w-full justify-start">
                      Sign In
                    </Button>
                  </Link>
                  <Link href="/register" onClick={() => setIsMobileMenuOpen(false)}>
                    <Button className="w-full">Sign Up</Button>
                  </Link>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </header>
  );
}
