import Link from 'next/link';

export function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-gray-50 border-t">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* Company Info */}
          <div className="col-span-1 md:col-span-2">
            <Link href="/" className="flex items-center mb-4">
              <span className="text-xl font-bold text-gray-900">The Dish</span>
            </Link>
            <p className="text-gray-600 text-sm mb-4">
              Discover great restaurants and share your experiences with our community.
              Find places that match your dietary preferences and taste buds.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h3 className="font-semibold text-gray-900 mb-4">Explore</h3>
            <ul className="space-y-2 text-sm">
              <li>
                <Link href="/" className="text-gray-600 hover:text-gray-900">
                  Home
                </Link>
              </li>
              <li>
                <Link href="/places" className="text-gray-600 hover:text-gray-900">
                  Restaurants
                </Link>
              </li>
              <li>
                <Link href="/places/new" className="text-gray-600 hover:text-gray-900">
                  Add Restaurant
                </Link>
              </li>
            </ul>
          </div>

          {/* Support */}
          <div>
            <h3 className="font-semibold text-gray-900 mb-4">Support</h3>
            <ul className="space-y-2 text-sm">
              <li>
                <Link href="/about" className="text-gray-600 hover:text-gray-900">
                  About
                </Link>
              </li>
              <li>
                <Link href="/contact" className="text-gray-600 hover:text-gray-900">
                  Contact
                </Link>
              </li>
              <li>
                <Link href="/privacy" className="text-gray-600 hover:text-gray-900">
                  Privacy Policy
                </Link>
              </li>
              <li>
                <Link href="/terms" className="text-gray-600 hover:text-gray-900">
                  Terms of Service
                </Link>
              </li>
            </ul>
          </div>
        </div>

        <div className="mt-8 pt-8 border-t border-gray-200">
          <div className="flex flex-col sm:flex-row justify-between items-center">
            <p className="text-gray-500 text-sm">
              © {currentYear} The Dish. All rights reserved.
            </p>
            <div className="mt-4 sm:mt-0">
              <p className="text-gray-500 text-sm">
                Made with ❤️ for food lovers everywhere
              </p>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
}

