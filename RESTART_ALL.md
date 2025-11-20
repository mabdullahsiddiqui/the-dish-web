# Restart All Services - Quick Guide

## Backend Services

Run this command to restart all backend services:

```powershell
cd backend
.\scripts\restart-all-services.ps1
```

This will:
1. Stop all existing services (User, Place, Review, API Gateway)
2. Check Docker/PostgreSQL
3. Start all services fresh

**Or restart manually:**

1. **Stop all services:**
   - Close all PowerShell windows running backend services
   - Or kill processes on ports 5000, 5001, 5002, 5003

2. **Start services (in separate PowerShell windows):**
   ```powershell
   # User Service
   cd backend\src\Services\TheDish.User.API
   dotnet run --urls http://localhost:5001

   # Place Service
   cd backend\src\Services\TheDish.Place.API
   dotnet run --urls http://localhost:5002

   # Review Service
   cd backend\src\Services\TheDish.Review.API
   dotnet run --urls http://localhost:5003

   # API Gateway
   cd backend\src\TheDish.ApiGateway
   dotnet run --urls http://localhost:5000
   ```

## Web App

1. **Stop current web app:**
   - Find the terminal running `npm run dev`
   - Press `Ctrl+C`

2. **Start web app:**
   ```powershell
   cd web
   npm run dev
   ```

## Verify Everything is Running

After restart, check:

- **User Service**: http://localhost:5001/swagger
- **Place Service**: http://localhost:5002/swagger
- **Review Service**: http://localhost:5003/swagger
- **API Gateway**: http://localhost:5000/swagger
- **Web App**: http://localhost:3000

## After Restart

1. The improved error logging will be active in User Service
2. Try Google login again
3. Check User Service PowerShell window for detailed error messages
4. This will help us identify why token validation is failing

