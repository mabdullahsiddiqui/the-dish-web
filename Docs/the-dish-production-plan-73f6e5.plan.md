<!-- dae2fc94-bebb-493c-9447-6fdeba2a0d61 25291ffd-b80a-4d30-a06d-a43e3578b09c -->
# The Dish - Production Development Plan

## Executive Summary

**The Dish** is a next-generation restaurant and hotel review platform with intelligent dietary preference adaptation, community-verified certifications, and comprehensive business tools. This plan outlines a 20-month development roadmap from foundation to public launch.

**Timeline**: 20 months (84 weeks) to public launch

**Architecture**: .NET Core 8 Microservices + Next.js 14 + Expo React Native + React Admin Panel

**Deployment**: AWS Cloud-Native with Kubernetes

**Target**: 10,000+ users and 500+ business subscribers by Month 12

---

## Technology Stack

### Backend

- **.NET Core 8** - Microservices with Clean Architecture
- **PostgreSQL 15 + PostGIS** - Primary database with geospatial support
- **Redis** - Caching and session management
- **Elasticsearch** - Full-text search
- **RabbitMQ** - Event-driven messaging
- **Ocelot** - API Gateway

### Frontend

- **Next.js 14** - Web application (App Router + TypeScript)
- **Expo SDK 50+** - Mobile application (iOS/Android)
- **React + Vite** - Admin panel
- **Tailwind CSS** - Styling
- **Zustand + React Query** - State management

### Infrastructure

- **AWS** - EKS, RDS, ElastiCache, S3, CloudFront
- **Docker + Kubernetes** - Containerization and orchestration
- **Terraform** - Infrastructure as Code
- **GitHub Actions** - CI/CD pipelines

---

## Phase 0: Foundation & Infrastructure (Weeks 1-4)

### 0.1 Repository & Project Structure Setup

**Monorepo Structure**:

```
the-dish/
├── backend/          # .NET Core microservices
│   ├── src/
│   │   ├── ApiGateway/
│   │   ├── Services/
│   │   │   ├── User/
│   │   │   ├── Place/
│   │   │   ├── Review/
│   │   │   ├── Dietary/
│   │   │   ├── Social/
│   │   │   └── Business/
│   │   └── Common/
│   └── tests/
├── web/              # Next.js 14 web app
├── mobile/           # Expo React Native
├── admin/            # React admin panel
├── docs/             # Documentation (existing)
├── infrastructure/   # Terraform/IaC
└── .github/          # CI/CD workflows
```

**Setup Tasks**:

- Initialize Git repository with proper .gitignore
- Configure branch strategy (main, develop, feature/*)
- Set up GitHub Actions workflow templates
- Create README and contribution guidelines

### 0.2 Development Environment

**Docker Compose Setup**:

- PostgreSQL 15 with PostGIS extension
- Redis 7 for caching
- Elasticsearch 8 for search
- RabbitMQ 3 for messaging
- pgAdmin and Redis Commander for development

**Development Tooling**:

- ESLint + Prettier for code formatting
- EditorConfig for consistency
- Husky for pre-commit hooks
- Environment variable management (.env templates)

### 0.3 Database Schema Design

**Core Tables** (Based on Business Logic docs):

- **Users**: Users, UserPreferences, UserBadges, UserRoles, UserReputations
- **Places**: Places, PlacePhotos, PlaceMenus, PlaceHours, PlaceAmenities
- **Reviews**: Reviews, ReviewPhotos, ReviewVotes, ReviewReports
- **Dietary**: DietaryCertifications, DietaryConfirmations, DietaryDisputes, TrustScores
- **Social**: Friendships, ActivityFeed, Collections, UserFollows
- **Business**: BusinessAccounts, Subscriptions, BusinessAnalytics
- **Gamification**: Badges, UserPoints, Leaderboards

**Implementation**:

- Create Entity Framework Core entities
- Design migration scripts
- Set up database seeding for development
- Document schema relationships

### 0.4 AWS Infrastructure Foundation

**Terraform Modules**:

- VPC with public/private subnets across 3 AZs
- RDS PostgreSQL (Multi-AZ for production)
- ElastiCache Redis cluster
- EKS cluster configuration (or ECS Fargate for MVP)
- S3 buckets for media storage with versioning
- CloudFront CDN configuration
- Route 53 DNS setup
- IAM roles and security groups

---

## Phase 1: Core Backend Services - MVP (Weeks 5-14)

### 1.1 User Service (Weeks 5-7)

**Clean Architecture Structure**:

- `User.Domain` - Entities, value objects, domain events
- `User.Application` - Use cases, DTOs, interfaces
- `User.Infrastructure` - Database, external services
- `User.API` - Controllers, middleware

**Features**:

- User registration (email/password + OAuth prep)
- JWT authentication with refresh tokens
- Email verification workflow
- User profile CRUD operations
- Dietary preferences storage
- User reputation system foundation
- Role-based authorization (User, BusinessOwner, Admin, Moderator)

**Key Endpoints**:

- `POST /api/v1/users/register`
- `POST /api/v1/users/login`
- `POST /api/v1/users/refresh-token`
- `GET/PUT /api/v1/users/profile`
- `PUT /api/v1/users/preferences`

### 1.2 Place Service (Weeks 8-10)

**Features**:

- Place CRUD operations
- Geospatial queries using PostGIS (nearby places, distance calculation)
- Place search with filters (cuisine, price, amenities)
- Basic dietary tagging
- Photo upload to S3 with image resizing
- Business hours management
- Menu item storage

**Key Endpoints**:

- `POST /api/v1/places`
- `GET /api/v1/places/search`
- `GET /api/v1/places/nearby`
- `GET /api/v1/places/{id}`
- `POST /api/v1/places/{id}/photos`

**Geospatial Implementation**:

- PostGIS GEOGRAPHY type for coordinates
- Spatial indexes for performance
- Distance calculations in kilometers/miles

### 1.3 Review Service (Weeks 11-13)

**Features**:

- Review submission with GPS verification (proximity check)
- Star rating system (1-5 stars)
- Review photo upload (multiple photos)
- Review aggregation (average ratings, counts)
- Helpful votes system
- Review moderation queue
- Review editing with history tracking

**GPS Verification**:

- Validate user is within 200m of place when submitting
- Store submission coordinates
- Timestamp verification

**Key Endpoints**:

- `POST /api/v1/reviews`
- `GET /api/v1/reviews/place/{placeId}`
- `PUT /api/v1/reviews/{id}`
- `POST /api/v1/reviews/{id}/vote`
- `GET /api/v1/reviews/moderation/queue`

### 1.4 API Gateway & Infrastructure (Week 14)

**Ocelot Configuration**:

- Route configuration for all services
- JWT authentication middleware
- Rate limiting (100 requests/minute per user)
- Request/response logging
- CORS configuration
- Request aggregation for complex queries

**Shared Libraries**:

- `Common.Domain` - Base entities, value objects, domain events
- `Common.Application` - Common interfaces, behaviors
- `Common.Infrastructure` - Event bus, logging (Serilog), caching
- Exception handling middleware
- Standardized API response format

**Testing**:

- Unit tests (target: 80% coverage)
- Integration tests for API endpoints
- Health check endpoints
- Swagger/OpenAPI documentation

---

## Phase 2: Web Application - MVP (Weeks 15-24)

### 2.1 Next.js Foundation (Week 15)

**Project Setup**:

- Next.js 14 with App Router
- TypeScript strict mode
- Tailwind CSS configuration with custom theme
- Folder structure:
  - `app/` - Routes and layouts
  - `components/` - Reusable UI components
  - `lib/` - Utilities and API client
  - `hooks/` - Custom React hooks
  - `stores/` - Zustand stores
  - `types/` - TypeScript definitions

**State Management**:

- Zustand for global state
- React Query for server state
- API client with Axios/Fetch wrapper

### 2.2 Authentication & Onboarding (Weeks 16-17)

**Authentication Pages**:

- Login page with email/password
- Registration page with validation
- Password reset flow
- JWT token management (storage, refresh)
- Protected route middleware

**Onboarding Flow**:

1. Welcome screen
2. Location permission request
3. Dietary preferences selection (halal, kosher, vegan, vegetarian, gluten-free, etc.)
4. Cuisine interests
5. Profile completion (photo, bio)

### 2.3 Search & Discovery (Weeks 18-20)

**Homepage**:

- Personalized place recommendations
- Trending places
- Nearby places (based on location)
- Featured collections

**Search Page**:

- Search bar with autocomplete
- Advanced filters:
  - Dietary preferences (adaptive based on user)
  - Cuisine types
  - Price range
  - Distance
  - Rating threshold
  - Amenities
- Map view integration (Leaflet with OpenStreetMap)
- List view with place cards
- Sorting options (distance, rating, price)

**Place Detail Page**:

- Place information (name, address, hours, contact)
- Photo gallery with lightbox
- Dietary badges with trust scores
- Average rating and review count
- Review list with filtering
- Map location
- Call-to-action buttons (directions, call, website)

### 2.4 Review System UI (Weeks 21-22)

**Review Submission**:

- GPS check-in verification with distance indicator
- Star rating input
- Text review with rich formatting
- Photo upload (drag & drop, multiple files)
- Dietary confirmation questions
- Preview before submission

**Review Display**:

- Review cards with user info, rating, photos
- Helpful vote buttons
- Sort by: most recent, most helpful, highest/lowest rating
- Filter by rating
- Photo gallery view
- Report inappropriate content

### 2.5 User Profile & Settings (Week 23)

**Profile Page**:

- User stats (review count, helpful votes, badges)
- Review history with filters
- Badge showcase
- Reputation level indicator
- Social stats (friends, followers)

**Settings Page**:

- Profile editing (photo, bio, location)
- Dietary preferences management
- Privacy settings
- Notification preferences
- Account management (password change, delete account)

### 2.6 Polish & Optimization (Week 24)

- Mobile-responsive design (all breakpoints)
- Loading skeletons and states
- Error handling with user-friendly messages
- Toast notifications system
- Accessibility audit (WCAG 2.1 AA compliance)
- Performance optimization:
  - Code splitting
  - Image optimization (next/image)
  - Lazy loading components
  - Font optimization

---

## Phase 3: Dietary Verification System (Weeks 25-32)

### 3.1 Dietary Service Backend (Weeks 25-27)

**Features**:

- Certification document upload (PDF, images)
- Trust score calculation engine:
  - Official certification weight: 70%
  - Community confirmations: 20%
  - Business reputation: 10%
- Multi-layer verification workflow
- Certificate expiry tracking with notifications
- Dispute resolution system
- Verification history and audit trail

**Trust Score Algorithm**:

```
trustScore = (officialCertScore * 0.7) + (communityScore * 0.2) + (businessScore * 0.1)
officialCertScore = has valid cert ? 100 : 0
communityScore = (confirmations - denials) / total_verifications * 100
businessScore = business_reputation_score
```

**Key Endpoints**:

- `POST /api/v1/dietary/certifications`
- `GET /api/v1/dietary/places/{placeId}/certifications`
- `POST /api/v1/dietary/confirmations`
- `POST /api/v1/dietary/disputes`
- `GET /api/v1/dietary/trust-score/{placeId}`

### 3.2 Verification UI (Weeks 28-29)

**Business Owner Interface**:

- Certification upload form with drag & drop
- Certificate type selection (halal, kosher, vegan-certified, etc.)
- Issuing authority information
- Expiry date management
- Certification status dashboard

**Community Verification**:

- Confirmation flow for users who visited
- Simple yes/no dietary compliance questions
- Photo evidence upload (optional)
- Verification reputation (accurate verifiers get badges)

**Trust Score Display**:

- Visual trust score indicator (0-100)
- Badge with color coding (green >80, yellow 60-80, red <60)
- Breakdown of score components
- Certificate details (issuer, expiry)
- Community verification count

### 3.3 Adaptive Interface (Weeks 30-31)

**Dynamic UI Based on Preferences**:

- Dietary filters prominently displayed for user's preferences
- Halal-specific features (when user selects halal):
  - Prayer times integration
  - Qibla direction indicator
  - Halal certification prominence
- Vegan-specific features:
  - Plant-based options highlighting
  - Vegan-friendly tags
- Kosher-specific features:
  - Kosher supervision display
  - Passover availability

**Filter System Enhancement**:

- Smart dietary filters (auto-selected based on preferences)
- Trust score threshold slider
- Certification type filters
- "Strictly verified only" toggle

### 3.4 Testing & Refinement (Week 32)

- End-to-end verification workflows
- Trust score calculation validation
- Performance testing with large datasets
- Security audit for document uploads
- User acceptance testing

---

## Phase 4: Social Features & Gamification (Weeks 33-40)

### 4.1 Social Service Backend (Weeks 33-35)

**Features**:

- Friend request system (send, accept, reject)
- User following system
- Activity feed generation (friend reviews, check-ins, badges)
- Social recommendations (places friends liked)
- Privacy controls (public, friends-only, private)
- User blocking and reporting

**Activity Feed Algorithm**:

- Time-based feed with relevance scoring
- Friend activity prioritization
- Personalized content based on preferences

**Key Endpoints**:

- `POST /api/v1/social/friends/request`
- `GET /api/v1/social/friends`
- `GET /api/v1/social/feed`
- `POST /api/v1/social/follow/{userId}`
- `GET /api/v1/social/recommendations`

### 4.2 Gamification System (Weeks 36-37)

**Points System**:

- Review submission: 10 points
- Photo upload: 5 points per photo
- Helpful vote received: 2 points
- Dietary confirmation: 5 points
- First review at place: 15 points bonus

**Badge System**:

- First Review - Submit first review
- Explorer - Review 10 different places
- Food Critic - Write 50 reviews
- Trusted Verifier - 100 accurate dietary confirmations
- Community Helper - Receive 100 helpful votes
- Local Guide - Review 25 places in home city
- Globe Trotter - Review places in 5 different cities
- Elite Reviewer - Reach 1000 reputation points

**Leaderboard Types**:

- Global leaderboard
- City-based leaderboard
- Friends leaderboard
- Weekly/monthly competitions

**Reputation Levels**:

- Newbie: 0-100 points
- Contributor: 101-500 points
- Regular: 501-1000 points
- Expert: 1001-5000 points
- Elite: 5000+ points

### 4.3 Social UI (Weeks 38-39)

**Friend Management**:

- Friend search and discovery
- Friend requests page
- Friend list with filters
- User profile pages (public view)

**Activity Feed**:

- Infinite scroll feed
- Activity cards (reviews, check-ins, badges)
- Like and comment on activities
- Share functionality

**Collections Feature**:

- Create custom place collections ("My Favorites", "Want to Visit")
- Share collections with friends
- Follow other users' collections

### 4.4 Integration & Testing (Week 40)

- Social feature integration testing
- Feed performance optimization
- Gamification points validation
- Load testing for activity feed

---

## Phase 5: Business Features (Weeks 41-48)

### 5.1 Business Service Backend (Weeks 41-43)

**Features**:

- Business account registration
- Place claim/verification workflow
- Review response system
- Analytics data aggregation:
  - View counts
  - Review sentiment analysis
  - Customer demographics
  - Peak hours analysis
  - Competitor comparison
- Subscription management

**Stripe Integration**:

- Payment processing
- Subscription tiers (Free, Basic, Premium, Enterprise)
- Billing cycle management
- Invoice generation
- Payment method management

**Key Endpoints**:

- `POST /api/v1/business/register`
- `POST /api/v1/business/claim/{placeId}`
- `POST /api/v1/business/reviews/{reviewId}/respond`
- `GET /api/v1/business/analytics`
- `POST /api/v1/business/subscriptions`

### 5.2 Subscription Tiers

**Free Tier**:

- Claim 1 place
- Basic analytics (30 days)
- Respond to reviews
- 5 photos per place

**Basic Tier** ($29/month):

- Claim up to 3 places
- Advanced analytics (90 days)
- Priority review responses
- 20 photos per place
- Export data (CSV)

**Premium Tier** ($99/month):

- Claim up to 10 places
- Full analytics (12 months)
- Sentiment analysis
- 50 photos per place
- Competitor insights
- API access

**Enterprise Tier** (Custom pricing):

- Unlimited places
- Custom integrations
- Dedicated support
- White-label options
- Advanced API access

### 5.3 Business Dashboard (Weeks 44-46)

**Dashboard Sections**:

- Overview (key metrics, recent reviews)
- Reviews Management:
  - List all reviews
  - Respond to reviews
  - Flag inappropriate reviews
  - Review sentiment breakdown
- Analytics:
  - Views over time (Recharts)
  - Rating trends
  - Customer demographics
  - Peak hours heatmap
  - Review sources
- Profile Management:
  - Business information editing
  - Photo gallery management
  - Menu management
  - Hours and amenities
- Subscription:
  - Current plan details
  - Usage metrics
  - Billing history
  - Upgrade/downgrade options

### 5.4 Testing & Documentation (Weeks 47-48)

- Business workflow end-to-end testing
- Subscription flow validation
- Payment processing testing (Stripe test mode)
- Business user documentation
- API documentation for Premium/Enterprise

---

## Phase 6: Admin Panel (Weeks 49-56)

### 6.1 Admin Service Backend (Weeks 49-50)

**Features**:

- Admin authentication with 2FA
- Role-based access control (Super Admin, Moderator, Analyst)
- Admin action audit logging
- Moderation queue management
- User suspension/ban system
- Analytics aggregation

**Key Endpoints**:

- `GET /api/v1/admin/users`
- `POST /api/v1/admin/users/{id}/suspend`
- `GET /api/v1/admin/reviews/flagged`
- `POST /api/v1/admin/reviews/{id}/approve`
- `GET /api/v1/admin/certifications/queue`
- `GET /api/v1/admin/analytics`

### 6.2 Admin UI Development (Weeks 51-55)

**Tech Stack**: React + Vite + TypeScript + Tailwind + React Router

**Dashboard** (Week 51):

- Real-time statistics (users, places, reviews)
- Pending actions count
- Recent activity feed
- System health indicators

**User Management** (Week 52):

- User list with search/filter
- User detail view
- Suspend/ban actions
- Role assignment
- Reputation management

**Content Moderation** (Week 53):

- Review moderation queue
- Flagged content review
- Approve/reject actions
- User warning system
- Ban management

**Place & Certification Management** (Week 54):

- Place approval queue
- Place editing capabilities
- Certification verification queue
- Trust score adjustments
- Dispute resolution interface

**Analytics & Reports** (Week 55):

- User growth charts
- Engagement metrics
- Revenue reports (business subscriptions)
- Geographic distribution
- Feature usage statistics
- Export capabilities

### 6.3 Testing & Security (Week 56)

- Admin panel security audit
- Permission-based access testing
- Audit log validation
- Performance optimization
- Documentation for admins

---

## Phase 7: Search Service & External APIs (Weeks 57-62)

### 7.1 Elasticsearch Integration (Weeks 57-58)

**Index Design**:

- Places index with mappings:
  - Name, description (text fields)
  - Cuisine, dietary tags (keyword fields)
  - Location (geo_point)
  - Rating, price (numeric)
  - Trust scores (nested objects)

**Search Features**:

- Full-text search with relevance scoring
- Faceted search (filters with counts)
- Geospatial search (nearby + text)
- Autocomplete suggestions
- "Did you mean?" spelling corrections
- Search analytics tracking

### 7.2 External API Integration (Weeks 59-60)

**APIs to Integrate**:

- **Yelp Fusion API** (free tier):
  - Place data enrichment
  - Photo supplementation
  - Business information
- **OpenStreetMap**:
  - Map data
  - Place geocoding
  - Routing
- **Prayer Times API** (for halal users):
  - Prayer time calculations
  - Qibla direction

**Caching Strategy**:

- Redis cache for external API responses
- TTL: 24 hours for place data, 7 days for prayer times
- Cache invalidation on user updates

### 7.3 Recommendation Engine (Weeks 61-62)

**Algorithms**:

- **Collaborative Filtering**: Users who liked X also liked Y
- **Content-Based**: Similar places based on cuisine, dietary, price
- **Personalized**: Based on user preferences and history
- **Social**: Places friends liked
- **Location-Based**: Popular nearby places

**"Surprise Me" Feature**:

- Random high-rated place matching user preferences
- Outside usual cuisine choices (for exploration)
- Distance-limited

---

## Phase 8: Mobile Application (Weeks 63-72)

### 8.1 Expo Foundation (Week 63)

**Project Setup**:

- Expo SDK 50+ initialization
- TypeScript configuration
- React Navigation 6 setup
- Zustand + React Query
- API client configuration
- Theme configuration

### 8.2 Core Features (Weeks 64-68)

**Authentication** (Week 64):

- Login/register screens
- JWT token management (SecureStore)
- Biometric authentication option

**Place Discovery** (Week 65-66):

- Map view with place markers (react-native-maps)
- Place search with filters
- Place detail screen
- Photo gallery

**Review Submission** (Week 67):

- GPS check-in with accuracy indicator
- Camera integration (expo-camera)
- Photo picker (expo-image-picker)
- Rating input
- Text input with keyboard handling

**User Profile** (Week 68):

- Profile screen
- Settings screen
- Review history
- Badges display

### 8.3 Native Features (Weeks 69-70)

**Location Services**:

- Background location updates (with permission)
- Geofencing for check-in reminders
- Location accuracy indicators

**Push Notifications**:

- Firebase Cloud Messaging setup
- Notification permissions
- Badge updates (review likes, friend requests)
- Deep linking

**Offline Support**:

- Cache recently viewed places
- Queue actions when offline
- Sync when connection restored

### 8.4 Testing & Store Preparation (Weeks 71-72)

**Testing**:

- Device testing (iOS & Android, various screen sizes)
- Performance profiling
- Network error handling
- Permission flow testing

**App Store Assets**:

- App icons (all sizes)
- Screenshots (all required sizes)
- App descriptions
- Privacy policy
- Beta testing program (TestFlight, Google Play Beta)

---

## Phase 9: Advanced Features & Optimization (Weeks 73-78)

### 9.1 Practical Tools (Weeks 73-74)

**Reservation Integration**:

- OpenTable API integration (if available)
- Reservation availability display
- Deep linking to reservation apps

**Wait Time Feature**:

- User-reported wait times
- Real-time updates
- Historical data (average wait by day/time)

**Menu Browsing**:

- Menu photo upload
- OCR for text extraction (Google Vision API)
- Menu item search
- Dietary filtering on menu items

**Parking Information**:

- Parking availability (user-reported)
- Parking type (street, lot, garage, valet)
- Cost information

### 9.2 Performance Optimization (Weeks 75-76)

**Backend**:

- Database query optimization (indexes, query plans)
- N+1 query prevention
- Redis caching strategy refinement
- Connection pooling optimization

**Frontend**:

- Code splitting and lazy loading
- Image optimization (WebP, responsive images)
- Service worker for caching (PWA)
- Bundle size reduction

**Infrastructure**:

- CDN configuration for static assets
- Database read replicas
- Auto-scaling policies

### 9.3 Monitoring & Observability (Weeks 77-78)

**APM Setup**:

- Application Performance Monitoring (DataDog or Grafana)
- Error tracking (Sentry)
- Log aggregation (Elasticsearch + Kibana)
- Real-time dashboards

**Metrics**:

- API response times (p50, p95, p99)
- Database query times
- Cache hit rates
- Error rates
- User engagement metrics

**Alerting**:

- Error rate thresholds
- Performance degradation
- Database connection issues
- API downtime

---

## Phase 10: Security & Testing (Weeks 79-82)

### 10.1 Security Hardening (Weeks 79-80)

**Security Measures**:

- Rate limiting refinement (per endpoint)
- Input validation and sanitization (all endpoints)
- SQL injection prevention verification
- XSS protection (CSP headers)
- CSRF tokens for state-changing operations
- File upload security (type validation, size limits, virus scanning)
- Secrets management (AWS Secrets Manager)
- Security headers (HSTS, X-Frame-Options, etc.)

**Compliance**:

- GDPR compliance checklist
- Data export functionality
- Right to deletion implementation
- Privacy policy updates
- Cookie consent management

**Security Audit**:

- OWASP Top 10 review
- Penetration testing
- Dependency vulnerability scanning
- Security code review

### 10.2 Comprehensive Testing (Weeks 81-82)

**Backend Testing**:

- Achieve 80%+ code coverage
- Integration tests for all services
- API contract testing
- Load testing with k6 (target: 1000 concurrent users)

**Frontend Testing**:

- Unit tests (React Testing Library)
- E2E tests (Playwright)
- Visual regression testing
- Accessibility testing

**Mobile Testing**:

- Unit tests
- E2E tests (Detox)
- Device farm testing

---

## Phase 11: Production Infrastructure (Weeks 83-86)

### 11.1 AWS Infrastructure (Weeks 83-84)

**EKS Setup**:

- Kubernetes cluster configuration
- Node groups with auto-scaling
- Load balancer configuration
- Ingress controller (NGINX)

**Database**:

- RDS PostgreSQL Multi-AZ
- Read replicas for scaling
- Automated backups (point-in-time recovery)
- Performance Insights enabled

**Caching & Search**:

- ElastiCache Redis cluster mode
- Elasticsearch domain (3+ nodes)

**Storage**:

- S3 buckets with lifecycle policies
- CloudFront CDN distribution
- Image resizing Lambda@Edge

### 11.2 CI/CD Pipelines (Week 85)

**GitHub Actions Workflows**:

- Backend: Build → Test → Docker build → Push to ECR → Deploy to EKS
- Frontend: Build → Test → Deploy to S3 → CloudFront invalidation
- Mobile: Build → Test → Submit to App Store/Play Store

**Deployment Strategy**:

- Blue-green deployments
- Canary releases for critical changes
- Automated rollback on errors

### 11.3 Monitoring & Disaster Recovery (Week 86)

**Backup Strategy**:

- Database: Automated daily backups (30-day retention)
- User uploads: S3 versioning + cross-region replication
- Configuration: Infrastructure as Code in Git

**Disaster Recovery Plan**:

- RTO: 1 hour
- RPO: 5 minutes (for database)
- Multi-AZ deployment
- Incident response playbook

---

## Phase 12: Beta Launch & Iteration (Weeks 87-92)

### 12.1 Beta Program (Weeks 87-89)

**Beta Launch**:

- Recruit 150-200 beta testers
- Soft launch in 1-2 cities (geographically limited)
- Feedback collection system (in-app + surveys)
- Bug tracking and prioritization
- Weekly iteration cycles

**Success Metrics**:

- App stability (crash-free rate >99%)
- User retention (7-day retention >40%)
- Review submission rate
- Feature usage analytics

### 12.2 Iteration & Fixes (Weeks 90-91)

**Priority Fixes**:

- Critical bugs (P0/P1)
- Performance improvements
- UX refinements based on feedback
- Feature adjustments

**Content Seeding**:

- Manually seed 100+ places
- Partner with food bloggers for initial reviews
- Business partnerships for certifications

### 12.3 Pre-Launch Preparation (Week 92)

**Final Checklist**:

- Marketing website launch
- Help documentation and FAQs
- Terms of Service and Privacy Policy finalization
- Legal compliance verification
- Payment processing finalization
- App store optimization (ASO)
- Press kit preparation

---

## Phase 13: Public Launch (Weeks 93-96 / Months 20-21)

### 13.1 Soft Launch (Weeks 93-94)

- Limited geographic rollout (expand to 5 cities)
- Marketing campaign (social media, influencers)
- User acquisition tracking
- Real-time monitoring and rapid response

### 13.2 Scale & Optimize (Weeks 95-96)

- Infrastructure scaling based on load
- Performance tuning
- Cost optimization
- User feedback integration

### 13.3 Full Launch (Month 21+)

- Nationwide/international expansion
- Full feature set activation
- Major marketing push
- Partnership development (restaurants, food delivery apps)
- Press coverage

---

## Success Metrics & KPIs

### Technical Metrics

**Performance Targets**:

- API response time: <200ms (p50), <500ms (p95), <1s (p99)
- Page load time: <2s First Contentful Paint, <3s Time to Interactive
- Database queries: <100ms (p95)
- Cache hit rate: >80%
- Uptime: 99.9% (< 8.76 hours downtime/year)

**Quality Targets**:

- Test coverage: >80%
- Code review coverage: 100%
- Zero critical security vulnerabilities
- Crash-free rate: >99%

### Business Metrics

**Month 1**:

- 500-1,000 registered users
- 250+ reviews submitted
- 100+ places with reviews
- 10 business subscribers

**Month 6**:

- 5,000-10,000 registered users
- 5,000+ reviews submitted
- 500+ places with reviews
- 30% 7-day retention rate
- 50 business subscribers
- $1,500+ MRR

**Month 12**:

- 25,000-50,000 registered users
- 25,000+ reviews submitted
- 2,000+ places with reviews
- 30% DAU/MAU retention
- 200 business subscribers
- $10,000+ MRR

**User Engagement**:

- Average reviews per user: 2+ in first month
- Daily active users: 20-30% of monthly actives
- Session duration: 5+ minutes average
- Return rate: 40%+ within 7 days

---

## Team Requirements

### Phase 0-2 (Foundation & MVP) - Months 1-6

**Core Team** (6 people):

- 2 Backend Engineers (.NET Core expertise)
- 2 Frontend Engineers (React/Next.js)
- 1 DevOps Engineer
- 1 Product Manager / Designer

### Phase 3-5 (Features) - Months 7-12

**Expanded Team** (10 people):

- 3 Backend Engineers
- 2 Frontend Engineers
- 2 Mobile Engineers (iOS/Android)
- 1 DevOps Engineer
- 1 QA Engineer
- 1 Product Manager

### Phase 6+ (Scale) - Months 13-20

**Full Team** (15 people):

- 4 Backend Engineers
- 3 Frontend Engineers
- 2 Mobile Engineers
- 1 DevOps Engineer
- 2 QA Engineers
- 1 Product Manager
- 1 UI/UX Designer
- 1 Data Engineer

---

## Budget Considerations

### Development Costs (Monthly)

**Team Salaries** (varies by location):

- Small team (6): $60k-120k/month
- Medium team (10): $100k-200k/month
- Large team (15): $150k-300k/month

### Infrastructure Costs (Monthly)

**Year 1 (MVP)**:

- AWS infrastructure: $500-2,000
- Third-party services: $200-500
- Tools & licenses: $500-1,000
- **Total**: $1,200-3,500/month

**Year 2 (Growth)**:

- AWS infrastructure: $5,000-15,000
- Third-party services: $1,000-2,000
- Tools & licenses: $1,000-2,000
- **Total**: $7,000-19,000/month

**Ongoing Costs**:

- Domain & SSL: $100/year
- Email service (SendGrid): $50-500/month
- SMS verification: $100-500/month
- Monitoring (DataDog): $300-1,000/month
- Error tracking (Sentry): $100-300/month
- CI/CD: $0 (GitHub Actions free tier initially)

---

## Risk Mitigation

### Technical Risks

**Risk**: Microservices complexity

**Mitigation**: Start with well-defined service boundaries, use API gateway, invest in logging/monitoring early

**Risk**: Database performance at scale

**Mitigation**: Implement caching early, optimize queries proactively, use read replicas, plan sharding strategy

**Risk**: Third-party API costs and limits

**Mitigation**: Use free tiers initially, implement aggressive caching, have fallback strategies

**Risk**: Mobile app complexity

**Mitigation**: Use Expo for faster development, only use native modules when absolutely necessary

**Risk**: Search performance with Elasticsearch

**Mitigation**: Proper index design, caching search results, pagination, query optimization

### Business Risks

**Risk**: Chicken-and-egg problem (users need places, places need users)

**Mitigation**: Manually seed initial content, partner with food bloggers, incentivize early adopters with badges

**Risk**: Fake reviews and spam

**Mitigation**: GPS verification, ML spam detection, community moderation, reputation system

**Risk**: Dietary verification trust

**Mitigation**: Multi-layer verification (official + community), clear trust scores, dispute mechanism

**Risk**: Competition from established players

**Mitigation**: Focus on unique dietary verification features, superior UX, community-driven trust, target underserved communities

**Risk**: User acquisition costs

**Mitigation**: Leverage social features, referral program, community partnerships, organic growth through quality

### Operational Risks

**Risk**: Data loss or security breach

**Mitigation**: Automated backups, encryption at rest and in transit, regular security audits, incident response plan

**Risk**: Key person dependency

**Mitigation**: Documentation, code reviews, knowledge sharing, cross-training

**Risk**: Scope creep

**Mitigation**: Clear phase definitions, MVP-first approach, feature prioritization framework

---

## Technology Decision Rationale

### Why Microservices?

- Independent scaling of services
- Team autonomy and parallel development
- Technology flexibility per service
- Fault isolation

### Why .NET Core 8?

- High performance
- Strong typing and tooling
- Excellent async/await support
- Cross-platform
- Mature ecosystem

### Why Next.js 14?

- Server-side rendering for SEO
- App Router for better performance
- Built-in optimization (images, fonts)
- Strong TypeScript support
- Excellent developer experience

### Why PostgreSQL?

- PostGIS for geospatial queries (critical feature)
- JSONB for flexible schemas
- Strong consistency
- Mature and reliable
- Open source

### Why Expo for Mobile?

- Faster development than React Native CLI
- Over-the-air updates
- Easier deployment
- Good performance for most use cases
- Large community

---

## Next Immediate Steps (Week 1)

1. **Day 1-2**: Set up Git repository and monorepo structure
2. **Day 3-4**: Configure Docker Compose for local development
3. **Day 5**: Begin database schema design (first draft)
4. **Day 6-7**: Set up CI/CD workflow templates and AWS account

**Deliverables End of Week 1**:

- ✅ Git repository with proper structure
- ✅ Docker Compose running locally
- ✅ Database schema document (draft)
- ✅ AWS account configured
- ✅ Team onboarded with development environment

---

## Documentation References

- **Business Logic**: `Docs/Business Logic Part 1.md` & `Part 2.md`
- **Technical Stack**: `Docs/Technical Stack User Docs.md`
- **Database Schema**: Business Logic Part 2, Section 17
- **API Endpoints**: Business Logic Part 2, API Reference
- **Architecture Diagrams**: To be created in Phase 0

---

## Appendix: Development Standards

### Code Quality

- Mandatory code reviews (minimum 1 approver)
- Automated linting and formatting
- Pre-commit hooks for quality checks
- 80%+ test coverage requirement

### Git Workflow

- Feature branches from `develop`
- Pull requests with templates
- Squash merging to keep history clean
- Semantic commit messages

### API Design

- RESTful conventions
- URL versioning (`/api/v1/`)
- Consistent error responses
- Pagination for list endpoints
- HATEOAS principles where applicable

### Documentation

- OpenAPI/Swagger for all APIs
- README in every service
- Architecture Decision Records (ADRs)
- Runbook for operations

---

**This plan provides a comprehensive, realistic roadmap for building "The Dish" from foundation to public launch in 20 months with clear milestones, deliverables, and success metrics.**

### To-dos

- [x] Set up project structure, repositories, development environment (Docker), database schema design, and AWS infrastructure foundation with Terraform
- [ ] Build MVP backend services: User Service (auth, profiles), Place Service (CRUD, geospatial), Review Service (GPS verification), and API Gateway with shared libraries
- [ ] Develop MVP Next.js web application with authentication, onboarding, search/discovery, review submission, user profiles, and responsive design
- [ ] Implement dietary verification system with certification uploads, trust score calculation, community verification, adaptive UI, and testing
- [ ] Build social features (friends, following, activity feed) and gamification system (points, badges, leaderboards, reputation levels)
- [ ] Develop business owner features: account registration, place claiming, review responses, analytics dashboard, and Stripe subscription system
- [ ] Create React admin panel with user management, content moderation, place/certification management, analytics, and RBAC security
- [ ] Integrate Elasticsearch for advanced search, external APIs (Yelp, OpenStreetMap, Prayer Times), and implement recommendation engine
- [ ] Build Expo mobile application with authentication, map-based discovery, GPS review submission, native features (camera, push notifications, offline support)
- [ ] Implement practical tools (reservations, wait times, menu browsing), performance optimization, and monitoring/observability setup
- [ ] Complete security hardening (OWASP Top 10, penetration testing, GDPR compliance) and comprehensive testing (unit, integration, E2E, load testing)
- [ ] Set up production AWS infrastructure (EKS, RDS, ElastiCache), CI/CD pipelines with blue-green deployment, and disaster recovery plan
- [ ] Launch beta program with 150-200 testers, collect feedback, iterate on critical bugs/UX, seed initial content, and complete pre-launch preparation
- [ ] Execute soft launch in 5 cities, scale infrastructure based on load, and complete full public launch with marketing campaign