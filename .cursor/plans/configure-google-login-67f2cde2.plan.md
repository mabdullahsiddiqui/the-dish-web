<!-- 67f2cde2-3b01-4b24-be7d-d3a3f3cbacbb f9d11772-515d-4113-ad09-0069714223dc -->
# Configure Google Login

## Problem

The Google login button shows "Continue with Google (Not Configured)" because the frontend is missing the `NEXT_PUBLIC_GOOGLE_CLIENT_ID` environment variable. The backend already has the Client ID configured in `appsettings.json`.

## Solution

Create `web/.env.local` file with the required environment variables to enable Google OAuth.

## Implementation Steps

1. **Create `web/.env.local` file** with:

- `NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1` (already used by the API client)
- `NEXT_PUBLIC_GOOGLE_CLIENT_ID=628105888738-tene6lgmejcfc8elmfgsqd947mv8qp0q.apps.googleusercontent.com` (matches backend config)

2. **User Action Required**: Restart the Next.js dev server after creating the file so it picks up the new environment variables.

## Files to Modify

- **Create**: `web/.env.local` - New environment configuration file

## Notes

- The Google Client ID (`628105888738-tene6lgmejcfc8elmfgsqd947mv8qp0q.apps.googleusercontent.com`) is already configured in the backend (`backend/src/Services/TheDish.User.API/appsettings.json`)
- The `SocialLoginButtons.tsx` component checks for `process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID` on line 27
- When the environment variable is set, the component will render the actual Google Login button instead of the disabled "Not Configured" button
- After creating the file, the user must restart `npm run dev` for Next.js to load the new environment variables