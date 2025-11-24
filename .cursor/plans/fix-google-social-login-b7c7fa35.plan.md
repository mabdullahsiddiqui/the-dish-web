<!-- b7c7fa35-0c62-4780-a5e5-562334d71c8e e5451375-f1d7-4f20-bea4-eb10b6537d98 -->
# Fix Google Social Login - Detailed Implementation Plan

## Problem Analysis

The Google login button is disabled, which indicates the `NEXT_PUBLIC_GOOGLE_CLIENT_ID` environment variable is missing or not configured. This plan covers the complete setup from Google Cloud Console to local configuration.

## Implementation Steps

### Step 1: Verify Current Configuration

**Files to check:**

- `web/.env.local` - Check if file exists and contains `NEXT_PUBLIC_GOOGLE_CLIENT_ID`
- `backend/src/Services/TheDish.User.API/appsettings.json` - Check if `OAuth:Google:ClientId` is configured

**Actions:**

1. Check if `web/.env.local` exists
2. If exists, verify `NEXT_PUBLIC_GOOGLE_CLIENT_ID` is present and not empty
3. Check backend `appsettings.json` for Google Client ID configuration
4. Document current state

### Step 2: Google Cloud Console Setup

**If credentials don't exist or need to be recreated:**

1. **Access Google Cloud Console:**

   - Go to https://console.cloud.google.com/
   - Select or create a project

2. **Configure OAuth Consent Screen:**

   - Navigate to "APIs & Services" > "OAuth consent screen"
   - Select "External" user type (for development)
   - Fill required fields:
     - App name: "The Dish"
     - User support email: Your email
     - Developer contact: Your email
   - Add scopes: `email`, `profile`, `openid`
   - Add test users (your Google account email) if app is in "Testing" mode
   - Save and continue

3. **Create OAuth 2.0 Client ID:**

   - Navigate to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "OAuth client ID"
   - Application type: "Web application"
   - Name: "The Dish Web Client"
   - **Authorized JavaScript origins:**
     - `http://localhost:3000`
     - Add production domain if needed
   - **Authorized redirect URIs:**
     - `http://localhost:3000`
     - Add production domain if needed
   - Click "Create"
   - **Copy the Client ID** (format: `xxxxx-xxxxx.apps.googleusercontent.com`)

### Step 3: Configure Frontend Environment

**File:** `web/.env.local`

**Actions:**

1. Create `web/.env.local` if it doesn't exist
2. Add or update the following variables:
   ```
   NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1
   NEXT_PUBLE_GOOGLE_CLIENT_ID=your-google-client-id-here
   ```

3. Replace `your-google-client-id-here` with the actual Client ID from Step 2
4. Ensure no trailing spaces or quotes around the Client ID
5. Verify file is in `.gitignore` (should not be committed)

**Note:** The `.env.local` file is already in `.gitignore` based on standard Next.js setup.

### Step 4: Configure Backend

**File:** `backend/src/Services/TheDish.User.API/appsettings.json`

**Actions:**

1. Open `appsettings.json`
2. Locate the `OAuth` section (lines 17-25)
3. Update or verify the `Google:ClientId` value matches the frontend Client ID:
   ```json
   "OAuth": {
     "Google": {
       "ClientId": "your-google-client-id-here"
     }
   }
   ```

4. Ensure the Client ID exactly matches the one in `web/.env.local`
5. Save the file

**Important:** Both frontend and backend MUST use the same Client ID for token validation to work.

### Step 5: Verify Google Cloud Console Settings

**Critical settings to verify:**

1. **Authorized JavaScript Origins:**

   - Must include: `http://localhost:3000` (exact match, no trailing slash)
   - Check for typos or extra spaces

2. **Authorized Redirect URIs:**

   - Must include: `http://localhost:3000`
   - Not required for ID token flow, but good to have

3. **OAuth Consent Screen:**

   - If app is in "Testing" mode, ensure your Google account is added as a test user
   - Test users section: "APIs & Services" > "OAuth consent screen" > "Test users"
   - Click "+ Add users" and add your email

4. **Wait for Propagation:**

   - Changes in Google Cloud Console can take 5-10 minutes to propagate
   - Don't test immediately after making changes

### Step 6: Restart Services

**Services to restart:**

1. **User Service (Backend):**

   - Stop the current User Service process (Ctrl+C in PowerShell)
   - Restart using:
     ```powershell
     cd backend\src\Services\TheDish.User.API
     dotnet run --urls http://localhost:5001
     ```

   - Or use: `backend\scripts\restart-user-service.ps1`
   - Verify it's running: http://localhost:5001/swagger

2. **Web Application (Frontend):**

   - Stop the current Next.js dev server (Ctrl+C)
   - Restart using:
     ```powershell
     cd web
     npm run dev
     ```

   - Wait for "Ready" message
   - Verify: http://localhost:3000

3. **API Gateway (if running separately):**

   - Restart if needed to ensure routes are loaded
   - Verify: http://localhost:5000/swagger

### Step 7: Test Google Login Flow

**Testing steps:**

1. **Navigate to registration page:**

   - Go to: http://localhost:3000/register
   - Verify Google login button is now enabled (not grayed out)

2. **Click "Sign in with Google":**

   - Google popup should open
   - If "origin not allowed" error appears:
     - Wait 5-10 more minutes for Google changes to propagate
     - Double-check authorized origins in Google Console
   - If popup opens successfully:
     - Select your Google account (must be test user if app is in Testing mode)
     - Click "Continue" or "Allow"

3. **Expected result:**

   - Popup closes
   - Redirected to homepage
   - User is logged in
   - Check browser console for errors
   - Check User Service logs for any backend errors

### Step 8: Troubleshooting Common Issues

**If button is still disabled:**

- Verify `.env.local` file exists in `web/` directory
- Check `NEXT_PUBLIC_GOOGLE_CLIENT_ID` has no extra spaces or quotes
- Restart web app after creating/updating `.env.local`
- Clear browser cache or use incognito mode

**If "origin not allowed" error:**

- Verify `http://localhost:3000` is exactly in authorized origins (no trailing slash)
- Wait 5-10 minutes after adding origin
- Try incognito mode to avoid cached errors

**If 401 Unauthorized:**

- Check User Service logs for specific error message
- Verify Client ID matches exactly in both frontend and backend
- Ensure User Service is running and accessible
- Check API Gateway is routing correctly (see `ocelot.json`)

**If token validation fails:**

- Check User Service PowerShell window for detailed error
- Verify Google Client ID in `appsettings.json` matches the one used to generate token
- Ensure token is a valid Google ID token (not access token)

### Step 9: Verify Complete Flow

**End-to-end verification:**

1. Frontend button is enabled
2. Google popup opens without errors
3. User can select Google account
4. Backend receives and validates token
5. User account is created/linked
6. JWT token is returned
7. Frontend stores token and redirects
8. User is authenticated and logged in

## Files to Modify

1. **Create/Update:** `web/.env.local`

   - Add `NEXT_PUBLIC_GOOGLE_CLIENT_ID`

2. **Update:** `backend/src/Services/TheDish.User.API/appsettings.json`

   - Verify/update `OAuth:Google:ClientId`

## Files Already Configured (No Changes Needed)

- `web/app/layout.tsx` - GoogleOAuthProvider wrapper is correctly implemented
- `web/components/features/auth/SocialLoginButtons.tsx` - Google login component is ready
- `web/lib/api/auth.ts` - Google login API call is implemented
- `web/hooks/useAuth.ts` - Google login hook is implemented
- `backend/src/Services/TheDish.User.Infrastructure/Services/OAuthService.cs` - Token validation is implemented
- `backend/src/Services/TheDish.User.Application/Commands/SocialLoginCommandHandler.cs` - Handler is implemented
- `backend/src/TheDish.ApiGateway/ocelot.json` - Route is configured correctly (public, no auth required)

## Success Criteria

- [ ] `web/.env.local` exists with `NEXT_PUBLIC_GOOGLE_CLIENT_ID` configured
- [ ] `backend/src/Services/TheDish.User.API/appsettings.json` has matching Client ID
- [ ] Google Cloud Console has correct origins and redirect URIs configured
- [ ] Test user is added if app is in Testing mode
- [ ] All services restarted after configuration changes
- [ ] Google login button is enabled (not grayed out)
- [ ] Google popup opens without origin errors
- [ ] Login completes successfully and user is authenticated
- [ ] No errors in browser console or backend logs

## Additional Notes

- The Client ID is public and safe to use in frontend code
- Never commit `.env.local` or `appsettings.json` with real credentials to Git
- For production, add production domain to Google Console authorized origins
- Consider publishing the OAuth app if you want anyone to sign in (not just test users)