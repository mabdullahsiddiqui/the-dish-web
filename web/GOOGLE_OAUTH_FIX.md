# Fix Google OAuth Errors

## Issues Found

1. ✅ **Fixed**: GoogleLogin `width="100%"` prop error - removed invalid prop
2. ⚠️ **Action Required**: Google OAuth origin not allowed
3. ⚠️ **Action Required**: 401 Unauthorized from backend

## Fix 1: Google OAuth Origin (Required)

The error `The given origin is not allowed for the given client ID` means you need to add `http://localhost:3000` to your Google OAuth authorized origins.

### Steps:

1. **Go to Google Cloud Console**:
   - https://console.cloud.google.com/apis/credentials

2. **Find your OAuth 2.0 Client ID**:
   - Click on the client ID you're using (the one in your `.env.local`)

3. **Add Authorized JavaScript origins**:
   - Under "Authorized JavaScript origins", click "ADD URI"
   - Add: `http://localhost:3000`
   - Click "SAVE"

4. **Add Authorized redirect URIs** (if needed):
   - Under "Authorized redirect URIs", add:
     - `http://localhost:3000`
     - `http://localhost:3000/api/auth/callback/google` (if using NextAuth)

5. **Wait 5-10 minutes** for changes to propagate

6. **Restart your web app**:
   ```powershell
   # Stop current npm run dev (Ctrl+C)
   cd web
   npm run dev
   ```

## Fix 2: Verify Backend Configuration

The 401 error might be because:

1. **User Service is not running** - Check if port 5001 is active
2. **Google Client ID mismatch** - Backend and frontend must use the same Client ID
3. **Token validation failing** - Check User Service logs

### Check User Service:

```powershell
# Check if User Service is running
Test-NetConnection -ComputerName localhost -Port 5001

# Check User Service logs in the PowerShell window
# Look for errors when Google login is attempted
```

### Verify Configuration:

1. **Frontend** (`.env.local`):
   ```
   NEXT_PUBLIC_GOOGLE_CLIENT_ID=your-client-id-here
   ```

2. **Backend** (`appsettings.json`):
   ```json
   "OAuth": {
     "Google": {
       "ClientId": "your-client-id-here"
     }
   }
   ```

   **Important**: Both must use the **same** Client ID!

## Fix 3: Test After Fixes

After fixing the origin issue:

1. **Clear browser cache** or use incognito mode
2. **Go to**: http://localhost:3000/register
3. **Click**: "Sign in with Google"
4. **Expected**: Should open Google login popup without origin error
5. **After login**: Should redirect and authenticate successfully

## Debugging

If still getting 401:

1. **Check browser Network tab**:
   - Look at the request to `/api/v1/users/auth/google`
   - Check the request payload
   - Check the response body for error details

2. **Check User Service logs**:
   - Look for validation errors
   - Check if token is being received
   - Check if Google token validation is failing

3. **Test directly with User Service**:
   ```powershell
   # Test the endpoint directly
   curl -X POST http://localhost:5001/api/v1/users/auth/google `
     -H "Content-Type: application/json" `
     -d '{\"provider\":\"Google\",\"token\":\"test-token\"}'
   ```

## Common Issues

- **Origin error**: Add `http://localhost:3000` to Google Console
- **401 Unauthorized**: Check User Service is running and Client ID matches
- **Token validation fails**: Check backend logs for specific error
- **CORS error**: API Gateway should handle this, but check if User Service CORS is configured

