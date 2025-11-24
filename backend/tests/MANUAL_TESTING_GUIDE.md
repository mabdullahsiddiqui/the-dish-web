# The Dish - Comprehensive Manual Testing Guide

**Version**: 1.0  
**Date**: November 24, 2025  
**Purpose**: Step-by-step manual testing instructions for The Dish application

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Test Environment Setup](#test-environment-setup)
3. [Test Data](#test-data)
4. [Authentication Testing](#authentication-testing)
5. [Search & Discovery Testing](#search--discovery-testing)
6. [Review System Testing](#review-system-testing)
7. [Review Management Testing](#review-management-testing)
8. [User Profile Testing](#user-profile-testing)
9. [Map Integration Testing](#map-integration-testing)
10. [Error Handling Testing](#error-handling-testing)
11. [Responsive Design Testing](#responsive-design-testing)
12. [Performance Testing](#performance-testing)
13. [Accessibility Testing](#accessibility-testing)
14. [Cross-Browser Testing](#cross-browser-testing)
15. [Browser Console & Network Analysis](#browser-console--network-analysis)
16. [Test Results Template](#test-results-template)

---

## Prerequisites

### Required Software
- ✅ Node.js 18+ installed
- ✅ Docker Desktop installed and running
- ✅ Modern web browser (Chrome, Firefox, Edge, or Safari)
- ✅ Code editor (optional, for viewing logs)

### Required Services
- ✅ Docker containers running (PostgreSQL, Redis, RabbitMQ, Elasticsearch)
- ✅ Backend microservices running:
  - API Gateway (port 5000)
  - User Service (port 5001)
  - Place Service (port 5002)
  - Review Service (port 5003)
- ✅ Web application running (port 3000)

### Verification Steps

1. **Check Docker Containers**:
   ```powershell
   docker ps
   ```
   **Expected**: Should show containers for postgres, redis, rabbitmq, elasticsearch

2. **Check Backend Services**:
   ```powershell
   cd backend
   .\scripts\check-backend-comprehensive.ps1
   ```
   **Expected**: All services should show as healthy

3. **Check Web Application**:
   - Navigate to `http://localhost:3000`
   - Check backend health widget (bottom-right corner)
   - **Expected**: All services should show green/healthy status

---

## Test Environment Setup

### Step 1: Start Docker Containers

```powershell
cd backend
docker compose up -d
```

**Wait for**: All containers to show "healthy" status (check with `docker ps`)

### Step 2: Start Backend Services

```powershell
cd backend
.\scripts\start-services.ps1
```

**Expected**: 4 PowerShell windows will open (one for each service)

**Verify**: 
- API Gateway: `http://localhost:5000/swagger` should load
- User Service: `http://localhost:5001/swagger` should load
- Place Service: `http://localhost:5002/swagger` should load
- Review Service: `http://localhost:5003/swagger` should load

### Step 3: Start Web Application

```powershell
cd web
npm run dev
```

**Expected**: Application starts at `http://localhost:3000`

### Step 4: Open Browser DevTools

1. Open browser (Chrome recommended)
2. Press `F12` to open DevTools
3. Navigate to:
   - **Console** tab (for JavaScript errors)
   - **Network** tab (for API requests)
   - **Application** tab (for localStorage inspection)

---

## Test Data

### Test User Accounts

Create these test accounts for testing:

#### Account 1: Standard User
- **First Name**: Test
- **Last Name**: User
- **Email**: testuser@example.com
- **Password**: Test123!@#

#### Account 2: Reviewer User
- **First Name**: Review
- **Last Name**: Tester
- **Email**: reviewer@example.com
- **Password**: Review123!@#

#### Account 3: Social Login Test
- Use Google account (if Google OAuth configured)
- Use Facebook account (if Facebook OAuth configured)

### Test Places

Use these search terms to find test places:
- "pizza"
- "italian"
- "coffee"
- "restaurant"

Or create new places via:
- Navigate to `/places/new`
- Fill in place details
- Submit

---

## Authentication Testing

### Test 1: User Registration

**Objective**: Verify new user can register successfully

**Steps**:
1. Navigate to `http://localhost:3000/register`
2. Fill in registration form:
   - First Name: `Test`
   - Last Name: `User`
   - Email: `testuser@example.com`
   - Password: `Test123!@#`
   - Confirm Password: `Test123!@#`
3. Click "Create Account" button
4. Observe browser console (F12 → Console tab)

**Expected Results**:
- ✅ Success toast notification appears: "Account created successfully!"
- ✅ Redirected to homepage (`/`)
- ✅ User is logged in (header shows user name)
- ✅ No JavaScript errors in console
- ✅ JWT token stored in localStorage (check Application tab → Local Storage)

**Check localStorage**:
```javascript
// In browser console:
localStorage.getItem('auth_token')  // Should return JWT token
localStorage.getItem('user')       // Should return user object
```

**Failure Cases to Test**:
- ❌ Invalid email format → Should show validation error
- ❌ Weak password → Should show password requirements
- ❌ Password mismatch → Should show "Passwords do not match"
- ❌ Duplicate email → Should show "Email already exists" error

---

### Test 2: User Login

**Objective**: Verify existing user can log in

**Steps**:
1. Navigate to `http://localhost:3000/login`
2. Enter credentials:
   - Email: `testuser@example.com`
   - Password: `Test123!@#`
3. Click "Sign In" button
4. Observe browser console

**Expected Results**:
- ✅ Success toast notification: "Welcome back!"
- ✅ Redirected to homepage
- ✅ User is logged in (header shows user name)
- ✅ JWT token stored in localStorage
- ✅ No console errors

**Failure Cases to Test**:
- ❌ Wrong password → Should show "Invalid email or password"
- ❌ Non-existent email → Should show "Invalid email or password"
- ❌ Empty fields → Should show validation errors

---

### Test 3: Social Login (Google OAuth)

**Objective**: Verify Google OAuth login works (if configured)

**Prerequisites**: 
- Google OAuth configured in `.env.local`
- Google Client ID set in backend `appsettings.json`

**Steps**:
1. Navigate to `http://localhost:3000/login`
2. Click "Sign in with Google" button
3. Select Google account in popup
4. Grant permissions
5. Observe redirect

**Expected Results**:
- ✅ Google OAuth popup appears
- ✅ After authentication, redirected to homepage
- ✅ User is logged in
- ✅ JWT token stored in localStorage
- ✅ User data synced with backend

**Failure Cases**:
- ❌ OAuth not configured → Button should be hidden
- ❌ User cancels OAuth → Should return to login page
- ❌ Invalid OAuth credentials → Should show error message

---

### Test 4: Logout

**Objective**: Verify user can log out successfully

**Steps**:
1. Ensure you're logged in
2. Click user menu in header (top-right)
3. Click "Logout" option
4. Observe behavior

**Expected Results**:
- ✅ Success toast: "Logged out successfully"
- ✅ Redirected to homepage
- ✅ User is logged out (header shows "Login" button)
- ✅ JWT token removed from localStorage
- ✅ User data cleared from localStorage

**Verify localStorage cleared**:
```javascript
// In browser console:
localStorage.getItem('auth_token')  // Should return null
localStorage.getItem('user')        // Should return null
```

---

### Test 5: Protected Route Access

**Objective**: Verify protected routes redirect to login when not authenticated

**Steps**:
1. Ensure you're logged out
2. Try to access protected routes:
   - Navigate to `http://localhost:3000/profile`
   - Navigate to `http://localhost:3000/places/new`
   - Navigate to `http://localhost:3000/places/[id]/review`

**Expected Results**:
- ✅ All protected routes redirect to `/login`
- ✅ After login, redirect back to originally requested page
- ✅ No console errors

---

## Search & Discovery Testing

### Test 6: Homepage Search

**Objective**: Verify search functionality from homepage

**Steps**:
1. Navigate to `http://localhost:3000/` (homepage)
2. Locate search bar (center of page)
3. Enter search term: `pizza`
4. Click search button or press `Enter`
5. Observe results

**Expected Results**:
- ✅ Redirected to `/search?q=pizza`
- ✅ Search results displayed
- ✅ URL contains search query
- ✅ Search term appears in search bar
- ✅ Results show place cards with:
  - Place name
  - Address
  - Rating (stars)
  - Review count
  - Cuisine types
  - Distance (if location available)

**Check Network Tab**:
- Open DevTools → Network tab
- Filter by "XHR" or "Fetch"
- Look for request to `/api/v1/places/search?query=pizza`
- **Expected**: Status 200, response contains places array

---

### Test 7: Search Filters

**Objective**: Verify search filters work correctly

**Steps**:
1. Navigate to search page (`/search?q=restaurant`)
2. Open filters panel (if available)
3. Apply filters:
   - Select cuisine type: `Italian`
   - Select dietary tag: `Vegetarian`
   - Set minimum rating: `4 stars`
   - Set price range: `$$` (if available)
4. Click "Apply Filters" or wait for auto-filter
5. Observe filtered results

**Expected Results**:
- ✅ Results update to match filters
- ✅ Filter state persists in URL
- ✅ Only places matching all filters shown
- ✅ Filter badges visible showing active filters
- ✅ "Clear Filters" button works

**Test Individual Filters**:
- Test each filter type separately
- Test multiple filters combined
- Test clearing filters

---

### Test 8: Empty Search Results

**Objective**: Verify empty state displays correctly

**Steps**:
1. Navigate to search page
2. Search for: `nonexistentrestaurant12345`
3. Observe empty state

**Expected Results**:
- ✅ Empty state message displayed: "No places found"
- ✅ "Clear Search" or "Try Different Search" button available
- ✅ Helpful message suggesting alternative searches
- ✅ No console errors

---

### Test 9: Place Detail Page

**Objective**: Verify place detail page displays all information

**Steps**:
1. Perform a search (e.g., "pizza")
2. Click on a place card from results
3. Navigate to place detail page (`/places/[id]`)
4. Scroll through entire page
5. Check all sections

**Expected Results**:
- ✅ Place name and address displayed
- ✅ Rating and review count visible
- ✅ Description/overview section present
- ✅ Contact information (phone, website) if available
- ✅ Map displays with place marker
- ✅ Reviews section visible with reviews listed
- ✅ "Write a Review" button visible (if logged in)
- ✅ Photos displayed (if available)
- ✅ Dietary tags displayed
- ✅ Cuisine types shown

**Check Map**:
- ✅ Map is interactive (can zoom, pan)
- ✅ Place marker visible on map
- ✅ Clicking marker shows popup with place info
- ✅ "Get Directions" button works (opens Google Maps)

---

### Test 10: Nearby Places

**Objective**: Verify nearby places feature works

**Steps**:
1. Allow browser location permission when prompted
2. Navigate to homepage or search page
3. Look for "Nearby Places" section or button
4. Click to view nearby places
5. Observe results

**Expected Results**:
- ✅ Browser prompts for location permission
- ✅ After allowing, nearby places displayed
- ✅ Places sorted by distance (closest first)
- ✅ Distance shown for each place
- ✅ Map shows nearby places (if available)

**Failure Cases**:
- ❌ Location permission denied → Should show message
- ❌ Location unavailable → Should show fallback message

---

## Review System Testing

### Test 11: Create Review (Not Logged In)

**Objective**: Verify unauthenticated users are redirected to login

**Steps**:
1. Ensure you're logged out
2. Navigate to a place detail page
3. Click "Write a Review" button
4. Observe behavior

**Expected Results**:
- ✅ Redirected to `/login` page
- ✅ After login, redirected back to review page
- ✅ No console errors

---

### Test 12: Create Review (Logged In)

**Objective**: Verify authenticated users can create reviews

**Steps**:
1. Log in with test account
2. Navigate to a place detail page
3. Click "Write a Review" button
4. Navigate to review form (`/places/[id]/review`)
5. Fill in review form:
   - Select rating: `5 stars`
   - Enter review text: `This is a great restaurant with excellent food and service. Highly recommended!`
   - (Optional) Select dietary accuracy
6. Click "Verify My Location" button
7. Allow location permission
8. Observe GPS verification
9. Click "Submit Review" button
10. Observe results

**Expected Results**:
- ✅ Review form loads correctly
- ✅ Rating selection works (stars highlight on hover/click)
- ✅ Text area accepts input
- ✅ Location verification works:
  - Browser prompts for location
  - Coordinates retrieved
  - Distance to place calculated
  - Verification status shown (within 200m = verified)
- ✅ Review submitted successfully
- ✅ Success toast: "Review submitted successfully!"
- ✅ Redirected to place detail page
- ✅ New review appears in reviews list
- ✅ Review shows "GPS Verified" badge (if within 200m)

**Check Network Tab**:
- Look for `POST /api/v1/reviews` request
- **Expected**: Status 201 Created
- Response contains review data

---

### Test 13: GPS Verification

**Objective**: Verify GPS verification works correctly

**Steps**:
1. Navigate to review form for a place
2. Click "Verify My Location" button
3. Allow location permission
4. Observe verification results

**Expected Results**:
- ✅ Browser prompts for location permission
- ✅ After allowing, coordinates retrieved
- ✅ Distance to place calculated and displayed
- ✅ Verification status shown:
  - ✅ "GPS Verified" badge if within 200m
  - ⚠️ "Not Verified" if > 200m
- ✅ Coordinates displayed (latitude, longitude)
- ✅ Distance displayed in meters

**Test Cases**:
- **Within 200m**: Should show "GPS Verified"
- **Beyond 200m**: Should show "Not Verified" but allow submission
- **Location denied**: Should show error message
- **Location unavailable**: Should show error message

---

### Test 14: Photo Upload

**Objective**: Verify photo upload functionality

**Steps**:
1. Navigate to review form
2. Scroll to "Add Photos" section
3. Click "Upload Photos" or drag & drop area
4. Select 1-2 valid images (JPEG/PNG/WebP, < 10MB each)
5. Observe preview
6. Try uploading invalid file types
7. Try uploading files > 10MB
8. Try uploading more than 5 files

**Expected Results**:
- ✅ Image previews displayed after selection
- ✅ File names and sizes shown
- ✅ Can remove images before submission
- ✅ Invalid file type → Error message: "Invalid file type. Please upload JPEG, PNG, or WebP images."
- ✅ File too large → Error message: "File size exceeds 10MB limit."
- ✅ Too many files → Error message: "Maximum 5 photos allowed."

**Valid File Types**:
- ✅ JPEG (.jpg, .jpeg)
- ✅ PNG (.png)
- ✅ WebP (.webp)

**Invalid File Types**:
- ❌ PDF (.pdf)
- ❌ Text (.txt)
- ❌ Video (.mp4)

---

### Test 15: Review Validation

**Objective**: Verify review form validation works

**Steps**:
1. Navigate to review form
2. Try submitting without filling required fields:
   - No rating selected
   - No review text
   - Review text < 10 characters
3. Observe validation errors

**Expected Results**:
- ✅ Rating required → Error: "Please select a rating"
- ✅ Review text required → Error: "Review must be at least 10 characters"
- ✅ Short text → Error: "Review must be at least 10 characters"
- ✅ Submit button disabled until form valid
- ✅ Errors clear when fields corrected

---

## Review Management Testing

### Test 16: View My Reviews

**Objective**: Verify user can view their own reviews

**Steps**:
1. Log in with test account
2. Navigate to `/profile`
3. Scroll to "My Reviews" section
4. Observe reviews list

**Expected Results**:
- ✅ All user's reviews displayed
- ✅ Each review shows:
  - Place name (clickable link to place)
  - Rating (stars)
  - Review text
  - Date submitted
  - GPS verification status
  - "Edit" button
  - "Delete" button
- ✅ Reviews sorted by date (newest first)
- ✅ Empty state if no reviews: "You haven't written any reviews yet"

---

### Test 17: Edit Review

**Objective**: Verify user can edit their own reviews

**Steps**:
1. Navigate to `/profile`
2. Find a review you created
3. Click "Edit" button
4. Navigate to edit page (`/reviews/[id]/edit`)
5. Observe form pre-filled with existing data
6. Make changes:
   - Change rating from 5 to 4 stars
   - Update review text
   - (Optional) Change dietary accuracy
7. Click "Save Changes" button
8. Observe results

**Expected Results**:
- ✅ Edit page loads correctly
- ✅ Form pre-filled with existing review data
- ✅ Can modify rating, text, dietary accuracy
- ✅ Changes saved successfully
- ✅ Success toast: "Review updated successfully!"
- ✅ Redirected back to profile page
- ✅ Updated review appears in list with new data

**Test Ownership**:
- Try editing another user's review (change URL manually)
- **Expected**: Should redirect to profile or show error

---

### Test 18: Delete Review

**Objective**: Verify user can delete their own reviews

**Steps**:
1. Navigate to `/profile`
2. Find a review you created
3. Click "Delete" button
4. Observe confirmation dialog
5. Click "Cancel"
6. Observe: Review remains
7. Click "Delete" again
8. Click "Delete" in confirmation dialog
9. Observe results

**Expected Results**:
- ✅ Confirmation dialog appears: "Are you sure you want to delete this review?"
- ✅ "Cancel" button closes dialog, review remains
- ✅ "Delete" button removes review
- ✅ Success toast: "Review deleted successfully!"
- ✅ Review removed from list immediately
- ✅ Place's review count updated (if viewing place page)

**Test Ownership**:
- Try deleting another user's review
- **Expected**: Should not be possible (button not visible or error shown)

---

## Review Voting Testing

### Test 19: Mark Review Helpful (Not Logged In)

**Objective**: Verify unauthenticated users see appropriate message

**Steps**:
1. Ensure you're logged out
2. Navigate to a place detail page
3. Find a review
4. Look for "Helpful" button
5. Observe behavior

**Expected Results**:
- ✅ "Sign in to vote" message shown
- ✅ Button may be disabled or show login prompt
- ✅ Clicking prompts login

---

### Test 20: Mark Review Helpful (Logged In)

**Objective**: Verify authenticated users can vote on reviews

**Steps**:
1. Log in with test account
2. Navigate to a place detail page
3. Find a review (not your own)
4. Click "Helpful" button (thumbs up)
5. Observe behavior
6. Click "Not Helpful" button (thumbs down)
7. Observe behavior
8. Click same button again (toggle off)
9. Observe behavior

**Expected Results**:
- ✅ "Helpful" button highlights when clicked
- ✅ Helpful count increases
- ✅ Checkmark appears next to button
- ✅ Clicking "Not Helpful" changes vote:
  - Helpful count decreases
  - Not helpful count increases
- ✅ Clicking same button again removes vote:
  - Counts update accordingly
  - Button returns to normal state

**Check Network Tab**:
- Look for `POST /api/v1/reviews/[id]/helpful` or similar
- **Expected**: Status 200 OK

**Test Cases**:
- Vote on your own review → Should not be allowed or show message
- Vote on multiple reviews → All votes should work independently

---

## User Profile Testing

### Test 21: Profile Page Access

**Objective**: Verify profile page access control

**Steps**:
1. Ensure you're logged out
2. Navigate to `http://localhost:3000/profile`
3. Observe behavior

**Expected Results**:
- ✅ Redirected to `/login` page
- ✅ After login, redirected back to profile

---

### Test 22: Profile Page (Logged In)

**Objective**: Verify profile page displays user information

**Steps**:
1. Log in with test account
2. Navigate to `/profile`
3. Scroll through entire page
4. Check all sections

**Expected Results**:
- ✅ User information displayed:
  - Name (First + Last)
  - Email address
  - Join date (if available)
  - Profile picture (if available)
- ✅ "My Reviews" section visible
- ✅ Reviews list displayed (if any)
- ✅ Empty state if no reviews
- ✅ Edit profile button (if available)

---

### Test 23: Profile with No Reviews

**Objective**: Verify empty state for new users

**Steps**:
1. Create new test account
2. Log in with new account
3. Navigate to `/profile`
4. Observe empty state

**Expected Results**:
- ✅ Empty state message: "You haven't written any reviews yet"
- ✅ "Discover Restaurants" or "Write Your First Review" button
- ✅ Helpful message encouraging user to write reviews

---

## Map Integration Testing

### Test 24: Map Display

**Objective**: Verify map displays correctly on place detail page

**Steps**:
1. Navigate to a place detail page
2. Scroll to "Location" section
3. Observe map
4. Interact with map:
   - Zoom in/out (mouse wheel or buttons)
   - Pan (click and drag)
   - Click on marker

**Expected Results**:
- ✅ Interactive map displays (Leaflet map)
- ✅ Place marker visible on map
- ✅ Map is interactive:
  - Can zoom in/out
  - Can pan around
  - Smooth animations
- ✅ Map loads tiles correctly
- ✅ No console errors related to map

**Check Console**:
- Look for Leaflet/map-related errors
- **Expected**: No errors

---

### Test 25: Map Marker Popup

**Objective**: Verify map marker popup displays place information

**Steps**:
1. Navigate to place detail page
2. Click on map marker
3. Observe popup

**Expected Results**:
- ✅ Popup appears when marker clicked
- ✅ Popup shows:
  - Place name
  - Address
  - Phone number (if available)
  - "Directions" button
- ✅ Popup can be closed (X button or click outside)

---

### Test 26: Get Directions

**Objective**: Verify directions functionality

**Steps**:
1. Navigate to place detail page
2. Click on map marker
3. Click "Directions" button in popup
4. Observe behavior

**Expected Results**:
- ✅ Google Maps opens in new tab
- ✅ Correct location shown in Google Maps
- ✅ Directions from current location (if available)

---

## Error Handling Testing

### Test 27: Network Error

**Objective**: Verify graceful handling of network errors

**Steps**:
1. Open browser DevTools → Network tab
2. Set network throttling to "Offline" (Chrome DevTools)
3. Try to:
   - Search for places
   - Load place details
   - Submit a review
4. Observe error handling
5. Set network back to "Online"
6. Try again

**Expected Results**:
- ✅ Error toast appears: "Network error. Please check your internet connection."
- ✅ Error state displayed (not blank page)
- ✅ Retry option available (if applicable)
- ✅ After network restored, operations work normally

---

### Test 28: Invalid API Response

**Objective**: Verify handling of invalid API responses

**Steps**:
1. Open browser DevTools → Console tab
2. Perform various operations:
   - Search
   - Load place details
   - Submit review
3. Check console for errors
4. Check Network tab for failed requests

**Expected Results**:
- ✅ No unhandled JavaScript errors
- ✅ User-friendly error messages displayed
- ✅ Failed requests show appropriate status codes:
  - 404 → "The requested resource was not found."
  - 400 → "Invalid request. Please check your input and try again."
  - 401 → "Your session has expired. Please log in again."
  - 500 → "Server error. Please try again later."

---

### Test 29: 401 Unauthorized Handling

**Objective**: Verify handling of expired/invalid tokens

**Steps**:
1. Log in with test account
2. Open browser DevTools → Application tab → Local Storage
3. Delete or modify `auth_token`
4. Try to access protected route or perform action
5. Observe behavior

**Expected Results**:
- ✅ Redirected to `/login` page
- ✅ Error toast: "Your session has expired. Please log in again."
- ✅ Token cleared from localStorage
- ✅ After login, can access protected routes again

---

## Responsive Design Testing

### Test 30: Mobile View (< 768px)

**Objective**: Verify application works on mobile devices

**Steps**:
1. Open browser DevTools
2. Toggle device toolbar (Ctrl+Shift+M or Cmd+Shift+M)
3. Select mobile device (e.g., iPhone 12, Pixel 5)
4. Test all major pages:
   - Homepage
   - Search results
   - Place details
   - Review form
   - Profile
5. Check layout and functionality

**Expected Results**:
- ✅ All pages are usable on mobile
- ✅ No horizontal scrolling
- ✅ Touch targets are adequate size (min 44x44px)
- ✅ Text is readable (not too small)
- ✅ Navigation menu works (hamburger menu if applicable)
- ✅ Forms are usable
- ✅ Buttons are easily tappable

**Test Specific Features**:
- ✅ Search bar works
- ✅ Place cards display correctly
- ✅ Map is interactive (touch gestures work)
- ✅ Review form is usable
- ✅ Photo upload works

---

### Test 31: Tablet View (768px - 1024px)

**Objective**: Verify application works on tablets

**Steps**:
1. Open browser DevTools
2. Set viewport to tablet size (e.g., iPad, 768px width)
3. Test key pages
4. Check layout

**Expected Results**:
- ✅ Layout adapts correctly for tablet
- ✅ Content is well-spaced
- ✅ Navigation works
- ✅ All features accessible

---

### Test 32: Desktop View (> 1024px)

**Objective**: Verify application works on desktop

**Steps**:
1. Open browser in full desktop view (> 1024px width)
2. Test all pages
3. Check layout

**Expected Results**:
- ✅ Full layout displayed
- ✅ Sidebars/panels visible (if applicable)
- ✅ Optimal use of screen space
- ✅ All features accessible

---

## Performance Testing

### Test 33: Page Load Times

**Objective**: Verify pages load quickly

**Steps**:
1. Open browser DevTools → Network tab
2. Clear browser cache (Ctrl+Shift+Delete)
3. Navigate to homepage
4. Check load times:
   - First Contentful Paint (FCP)
   - Time to Interactive (TTI)
   - Total load time
5. Repeat for other pages

**Expected Results**:
- ✅ First Contentful Paint < 2 seconds
- ✅ Page fully loaded < 3 seconds
- ✅ Time to Interactive < 4 seconds

**Check Performance Tab** (Chrome DevTools):
- Open Performance tab
- Record page load
- Check for:
  - Long tasks
  - Layout shifts
  - Unused JavaScript

---

### Test 34: Image Loading

**Objective**: Verify images load efficiently

**Steps**:
1. Navigate to place detail page with photos
2. Observe image loading
3. Check Network tab for image requests

**Expected Results**:
- ✅ Images load progressively
- ✅ No layout shift when images load
- ✅ Images are optimized (not too large)
- ✅ Lazy loading works (images load as you scroll)

---

### Test 35: Search Performance

**Objective**: Verify search is fast

**Steps**:
1. Open browser DevTools → Network tab
2. Perform a search
3. Check response time

**Expected Results**:
- ✅ Search results appear quickly (< 1 second)
- ✅ No noticeable delay
- ✅ Smooth user experience

---

## Accessibility Testing

### Test 36: Keyboard Navigation

**Objective**: Verify application is keyboard accessible

**Steps**:
1. Use only keyboard (no mouse)
2. Tab through all interactive elements:
   - Links
   - Buttons
   - Form inputs
   - Search bar
3. Check focus indicators
4. Test keyboard shortcuts:
   - `Ctrl+K` or `Cmd+K` for search (if implemented)
   - `Escape` to close modals
   - `Enter` to submit forms

**Expected Results**:
- ✅ All interactive elements reachable via Tab
- ✅ Focus visible on all elements (focus ring/outline)
- ✅ Logical tab order
- ✅ Can navigate entire application without mouse
- ✅ Keyboard shortcuts work

---

### Test 37: Screen Reader (if available)

**Objective**: Verify screen reader compatibility

**Steps**:
1. Enable screen reader (NVDA, JAWS, or VoiceOver)
2. Navigate through application
3. Check if content is readable

**Expected Results**:
- ✅ All content is readable
- ✅ Buttons have labels
- ✅ Form inputs have labels
- ✅ Images have alt text
- ✅ Headings are properly structured
- ✅ ARIA labels used where appropriate

---

### Test 38: ARIA Labels and Semantic HTML

**Objective**: Verify proper accessibility markup

**Steps**:
1. Open browser DevTools → Accessibility tab (Chrome)
2. Check for accessibility issues
3. Inspect HTML for semantic elements

**Expected Results**:
- ✅ No major accessibility issues
- ✅ Proper use of semantic HTML (`<nav>`, `<main>`, `<article>`, etc.)
- ✅ ARIA labels on interactive elements
- ✅ Form inputs have associated labels
- ✅ Buttons have descriptive text

---

## Cross-Browser Testing

### Test 39: Chrome

**Objective**: Verify application works in Chrome

**Steps**:
1. Open application in Google Chrome
2. Test all major features:
   - Authentication
   - Search
   - Reviews
   - Maps
3. Check for browser-specific issues

**Expected Results**:
- ✅ All features work correctly
- ✅ No browser-specific errors
- ✅ Consistent appearance

---

### Test 40: Firefox

**Objective**: Verify application works in Firefox

**Steps**:
1. Open application in Mozilla Firefox
2. Test all major features
3. Check for Firefox-specific issues

**Expected Results**:
- ✅ All features work correctly
- ✅ No browser-specific errors
- ✅ Consistent appearance

---

### Test 41: Edge

**Objective**: Verify application works in Microsoft Edge

**Steps**:
1. Open application in Microsoft Edge
2. Test all major features
3. Check for Edge-specific issues

**Expected Results**:
- ✅ All features work correctly
- ✅ No browser-specific errors
- ✅ Consistent appearance

---

### Test 42: Safari (if available)

**Objective**: Verify application works in Safari

**Steps**:
1. Open application in Safari (Mac)
2. Test all major features
3. Check for Safari-specific issues

**Expected Results**:
- ✅ All features work correctly
- ✅ No browser-specific errors
- ✅ Consistent appearance

---

## Browser Console & Network Analysis

### Test 43: Console Error Check

**Objective**: Verify no critical JavaScript errors

**Steps**:
1. Open browser DevTools → Console tab
2. Navigate through entire application:
   - Homepage
   - Search
   - Place details
   - Review form
   - Profile
3. Check for errors and warnings

**Expected Results**:
- ✅ No critical JavaScript errors (red errors)
- ✅ Warnings are acceptable (non-critical)
- ✅ No failed API calls in console
- ✅ No CORS errors

**Common Issues to Check**:
- ❌ Uncaught exceptions
- ❌ Failed fetch requests
- ❌ CORS errors
- ❌ Missing resources (404s)

---

### Test 44: Network Request Analysis

**Objective**: Verify API calls work correctly

**Steps**:
1. Open browser DevTools → Network tab
2. Filter by "XHR" or "Fetch"
3. Perform various operations:
   - Search
   - Load place details
   - Submit review
   - Vote on review
4. Check each request:
   - Status code
   - Response time
   - Request/response data

**Expected Results**:
- ✅ All API calls succeed (status 200, 201)
- ✅ Response times < 1 second (most requests)
- ✅ No failed requests (404, 500)
- ✅ No CORS errors
- ✅ Proper request headers (Authorization token included)

**Check Specific Requests**:
- `GET /api/v1/places/search` → Status 200
- `GET /api/v1/places/[id]` → Status 200
- `POST /api/v1/reviews` → Status 201
- `GET /api/v1/users/me` → Status 200 (with auth token)

---

### Test 45: JWT Token Storage

**Objective**: Verify JWT token is stored and used correctly

**Steps**:
1. Log in with test account
2. Open browser DevTools → Application tab → Local Storage
3. Check for `auth_token`
4. Copy token value
5. Check Network tab for API requests
6. Verify token is included in Authorization header

**Expected Results**:
- ✅ Token stored in localStorage after login
- ✅ Token key: `auth_token`
- ✅ Token included in API requests:
   - Header: `Authorization: Bearer [token]`
- ✅ Token persists across page refreshes
- ✅ Token cleared on logout

**Verify Token in Requests**:
```javascript
// In Network tab, check request headers:
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Test Results Template

Use this template to document your test results:

```markdown
# Manual Testing Results

**Date**: [Date]  
**Tester**: [Your Name]  
**Browser**: [Chrome/Firefox/Edge/Safari]  
**OS**: [Windows/Mac/Linux]  
**Version**: [Application Version]

## Test Summary

- **Total Tests**: [Number]
- **Passed**: [Number]
- **Failed**: [Number]
- **Skipped**: [Number]
- **Success Rate**: [Percentage]%

## Detailed Results

### Test 1: [Test Name]
- **Status**: ✅ Pass / ❌ Fail / ⚠️ Warning
- **Notes**: [Any observations]
- **Screenshots**: [If applicable]

### Test 2: [Test Name]
- **Status**: ✅ Pass / ❌ Fail / ⚠️ Warning
- **Notes**: [Any observations]

...

## Issues Found

### Issue 1: [Title]
- **Severity**: High/Medium/Low
- **Steps to Reproduce**:
  1. [Step 1]
  2. [Step 2]
- **Expected**: [Expected behavior]
- **Actual**: [Actual behavior]
- **Screenshots**: [If applicable]

### Issue 2: [Title]
...

## Recommendations

- [Recommendation 1]
- [Recommendation 2]

## Notes

[Any additional notes or observations]
```

---

## Quick Reference

### Test Priority

**High Priority** (Test First):
1. ✅ Backend Connectivity
2. ✅ User Registration
3. ✅ User Login
4. ✅ Search Functionality
5. ✅ Place Details
6. ✅ Review Submission

**Medium Priority**:
7. ✅ Review Management
8. ✅ Review Voting
9. ✅ Photo Upload
10. ✅ Map Integration

**Low Priority**:
11. ✅ Responsive Design
12. ✅ Error Handling
13. ✅ Performance
14. ✅ Accessibility
15. ✅ Cross-Browser

### Quick Test Flow

For a quick validation, follow this flow:

1. **Register** → `/register` → Create account
2. **Search** → Homepage → Search "pizza"
3. **View Place** → Click first result
4. **Write Review** → Click "Write a Review" → Submit with GPS
5. **View Profile** → `/profile` → See your review
6. **Edit Review** → Click "Edit" → Update and save
7. **Vote** → Go back to place → Vote on another review

If all these work, **core functionality is validated!** ✅

---

## Troubleshooting

### Common Issues

**Backend Services Not Running**:
- Check: `.\scripts\check-backend-comprehensive.ps1`
- Fix: Start services with `.\scripts\start-services.ps1`

**"Failed to fetch" Errors**:
- Check: API Gateway at `http://localhost:5000/swagger`
- Fix: Verify services are running, check `.env.local`

**Maps Not Loading**:
- Check: Browser console for errors
- Fix: Verify Leaflet CSS loaded, check coordinates

**GPS Not Working**:
- Check: Browser location permission
- Fix: Allow location access, note: requires HTTPS in production

**Token Issues**:
- Check: localStorage for `auth_token`
- Fix: Clear localStorage and log in again

---

**End of Manual Testing Guide**

*For automated test results, see `TEST_RESULTS.md`*  
*For testing checklist, see `web/TESTING_CHECKLIST.md`*

