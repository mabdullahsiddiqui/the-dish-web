# The Dish - Web Application

A modern Next.js 14 web application for restaurant reviews and discovery, featuring GPS-verified reviews and dietary preference filtering.

## Features

- **User Authentication**: Secure registration and login with JWT tokens
- **Restaurant Discovery**: Search and filter restaurants by cuisine, dietary preferences, and location
- **GPS-Verified Reviews**: Location-based review verification for authenticity
- **Dietary Filtering**: Find restaurants that cater to specific dietary needs (Halal, Kosher, Vegan, etc.)
- **Responsive Design**: Optimized for mobile, tablet, and desktop devices
- **Real-time Updates**: Live data with caching and optimistic updates

## Prerequisites

- Node.js 18+ and npm
- Backend services running (see `../backend/DEPLOYMENT_GUIDE.md`)

## Getting Started

### Quick Start

1. **Install dependencies**:
```bash
npm install
```

2. **Environment setup**:
   - Copy `.env.local.example` to `.env.local`:
   ```bash
   cp .env.local.example .env.local
   ```
   - Or create `.env.local` manually with:
   ```
   NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1
   NEXT_PUBLIC_APP_NAME="The Dish"
   NEXT_PUBLIC_APP_URL=http://localhost:3000
   NEXT_PUBLIC_DEBUG=false
   ```

3. **Start backend services** (required):
   ```bash
   cd ../backend
   # Make sure Docker is running
   docker compose up -d
   # Start all microservices
   .\scripts\start-services.ps1  # Windows
   # or
   ./scripts/start-services.sh   # Linux/Mac
   ```

4. **Verify backend connectivity** (optional):
   ```bash
   # Windows
   .\scripts\check-backend.ps1
   # Linux/Mac
   chmod +x ./scripts/check-backend.sh
   ./scripts/check-backend.sh
   ```

5. **Start the development server**:
   ```bash
   npm run dev
   ```

6. **Open your browser**:
   Navigate to [http://localhost:3000](http://localhost:3000)

### Development Features

- **Backend Health Monitor**: In development mode, a health status widget appears in the bottom-right corner showing backend service status
- **Hot Reload**: Automatic page refresh on code changes
- **TypeScript**: Full type checking and IntelliSense support
- **Error Overlay**: Helpful error messages during development

## Backend Integration

This web application connects to The Dish backend services:

- **API Gateway**: `http://localhost:5000/api/v1` (Main entry point)
- **User Service**: `http://localhost:5001` (Authentication and user management)
- **Place Service**: `http://localhost:5002` (Restaurant data and photos)
- **Review Service**: `http://localhost:5003` (Reviews, GPS verification, helpfulness voting)

### Backend Requirements

1. **Docker Containers**: PostgreSQL, Redis, and RabbitMQ must be running
   ```bash
   cd ../backend
   docker compose up -d
   ```

2. **Microservices**: All four services must be running
   - Use the provided startup scripts: `backend/scripts/start-services.ps1` or `.sh`
   - Or start each service manually in separate terminals

3. **API Gateway**: Must be accessible at `http://localhost:5000`
   - Swagger UI available at: `http://localhost:5000/swagger`

### Verifying Backend Connection

The application includes health check utilities:

- **Development Widget**: Automatically shows backend health status in dev mode
- **Manual Check**: Run `scripts/check-backend.ps1` or `check-backend.sh`
- **Programmatic**: Use `lib/utils/health-check.ts` in your code

If services are not running, you'll see connection errors in the browser console and toast notifications.

## Project Structure

```
web/
├── app/                    # Next.js 14 App Router
│   ├── (auth)/            # Authentication routes
│   ├── (main)/            # Main application routes
│   └── layout.tsx         # Root layout
├── components/            # Reusable components
│   ├── ui/               # Base UI components
│   ├── features/         # Feature-specific components
│   └── layout/           # Layout components
├── lib/                  # Core libraries
│   ├── api/             # API client and services
│   ├── auth/            # Authentication utilities
│   └── utils/           # Utility functions
├── hooks/               # Custom React hooks
├── types/              # TypeScript type definitions
└── public/            # Static assets
```

## Key Features

### Authentication
- User registration with validation
- Secure login with JWT tokens
- Protected routes and middleware
- Automatic token refresh

### Restaurant Discovery
- Advanced search with multiple filters
- Location-based nearby restaurant discovery
- Cuisine type and dietary preference filtering
- Price range and rating filters

### Review System
- GPS-verified review submission with distance calculation
- Photo upload with drag & drop support (up to 5 photos per review)
- Helpfulness voting system with optimistic updates
- Dietary accuracy feedback
- Review editing and deletion
- Review management from user profile

### User Experience
- Responsive design for all devices (mobile, tablet, desktop)
- Fast loading with optimized images and code splitting
- Comprehensive error handling with user-friendly messages
- Toast notifications for all user actions
- Loading skeletons and states throughout
- Accessible design (WCAG 2.1 AA compliant)
- Keyboard shortcuts (Ctrl+K for search, Escape to close)
- Interactive maps with Leaflet integration

## Available Scripts

- `npm run dev` - Start development server with hot reload
- `npm run build` - Build optimized production bundle
- `npm run start` - Start production server (after build)
- `npm run lint` - Run ESLint for code quality

### Utility Scripts

- `scripts/check-backend.ps1` (Windows) - Check backend service health
- `scripts/check-backend.sh` (Linux/Mac) - Check backend service health

## Technology Stack

- **Framework**: Next.js 14 (App Router) with Server Components
- **Language**: TypeScript (strict mode)
- **Styling**: Tailwind CSS with custom design system
- **Forms**: React Hook Form + Zod validation
- **State Management**: React Context + TanStack Query (React Query)
- **HTTP Client**: Axios with interceptors
- **Icons**: Lucide React
- **Maps**: Leaflet with React-Leaflet (OpenStreetMap tiles)
- **Notifications**: React Hot Toast
- **UI Components**: Custom component library with shadcn/ui patterns

## Development Notes

### API Integration
- All API calls are routed through the API Gateway at `/api/v1`
- JWT tokens are stored in localStorage and automatically included in requests
- Automatic token refresh and logout on 401 errors
- Request/response interceptors handle authentication

### Features
- **GPS Verification**: Requires HTTPS in production (browser security requirement)
- **Photo Uploads**: Supports JPEG, PNG, WebP up to 10MB each (max 5 photos)
- **Maps**: Uses OpenStreetMap tiles (no API key required)
- **Search**: Full-text search with filters, pagination, and location-based sorting
- **Reviews**: GPS-verified submissions, helpful voting, edit/delete functionality

### Error Handling
- User-friendly error messages with toast notifications
- Automatic retry mechanisms for failed requests
- Graceful degradation when services are unavailable
- Comprehensive error boundaries for React components

### Performance
- Code splitting and lazy loading
- Image optimization with Next.js Image component
- Query caching with TanStack Query
- Optimistic UI updates for better perceived performance

## Production Deployment

### Environment Variables

Set the following in your production environment:

```bash
NEXT_PUBLIC_API_BASE_URL=https://api.yourdomain.com/api/v1
NEXT_PUBLIC_APP_NAME="The Dish"
NEXT_PUBLIC_APP_URL=https://yourdomain.com
NEXT_PUBLIC_DEBUG=false
```

### Deployment Checklist

1. ✅ **Environment Configuration**
   - Set production API URLs
   - Configure HTTPS endpoints
   - Disable debug mode

2. ✅ **Security**
   - Enable HTTPS (required for GPS geolocation)
   - Configure CORS on backend
   - Set secure cookie flags
   - Implement rate limiting

3. ✅ **Performance**
   - Enable Next.js production optimizations
   - Configure CDN for static assets
   - Set up image optimization
   - Enable caching strategies

4. ✅ **Monitoring**
   - Set up error tracking (Sentry, LogRocket, etc.)
   - Configure analytics
   - Monitor API response times
   - Track user engagement metrics

5. ✅ **Testing**
   - End-to-end testing with real backend
   - Load testing for API endpoints
   - Cross-browser compatibility
   - Mobile device testing

### Build for Production

```bash
npm run build
npm run start
```

The production build includes:
- Optimized JavaScript bundles
- Minified CSS
- Static page generation where possible
- Image optimization
- Tree shaking for smaller bundle sizes

## Troubleshooting

### Backend Connection Issues

**Problem**: "Failed to fetch" or connection errors

**Solutions**:
1. Verify backend services are running: `scripts/check-backend.ps1`
2. Check API Gateway is accessible: `http://localhost:5000/swagger`
3. Verify `.env.local` has correct `NEXT_PUBLIC_API_BASE_URL`
4. Check Docker containers are running: `docker ps`
5. Review backend logs for errors

### Build Errors

**Problem**: TypeScript or build errors

**Solutions**:
1. Run `npm run lint` to see specific errors
2. Ensure all dependencies are installed: `npm install`
3. Clear Next.js cache: `rm -rf .next` (or `Remove-Item -Recurse -Force .next` on Windows)
4. Check TypeScript version compatibility

### Map Not Loading

**Problem**: Maps show "Map unavailable" or blank

**Solutions**:
1. Check browser console for CORS errors
2. Verify Leaflet CSS is imported in `globals.css`
3. Ensure place has valid latitude/longitude
4. Check network tab for failed tile requests

## Contributing

### Code Standards

1. **TypeScript**: Follow strict mode guidelines, no `any` types
2. **Forms**: Use React Hook Form with Zod validation
3. **Components**: Follow component composition patterns
4. **Error Handling**: Implement proper error boundaries
5. **Accessibility**: Write accessible HTML with ARIA labels
6. **Testing**: Test responsive design on multiple devices
7. **Performance**: Optimize images and use code splitting

### Development Workflow

1. Create feature branch from `main`
2. Implement changes with proper TypeScript types
3. Test locally with backend services running
4. Run `npm run lint` and fix any issues
5. Test on multiple browsers and devices
6. Submit pull request with description

### File Structure Guidelines

- **Components**: Place in `components/` with clear naming
- **Hooks**: Custom hooks in `hooks/` directory
- **API**: API clients in `lib/api/` directory
- **Types**: TypeScript definitions in `types/` directory
- **Utils**: Utility functions in `lib/utils/` directory
