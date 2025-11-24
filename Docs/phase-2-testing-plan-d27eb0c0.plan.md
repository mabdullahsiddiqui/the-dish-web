<!-- d27eb0c0-7deb-475a-b3dd-d0ae4c2d7968 3a3368a4-ea71-4ce2-b5da-36171373e899 -->
# Office Machine Manual Testing Setup Plan

## Overview

This plan provides step-by-step instructions for setting up and executing manual testing of The Dish application on your office/home machine. It includes environment configuration, service verification, and a structured testing workflow.

## Prerequisites Verification

### Step 1: Verify Required Software

Check if all required software is installed:

```powershell
# Check .NET SDK
dotnet --version
# Expected: 8.0.x or higher

# Check Node.js
node --version
# Expected: 18.x or higher

# Check npm
npm --version
# Expected: 9.x or higher

# Check Docker
docker --version
# Expected: Docker version 24.x.x or higher

# Check Docker Compose
docker compose version
# Expected: Docker Compose version v2.x.x
```

**Action**: If any are missing, install them before proceeding.

### Step 2: Verify Codebase Location

Ensure the codebase is available on the office machine:

```powershell
# Navigate to project root
cd "C:\DotNet\The Dish"
# Or wherever the project is located

# Verify structure
ls backend, web
# Should show backend and web directories
```

## Environment Configuration

### Step 3: Configure Web Application Environment

Create or verify `web/.env.local` file exists with required variables:

**File**: `web/.env.local`

**Content**:

```
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1
NEXT_PUBLIC_GOOGLE_CLIENT_ID=628105888738-tene6lgmejcfc8elmfgsqd947mv8qp0q.apps.googleusercontent.com
```

**Actions**:

1. Navigate to `web` directory
2. Check if `.env.local` exists
3. If missing, create it with the content above
4. If exists, verify it contains the Google Client ID

**Note**: The Google Client ID matches the backend configuration in `backend/src/Services/TheDish.User.API/appsettings.json`

### Step 4: Verify Backend Configuration

Check backend Google OAuth configuration:

**File**: `backend/src/Services/TheDish.User.API/appsettings.json`

**Verify**: Contains Google Client ID:

```json
"Google": {
  "ClientId": "628105888738-tene6lgmejcfc8elmfgsqd947mv8qp0q.apps.googleusercontent.com"
}
```

## Service Startup Sequence

### Step 5: Start Docker Containers

Start all required infrastructure services:

```powershell
# Navigate to backend directory
cd backend

# Start Docker containers
docker compose up -d

# Wait for containers to be healthy (30-60 seconds)
# Verify containers are running
docker ps
```

**Expected Output**: Should show 6 containers:

- thedish-postgres
- thedish-redis
- thedish-rabbitmq
- thedish-elasticsearch
- thedish-pgadmin
- thedish-redis-commander

**Verification**:

- Check container health: `docker ps` should show "healthy" status
- If containers fail, check logs: `docker compose logs`

### Step 6: Apply Database Migrations

Ensure database schema is up to date:

```powershell
# From backend directory
.\scripts\apply-migrations.ps1
```

**Alternative Manual Method**:

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

### Step 7: Start Backend Microservices

Start all backend services:

```powershell
# From backend directory
.\scripts\start-services.ps1
```

**Expected**: 4 PowerShell windows will open, one for each service:

- API Gateway (port 5000)
- User Service (port 5001)
- Place Service (port 5002)
- Review Service (port 5003)

**Verification**:

- API Gateway: http://localhost:5000/swagger should load
- User Service: http://localhost:5001/swagger should load
- Place Service: http://localhost:5002/swagger should load
- Review Service: http://localhost:5003/swagger should load

**Alternative Manual Start** (if script doesn't work):

```powershell
# Terminal 1 - API Gateway
cd backend\src\TheDish.ApiGateway
dotnet run --urls http://localhost:5000

# Terminal 2 - User Service
cd backend\src\Services\TheDish.User.API
dotnet run --urls http://localhost:5001

# Terminal 3 - Place Service
cd backend\src\Services\TheDish.Place.API
dotnet run --urls http://localhost:5002

# Terminal 4 - Review Service
cd backend\src\Services\TheDish.Review.API
dotnet run --urls http://localhost:5003
```

### Step 8: Verify Backend Health

Run comprehensive health check:

```powershell
# From backend directory
.\scripts\check-backend-comprehensive.ps1
```

**Expected**: All services should show as healthy/accessible

### Step 9: Start Web Application

Start the Next.js development server:

```powershell
# Navigate to web directory
cd web

# Install dependencies (if not already done)
npm install

# Start development server
npm run dev
```

**Expected**:

- Server starts on http://localhost:3000
- No critical errors in console
- Backend health widget (bottom-right) shows all services green

**Important**: If `.env.local` was just created, restart the dev server to load new environment variables.

## Pre-Testing Verification

### Step 10: Final System Check

Before starting manual tests, verify:

1. **Browser DevTools Open**:

   - Press F12
   - Open Console tab
   - Open Network tab
   - Open Application tab (for localStorage inspection)

2. **Backend Health Widget**:

   - Navigate to http://localhost:3000
   - Check bottom-right corner widget
   - All services should show green/healthy

3. **Google Login Button**:

   - Navigate to http://localhost:3000/login
   - Check if "Sign in with Google" button is enabled
   - Should NOT show "Not Configured" text

## Manual Testing Workflow

### Phase 1: Quick Smoke Tests (5 minutes)

**Priority**: Critical functionality verification

1. **Homepage Load**

   - Navigate to http://localhost:3000
   - Verify page loads without errors
   - Check console for JavaScript errors

2. **User Registration**

   - Navigate to /register
   - Create test account: testuser@example.com / Test123!@#
   - Verify successful registration and auto-login

3. **User Login**

   - Logout if logged in
   - Navigate to /login
   - Login with test account
   - Verify successful login

4. **Search Functionality**

   - Search for "pizza"
   - Verify results display
   - Click on a place
   - Verify place detail page loads

**Success Criteria**: All 4 tests pass without errors

### Phase 2: Core Feature Testing (15-20 minutes)

Follow the structured testing guide:

**Reference**: `backend/tests/MANUAL_TESTING_GUIDE.md`

**Key Test Areas**:

1. Authentication (Tests 1-5)
2. Search & Discovery (Tests 6-10)
3. Review System (Tests 11-15)
4. Review Management (Tests 16-18)
5. Review Voting (Tests 19-20)

**Documentation**: Use the test results template in the guide to document findings

### Phase 3: Advanced Testing (Optional, 30+ minutes)

1. **GPS Verification Testing**

   - Test location permission flow
   - Verify distance calculation
   - Test GPS verification badge

2. **Photo Upload Testing**

   - Test valid file uploads
   - Test invalid file types
   - Test file size limits

3. **Error Handling Testing**

   - Test network errors
   - Test invalid API responses
   - Test 401 unauthorized handling

4. **Responsive Design Testing**

   - Test mobile view (< 768px)
   - Test tablet view (768px - 1024px)
   - Test desktop view (> 1024px)

## Testing Documentation

### Step 11: Document Test Results

Use the test results template from `backend/tests/MANUAL_TESTING_GUIDE.md`:

**Template Location**: End of `backend/tests/MANUAL_TESTING_GUIDE.md`

**Document**:

- Test execution date
- Browser and OS information
- Test results (Pass/Fail/Warning)
- Issues found with steps to reproduce
- Screenshots (if applicable)
- Recommendations

## Troubleshooting Common Issues

### Issue: Docker Containers Not Starting

**Check**:

```powershell
docker compose logs
```

**Common Fixes**:

- Ensure Docker Desktop is running
- Check port conflicts (5432, 6379, 5672, 9200)
- Restart Docker Desktop

### Issue: Backend Services Not Accessible

**Check**:

- Verify services are running: `Get-Process | Where-Object {$_.ProcessName -like "*dotnet*"}`
- Check Swagger URLs manually
- Review service logs in PowerShell windows

### Issue: Web App Shows "Failed to fetch"

**Check**:

- Verify API Gateway is running: http://localhost:5000/swagger
- Check `.env.local` has correct `NEXT_PUBLIC_API_BASE_URL`
- Restart web app after changing `.env.local`

### Issue: Google Login Button Shows "Not Configured"

**Check**:

- Verify `web/.env.local` exists and contains `NEXT_PUBLIC_GOOGLE_CLIENT_ID`
- Restart Next.js dev server after creating/modifying `.env.local`
- Verify Google Client ID matches backend configuration

### Issue: Database Connection Errors

**Check**:

- Verify PostgreSQL container is running: `docker ps`
- Check container logs: `docker logs thedish-postgres`
- Verify migrations applied: Run `.\scripts\apply-migrations.ps1`

## Cleanup After Testing

### Step 12: Stop Services (When Done Testing)

```powershell
# Stop web application: Ctrl+C in web terminal

# Stop backend services: Close PowerShell windows or Ctrl+C

# Stop Docker containers (optional - can leave running)
cd backend
docker compose down
```

## Quick Reference Checklist

Use this checklist for quick verification:

- [ ] Docker Desktop running
- [ ] Docker containers started and healthy
- [ ] Database migrations applied
- [ ] Backend services running (4 services)
- [ ] Web application running on port 3000
- [ ] `.env.local` configured with Google Client ID
- [ ] Browser DevTools open (Console, Network, Application tabs)
- [ ] Backend health widget shows all green
- [ ] Google login button enabled (not "Not Configured")
- [ ] Test account created
- [ ] Manual testing guide open (`backend/tests/MANUAL_TESTING_GUIDE.md`)

## Files to Reference

- **Manual Testing Guide**: `backend/tests/MANUAL_TESTING_GUIDE.md`
- **Testing Checklist**: `web/TESTING_CHECKLIST.md`
- **Quick Start**: `web/START_TESTING.md`
- **Backend Deployment**: `backend/DEPLOYMENT_GUIDE.md`
- **Google Login Config**: `.cursor/plans/configure-google-login-67f2cde2.plan.md`

## Expected Testing Duration

- **Setup Time**: 10-15 minutes (first time)
- **Quick Smoke Tests**: 5 minutes
- **Core Feature Testing**: 15-20 minutes
- **Advanced Testing**: 30+ minutes (optional)
- **Total**: 30-60 minutes for comprehensive testing

## Success Criteria

Testing is successful when:

1. All services start without errors
2. Web application loads and connects to backend
3. User can register and login
4. Search functionality works
5. Reviews can be created and managed
6. No critical JavaScript errors in console
7. All API calls succeed (check Network tab)