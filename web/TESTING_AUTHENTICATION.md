# Authentication Testing Guide

## Current Issues Identified

### 1. User Service Not Running
**Error**: "Service temporarily unavailable. Please try again later."

**Solution**: Start the User Service backend
```powershell
cd backend\src\Services\TheDish.User.API
dotnet run --urls http://localhost:5001
```

Or use the start-services script:
```powershell
cd backend
.\scripts\start-services.ps1
```

### 2. Google OAuth Not Configured
**Error**: "Missing required parameter: client_id"

**Solution**: 
- Social login buttons are now hidden if OAuth credentials are not configured
- Email/password authentication will work without OAuth credentials
- To enable social login, follow `SOCIAL_LOGIN_SETUP.md`

## Testing Email/Password Authentication

### Prerequisites
1. **Start Docker containers**:
   ```powershell
   docker compose up -d
   ```

2. **Start User Service**:
   ```powershell
   cd backend\src\Services\TheDish.User.API
   dotnet run --urls http://localhost:5001
   ```

3. **Start API Gateway**:
   ```powershell
   cd backend\src\TheDish.ApiGateway
   dotnet run --urls http://localhost:5000
   ```

4. **Start Web Application**:
   ```powershell
   cd web
   npm run dev
   ```

### Test Registration Flow

1. Navigate to `http://localhost:3000/register`
2. Fill in the registration form:
   - First Name: Test
   - Last Name: User
   - Email: test@example.com
   - Password: Test123!@#
   - Confirm Password: Test123!@#
3. Click "Create Account"
4. **Expected**: 
   - Success toast message
   - Redirect to homepage
   - User logged in

### Test Login Flow

1. Navigate to `http://localhost:3000/login`
2. Enter credentials:
   - Email: test@example.com
   - Password: Test123!@#
3. Click "Sign In"
4. **Expected**:
   - Success toast message
   - Redirect to homepage
   - User logged in

### Test Error Cases

1. **Invalid Email**:
   - Enter invalid email format
   - **Expected**: Validation error message

2. **Wrong Password**:
   - Enter correct email, wrong password
   - **Expected**: "Invalid email or password" error

3. **Non-existent User**:
   - Enter email that doesn't exist
   - **Expected**: "Invalid email or password" error

4. **Duplicate Email**:
   - Try to register with existing email
   - **Expected**: "Email already registered" error

## Testing Social Login (After OAuth Setup)

### Prerequisites
1. Configure OAuth credentials (see `SOCIAL_LOGIN_SETUP.md`)
2. Add to `web/.env.local`:
   ```
   NEXT_PUBLIC_GOOGLE_CLIENT_ID=your-client-id
   NEXT_PUBLIC_FACEBOOK_APP_ID=your-app-id
   ```
3. Add to `backend/src/Services/TheDish.User.API/appsettings.json`:
   ```json
   {
     "OAuth": {
       "Google": {
         "ClientId": "your-client-id"
       },
       "Facebook": {
         "AppId": "your-app-id",
         "AppSecret": "your-app-secret"
       }
     }
   }
   ```
4. Restart all services

### Test Google Login
1. Navigate to `/register` or `/login`
2. Click "Sign in with Google"
3. Select Google account
4. **Expected**: Redirect to homepage, logged in

### Test Facebook Login
1. Navigate to `/register` or `/login`
2. Click "Continue with Facebook"
3. Authorize the app
4. **Expected**: Redirect to homepage, logged in

### Test Account Linking
1. Register with email: `test@example.com`
2. Log out
3. Log in with Google using the same email
4. **Expected**: Accounts automatically linked, same user profile

## Troubleshooting

### "Service temporarily unavailable"
- Check if User Service is running on port 5001
- Check if API Gateway is running on port 5000
- Verify Docker containers are running (PostgreSQL, Redis, etc.)

### "Missing required parameter: client_id"
- Social login buttons are hidden if credentials not configured
- This is expected behavior - email/password auth works without OAuth
- To enable social login, configure OAuth credentials

### Registration/Login not working
- Check browser console for errors
- Check backend service logs
- Verify database connection
- Check API Gateway logs for routing issues

### CORS Errors
- Verify API Gateway CORS configuration
- Check that services are running on correct ports
- Ensure frontend is calling correct API base URL

