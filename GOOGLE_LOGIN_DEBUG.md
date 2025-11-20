# Google Login Debugging Guide

## Current Status

✅ **Fixed:**
- Google OAuth origin configured in Google Cloud Console (`http://localhost:3000`)
- Client IDs match between frontend and backend
- Frontend is sending valid Google ID tokens
- Backend is receiving requests

❌ **Issue:**
- Backend returns 401 with "An error occurred during social login"
- Token validation is likely failing silently

## Next Steps to Debug

### 1. Check User Service Logs

The User Service PowerShell window should show detailed error messages after the improved logging is applied.

**To see the actual error:**

1. **Restart User Service** to load the improved error logging:
   ```powershell
   cd backend
   .\scripts\restart-user-service.ps1
   ```

2. **Try Google login again** from the web app

3. **Check the User Service PowerShell window** for error messages like:
   - "Google token validation failed: [specific error]"
   - "Error validating Google token: [exception type] - [message]"

### 2. Common Token Validation Issues

**Issue: Client ID Mismatch**
- **Symptom**: Token validation fails with "Invalid token" or "Invalid audience"
- **Fix**: Ensure the Client ID in `appsettings.json` exactly matches the one used to generate the token
- **Check**: Both frontend `.env.local` and backend `appsettings.json` use the same Client ID

**Issue: Token Expired**
- **Symptom**: Token validation fails immediately
- **Fix**: Google ID tokens expire quickly. The frontend should generate a fresh token on each login attempt

**Issue: Token Format**
- **Symptom**: Validation fails with parsing errors
- **Fix**: Ensure the frontend is sending the ID token (not access token) from Google

### 3. Verify Token Payload

The Google ID token should contain:
- `aud` (audience) = Your Client ID
- `iss` (issuer) = `https://accounts.google.com`
- `exp` (expiration) = Future timestamp
- `email` = User's email
- `sub` (subject) = Google user ID

### 4. Test Token Validation Directly

You can test the token validation by checking the User Service logs when a login attempt is made. The improved error logging will show:
- The exact exception type
- The error message
- The stack trace (in logs)

## Quick Fix Checklist

- [ ] Restart User Service to load improved error logging
- [ ] Try Google login again
- [ ] Check User Service logs for specific error message
- [ ] Verify Client ID matches exactly in both frontend and backend
- [ ] Ensure Google origin is saved in Google Cloud Console (wait 5-10 minutes if just added)
- [ ] Check that the token is a valid Google ID token (not access token)

## Expected Behavior After Fix

1. User clicks "Sign in with Google"
2. Google popup opens (no origin error)
3. User selects account
4. Frontend receives ID token
5. Frontend sends token to backend `/api/v1/users/auth/google`
6. Backend validates token with Google
7. Backend creates/links user account
8. Backend returns JWT token
9. Frontend stores token and redirects to homepage
10. User is logged in ✅

## If Still Not Working

After checking the User Service logs, share the specific error message and we can fix it!

