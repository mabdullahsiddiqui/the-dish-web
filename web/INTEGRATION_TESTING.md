# Integration Testing Guide

This guide helps you test the web application with the backend services to ensure everything works end-to-end.

## Prerequisites

1. **Backend Services Running**
   ```bash
   cd ../backend
   docker compose up -d
   .\scripts\start-services.ps1
   ```

2. **Web Application Running**
   ```bash
   npm run dev
   ```

3. **Verify Connectivity**
   ```bash
   .\scripts\check-backend.ps1
   ```

## Test Scenarios

### 1. User Authentication

#### Registration
1. Navigate to `/register`
2. Fill in registration form:
   - First Name: Test
   - Last Name: User
   - Email: test@example.com
   - Password: Test123!@#
   - Confirm Password: Test123!@#
3. Submit form
4. **Expected**: Success toast, redirect to homepage, user logged in

#### Login
1. Navigate to `/login`
2. Enter credentials:
   - Email: test@example.com
   - Password: Test123!@#
3. Submit form
4. **Expected**: Success toast, redirect to homepage, user logged in

#### Logout
1. Click user menu in header
2. Click "Logout"
3. **Expected**: Success toast, redirect to homepage, user logged out

### 2. Restaurant Discovery

#### Search
1. Navigate to homepage
2. Enter search query: "pizza"
3. Click search or press Enter
4. **Expected**: Redirect to `/search?q=pizza` with results

#### Filters
1. On search page, open filters
2. Select cuisine type: "Italian"
3. Select dietary tag: "Vegetarian"
4. Set min rating: 4
5. Apply filters
6. **Expected**: Results filtered accordingly

#### Place Details
1. Click on any restaurant from search results
2. **Expected**: 
   - Place details page loads
   - Map displays with marker
   - Reviews section visible
   - Photos displayed (if available)

### 3. Review Submission

#### Create Review (GPS Verified)
1. Navigate to a place detail page
2. Click "Write a Review"
3. **Expected**: Redirect to review page (login if not authenticated)

4. On review page:
   - Click "Verify My Location"
   - Allow location access
   - **Expected**: Location retrieved, distance calculated, verification status shown

5. Fill review form:
   - Select rating: 5 stars
   - Review text: "Great food and service!"
   - Dietary accuracy: "Accurate"
   - Upload photos (optional): Select 1-2 images
   
6. Submit review
7. **Expected**: 
   - Success toast notification
   - Redirect to place detail page
   - New review appears in reviews list
   - Review marked as "GPS Verified" if within 200m

### 4. Review Management

#### View Reviews
1. Navigate to `/profile`
2. **Expected**: User's reviews displayed with pagination

#### Edit Review
1. On profile page, find a review
2. Click "Edit" button
3. **Expected**: Redirect to edit page with form pre-filled

4. Modify review:
   - Change rating to 4 stars
   - Update review text
   - Submit
5. **Expected**: Success toast, redirect to profile, review updated

#### Delete Review
1. On profile page, find a review
2. Click "Delete" button
3. **Expected**: Confirmation dialog appears

4. Confirm deletion
5. **Expected**: Success toast, review removed from list

### 5. Review Voting

#### Mark Helpful
1. Navigate to a place detail page
2. Find a review (not your own)
3. Click "Helpful" button
4. **Expected**: 
   - Button highlights
   - Helpful count increases
   - Success toast (optional)

#### Mark Not Helpful
1. Click "Not Helpful" on same review
2. **Expected**: 
   - Vote changes
   - Helpful count decreases
   - Not helpful count increases

### 6. Map Integration

#### View Map
1. Navigate to any place detail page
2. Scroll to "Location" section
3. **Expected**: 
   - Interactive map displays
   - Place marker visible
   - Map is interactive (zoom, pan)

#### Get Directions
1. Click marker on map
2. Click "Directions" in popup
3. **Expected**: Opens Google Maps in new tab with directions

### 7. Photo Upload

#### Upload Photos
1. Navigate to review submission page
2. Scroll to "Add Photos" section
3. Drag and drop images or click to select
4. **Expected**: 
   - Images preview displayed
   - File names and sizes shown
   - Remove buttons available

5. Try uploading:
   - Valid images (JPEG, PNG, WebP)
   - Invalid file type (should show error)
   - File > 10MB (should show error)
   - More than 5 files (should show error)

### 8. Error Handling

#### Backend Unavailable
1. Stop backend services
2. Try to search or load data
3. **Expected**: 
   - Error toast notification
   - Error state displayed
   - Retry option available

#### Invalid Credentials
1. Try logging in with wrong password
2. **Expected**: Error toast with message

#### Network Error
1. Disconnect internet
2. Try any API operation
3. **Expected**: Network error message displayed

## Automated Testing Checklist

### Manual Test Checklist

- [ ] User registration works
- [ ] User login works
- [ ] User logout works
- [ ] Search functionality works
- [ ] Filters apply correctly
- [ ] Place details load correctly
- [ ] Maps display and are interactive
- [ ] Review submission works
- [ ] GPS verification works (within 200m)
- [ ] Photo upload works
- [ ] Review editing works
- [ ] Review deletion works
- [ ] Helpful voting works
- [ ] Error states display correctly
- [ ] Loading states display correctly
- [ ] Toast notifications appear
- [ ] Responsive design works on mobile
- [ ] Keyboard shortcuts work (Ctrl+K)

### Browser Compatibility

Test on:
- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Edge (latest)
- [ ] Mobile Safari (iOS)
- [ ] Chrome Mobile (Android)

### Performance Checks

- [ ] Page load < 2 seconds
- [ ] Images load progressively
- [ ] No console errors
- [ ] No memory leaks
- [ ] Smooth scrolling
- [ ] Fast search results

## Common Issues & Solutions

### Issue: "Failed to fetch" errors
**Solution**: Check backend services are running

### Issue: Maps not loading
**Solution**: Check browser console for CORS errors, verify Leaflet CSS imported

### Issue: GPS not working
**Solution**: Ensure HTTPS in production, check browser permissions

### Issue: Photos not uploading
**Solution**: Check file size/type, verify backend endpoint accepts multipart/form-data

### Issue: Reviews not appearing
**Solution**: Check review service is running, verify API response

## Next Steps

After completing integration testing:

1. Document any bugs found
2. Create issues for improvements
3. Update test documentation
4. Set up automated E2E tests (optional)
5. Performance testing with load tools
6. Security testing (authentication, authorization)

## Test Data

For testing, you can use:

- **Test User**: test@example.com / Test123!@#
- **Test Places**: Search for common terms like "pizza", "coffee", "sushi"
- **Test Reviews**: Create reviews with various ratings and content

---

**Note**: This is a manual testing guide. For automated E2E testing, consider using tools like Playwright, Cypress, or Selenium.
