import { NextRequest, NextResponse } from 'next/server';

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Public routes that don't require authentication
  const publicRoutes = [
    '/',
    '/login',
    '/register',
    '/search',
  ];

  // Routes that should redirect to main app if authenticated
  const guestOnlyRoutes = ['/login', '/register'];

  // Check if current route is public (including place detail pages)
  // Note: /places/new requires authentication (handled in page component)
  const isPublicRoute = 
    publicRoutes.includes(pathname) || 
    (pathname.startsWith('/places/') && !pathname.includes('/review') && pathname !== '/places/new');

  // Get token from cookies or localStorage (will be available in browser)
  // Note: This middleware runs on the server, so we can't access localStorage
  // We'll handle auth checks in the client-side components instead
  
  return NextResponse.next();
}

export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     * - public files
     */
    '/((?!api|_next/static|_next/image|favicon.ico|.*\\.).*)',
  ],
};

