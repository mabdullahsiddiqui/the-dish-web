# API Gateway Routes Configuration

## ✅ Current Configuration is CORRECT

The social login endpoints (`/api/v1/users/auth/google` and `/api/v1/users/auth/facebook`) are **correctly configured as PUBLIC** (no authentication required).

### Route Order in Ocelot

Ocelot matches routes **in order**, from top to bottom. More specific routes must come **before** catch-all routes.

### Current Route Order:

1. **`/api/v1/users/register`** - PUBLIC (no `AuthenticationOptions`)
2. **`/api/v1/users/login`** - PUBLIC (no `AuthenticationOptions`)
3. **`/api/v1/users/auth/google`** - PUBLIC (no `AuthenticationOptions`) ✅
4. **`/api/v1/users/auth/facebook`** - PUBLIC (no `AuthenticationOptions`) ✅
5. **`/api/v1/users/{everything}`** - PROTECTED (has `AuthenticationOptions`)

### Why This Works:

- When a request comes to `/api/v1/users/auth/google`, Ocelot checks routes in order
- It matches route #3 first (specific route)
- Since route #3 has no `AuthenticationOptions`, it's public
- The request is forwarded to the User Service **without requiring authentication**

### The 401 Error Explained:

The 401 error you're seeing is **NOT from the API Gateway blocking the request**. It's from the **User Service** returning 401 because:

1. ✅ The request successfully passes through the API Gateway (no auth required)
2. ✅ The request reaches the User Service
3. ❌ The User Service tries to validate the Google token
4. ❌ Token validation fails (likely due to Client ID mismatch or token format issue)
5. ❌ User Service returns 401 with error message

### To Verify API Gateway is Working:

Check the API Gateway logs. If you see the request being forwarded to the User Service, then the API Gateway is working correctly.

### Next Steps:

1. **Restart API Gateway** (to ensure config is loaded):
   ```powershell
   # Find and restart the API Gateway process
   Get-Process | Where-Object {$_.ProcessName -like "*ApiGateway*"} | Stop-Process -Force
   cd backend\src\TheDish.ApiGateway
   dotnet run --urls http://localhost:5000
   ```

2. **Check User Service logs** for the actual token validation error

3. **Verify Google Client ID** matches exactly in both frontend and backend

## Summary

- ✅ API Gateway routes are correctly configured as PUBLIC
- ✅ Social login endpoints don't require authentication
- ❌ The 401 is from User Service token validation failing
- 🔍 Need to check User Service logs for specific error

