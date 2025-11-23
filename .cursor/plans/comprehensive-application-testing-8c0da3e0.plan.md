<!-- 8c0da3e0-f29c-472b-ba56-aa0ad6682b28 0654fa9a-be61-4b00-9ea8-867190c05177 -->
# Comprehensive Application Testing Plan

## Testing Scope

This plan covers automated testing that can be performed programmatically. Manual testing (visual inspection, complex user flows, performance judgment) will be handled by the user.

## Phase 1: Backend Unit Tests

**Location**: `backend/tests/`

**Actions**:

1. Run all unit tests: `dotnet test` from `backend/` directory
2. Verify test results and identify any failures
3. Check test coverage if available
4. Document test results

**Expected Test Projects**:

- TheDish.Place.Domain.Tests (8 tests)
- TheDish.Review.Domain.Tests (21 tests)
- TheDish.Place.Application.Tests (4 tests)
- TheDish.Review.Application.Tests (4 tests)
- TheDish.Place.Integration.Tests
- TheDish.Review.Integration.Tests
- TheDish.Place.API.Tests (3 tests)
- TheDish.Review.API.Tests

**Success Criteria**: All tests pass, no compilation errors

## Phase 2: Backend Service Health Checks

**Location**: Service endpoints

**Actions**:

1. Check Docker containers status
2. Verify service health endpoints:

- API Gateway: `http://localhost:5000/swagger`
- User Service: `http://localhost:5001/swagger`
- Place Service: `http://localhost:5002/swagger`
- Review Service: `http://localhost:5003/swagger`

3. Use `web/scripts/check-backend.ps1` script
4. Test database connectivity (PostgreSQL on port 5432)
5. Test Redis connectivity (port 6379)

**Success Criteria**: All services respond with HTTP 200, containers running

## Phase 3: API Endpoint Testing

**Base URL**: `http://localhost:5000/api/v1`

**Actions**:

1. **User Service Endpoints**:

- Test `POST /users/register` with valid data
- Test `POST /users/login` with valid credentials
- Test `POST /users/login` with invalid credentials (should fail)
- Test `GET /users/{id}` (if test user exists)
- Test `POST /users/forgot-password`
- Test `POST /users/auth/google` (if configured)

2. **Place Service Endpoints**:

- Test `GET /places/{id}` with valid place ID
- Test `GET /places/nearby` with coordinates
- Test `GET /places/search` with search term
- Test `GET /places/{id}` with invalid ID (should return 404)

3. **Review Service Endpoints**:

- Test `GET /reviews/{id}` with valid review ID
- Test `GET /reviews/place/{placeId}` with valid place ID
- Test `GET /reviews/user/{userId}` with valid user ID
- Test `GET /reviews/{id}` with invalid ID (should return 404)

**Success Criteria**: Endpoints return expected status codes and response formats

## Phase 4: Web Application Testing

**Base URL**: `http://localhost:3000`

**Actions**:

1. **Page Load Testing**:

- Navigate to homepage (`/`)
- Check page loads without errors
- Verify console has no critical errors
- Check backend health widget displays

2. **Navigation Testing**:

- Test route navigation:
- `/login`
- `/register`
- `/search?q=pizza`
- `/places` (if accessible)
- Verify pages load correctly
- Check for 404 errors on invalid routes

3. **Authentication UI Testing**:

- Navigate to `/login`
- Check form elements are present
- Navigate to `/register`
- Check form elements are present
- Test form validation (submit empty forms)
- Check error messages display

4. **Search Functionality**:

- Navigate to homepage
- Test search bar interaction
- Submit search query
- Verify redirect to search results page
- Check URL contains query parameter

5. **Place Detail Page**:

- Navigate to a place detail page (if place ID known)
- Check page loads
- Verify map component renders (check for Leaflet)
- Check reviews section exists
- Verify no console errors

6. **Review Form** (if accessible):

- Navigate to review submission page
- Check form elements present
- Test GPS verification button (if present)
- Check form validation

**Success Criteria**: Pages load, no critical JavaScript errors, UI elements present

## Phase 5: Browser Console & Network Analysis

**Actions**:

1. Open browser DevTools
2. Check Console tab for:

- JavaScript errors
- Warnings
- Failed API calls

3. Check Network tab for:

- Failed requests
- Slow requests (>1s)
- CORS errors
- 404/500 errors

4. Test API calls from browser console
5. Verify JWT token storage (localStorage/sessionStorage)

**Success Criteria**: No critical errors, API calls succeed, proper error handling

## Phase 6: Error Scenario Testing

**Actions**:

1. Test with backend services stopped:

- Stop one service
- Check error handling in web app
- Verify error messages display

2. Test invalid API responses:

- Check error handling for 404/500 responses

3. Test network errors:

- Simulate offline mode (if possible)
- Check error states

**Success Criteria**: Graceful error handling, user-friendly error messages

## Phase 7: Documentation & Reporting

**Actions**:

1. Create test results document
2. List all passing tests
3. Document any failures with:

- Test name
- Expected behavior
- Actual behavior
- Error messages
- Steps to reproduce

4. Provide recommendations for fixes

**Output**: `TEST_RESULTS.md` with comprehensive test results

## What User Will Test Manually

The following require manual testing:

- Visual design and layout
- Complex user flows (registration → search → review submission)
- GPS location permission flow
- Photo upload functionality
- Review editing/deletion workflows
- Responsive design on different screen sizes
- Performance judgment (load times, smoothness)
- Accessibility (screen readers, keyboard navigation)
- Cross-browser compatibility
- Mobile device testing

## Files to Reference

- `backend/tests/TESTING_SUMMARY.md` - Backend test status
- `web/TESTING_CHECKLIST.md` - Manual testing checklist
- `web/START_TESTING.md` - Testing quick start
- `web/scripts/check-backend.ps1` - Backend health check script
- `docker-compose.yml` - Service configuration

## Execution Order

1. Backend unit tests (fastest, no dependencies)
2. Service health checks (requires Docker)
3. API endpoint testing (requires services running)
4. Web application testing (requires web app running)
5. Browser analysis (requires web app in browser)
6. Error scenario testing (requires controlled environment)
7. Documentation (final step)

### To-dos

- [ ] Run all backend unit tests using `dotnet test` and document results
- [ ] Verify Docker containers are running and check service health endpoints
- [ ] Test all API endpoints (User, Place, Review services) via HTTP requests
- [ ] Navigate and test all web application pages using browser automation
- [ ] Check browser console and network tab for errors and failed requests
- [ ] Test error handling scenarios (services down, invalid requests, etc.)
- [ ] Document all test results, failures, and recommendations in TEST_RESULTS.md