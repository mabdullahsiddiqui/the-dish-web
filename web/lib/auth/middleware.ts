import { NextRequest, NextResponse } from 'next/server';

export function authMiddleware(request: NextRequest) {
  const token = request.cookies.get('auth_token')?.value || 
                request.headers.get('authorization')?.replace('Bearer ', '');

  const { pathname } = request.nextUrl;

  // Public routes that don't require authentication
  const publicRoutes = [
    '/',
    '/login',
    '/register',
  ];

  // Routes that should redirect to main app if authenticated
  const guestOnlyRoutes = ['/login', '/register'];

  // Check if current route is public
  const isPublicRoute = publicRoutes.includes(pathname) || 
                       pathname.startsWith('/places/') && pathname.split('/').length === 3; // Allow viewing place details

  // If accessing guest-only route while authenticated, redirect to home
  if (token && guestOnlyRoutes.includes(pathname)) {
    return NextResponse.redirect(new URL('/', request.url));
  }

  // If accessing protected route without token, redirect to login
  if (!token && !isPublicRoute) {
    const loginUrl = new URL('/login', request.url);
    loginUrl.searchParams.set('redirect', pathname);
    return NextResponse.redirect(loginUrl);
  }

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
     * - public files (*.svg, *.png, *.jpg, etc.)
     */
    '/((?!api|_next/static|_next/image|favicon.ico|.*\\.).*)',
  ],
};

