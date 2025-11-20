# Quick Start - Fixing Authentication Issues

## Issue 1: Social Login Buttons Not Showing
✅ **FIXED** - Social login buttons now always show. They will be disabled (grayed out) if OAuth credentials aren't configured, with a tooltip explaining why.

## Issue 2: "Service temporarily unavailable" Error
This error occurs because the **User Service backend is not running**.

### Quick Fix - Start User Service:

**Option 1: Use the start-services script (Recommended)**
```powershell
cd backend
.\scripts\start-services.ps1
```
This will start all services including User Service on port 5001.

**Option 2: Restart User Service (if port is in use)**
```powershell
cd backend
.\scripts\restart-user-service.ps1
```
This script will automatically kill any existing process on port 5001 and start a fresh instance.

**Option 3: Start User Service manually**
```powershell
cd backend\src\Services\TheDish.User.API
dotnet run --urls http://localhost:5001
```

**If you get "port already in use" error:**
1. Find the process: `Get-NetTCPConnection -LocalPort 5001`
2. Kill it: `Stop-Process -Id <process-id> -Force`
3. Or use the restart script above

**Option 3: Start all services individually**
1. Start Docker containers:
   ```powershell
   docker compose up -d
   ```

2. Start User Service (port 5001):
   ```powershell
   cd backend\src\Services\TheDish.User.API
   dotnet run --urls http://localhost:5001
   ```

3. Start API Gateway (port 5000):
   ```powershell
   cd backend\src\TheDish.ApiGateway
   dotnet run --urls http://localhost:5000
   ```

### Verify Services Are Running:

Check if User Service is accessible:
```powershell
Test-NetConnection -ComputerName localhost -Port 5001
```

Or visit in browser:
- User Service Swagger: http://localhost:5001/swagger
- API Gateway Swagger: http://localhost:5000/swagger

### Test Registration After Starting Services:

1. Make sure User Service is running (port 5001)
2. Make sure API Gateway is running (port 5000)
3. Refresh the registration page
4. Fill in the form and click "Create Account"
5. Should work now! ✅

## Social Login Configuration (Optional)

Social login buttons are now visible but disabled until you configure OAuth credentials:

1. **For Google**: Add to `web/.env.local`:
   ```
   NEXT_PUBLIC_GOOGLE_CLIENT_ID=your-google-client-id
   ```

2. **For Facebook**: Add to `web/.env.local`:
   ```
   NEXT_PUBLIC_FACEBOOK_APP_ID=your-facebook-app-id
   ```

3. **Backend**: Add to `backend/src/Services/TheDish.User.API/appsettings.json`:
   ```json
   {
     "OAuth": {
       "Google": {
         "ClientId": "your-google-client-id"
       },
       "Facebook": {
         "AppId": "your-facebook-app-id",
         "AppSecret": "your-facebook-app-secret"
       }
     }
   }
   ```

4. Restart the web app and services after adding credentials.

See `SOCIAL_LOGIN_SETUP.md` for detailed OAuth setup instructions.

