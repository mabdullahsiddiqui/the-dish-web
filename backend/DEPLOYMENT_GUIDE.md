# The Dish - Backend Deployment Guide

## Prerequisites

1. **.NET 8 SDK** - Install from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Docker Desktop** - For running PostgreSQL, Redis, Elasticsearch, and RabbitMQ
3. **Entity Framework Core Tools** - Already installed globally: `dotnet tool install -g dotnet-ef`

## Step 1: Start Infrastructure Services

Start all required Docker containers:

```powershell
# From the project root directory
docker compose up -d
```

This will start:
- PostgreSQL (with PostGIS) on port 5432
- Redis on port 6379
- Elasticsearch on port 9200
- RabbitMQ on ports 5672 (AMQP) and 15672 (Management UI)
- pgAdmin on port 5050
- Redis Commander on port 8081

Verify containers are running:
```powershell
docker ps
```

## Step 2: Apply Database Migrations

Run the migration script:

```powershell
# From backend directory
.\scripts\apply-migrations.ps1
```

Or manually apply migrations:

```powershell
# Place Service
dotnet ef database update `
    --project src/Services/TheDish.Place.Infrastructure/TheDish.Place.Infrastructure.csproj `
    --startup-project src/Services/TheDish.Place.API/TheDish.Place.API.csproj `
    --context PlaceDbContext

# Review Service
dotnet ef database update `
    --project src/Services/TheDish.Review.Infrastructure/TheDish.Review.Infrastructure.csproj `
    --startup-project src/Services/TheDish.Review.API/TheDish.Review.API.csproj `
    --context ReviewDbContext
```

## Step 3: Start Backend Services

### Option 1: Use the Start Script

```powershell
# From backend directory
.\scripts\start-services.ps1
```

This will start all services in separate PowerShell windows.

### Option 2: Start Manually

Open separate terminal windows for each service:

**User Service:**
```powershell
cd src/Services/TheDish.User.API
dotnet run --urls http://localhost:5001
```

**Place Service:**
```powershell
cd src/Services/TheDish.Place.API
dotnet run --urls http://localhost:5002
```

**Review Service:**
```powershell
cd src/Services/TheDish.Review.API
dotnet run --urls http://localhost:5003
```

**API Gateway:**
```powershell
cd src/TheDish.ApiGateway
dotnet run --urls http://localhost:5000
```

## Service URLs

| Service | URL | Swagger UI |
|---------|-----|------------|
| User Service | http://localhost:5001 | http://localhost:5001/swagger |
| Place Service | http://localhost:5002 | http://localhost:5002/swagger |
| Review Service | http://localhost:5003 | http://localhost:5003/swagger |
| API Gateway | http://localhost:5000 | http://localhost:5000/swagger |

## Testing the Services

### 1. Test User Service

1. Open http://localhost:5001/swagger
2. Test `POST /api/v1/users/register`:
   ```json
   {
     "email": "test@example.com",
     "password": "Test123!",
     "firstName": "Test",
     "lastName": "User"
   }
   ```
3. Test `POST /api/v1/users/login` with the same credentials
4. Copy the JWT token from the response

### 2. Test Place Service

1. Open http://localhost:5002/swagger
2. Test `POST /api/v1/places` (requires authentication):
   ```json
   {
     "name": "Test Restaurant",
     "address": "123 Main St",
     "latitude": 40.7128,
     "longitude": -74.0060,
     "priceRange": 2,
     "cuisineTypes": ["Italian"]
   }
   ```
3. Test `GET /api/v1/places/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=5`

### 3. Test Review Service

1. Open http://localhost:5003/swagger
2. Test `POST /api/v1/reviews` (requires authentication):
   ```json
   {
     "placeId": "<place-id-from-step-2>",
     "rating": 5,
     "text": "Great food!",
     "checkInLatitude": 40.7128,
     "checkInLongitude": -74.0060,
     "placeLatitude": 40.7128,
     "placeLongitude": -74.0060
   }
   ```

### 4. Test API Gateway

1. Open http://localhost:5000/swagger
2. All routes are configured in `ocelot.json`
3. Test endpoints through the gateway (they will route to appropriate services)

## Configuration

### Connection Strings

Connection strings are configured in `appsettings.json` for each service:

- **Place Service**: `ConnectionStrings:PlaceDb`
- **Review Service**: `ConnectionStrings:ReviewDb`
- **User Service**: `ConnectionStrings:UserDb`

Default connection string:
```
Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password
```

### AWS S3 Configuration

For photo uploads, configure AWS credentials:

1. Set environment variables:
   ```powershell
   $env:AWS_ACCESS_KEY_ID="your-access-key"
   $env:AWS_SECRET_ACCESS_KEY="your-secret-key"
   $env:AWS_REGION="us-east-1"
   ```

2. Or use AWS credentials file: `~/.aws/credentials`

### JWT Configuration

JWT settings are in `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-minimum-32-characters-long",
    "Issuer": "TheDish",
    "Audience": "TheDishUsers",
    "ExpirationMinutes": 60
  }
}
```

## Troubleshooting

### Database Connection Issues

1. Verify PostgreSQL is running:
   ```powershell
   docker ps --filter "name=postgres"
   ```

2. Check connection string in `appsettings.json`

3. Test connection manually:
   ```powershell
   psql -h localhost -U thedish -d thedish
   ```

### Migration Issues

1. Check if migrations exist:
   ```powershell
   dotnet ef migrations list --project src/Services/TheDish.Place.Infrastructure --startup-project src/Services/TheDish.Place.API
   ```

2. Remove and recreate migrations if needed:
   ```powershell
   dotnet ef migrations remove --project src/Services/TheDish.Place.Infrastructure --startup-project src/Services/TheDish.Place.API
   dotnet ef migrations add InitialCreate --project src/Services/TheDish.Place.Infrastructure --startup-project src/Services/TheDish.Place.API
   ```

### Port Conflicts

If ports are already in use, modify `launchSettings.json` or use different ports:

```powershell
dotnet run --urls http://localhost:5004
```

## Production Deployment

For production deployment:

1. Update connection strings to production database
2. Configure AWS credentials and S3 buckets
3. Set strong JWT secret key
4. Enable HTTPS
5. Configure CORS for production domains
6. Set up proper logging (Serilog, Application Insights, etc.)
7. Configure rate limiting for production traffic
8. Set up health checks and monitoring

## Next Steps

- Phase 2: Web Application (Next.js)
- Phase 3: Mobile Application (React Native/Expo)
- Phase 4: Admin Panel
- Phase 5: Additional microservices (Dietary, Social, Business)










