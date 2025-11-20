# Social Login Setup Guide

## Overview
This guide explains how to set up Google and Facebook OAuth for The Dish application.

## Prerequisites
- Google Cloud Console account
- Facebook Developer account

## Google OAuth Setup

### 1. Create Google OAuth Credentials
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Navigate to "APIs & Services" > "Credentials"
4. Click "Create Credentials" > "OAuth client ID"
5. Configure OAuth consent screen if prompted
6. Select "Web application" as the application type
7. Add authorized JavaScript origins:
   - `http://localhost:3000` (for development)
   - Your production domain (for production)
8. Add authorized redirect URIs:
   - `http://localhost:3000` (for development)
   - Your production domain (for production)
9. Copy the Client ID

### 2. Configure Frontend
Add to `web/.env.local`:
```
NEXT_PUBLIC_GOOGLE_CLIENT_ID=your-google-client-id-here
```

### 3. Configure Backend
Add to `backend/src/Services/TheDish.User.API/appsettings.json` or `appsettings.Development.json`:
```json
{
  "OAuth": {
    "Google": {
      "ClientId": "your-google-client-id-here"
    }
  }
}
```

## Facebook OAuth Setup

### 1. Create Facebook App
1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Click "My Apps" > "Create App"
3. Select "Consumer" as the app type
4. Fill in app details and create the app
5. Go to "Settings" > "Basic"
6. Copy the App ID and App Secret
7. Add "Facebook Login" product
8. Configure Facebook Login settings:
   - Valid OAuth Redirect URIs: `http://localhost:3000` (and production domain)
   - Deauthorize Callback URL: Your production domain

### 2. Configure Frontend
Add to `web/.env.local`:
```
NEXT_PUBLIC_FACEBOOK_APP_ID=your-facebook-app-id-here
```

### 3. Configure Backend
Add to `backend/src/Services/TheDish.User.API/appsettings.json` or `appsettings.Development.json`:
```json
{
  "OAuth": {
    "Facebook": {
      "AppId": "your-facebook-app-id-here",
      "AppSecret": "your-facebook-app-secret-here"
    }
  }
}
```

## Testing

### Test Google Login
1. Start the web application: `npm run dev`
2. Navigate to `/login` or `/register`
3. Click "Continue with Google"
4. Sign in with a Google account
5. Verify you're redirected to the homepage and logged in

### Test Facebook Login
1. Start the web application: `npm run dev`
2. Navigate to `/login` or `/register`
3. Click "Continue with Facebook"
4. Sign in with a Facebook account
5. Verify you're redirected to the homepage and logged in

### Test Account Linking
1. Register with email/password
2. Log out
3. Log in with Google/Facebook using the same email
4. Verify accounts are automatically linked

## Troubleshooting

### Google Login Issues
- **"Invalid client"**: Check that Client ID is correct in both frontend and backend
- **"Redirect URI mismatch"**: Ensure redirect URIs match exactly in Google Cloud Console
- **"Token validation failed"**: Verify backend has correct Client ID configured

### Facebook Login Issues
- **"App not setup"**: Ensure Facebook Login product is added to your app
- **"Invalid App ID"**: Check App ID in frontend .env.local
- **"Invalid App Secret"**: Verify App Secret in backend appsettings.json
- **SDK not loading**: Check browser console for script loading errors

## Security Notes
- Never commit `.env.local` or `appsettings.json` with real credentials to version control
- Use environment variables or secure vaults in production
- Rotate OAuth credentials regularly
- Use HTTPS in production

