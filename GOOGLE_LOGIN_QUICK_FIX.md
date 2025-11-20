# Google Login Quick Fix - Visual Checklist

## 🎯 The Problem

Your Google OAuth app is in **"Testing" mode**, which means **only test users can sign in**. You need to either:
- ✅ Add yourself as a test user (recommended for development)
- OR
- ✅ Publish the app (for production)

---

## ⚡ Quick Fix (5 Minutes)

### 1️⃣ Add Test User (2 minutes)

1. Go to: https://console.cloud.google.com/apis/credentials/consent
2. Scroll to **"Test users"** section
3. Click **"+ Add users"**
4. Enter your Google email
5. Click **"Add"**

**✅ Done when:** You see your email in the test users list

---

### 2️⃣ Verify Origin (1 minute)

1. Go to: https://console.cloud.google.com/apis/credentials
2. Click your OAuth Client ID
3. Check **"Authorized JavaScript origins"** has: `http://localhost:3000`
4. Check **"Authorized redirect URIs"** has: `http://localhost:3000`
5. Click **"Save"** if you made changes

**✅ Done when:** Both fields show `http://localhost:3000`

---

### 3️⃣ Wait (5-10 minutes)

**Why:** Google needs time to propagate changes

**✅ Done when:** 5-10 minutes have passed

---

### 4️⃣ Restart Services (2 minutes)

**Backend:**
```powershell
cd backend
.\scripts\restart-user-service.ps1
```

**Frontend:**
```powershell
# Stop current npm run dev (Ctrl+C)
cd web
npm run dev
```

**✅ Done when:** Both services show "Ready" or "listening"

---

### 5️⃣ Test (1 minute)

1. Go to: http://localhost:3000/register
2. Click **"Sign in with Google"**
3. Select your Google account (must be the one you added as test user)
4. Should redirect and log you in! ✅

**✅ Done when:** You're logged in and on the homepage

---

## 🚨 Common Issues

| Error | Fix |
|-------|-----|
| "Origin not allowed" | Wait 5-10 more minutes, verify origin is saved |
| "Access blocked" | Add yourself as test user (Step 1) |
| 401 Unauthorized | Restart User Service (Step 4) |
| Button disabled | Check `.env.local` file, restart web app |

---

## 📋 Full Guide

For detailed instructions, see: **`FIX_GOOGLE_LOGIN_STEP_BY_STEP.md`**

---

## ✅ Success Checklist

- [ ] Added as test user in Google Console
- [ ] Origin verified in Google Console
- [ ] Waited 5-10 minutes
- [ ] User Service restarted
- [ ] Web app restarted
- [ ] Google login works! 🎉

---

**Total Time:** ~15-20 minutes (mostly waiting for Google)

