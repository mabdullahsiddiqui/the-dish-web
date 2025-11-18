# Testing Checklist - The Dish Web Application

Use this checklist to systematically test all features. Check off items as you complete them.

## Pre-Testing Setup

- [ ] Backend services are running (check with `.\scripts\check-backend.ps1`)
- [ ] Docker containers are running (`docker ps` should show containers)
- [ ] Web application is running (`npm run dev`)
- [ ] Browser console is open (F12) to catch errors
- [ ] Backend health widget shows all services healthy (bottom-right corner)

## 1. Authentication Testing

### Registration
- [ ] Navigate to `/register`
- [ ] Fill form with valid data
- [ ] Submit form
- [ ] **Expected**: Success toast appears
- [ ] **Expected**: Redirected to homepage
- [ ] **Expected**: User is logged in (header shows user name)
- [ ] **Expected**: No console errors

### Login
- [ ] Navigate to `/login`
- [ ] Enter valid credentials
- [ ] Submit form
- [ ] **Expected**: Success toast appears
- [ ] **Expected**: Redirected to homepage
- [ ] **Expected**: User is logged in
- [ ] **Expected**: No console errors

### Login with Invalid Credentials
- [ ] Enter wrong password
- [ ] Submit form
- [ ] **Expected**: Error toast appears
- [ ] **Expected**: Remains on login page
- [ ] **Expected**: User is NOT logged in

### Logout
- [ ] Click user menu in header
- [ ] Click "Logout"
- [ ] **Expected**: Success toast appears
- [ ] **Expected**: Redirected to homepage
- [ ] **Expected**: User is logged out (header shows "Login" button)

## 2. Search & Discovery Testing

### Homepage Search
- [ ] Navigate to homepage (`/`)
- [ ] Enter search term: "pizza"
- [ ] Click search button or press Enter
- [ ] **Expected**: Redirected to `/search?q=pizza`
- [ ] **Expected**: Search results displayed
- [ ] **Expected**: Search term appears in URL

### Search Results Display
- [ ] Verify results are displayed in grid/list
- [ ] Check that place cards show:
  - [ ] Place name
  - [ ] Address
  - [ ] Rating
  - [ ] Review count
  - [ ] Cuisine types
- [ ] **Expected**: No console errors

### Search Filters
- [ ] Open filters panel
- [ ] Select cuisine type: "Italian"
- [ ] Select dietary tag: "Vegetarian"
- [ ] Set minimum rating: 4
- [ ] Apply filters
- [ ] **Expected**: Results update to match filters
- [ ] **Expected**: Filter state persists

### Empty Search Results
- [ ] Search for: "nonexistentrestaurant12345"
- [ ] **Expected**: Empty state message displayed
- [ ] **Expected**: "Clear Search" button available

### Place Details Page
- [ ] Click on a restaurant from search results
- [ ] **Expected**: Navigate to `/places/[id]`
- [ ] **Expected**: Place details load:
  - [ ] Name and address
  - [ ] Rating and review count
  - [ ] Description
  - [ ] Contact information
  - [ ] Map displays
  - [ ] Reviews section visible
  - [ ] Photos displayed (if available)

## 3. Review System Testing

### Create Review (Not Logged In)
- [ ] Logout if logged in
- [ ] Navigate to a place detail page
- [ ] Click "Write a Review"
- [ ] **Expected**: Redirected to `/login`

### Create Review (Logged In)
- [ ] Login with test account
- [ ] Navigate to a place detail page
- [ ] Click "Write a Review"
- [ ] **Expected**: Navigate to `/places/[id]/review`

### GPS Verification
- [ ] On review page, click "Verify My Location"
- [ ] Allow browser location permission
- [ ] **Expected**: Location retrieved
- [ ] **Expected**: Distance to place calculated
- [ ] **Expected**: Verification status shown (within 200m = verified)
- [ ] **Expected**: Coordinates displayed

### Submit Review
- [ ] Select rating: 5 stars
- [ ] Enter review text (min 10 characters)
- [ ] Select dietary accuracy (optional)
- [ ] Submit review
- [ ] **Expected**: Success toast appears
- [ ] **Expected**: Redirected to place detail page
- [ ] **Expected**: New review appears in reviews list
- [ ] **Expected**: Review shows "GPS Verified" badge (if within 200m)

### Photo Upload
- [ ] On review page, scroll to "Add Photos"
- [ ] Upload 1-2 valid images (JPEG/PNG/WebP)
- [ ] **Expected**: Image previews displayed
- [ ] **Expected**: File names and sizes shown
- [ ] Try uploading invalid file type
- [ ] **Expected**: Error message displayed
- [ ] Try uploading file > 10MB
- [ ] **Expected**: Error message displayed
- [ ] Try uploading more than 5 files
- [ ] **Expected**: Error message displayed

## 4. Review Management Testing

### View My Reviews
- [ ] Navigate to `/profile`
- [ ] **Expected**: User's reviews displayed
- [ ] **Expected**: Reviews show:
  - [ ] Place name (clickable link)
  - [ ] Rating
  - [ ] Review text
  - [ ] Date
  - [ ] GPS verification status
  - [ ] Edit/Delete buttons

### Edit Review
- [ ] On profile page, click "Edit" on a review
- [ ] **Expected**: Navigate to `/reviews/[id]/edit`
- [ ] **Expected**: Form pre-filled with review data
- [ ] Modify rating to 4 stars
- [ ] Update review text
- [ ] Submit changes
- [ ] **Expected**: Success toast appears
- [ ] **Expected**: Redirected to profile
- [ ] **Expected**: Review updated in list

### Delete Review
- [ ] On profile page, click "Delete" on a review
- [ ] **Expected**: Confirmation dialog appears
- [ ] Click "Cancel"
- [ ] **Expected**: Dialog closes, review remains
- [ ] Click "Delete" again
- [ ] Click "Delete" in confirmation
- [ ] **Expected**: Success toast appears
- [ ] **Expected**: Review removed from list

## 5. Review Voting Testing

### Mark Review Helpful (Not Logged In)
- [ ] Logout if logged in
- [ ] Navigate to a place detail page
- [ ] Find a review
- [ ] **Expected**: "Sign in to vote" message shown

### Mark Review Helpful (Logged In)
- [ ] Login with test account
- [ ] Navigate to a place detail page
- [ ] Find a review (not your own)
- [ ] Click "Helpful" button
- [ ] **Expected**: Button highlights
- [ ] **Expected**: Helpful count increases
- [ ] **Expected**: Checkmark appears

### Change Vote
- [ ] Click "Not Helpful" on same review
- [ ] **Expected**: Vote changes
- [ ] **Expected**: Helpful count decreases
- [ ] **Expected**: Not helpful count increases

### Remove Vote
- [ ] Click same button again (toggle off)
- [ ] **Expected**: Vote removed
- [ ] **Expected**: Counts update accordingly

## 6. Map Integration Testing

### Map Display
- [ ] Navigate to a place detail page
- [ ] Scroll to "Location" section
- [ ] **Expected**: Interactive map displays
- [ ] **Expected**: Place marker visible on map
- [ ] **Expected**: Map is interactive (can zoom, pan)

### Map Marker
- [ ] Click on map marker
- [ ] **Expected**: Popup appears with:
  - [ ] Place name
  - [ ] Address
  - [ ] Phone (if available)
  - [ ] "Directions" button

### Get Directions
- [ ] Click "Directions" in map popup
- [ ] **Expected**: Google Maps opens in new tab
- [ ] **Expected**: Correct location shown

### Map Error Handling
- [ ] Check browser console for map errors
- [ ] **Expected**: No CORS errors
- [ ] **Expected**: No tile loading errors

## 7. User Profile Testing

### Profile Page Access
- [ ] Navigate to `/profile` (not logged in)
- [ ] **Expected**: Redirected to `/login`

### Profile Page (Logged In)
- [ ] Login with test account
- [ ] Navigate to `/profile`
- [ ] **Expected**: User information displayed:
  - [ ] Name
  - [ ] Email
  - [ ] Join date
- [ ] **Expected**: Reviews section visible

### Profile with No Reviews
- [ ] Create new test account
- [ ] Navigate to profile
- [ ] **Expected**: Empty state message
- [ ] **Expected**: "Discover Restaurants" button

## 8. Error Handling Testing

### Network Error
- [ ] Stop backend services
- [ ] Try to search or load data
- [ ] **Expected**: Error toast appears
- [ ] **Expected**: Error state displayed
- [ ] **Expected**: Retry option available

### Invalid API Response
- [ ] Check browser console
- [ ] **Expected**: No unhandled errors
- [ ] **Expected**: User-friendly error messages

### 401 Unauthorized
- [ ] Wait for token to expire (or manually clear token)
- [ ] Try to access protected route
- [ ] **Expected**: Redirected to login
- [ ] **Expected**: Error toast appears

## 9. Responsive Design Testing

### Mobile View (< 768px)
- [ ] Open browser DevTools
- [ ] Set to mobile viewport
- [ ] Test all pages:
  - [ ] Homepage
  - [ ] Search results
  - [ ] Place details
  - [ ] Review form
  - [ ] Profile
- [ ] **Expected**: All pages are usable
- [ ] **Expected**: No horizontal scrolling
- [ ] **Expected**: Touch targets are adequate size

### Tablet View (768px - 1024px)
- [ ] Set to tablet viewport
- [ ] Test key pages
- [ ] **Expected**: Layout adapts correctly

### Desktop View (> 1024px)
- [ ] Set to desktop viewport
- [ ] Test all pages
- [ ] **Expected**: Full layout displayed

## 10. Performance Testing

### Page Load Times
- [ ] Open browser DevTools → Network tab
- [ ] Navigate to homepage
- [ ] **Expected**: First Contentful Paint < 2s
- [ ] **Expected**: Page fully loaded < 3s

### Image Loading
- [ ] Navigate to place detail page
- [ ] **Expected**: Images load progressively
- [ ] **Expected**: No layout shift

### Search Performance
- [ ] Perform search
- [ ] **Expected**: Results appear quickly (< 1s)

## 11. Browser Compatibility

### Chrome
- [ ] Test all major features
- [ ] **Expected**: Everything works

### Firefox
- [ ] Test all major features
- [ ] **Expected**: Everything works

### Edge
- [ ] Test all major features
- [ ] **Expected**: Everything works

### Safari (if available)
- [ ] Test all major features
- [ ] **Expected**: Everything works

## 12. Accessibility Testing

### Keyboard Navigation
- [ ] Tab through all interactive elements
- [ ] **Expected**: Focus visible on all elements
- [ ] **Expected**: Can navigate without mouse

### Screen Reader (if available)
- [ ] Test with screen reader
- [ ] **Expected**: All content is readable
- [ ] **Expected**: Buttons have labels

### ARIA Labels
- [ ] Check browser DevTools → Accessibility
- [ ] **Expected**: No major issues

## Test Results Summary

### Issues Found
Document any bugs or issues:

1. **Issue**: [Description]
   - **Steps to Reproduce**: 
   - **Expected**: 
   - **Actual**: 
   - **Severity**: High/Medium/Low

2. **Issue**: [Description]
   - **Steps to Reproduce**: 
   - **Expected**: 
   - **Actual**: 
   - **Severity**: High/Medium/Low

### Features Working Correctly
- [ ] List features that work perfectly

### Recommendations
- [ ] List any UX improvements or enhancements

---

**Testing Date**: _______________
**Tester**: _______________
**Browser**: _______________
**OS**: _______________

