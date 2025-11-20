# Social Login - Ready to Test! 🚀

## ✅ What's Complete

1. **Backend Configuration**:
   - ✅ Google Client ID added to `appsettings.json`
   - ✅ User Service created with social login support
   - ✅ API endpoints configured (`/api/v1/users/auth/google`, `/api/v1/users/auth/facebook`)
   - ✅ API Gateway routes configured

2. **Frontend Configuration**:
   - ✅ Google Client ID added to `.env.local`
   - ✅ Social login buttons component created
   - ✅ Hydration error fixed
   - ✅ Google OAuth provider configured in layout

3. **Implementation**:
   - ✅ Account linking logic (same email = same user)
   - ✅ Error handling
   - ✅ UI components integrated

## 🔄 Final Steps to Activate

### 1. Restart User Service (IMPORTANT!)

The User Service needs to be restarted to pick up the new Google Client ID from `appsettings.json`:

```powershell
cd backend
.\scripts\restart-user-service.ps1
```

Or manually:
- Kill process on port 5001
- Start: `cd backend\src\Services\TheDish.User.API; dotnet run --urls http://localhost:5001`

### 2. Restart Web App

Restart the Next.js app to pick up the new `.env.local`:

```powershell
cd web
# Stop current npm run dev (Ctrl+C)
npm run dev
```

### 3. Test Google Login

1. Go to http://localhost:3000/register
2. Click "Sign in with Google" button
3. Select your Google account
4. Should redirect to homepage and log you in! ✅

## 🐛 Troubleshooting

### "Unable to access this resource" Error

This happens when:
- User Service hasn't been restarted (doesn't have Google Client ID)
- Google token validation fails
- Network/CORS issues

**Fix**: Restart User Service and try again.

### Google Button Not Showing

- Check `.env.local` has `NEXT_PUBLIC_GOOGLE_CLIENT_ID`
- Restart web app after adding env variable
- Check browser console for errors

### "Invalid client" Error

- Verify Client ID matches exactly in both frontend and backend
- Check Google Console that redirect URI is `http://localhost:3000`
- Restart both services after configuration

## 📝 Current Status

- ✅ Email/password registration: **Working**
- ✅ Email/password login: **Working**
- ⏳ Google login: **Ready - needs service restart**
- ⏳ Facebook login: **Not configured (optional)**

## 🎯 Next Steps After Testing

Once Google login works:
1. Test account linking (register with email, then login with Google using same email)
2. Optionally configure Facebook login (follow same process)
3. Test error scenarios (invalid tokens, network errors)

