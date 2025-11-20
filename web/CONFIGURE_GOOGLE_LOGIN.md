# Configure Google Login - Quick Steps

## ✅ Backend Configuration - DONE
I've already added your Google Client ID to:
- `backend/src/Services/TheDish.User.API/appsettings.json`
- `backend/src/Services/TheDish.User.API/appsettings.Development.json`

## 📝 Frontend Configuration - DO THIS NOW

**Create `web/.env.local` file** with this content:

```
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1
NEXT_PUBLIC_GOOGLE_CLIENT_ID=628105888738-tene6lgmejcfc8elmfgsqd947mv8qp0q.apps.googleusercontent.com
```

## 🔄 Restart Services

1. **Restart User Service** (to pick up new config):
   ```powershell
   cd backend
   .\scripts\restart-user-service.ps1
   ```
   Or manually:
   - Kill existing process on port 5001
   - Start: `cd backend\src\Services\TheDish.User.API; dotnet run --urls http://localhost:5001`

2. **Restart Web App** (to pick up .env.local):
   - Stop current `npm run dev` (Ctrl+C)
   - Start again: `npm run dev`

## ✅ Test Google Login

1. Go to http://localhost:3000/register or http://localhost:3000/login
2. You should see "Sign in with Google" button (enabled, not grayed out)
3. Click it
4. Select your Google account
5. Should redirect to homepage and log you in! 🎉

## 🐛 Troubleshooting

- **Button still disabled**: Make sure `.env.local` file exists and has correct variable name
- **"Invalid client" error**: Check Client ID matches exactly (no extra spaces)
- **"Redirect URI mismatch"**: Verify in Google Console that `http://localhost:3000` is in authorized redirect URIs
- **Backend error**: Check User Service logs, verify it restarted after config change

