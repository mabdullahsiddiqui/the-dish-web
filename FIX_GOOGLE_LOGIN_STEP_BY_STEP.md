# Fix Google Login - Step-by-Step Guide

## 🔴 Current Issues

1. **Google OAuth app is in "Testing" mode** - Only test users can sign in
2. **Google origin error** - Origin may not be configured correctly
3. **401 Unauthorized** - Backend error (likely due to invalid token from origin issue)

## ✅ Solution: Step-by-Step

### Step 1: Add Test Users (Quick Fix - Recommended for Development)

**Why:** Your Google OAuth app is in "Testing" mode, which means only test users can sign in.

**Steps:**

1. **Go to Google Cloud Console:**
   - Open: https://console.cloud.google.com/
   - Select your project (AskAI)

2. **Navigate to OAuth Consent Screen:**
   - Click the hamburger menu (☰) in the top left
   - Go to **"APIs & Services"** > **"OAuth consent screen"**
   - OR go directly to: https://console.cloud.google.com/apis/credentials/consent

3. **Add Test Users:**
   - Scroll down to the **"Test users"** section
   - Click **"+ Add users"** button
   - Enter your Google account email address (the one you want to use for testing)
   - Click **"Add"**
   - Repeat for any other test accounts

4. **Verify:**
   - You should see your email in the "Test users" list
   - Status should show "0 users (1 test, 0 other) / 100 user cap"

**Alternative: Publish the App (For Production)**

If you want anyone to be able to sign in (not just test users):

1. In the same "OAuth consent screen" page
2. Scroll to the top
3. Click **"Publish app"** button
4. Confirm the action
5. **Note:** For production use, you may need to complete app verification later

---

### Step 2: Verify Origin Configuration

**Why:** The origin error means `http://localhost:3000` might not be properly configured.

**Steps:**

1. **Go to Credentials:**
   - In Google Cloud Console, go to **"APIs & Services"** > **"Credentials"**
   - OR: https://console.cloud.google.com/apis/credentials

2. **Open Your OAuth Client ID:**
   - Find your OAuth 2.0 Client ID (the one ending in `.apps.googleusercontent.com`)
   - Click on it to edit

3. **Check "Authorized JavaScript origins":**
   - Look for the section **"Authorized JavaScript origins"**
   - Verify `http://localhost:3000` is listed (exact match, no trailing slash)
   - If missing, click **"+ Add URI"** and add: `http://localhost:3000`
   - Click **"Save"**

4. **Check "Authorized redirect URIs":**
   - Look for the section **"Authorized redirect URIs"**
   - Verify `http://localhost:3000` is listed
   - If missing, click **"+ Add URI"** and add: `http://localhost:3000`
   - Click **"Save"**

5. **Wait for Propagation:**
   - Changes can take **5-10 minutes** to propagate
   - Don't test immediately after saving

---

### Step 3: Restart Backend Services

**Why:** The User Service needs to be restarted to load the LINQ fix we applied.

**Steps:**

1. **Restart User Service:**
   ```powershell
   cd backend
   .\scripts\restart-user-service.ps1
   ```

   **Or manually:**
   - Find the PowerShell window running User Service
   - Press `Ctrl+C` to stop it
   - Run:
     ```powershell
     cd backend\src\Services\TheDish.User.API
     dotnet run --urls http://localhost:5001
     ```

2. **Verify User Service is Running:**
   - Check: http://localhost:5001/swagger
   - Should show Swagger UI

---

### Step 4: Restart Web App

**Why:** The web app needs to reload environment variables and clear any cached errors.

**Steps:**

1. **Stop Current Web App:**
   - Find the terminal/command prompt running `npm run dev`
   - Press `Ctrl+C` to stop it

2. **Start Web App Again:**
   ```powershell
   cd web
   npm run dev
   ```

3. **Wait for "Ready" Message:**
   - You should see: `✓ Ready in X.Xs`
   - The app should be available at http://localhost:3000

---

### Step 5: Test Google Login

**Steps:**

1. **Open Registration Page:**
   - Go to: http://localhost:3000/register
   - You should see the "Sign in with Google" button (enabled, not grayed out)

2. **Click "Sign in with Google":**
   - A Google popup should open
   - **If you see "origin not allowed" error:**
     - Wait 5-10 more minutes for Google changes to propagate
     - Double-check the origin is saved correctly
   - **If popup opens successfully:**
     - Select your Google account (must be one you added as test user)
     - Click "Continue" or "Allow"

3. **Expected Result:**
   - Popup closes
   - You're redirected to homepage
   - You're logged in! ✅
   - Check browser console - should see no errors

4. **If Still Getting Errors:**
   - Check browser console for specific error messages
   - Check User Service PowerShell window for backend errors
   - Verify you're using the Google account you added as test user

---

## 🔍 Troubleshooting

### Error: "The given origin is not allowed"
- **Fix:** Wait 5-10 minutes after saving origin in Google Console
- **Verify:** Origin is exactly `http://localhost:3000` (no trailing slash, no path)

### Error: "Access blocked: This app's request is invalid"
- **Fix:** You're not added as a test user, or app needs to be published
- **Action:** Add yourself as test user (Step 1)

### Error: 401 Unauthorized
- **Fix:** Check User Service logs for specific error
- **Action:** Restart User Service (Step 3)

### Button Still Disabled
- **Fix:** Check `.env.local` file exists with `NEXT_PUBLIC_GOOGLE_CLIENT_ID`
- **Action:** Restart web app (Step 4)

---

## ✅ Success Checklist

After completing all steps, you should have:

- [ ] Added yourself as test user in Google Cloud Console
- [ ] Verified `http://localhost:3000` in authorized origins
- [ ] Verified `http://localhost:3000` in authorized redirect URIs
- [ ] Waited 5-10 minutes for Google changes to propagate
- [ ] Restarted User Service
- [ ] Restarted web app
- [ ] Google login button is enabled
- [ ] Google popup opens without origin error
- [ ] Login succeeds and redirects to homepage

---

## 📝 Quick Reference

**Google Cloud Console Links:**
- OAuth Consent Screen: https://console.cloud.google.com/apis/credentials/consent
- Credentials: https://console.cloud.google.com/apis/credentials

**Service URLs:**
- Web App: http://localhost:3000
- User Service Swagger: http://localhost:5001/swagger
- API Gateway Swagger: http://localhost:5000/swagger

**Configuration Files:**
- Frontend: `web/.env.local`
- Backend: `backend/src/Services/TheDish.User.API/appsettings.json`

---

## 🎯 Expected Timeline

- **Step 1 (Add Test Users):** 2 minutes
- **Step 2 (Verify Origin):** 2 minutes
- **Step 3 (Restart Backend):** 1 minute
- **Step 4 (Restart Web App):** 1 minute
- **Wait for Google Propagation:** 5-10 minutes
- **Step 5 (Test):** 2 minutes

**Total:** ~15-20 minutes (mostly waiting for Google to propagate changes)

---

## 💡 Pro Tips

1. **Use Incognito/Private Mode:** Sometimes browser cache can cause issues. Test in incognito mode.

2. **Check Multiple Browsers:** If one browser doesn't work, try another.

3. **Clear Browser Cache:** If origin errors persist, clear browser cache and cookies.

4. **Check Network Tab:** In DevTools, check the Network tab to see the exact request/response.

5. **Check User Service Logs:** The PowerShell window running User Service will show detailed error messages.

---

## 🆘 Still Not Working?

If you've completed all steps and it's still not working:

1. **Share the exact error message** from:
   - Browser console
   - User Service PowerShell window
   - Network tab (request/response)

2. **Verify Configuration:**
   - Client ID matches in both frontend and backend
   - Origin is exactly `http://localhost:3000`
   - You're using the Google account you added as test user

3. **Check Service Status:**
   - All backend services are running
   - Web app is running
   - No port conflicts

Good luck! 🚀

