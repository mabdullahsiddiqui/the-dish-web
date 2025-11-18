# Technical Stack Admin Documentation

## Overview

This document provides comprehensive technical documentation for administrators and developers working on The Dish platform.

## System Architecture

### High-Level Architecture

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Web App   │     │ Mobile App │     │ Admin Panel│
│  (Next.js)  │     │  (Expo)    │     │  (React)   │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │
       └───────────────────┼───────────────────┘
                           │
                   ┌───────▼────────┐
                   │  API Gateway   │
                   │    (Ocelot)   │
                   └───────┬────────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
┌───────▼──────┐  ┌────────▼──────┐  ┌───────▼──────┐
│ User Service │  │ Place Service │  │Review Service│
│   (.NET 8)   │  │   (.NET 8)    │  │  (.NET 8)   │
└───────┬──────┘  └────────┬──────┘  └───────┬──────┘
        │                  │                  │
        └──────────────────┼──────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
┌───────▼──────┐  ┌────────▼──────┐  ┌───────▼──────┐
│  PostgreSQL  │  │     Redis     │  │ Elasticsearch│
│  (with       │  │   (Cache)     │  │   (Search)   │
│  PostGIS)    │  │               │  │              │
└──────────────┘  └───────────────┘  └──────────────┘
```

### Microservices Architecture

**Services**:
1. **User Service** (Port 5001): Authentication, user management
2. **Place Service** (Port 5002): Place CRUD, geospatial queries, photos
3. **Review Service** (Port 5003): Review management, GPS verification
4. **API Gateway** (Port 5000): Routing, authentication, rate limiting

**Communication**:
- RESTful APIs between services
- Message queues (RabbitMQ) for async operations
- Shared database per service (database per service pattern)

### Technology Stack

**Backend**:
- .NET 8 (C#)
- Entity Framework Core 8
- MediatR (CQRS pattern)
- Ocelot (API Gateway)
- JWT Authentication
- BCrypt (password hashing)

**Database**:
- PostgreSQL 15 with PostGIS extension
- Redis 7 (caching)
- Elasticsearch 8 (full-text search)

**Infrastructure**:
- Docker & Docker Compose (local development)
- AWS (production)
- Kubernetes (orchestration)
- Terraform (Infrastructure as Code)

**Frontend**:
- Next.js 14 (Web)
- React Native/Expo (Mobile)
- React + Vite (Admin Panel)
- TypeScript
- Tailwind CSS

## Development Environment Setup

### Prerequisites

1. **.NET 8 SDK**: https://dotnet.microsoft.com/download/dotnet/8.0
2. **Docker Desktop**: https://www.docker.com/products/docker-desktop
3. **Node.js 18+**: https://nodejs.org/
4. **Git**: https://git-scm.com/
5. **Entity Framework Core Tools**: `dotnet tool install -g dotnet-ef`

### Local Development Setup

1. **Clone Repository**:
   ```bash
   git clone <repository-url>
   cd the-dish-web
   ```

2. **Start Infrastructure**:
   ```powershell
   docker compose up -d
   ```

3. **Apply Migrations**:
   ```powershell
   cd backend
   .\scripts\apply-migrations.ps1
   ```

4. **Start Services**:
   ```powershell
   .\scripts\start-services.ps1
   ```

5. **Start Web App**:
   ```powershell
   cd web
   npm install
   npm run dev
   ```

## Project Structure

### Backend Structure

```
backend/
├── src/
│   ├── Common/
│   │   ├── TheDish.Common.Domain/      # Base entities, value objects
│   │   ├── TheDish.Common.Application/ # Interfaces, base classes
│   │   └── TheDish.Common.Infrastructure/ # Shared infrastructure
│   ├── Services/
│   │   ├── TheDish.User.API/
│   │   │   ├── Domain/                 # Entities, value objects, enums
│   │   │   ├── Application/            # Commands, queries, handlers, DTOs
│   │   │   ├── Infrastructure/         # DbContext, repositories, services
│   │   │   └── API/                    # Controllers, Program.cs
│   │   ├── TheDish.Place.API/          # Same structure
│   │   └── TheDish.Review.API/         # Same structure
│   └── TheDish.ApiGateway/             # Ocelot configuration
├── tests/                               # Unit and integration tests
└── scripts/                             # PowerShell scripts
```

### Clean Architecture Layers

**Domain Layer**:
- Entities (User, Place, Review)
- Value Objects (Location, etc.)
- Domain Events
- Business Rules

**Application Layer**:
- Commands (CreatePlace, UpdatePlace, etc.)
- Queries (GetPlaceById, SearchPlaces, etc.)
- Handlers (Command/Query handlers)
- DTOs (Data Transfer Objects)
- Interfaces (IRepository, IUnitOfWork, etc.)

**Infrastructure Layer**:
- DbContext (Entity Framework)
- Repositories (data access)
- External Services (S3, Email, etc.)
- Configuration

**API Layer**:
- Controllers
- Middleware
- Dependency Injection setup
- Configuration

## Database Design

### Database Per Service Pattern

Each microservice has its own database:
- `thedish_user` - User Service
- `thedish_place` - Place Service
- `thedish_review` - Review Service

### Key Tables

**User Service**:
- `Users`: User accounts
- `UserPreferences`: Dietary preferences

**Place Service**:
- `Places`: Restaurant/hotel information
- `PlacePhotos`: Photo metadata
- `MenuItems`: Menu items
- `DietaryCertifications`: Community certifications

**Review Service**:
- `Reviews`: Review content
- `ReviewPhotos`: Review photo metadata
- `ReviewHelpfulness`: Helpful/not helpful votes

### PostGIS Integration

**Geospatial Features**:
- `Location` column uses PostGIS `Point` type
- Spatial indexes for fast geospatial queries
- Functions: `ST_Distance`, `ST_Within`, `ST_Buffer`

**Example Query**:
```sql
SELECT * FROM Places
WHERE ST_DWithin(
    Location,
    ST_MakePoint(-74.0060, 40.7128)::geography,
    5000  -- 5km radius
);
```

## API Design

### RESTful Conventions

**Base URL**: `http://localhost:5000/api/v1`

**Endpoints**:
- `GET /users/{id}` - Get user
- `POST /users/register` - Register user
- `POST /users/login` - Login
- `GET /places/{id}` - Get place
- `GET /places/nearby` - Find nearby places
- `GET /places/search` - Search places
- `POST /places` - Create place (auth required)
- `GET /reviews/{id}` - Get review
- `POST /reviews` - Create review (auth required)

### Response Format

All endpoints return:
```json
{
  "success": true,
  "data": { ... },
  "message": "Optional message"
}
```

Error responses:
```json
{
  "success": false,
  "message": "Error message",
  "errors": [ ... ]  // Optional validation errors
}
```

### Authentication

**JWT Tokens**:
- Issued on login
- Expire after 60 minutes
- Include claims: UserId, Email, Role
- Sent in `Authorization: Bearer {token}` header

**Protected Endpoints**:
- Require valid JWT token
- Return 401 if token is missing or invalid
- Return 403 if user lacks required role

## Caching Strategy

### Redis Caching

**Cached Data**:
- Place search results (5-minute TTL)
- Place detail pages (10-minute TTL)
- User sessions
- Popular places list (1-hour TTL)

**Cache Keys**:
- `place:{id}` - Place detail
- `places:search:{hash}` - Search results
- `places:nearby:{lat}:{lon}:{radius}` - Nearby places
- `user:session:{token}` - User session

**Cache Invalidation**:
- On place update: Invalidate place cache
- On review creation: Invalidate place cache
- On user update: Invalidate user cache

## Search Implementation

### Elasticsearch Integration

**Indexed Data**:
- Place names, addresses, descriptions
- Cuisine types
- Dietary tags
- Reviews (for full-text search)

**Search Features**:
- Full-text search with fuzzy matching
- Faceted search (filter by multiple attributes)
- Sorting by relevance, rating, distance
- Pagination

**Example Query**:
```json
{
  "query": {
    "bool": {
      "must": [
        { "match": { "name": "italian restaurant" } }
      ],
      "filter": [
        { "term": { "dietaryTags": "vegetarian" } },
        { "range": { "priceRange": { "gte": 1, "lte": 2 } } }
      ]
    }
  },
  "sort": [
    { "averageRating": "desc" },
    { "_score": "desc" }
  ]
}
```

## File Storage

### AWS S3 Integration

**Storage**:
- Place photos: `s3://thedish-places/{placeId}/{photoId}.jpg`
- Review photos: `s3://thedish-reviews/{reviewId}/{photoId}.jpg`

**Photo Processing**:
- Resize to multiple sizes (thumbnail, medium, large)
- Optimize for web (JPEG compression)
- Generate thumbnails automatically

**Access Control**:
- Public read access for photos
- Private write access (requires AWS credentials)
- CDN integration for fast delivery

## Security

### Authentication & Authorization

**JWT Implementation**:
- Secret key: Minimum 32 characters
- Algorithm: HS256
- Claims: UserId, Email, Role
- Expiration: 60 minutes

**Password Security**:
- BCrypt hashing (work factor 12)
- Minimum 8 characters
- Complexity requirements (uppercase, lowercase, number, special char)

**Rate Limiting**:
- 100 requests per minute per IP
- 1000 requests per hour per user
- Implemented via AspNetCoreRateLimit

### Data Protection

**Encryption**:
- HTTPS for all API calls
- Encrypted database connections
- Encrypted S3 uploads

**Input Validation**:
- All inputs validated on API layer
- SQL injection prevention (parameterized queries)
- XSS prevention (content sanitization)

## Monitoring & Logging

### Logging

**Structured Logging**:
- Serilog for .NET services
- JSON format for easy parsing
- Log levels: Debug, Info, Warning, Error, Fatal

**Log Aggregation**:
- Centralized logging (ELK stack or CloudWatch)
- Log retention: 30 days
- Error alerting via email/Slack

### Monitoring

**Health Checks**:
- `/health` endpoint on each service
- Database connectivity check
- External service availability check

**Metrics**:
- Request rate
- Response times
- Error rates
- Database query performance

## Deployment

### Docker Compose (Development)

```yaml
services:
  postgres:
    image: postgis/postgis:15-3.3
    ports:
      - "5432:5432"
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
  elasticsearch:
    image: elasticsearch:8.11.0
    ports:
      - "9200:9200"
```

### Production Deployment

**AWS Infrastructure**:
- ECS/EKS for container orchestration
- RDS PostgreSQL (managed database)
- ElastiCache Redis (managed cache)
- S3 for file storage
- CloudFront for CDN
- Application Load Balancer

**Terraform Modules**:
- VPC with public/private subnets
- RDS PostgreSQL module
- ElastiCache Redis module
- S3 buckets module
- ECS/EKS cluster module

### CI/CD Pipeline

**GitHub Actions**:
1. Run tests on pull request
2. Build Docker images on merge to main
3. Deploy to staging environment
4. Run integration tests
5. Deploy to production (manual approval)

## Testing

### Test Types

**Unit Tests**:
- Domain logic
- Application handlers
- Utility functions

**Integration Tests**:
- Repository implementations
- API endpoints
- Database operations

**E2E Tests**:
- Complete user flows
- Cross-service interactions

### Test Coverage

**Target**: 80%+ code coverage

**Tools**:
- xUnit for .NET
- Jest for frontend
- Playwright for E2E

## Troubleshooting

### Common Issues

**Database Connection**:
- Check connection string
- Verify PostgreSQL is running
- Check network connectivity

**Migration Errors**:
- Check migration history
- Verify database schema
- Rollback if needed

**Service Startup**:
- Check port availability
- Verify dependencies are running
- Check configuration files

### Debugging

**Local Debugging**:
- Use Visual Studio or VS Code
- Attach debugger to running service
- Set breakpoints in code

**Log Analysis**:
- Check application logs
- Check database logs
- Check infrastructure logs

## Performance Optimization

### Database Optimization

**Indexes**:
- Primary keys (automatic)
- Foreign keys
- Frequently queried columns
- Spatial indexes for PostGIS

**Query Optimization**:
- Use projections (select only needed fields)
- Pagination for large result sets
- Avoid N+1 queries (use Include/ThenInclude)

### API Optimization

**Response Caching**:
- Cache static data
- Cache frequently accessed data
- Use ETags for conditional requests

**Compression**:
- Enable GZIP compression
- Compress large responses
- Optimize JSON payloads

## Backup & Recovery

### Database Backups

**Automated Backups**:
- Daily full backups
- Hourly incremental backups
- 30-day retention

**Backup Strategy**:
- RDS automated backups (production)
- Manual pg_dump (development)
- Point-in-time recovery available

### Disaster Recovery

**RTO (Recovery Time Objective)**: 4 hours
**RPO (Recovery Point Objective)**: 1 hour

**Recovery Procedures**:
1. Restore from latest backup
2. Apply transaction logs
3. Verify data integrity
4. Resume operations

## Documentation

### Code Documentation

**XML Comments**:
- All public APIs documented
- Generate API documentation from comments
- Swagger/OpenAPI integration

### Architecture Decision Records (ADRs)

**Location**: `/docs/adr/`

**Format**:
- Title
- Status (Proposed, Accepted, Rejected)
- Context
- Decision
- Consequences

## Contributing

### Development Workflow

1. Create feature branch from `develop`
2. Make changes and write tests
3. Ensure all tests pass
4. Submit pull request
5. Code review
6. Merge to `develop`
7. Deploy to staging
8. Merge to `main` for production

### Code Standards

**Backend**:
- Follow Clean Architecture principles
- Use CQRS pattern (Commands/Queries)
- 80%+ test coverage
- Follow C# coding conventions

**Frontend**:
- TypeScript strict mode
- ESLint + Prettier
- Component-based architecture
- Responsive design

### Git Conventions

**Branch Naming**:
- `feature/feature-name`
- `bugfix/bug-name`
- `hotfix/issue-name`

**Commit Messages**:
- `feat: add user authentication`
- `fix: resolve login issue`
- `docs: update API documentation`
- `refactor: improve code structure`


