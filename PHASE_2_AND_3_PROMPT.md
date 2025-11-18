# Phase 2: Web Application Development - Agent Prompt

## Project Context

You are working on **"The Dish"** - a universal restaurant and hotel review platform with adaptive dietary features. Phase 1 (Backend Services) has been **100% completed** and is production-ready.

**Note**: Phase 3 (Mobile Application) is deferred for now. This prompt focuses exclusively on Phase 2 (Web Application).

## Phase 1 Completion Status ✅

### Backend Services (All Complete)

- **User Service**: JWT authentication, registration, login (http://localhost:5001)
- **Place Service**: CRUD operations, geospatial queries, photo uploads (http://localhost:5002)
- **Review Service**: Review creation, GPS verification, helpfulness voting (http://localhost:5003)
- **API Gateway**: Ocelot routing, JWT auth, rate limiting (http://localhost:5000)

### Testing

- **41 tests passing** (Domain, Application, Integration, API layers)
- All services build successfully
- Database migrations created and ready

### Documentation

- `backend/DEPLOYMENT_GUIDE.md` - Complete deployment instructions
- `backend/MIGRATIONS_AND_TESTING_SUMMARY.md` - Testing details
- `backend/README_NEXT_STEPS.md` - Quick reference
- `IMPLEMENTATION_STATUS.md` - Overall project status

---

## Phase 2: Web Application (Next.js 14)

### Mission

Build a modern, responsive Next.js 14 web application that connects to the Phase 1 backend services.

### Technical Stack

- **Framework**: Next.js 14 (App Router) - Already initialized in `backend/web/`
- **Language**: TypeScript
- **Styling**: Tailwind CSS (already configured)
- **State Management**: React Context / Zustand / TanStack Query
- **Forms**: React Hook Form + Zod validation
- **HTTP Client**: Axios or Fetch API
- **Maps**: Leaflet / Mapbox / Google Maps
- **UI Components**: shadcn/ui or similar

### Project Location

- **Current**: `backend/web/` (needs to be moved to root `web/`)
- **Target**: Root-level `web/` directory

### API Endpoints

**Base URL**: `http://localhost:5000/api/v1` (API Gateway)

#### Authentication

```
POST /api/v1/users/register
Body: { email, password, firstName, lastName }
Response: { success: boolean, data: { token: string, user: UserDto }, message?: string }

POST /api/v1/users/login
Body: { email, password }
Response: { success: boolean, data: { token: string, user: UserDto }, message?: string }
```

#### Places

```
GET /api/v1/places/{id}
Response: { success: boolean, data: PlaceDto, message?: string }

GET /api/v1/places/nearby?latitude={lat}&longitude={lon}&radiusKm={km}&dietaryFilters={tags}&cuisineFilters={types}&priceRange={1-4}
Response: { success: boolean, data: PlaceDto[], message?: string }

GET /api/v1/places/search?searchTerm={term}&cuisineTypes={types}&dietaryTags={tags}&minPriceRange={1}&maxPriceRange={4}&minRating={0-5}&latitude={lat}&longitude={lon}&radiusKm={km}&page={1}&pageSize={20}
Response: { success: boolean, data: SearchPlacesResponseDto, message?: string }

POST /api/v1/places (requires auth)
Body: { name, address, latitude, longitude, phone?, website?, email?, priceRange, cuisineTypes, hoursOfOperation?, amenities?, parkingInfo? }
Response: { success: boolean, data: PlaceDto, message?: string }

PUT /api/v1/places/{id} (requires auth)
Body: { name, address, latitude, longitude, phone?, website?, email?, priceRange, cuisineTypes, hoursOfOperation?, amenities?, parkingInfo? }
Response: { success: boolean, data: PlaceDto, message?: string }

POST /api/v1/places/{id}/photos (requires auth)
Body: FormData with file, caption?, isFeatured?
Response: { success: boolean, data: PlacePhotoDto, message?: string }

POST /api/v1/places/{id}/claim (requires auth)
Response: { success: boolean, data: PlaceDto, message?: string }
```

#### Reviews

```
GET /api/v1/reviews/{id}
Response: { success: boolean, data: ReviewDto, message?: string }

GET /api/v1/reviews/place/{placeId}?page={1}&pageSize={20}
Response: { success: boolean, data: ReviewListResponseDto, message?: string }

GET /api/v1/reviews/user/{userId}?page={1}&pageSize={20}
Response: { success: boolean, data: ReviewListResponseDto, message?: string }

GET /api/v1/reviews/recent?page={1}&pageSize={20}
Response: { success: boolean, data: ReviewListResponseDto, message?: string }

POST /api/v1/reviews (requires auth)
Body: { placeId, rating, text, checkInLatitude?, checkInLongitude?, placeLatitude, placeLongitude, dietaryAccuracy? }
Response: { success: boolean, data: ReviewDto, message?: string }

PUT /api/v1/reviews/{id} (requires auth)
Body: { rating, text, dietaryAccuracy? }
Response: { success: boolean, data: ReviewDto, message?: string }

DELETE /api/v1/reviews/{id} (requires auth)
Response: { success: boolean, data: boolean, message?: string }

POST /api/v1/reviews/{id}/helpful (requires auth)
Body: boolean (true for helpful, false for not helpful)
Response: { success: boolean, data: ReviewDto, message?: string }
```

### Core Features to Implement

#### 1. Authentication Pages

- Register Page (`/register`) - Email, password, first name, last name
- Login Page (`/login`) - Email and password with "Remember me"
- Protected Routes - Middleware/guard with token validation

#### 2. Homepage & Search (`/`)

- Hero section with search bar
- Location-based or text search
- Advanced filters (dietary tags, cuisine types, price range, rating, distance)
- Results display (grid/list view, place cards, pagination)
- Map view option

#### 3. Place Detail Page (`/places/[id]`)

- Place information (photos, name, address, contact, map, hours, amenities, dietary tags, price range)
- Reviews section (list with user info, rating, text, photos, GPS verification badge, helpful buttons, dietary accuracy, pagination, sorting)
- Actions (Write Review, Claim Place, Share)

#### 4. Review Submission (`/places/[id]/review`)

- Review form (rating 1-5, text, photo upload, GPS check-in, dietary accuracy feedback)
- GPS verification (request location, show distance, verify within 200m)

#### 5. User Profile (`/profile`)

- User information display
- User reviews list
- Settings (edit profile, change password, dietary preferences)

#### 6. Navigation & Layout

- Header (logo, search, nav links, user menu)
- Footer (links, social media, copyright)
- Responsive design (mobile-first)

### Project Structure

```
web/
├── app/
│   ├── (auth)/
│   │   ├── login/
│   │   │   └── page.tsx
│   │   └── register/
│   │       └── page.tsx
│   ├── (main)/
│   │   ├── page.tsx
│   │   ├── places/
│   │   │   └── [id]/
│   │   │       ├── page.tsx
│   │   │       └── review/
│   │   │           └── page.tsx
│   │   └── profile/
│   │       └── page.tsx
│   └── layout.tsx
├── components/
│   ├── ui/ (base components)
│   ├── features/
│   │   ├── auth/
│   │   ├── places/
│   │   ├── reviews/
│   │   └── search/
│   └── layout/
│       ├── header.tsx
│       └── footer.tsx
├── lib/
│   ├── api/
│   │   ├── client.ts
│   │   ├── auth.ts
│   │   ├── places.ts
│   │   └── reviews.ts
│   ├── auth/
│   │   ├── context.tsx
│   │   └── middleware.ts
│   └── utils/
│       └── cn.ts
├── hooks/
│   ├── useAuth.ts
│   ├── usePlaces.ts
│   └── useReviews.ts
├── types/
│   ├── api.ts
│   ├── place.ts
│   ├── review.ts
│   └── user.ts
└── public/
    └── (static assets)
```

---

## Phase 3: Mobile Application (Deferred)

**Status**: Phase 3 (Mobile Application) is deferred and will be implemented in a future phase. This prompt focuses exclusively on Phase 2 (Web Application).

---

## Implementation Priorities

### Phase 2.1: Web Foundation (Week 1)

1. Move `backend/web` to root `web/`
2. Set up project structure
3. Configure Tailwind CSS
4. Create API client and auth utilities
5. Implement authentication pages

### Phase 2.2: Web Core Features (Week 2-3)

1. Homepage with search
2. Place listing and detail pages
3. Review display and submission
4. User profile page
5. Navigation and layout

### Phase 2.3: Web Polish (Week 4)

1. Responsive design refinement
2. Loading and error states
3. Performance optimization
4. Accessibility improvements
5. Final testing and bug fixes

---

## Important Notes

### Backend Integration

- All backend services are complete and tested
- Use API Gateway: `http://localhost:5000/api/v1`
- All endpoints return: `{ success: boolean, data?: T, message?: string }`
- JWT tokens in `Authorization: Bearer {token}` header
- Rate limiting: 100 requests per minute per IP

### Authentication

- Register and login endpoints do NOT require authentication
- All other endpoints (places POST/PUT, reviews POST, etc.) require JWT token
- Token should be stored securely and included in request headers
- Token expiration: 60 minutes (configurable in backend)

### API Response Format

All endpoints follow this consistent format:

```typescript
{
  success: boolean;
  data?: T;  // The actual response data
  message?: string;  // Optional success/error message
}
```

Error responses will have `success: false` and include a `message` field.

### Shared Considerations

- **Error handling**: User-friendly messages, proper error boundaries
- **Performance**: Optimize API calls, use caching where appropriate
- **Security**: Secure token storage, validate all inputs
- **Accessibility**: WCAG 2.1 AA compliance
- **Testing**: Unit and integration tests for critical paths

### Web-Specific

- Use Next.js Image component for optimized images
- Implement lazy loading for images and components
- SEO optimization with proper meta tags
- Server-side rendering where beneficial
- Client-side routing with Next.js router


---

## Reference Files

### Backend API Controllers

- `backend/src/Services/TheDish.User.API/Controllers/UsersController.cs`
- `backend/src/Services/TheDish.Place.API/Controllers/PlacesController.cs`
- `backend/src/Services/TheDish.Place.API/Controllers/PlacePhotosController.cs`
- `backend/src/Services/TheDish.Review.API/Controllers/ReviewsController.cs`
- `backend/src/TheDish.ApiGateway/ocelot.json`

### Documentation

- `backend/DEPLOYMENT_GUIDE.md` - Complete deployment instructions
- `backend/MIGRATIONS_AND_TESTING_SUMMARY.md` - Testing and migration details
- `backend/README_NEXT_STEPS.md` - Quick reference guide
- `IMPLEMENTATION_STATUS.md` - Overall project status

### Configuration

- `backend/src/TheDish.ApiGateway/ocelot.json` - API Gateway routing configuration
- `backend/src/Services/*/appsettings.json` - Service configurations

---

## Success Criteria

### Web Application

- ✅ Users can register and login
- ✅ Users can search for places with filters
- ✅ Users can view place details with photos and reviews
- ✅ Users can submit reviews with GPS verification
- ✅ Users can view their profile and review history
- ✅ All pages are responsive (mobile, tablet, desktop)
- ✅ Fast page loads (< 2s First Contentful Paint)
- ✅ Proper error handling and loading states
- ✅ Accessible to screen readers and keyboard navigation


---

## Getting Started

1. **Review Backend**: Read `backend/DEPLOYMENT_GUIDE.md` and understand API endpoints
2. **Start Backend Services**: Ensure all services are running (see deployment guide)
3. **Start Web Development (Phase 2)**: 
   - Move `backend/web/` to root `web/`
   - Set up project structure
   - Implement features following the priorities outlined above
4. **Test Integration**: 
   - Ensure backend services are running
   - Test all features end-to-end
   - Verify error handling
   - Test responsive design on multiple devices

---

## Development Tips

### API Client Setup

Create a centralized API client that:
- Handles base URL configuration
- Adds JWT token to requests automatically
- Handles token refresh
- Provides error handling
- Supports request/response interceptors

### State Management

For Web:
- Use React Context for auth state
- Use TanStack Query for server state (places, reviews)
- Use Zustand for client state if needed

### Error Handling

- Create custom error classes
- Implement error boundaries
- Show user-friendly error messages
- Log errors for debugging
- Handle network errors gracefully

### Testing Strategy

- Unit tests for utilities and hooks
- Integration tests for API calls
- E2E tests for critical user flows
- Test error scenarios
- Test loading states

---

**You have a solid, tested backend to build upon. Focus on creating a beautiful, functional web application that showcases the platform's capabilities!**

Good luck! 🚀

