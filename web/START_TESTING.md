# 🧪 Start Testing - Quick Reference

## ✅ Testing Tools Created

I've created comprehensive testing resources for you:

1. **TESTING_CHECKLIST.md** - Detailed checklist with all test scenarios
2. **TESTING_GUIDE.md** - Step-by-step testing guide
3. **INTEGRATION_TESTING.md** - Detailed test scenarios (already existed)
4. **scripts/check-backend.ps1** - Verify backend services
5. **scripts/quick-test.ps1** - Quick connectivity test

## 🚀 Quick Start Testing

### ⚠️ Prerequisites Check

**Before starting, make sure Docker is installed:**

```powershell
docker --version
```

If you see "command not found", you need to install Docker Desktop first.

**See**: `TESTING_SETUP_GUIDE.md` for Docker installation instructions.

### Step 1: Start Backend Services

**If Docker is installed:**

```powershell
# Navigate to backend
cd ..\backend

# Start Docker containers (if not running)
docker compose up -d

# Start all microservices
.\scripts\start-services.ps1
```

**Expected**: 4 PowerShell windows will open, one for each service.

**If Docker is NOT installed:**

See `TESTING_WITHOUT_DOCKER.md` for alternatives, or install Docker Desktop first.

### Step 2: Verify Backend

```powershell
# From web directory
cd web
.\scripts\check-backend.ps1
```

**Expected**: All 4 services show as "running" (green).

### Step 3: Start Web App

```powershell
# From web directory (if not already running)
npm run dev
```

**Expected**: Application starts at `http://localhost:3000`

### Step 4: Open Browser & Test

1. Open `http://localhost:3000`
2. Check backend health widget (bottom-right) - should show all green
3. Open browser console (F12) - should have no errors
4. Start testing!

## 📋 Testing Priority

### High Priority (Test First)
1. ✅ **Backend Connectivity** - Health widget shows all services
2. ✅ **User Registration** - Create a test account
3. ✅ **User Login** - Login with test account
4. ✅ **Search Functionality** - Search for restaurants
5. ✅ **Place Details** - View a restaurant page
6. ✅ **Review Submission** - Write a review with GPS verification

### Medium Priority
7. ✅ **Review Management** - Edit/delete reviews
8. ✅ **Review Voting** - Mark reviews helpful
9. ✅ **Photo Upload** - Upload photos with reviews
10. ✅ **Map Integration** - View maps and get directions

### Low Priority (Polish)
11. ✅ **Responsive Design** - Test on mobile/tablet
12. ✅ **Error Handling** - Test error scenarios
13. ✅ **Performance** - Check load times

## 🎯 Quick Test Flow

Follow this flow for a quick validation:

1. **Register** → `/register` → Create account
2. **Search** → Homepage → Search "pizza"
3. **View Place** → Click first result
4. **Write Review** → Click "Write a Review" → Submit with GPS
5. **View Profile** → `/profile` → See your review
6. **Edit Review** → Click "Edit" → Update and save
7. **Vote** → Go back to place → Vote on another review

If all these work, **core functionality is validated!** ✅

## 📝 Document Your Findings

As you test, keep notes:

### Bugs
- What happened?
- How to reproduce?
- Expected vs Actual?
- Severity (High/Medium/Low)?

### Working Features
- What works perfectly?
- Any pleasant surprises?

### Recommendations
- UX improvements?
- Performance optimizations?
- Feature enhancements?

## 🐛 Common Issues

### Backend Services Not Running
- **Check**: `.\scripts\check-backend.ps1`
- **Fix**: Start services with `.\scripts\start-services.ps1`

### "Failed to fetch" Errors
- **Check**: API Gateway at `http://localhost:5000/swagger`
- **Fix**: Verify services are running, check `.env.local`

### Maps Not Loading
- **Check**: Browser console for errors
- **Fix**: Verify Leaflet CSS loaded, check coordinates

### GPS Not Working
- **Check**: Browser location permission
- **Fix**: Allow location access, note: requires HTTPS in production

## 📚 Testing Resources

- **Quick Start**: This file (`START_TESTING.md`)
- **Step-by-Step Guide**: `TESTING_GUIDE.md`
- **Detailed Checklist**: `TESTING_CHECKLIST.md`
- **Test Scenarios**: `INTEGRATION_TESTING.md`

## 🎉 Ready to Test!

Everything is set up. Start with:

1. ✅ Start backend services
2. ✅ Verify connectivity
3. ✅ Start web app
4. ✅ Open browser
5. ✅ Follow testing checklist

**Good luck with testing!** If you find any issues, document them and we can fix them together.

---

**Next**: Open `TESTING_CHECKLIST.md` and start checking off items! ✅

