# Quick Setup Guide for Social Login

## ✅ What's Already Done

- ✅ Backend User Service with social login support
- ✅ Frontend social login buttons (visible but disabled)
- ✅ API endpoints configured
- ✅ Account linking logic implemented

## 🚀 Quick Start - Get OAuth Credentials

### Option 1: Google Only (Easiest)

1. **Get Google Client ID** (5 minutes):
   - Go to https://console.cloud.google.com/
   - Create a new project (or use existing)
   - Go to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "OAuth client ID"
   - Application type: "Web application"
   - Authorized JavaScript origins: `http://localhost:3000`
   - Authorized redirect URIs: `http://localhost:3000`
   - Copy the **Client ID** (looks like: `123456789-abc.apps.googleusercontent.com`)

2. **Configure Frontend**:
   - Create `web/.env.local` file:
     ```
     NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1
     NEXT_PUBLIC_GOOGLE_CLIENT_ID=your-client-id-here
     ```

3. **Configure Backend**:
   - Edit `backend/src/Services/TheDish.User.API/appsettings.json`:
     ```json
     {
       "OAuth": {
         "Google": {
           "ClientId": "your-client-id-here"
         }
       }
     }
     ```

4. **Restart Services**:
   - Restart User Service (port 5001)
   - Restart web app (`npm run dev`)

5. **Test**:
   - Go to http://localhost:3000/register
   - Click "Sign in with Google"
   - Should work! ✅

### Option 2: Facebook Only

1. **Get Facebook App ID** (10 minutes):
   - Go to https://developers.facebook.com/
   - Click "My Apps" > "Create App"
   - Select "Consumer" app type
   - Go to "Settings" > "Basic"
   - Copy **App ID** and **App Secret**

2. **Add Facebook Login Product**:
   - In your app, click "Add Product"
   - Add "Facebook Login"
   - Settings > Facebook Login > Settings:
     - Valid OAuth Redirect URIs: `http://localhost:3000`

3. **Configure Frontend**:
   - Add to `web/.env.local`:
     ```
     NEXT_PUBLIC_FACEBOOK_APP_ID=your-app-id-here
     ```

4. **Configure Backend**:
   - Edit `backend/src/Services/TheDish.User.API/appsettings.json`:
     ```json
     {
       "OAuth": {
         "Facebook": {
           "AppId": "your-app-id-here",
           "AppSecret": "your-app-secret-here"
         }
       }
     }
     ```

5. **Restart and Test** (same as Google)

### Option 3: Both Google & Facebook

Follow both Option 1 and Option 2, configure all credentials.

## 📝 Example Configuration Files

### `web/.env.local`
```
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1
NEXT_PUBLIC_GOOGLE_CLIENT_ID=123456789-abc.apps.googleusercontent.com
NEXT_PUBLIC_FACEBOOK_APP_ID=987654321
```

### `backend/src/Services/TheDish.User.API/appsettings.json`
```json
{
  "OAuth": {
    "Google": {
      "ClientId": "123456789-abc.apps.googleusercontent.com"
    },
    "Facebook": {
      "AppId": "987654321",
      "AppSecret": "your-secret-here"
    }
  }
}
```

## ⚠️ Important Notes

1. **Never commit `.env.local` or `appsettings.json` with real credentials to Git**
2. **Restart services after changing credentials**
3. **Google Client ID is public** (safe in frontend)
4. **Facebook App Secret is private** (only in backend)

## 🧪 Testing

After configuration:
1. Social login buttons should be **enabled** (not grayed out)
2. Clicking Google/Facebook should open OAuth popup
3. After login, you should be redirected to homepage
4. Check that user is logged in

## 🐛 Troubleshooting

- **Buttons still disabled**: Check `.env.local` file exists and has correct variable names
- **"Invalid client" error**: Verify Client ID matches exactly (no extra spaces)
- **"Redirect URI mismatch"**: Check OAuth console settings match `http://localhost:3000` exactly
- **Backend error**: Check User Service logs, verify `appsettings.json` has correct credentials

## 📚 Detailed Guide

For more detailed instructions, see `web/SOCIAL_LOGIN_SETUP.md`

