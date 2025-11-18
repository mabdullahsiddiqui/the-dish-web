# Testing Guide - Quick Start

This guide will help you systematically test The Dish web application.

## Step 1: Pre-Testing Setup

### 1.1 Start Backend Services

```powershell
# Navigate to backend directory
cd ..\backend

# Start Docker containers
docker compose up -d

# Start all microservices
.\scripts\start-services.ps1
```

This will open 4 PowerShell windows, one for each service:
- API Gateway (port 5000)
- User Service (port 5001)
- Place Service (port 5002)
- Review Service (port 5003)

### 1.2 Verify Backend Services

```powershell
# From web directory
cd web
.\scripts\check-backend.ps1
```

All services should show as "running" (green checkmarks).

### 1.3 Start Web Application

```powershell
# From web directory
npm run dev
```

The application will start at `http://localhost:3000`

### 1.4 Open Browser

1. Open Chrome/Firefox/Edge
2. Navigate to `http://localhost:3000`
3. Open Developer Tools (F12)
4. Check Console tab for errors
5. Look for backend health widget (bottom-right corner)

## Step 2: Quick Smoke Test

Before detailed testing, verify basic functionality:

1. **Homepage loads** - Should see hero section and featured places
2. **Backend health widget** - Should show all services healthy (green)
3. **No console errors** - Check browser console (F12)
4. **Search works** - Type "pizza" and search, should redirect to results page

If these work, proceed to detailed testing.

## Step 3: Detailed Testing

Follow the comprehensive checklist in `TESTING_CHECKLIST.md`. This includes:

- Authentication (register, login, logout)
- Search & Discovery
- Review System
- Review Management
- Review Voting
- Map Integration
- User Profile
- Error Handling
- Responsive Design
- Performance
- Browser Compatibility
- Accessibility

## Step 4: Document Findings

As you test, document:

### Bugs Found
For each bug, note:
- **Description**: What's wrong?
- **Steps to Reproduce**: How to see the bug
- **Expected**: What should happen
- **Actual**: What actually happens
- **Severity**: High/Medium/Low
- **Browser/OS**: Where it occurs

### Features Working
- List features that work perfectly
- Note any pleasant surprises

### Recommendations
- UX improvements
- Performance optimizations
- Feature enhancements

## Step 5: Common Issues & Solutions

### Issue: Backend services not running
**Solution**: 
1. Check Docker is running: `docker ps`
2. Start services: `cd ..\backend && .\scripts\start-services.ps1`
3. Wait 10-15 seconds for services to start

### Issue: "Failed to fetch" errors
**Solution**:
1. Verify API Gateway is running: `http://localhost:5000/swagger`
2. Check `.env.local` has correct `NEXT_PUBLIC_API_BASE_URL`
3. Check browser console for CORS errors

### Issue: Maps not loading
**Solution**:
1. Check browser console for errors
2. Verify Leaflet CSS is loaded (check Network tab)
3. Ensure place has valid coordinates

### Issue: GPS not working
**Solution**:
1. Allow browser location permission
2. Check browser console for permission errors
3. Note: Requires HTTPS in production

### Issue: Photos not uploading
**Solution**:
1. Check file size (max 10MB)
2. Check file type (JPEG, PNG, WebP only)
3. Verify backend photo upload endpoint

## Step 6: Test Results Summary

After completing testing, create a summary:

```markdown
# Test Results Summary

**Date**: [Date]
**Tester**: [Your Name]
**Browser**: [Chrome/Firefox/Edge]
**OS**: [Windows/Mac/Linux]

## Tests Completed
- [ ] Authentication
- [ ] Search & Discovery
- [ ] Review System
- [ ] Review Management
- [ ] Review Voting
- [ ] Map Integration
- [ ] User Profile
- [ ] Error Handling
- [ ] Responsive Design
- [ ] Performance
- [ ] Browser Compatibility

## Bugs Found
1. [Bug description]
2. [Bug description]

## Features Working
- [List working features]

## Recommendations
- [List recommendations]
```

## Quick Test Commands

```powershell
# Check backend services
.\scripts\check-backend.ps1

# Quick test (checks connectivity)
.\scripts\quick-test.ps1

# Start web app
npm run dev

# Build for production (test build)
npm run build
```

## Next Steps After Testing

1. **Fix Critical Bugs** - Address high-severity issues first
2. **Document Issues** - Create GitHub issues or tickets
3. **Performance Optimization** - Address any performance issues found
4. **UX Improvements** - Implement recommended enhancements
5. **Production Preparation** - Prepare for deployment

## Need Help?

- Check `INTEGRATION_TESTING.md` for detailed test scenarios
- Check `TESTING_CHECKLIST.md` for comprehensive checklist
- Review browser console for error messages
- Check backend logs in service windows
- Verify API endpoints in Swagger: `http://localhost:5000/swagger`

---

**Happy Testing!** 🧪

