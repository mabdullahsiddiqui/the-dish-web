# Business Logic Part 1 - Core Domain Rules

## Overview

The Dish is a next-generation restaurant and hotel review platform with intelligent dietary preference adaptation, community-verified certifications, and comprehensive business tools.

## Core Business Entities

### User Entity

**Purpose**: Represents platform users (reviewers, business owners, administrators)

**Key Attributes**:
- Email (unique identifier)
- Password (hashed with BCrypt)
- First Name, Last Name
- Reputation Score (calculated from review helpfulness)
- Reputation Level (Bronze, Silver, Gold, Platinum, Diamond)
- User Role (User, BusinessOwner, Admin)
- Location (optional, for location-based features)
- User Preferences (dietary restrictions, cuisine preferences)

**Business Rules**:
- Email must be unique across the platform
- Password must meet security requirements (minimum 8 characters, complexity)
- Reputation score increases when reviews receive "helpful" votes
- Reputation level is automatically calculated based on score thresholds
- Users can have multiple dietary preferences
- Location is optional but enables location-based search

### Place Entity

**Purpose**: Represents restaurants, hotels, and other establishments

**Key Attributes**:
- Name (required)
- Address (required)
- Location (PostGIS Point - latitude/longitude)
- Phone, Website, Email (optional contact information)
- Cuisine Types (list of cuisine categories)
- Price Range (1-4 scale)
- Dietary Tags (dictionary of dietary accommodations)
- Trust Scores (community-verified dietary accuracy scores)
- Average Rating (calculated from reviews)
- Review Count (total number of reviews)
- Claimed By (user ID if business owner claimed)
- Is Verified (platform verification status)
- Status (Active, Inactive, Pending, Suspended)
- Hours of Operation (JSON structure)
- Amenities (list of available amenities)
- Parking Info (text description)

**Business Rules**:
- Name and address are required for place creation
- Location must be a valid geographic coordinate
- Price range must be between 1-4
- Average rating is automatically calculated from all reviews
- Review count is automatically maintained
- Places can be claimed by business owners (one owner per place)
- Verified status requires platform admin approval
- Dietary tags are community-verified through review feedback

### Review Entity

**Purpose**: User-submitted reviews of places

**Key Attributes**:
- Place ID (foreign key)
- User ID (foreign key)
- Rating (1-5 stars)
- Text (review content)
- Check-In Location (GPS coordinates when review was submitted)
- Place Location (GPS coordinates of the place)
- Is GPS Verified (boolean - within 200m of place)
- Dietary Accuracy (user feedback on dietary tag accuracy)
- Helpful Count (number of users who marked as helpful)
- Not Helpful Count (number of users who marked as not helpful)
- Photos (collection of review photos)
- Created At, Updated At (timestamps)

**Business Rules**:
- Users can only submit one review per place (can update existing)
- Rating must be between 1-5
- GPS verification requires user to be within 200 meters of place location
- GPS verification is optional but increases review trustworthiness
- Dietary accuracy feedback helps improve place dietary tag accuracy
- Helpful/Not Helpful votes affect user reputation
- Reviews can include multiple photos
- Reviews can be edited by the author
- Reviews can be deleted by the author or platform admins

## Core Business Processes

### User Registration

1. User provides email, password, first name, last name
2. System validates email format and uniqueness
3. System validates password strength
4. Password is hashed using BCrypt
5. User account is created with default role (User)
6. Initial reputation score is set to 0
7. JWT token is generated and returned for immediate authentication

### User Login

1. User provides email and password
2. System retrieves user by email
3. System verifies password hash matches
4. JWT token is generated with user claims (ID, email, role)
5. Token is returned with user information
6. Token expires after 60 minutes (configurable)

### Place Creation

1. User provides place information (name, address, coordinates, etc.)
2. System validates required fields
3. System validates geographic coordinates
4. Place is created with status "Active"
5. Initial average rating is 0, review count is 0
6. Place is immediately available for reviews

### Place Claiming

1. Business owner requests to claim a place
2. System verifies user has BusinessOwner role
3. System checks if place is already claimed
4. If unclaimed, place is assigned to the business owner
5. Business owner can then update place information
6. Platform admin can verify the claim

### Review Submission

1. User navigates to place detail page
2. User optionally enables GPS location
3. System calculates distance between user location and place location
4. If within 200m, GPS verification is marked as true
5. User submits rating, text, optional photos, and dietary accuracy feedback
6. Review is created and associated with place and user
7. Place's average rating is recalculated
8. Place's review count is incremented
9. If dietary accuracy feedback is provided, place's trust scores are updated

### Review Helpfulness Voting

1. User views a review
2. User clicks "Helpful" or "Not Helpful"
3. System records the vote (one vote per user per review)
4. Review's helpful/not helpful count is updated
5. Review author's reputation score is adjusted
6. Reputation level is recalculated if threshold is crossed

## Reputation System

### Reputation Score Calculation

- Base score: 0 for new users
- +1 point for each "Helpful" vote on a review
- -0.5 points for each "Not Helpful" vote on a review
- Minimum score: 0 (cannot go negative)

### Reputation Levels

- **Bronze**: 0-49 points
- **Silver**: 50-149 points
- **Gold**: 150-299 points
- **Platinum**: 300-499 points
- **Diamond**: 500+ points

### Reputation Benefits

- Higher reputation users' reviews are prioritized in search results
- Business owners can see reputation levels when responding to reviews
- Platform may offer special features to high-reputation users

## Dietary Preference System

### Dietary Tags

Common dietary tags include:
- Vegetarian
- Vegan
- Gluten-Free
- Dairy-Free
- Nut-Free
- Halal
- Kosher
- Keto-Friendly
- Low-Sodium
- Sugar-Free

### Trust Score Calculation

- Each dietary tag has a trust score (0-100)
- Trust score is calculated from review dietary accuracy feedback
- Positive feedback increases trust score
- Negative feedback decreases trust score
- Trust scores help users find places that accurately accommodate their dietary needs

## Search and Discovery

### Location-Based Search

- Users can search for places near their current location
- Search radius is configurable (default: 5km)
- Results are sorted by distance and rating
- PostGIS enables efficient geospatial queries

### Text-Based Search

- Users can search by place name, cuisine type, or dietary tags
- Elasticsearch provides full-text search capabilities
- Results are ranked by relevance and rating

### Filtering

- Filter by cuisine types
- Filter by dietary tags
- Filter by price range (1-4)
- Filter by minimum rating (0-5)
- Filter by distance (radius in km)
- Multiple filters can be combined

## Data Integrity Rules

1. **Referential Integrity**: All foreign keys must reference valid entities
2. **Cascade Deletes**: When a user is deleted, their reviews are deleted
3. **Soft Deletes**: Places and reviews use status flags rather than hard deletes
4. **Audit Trail**: All entities track CreatedAt and UpdatedAt timestamps
5. **Unique Constraints**: Email addresses must be unique

## Security Rules

1. **Authentication**: All protected endpoints require valid JWT token
2. **Authorization**: Role-based access control (User, BusinessOwner, Admin)
3. **Password Security**: Passwords are hashed with BCrypt (work factor 12)
4. **Token Security**: JWT tokens expire after 60 minutes
5. **Rate Limiting**: API endpoints are rate-limited (100 requests/minute per IP)
6. **Input Validation**: All user inputs are validated before processing
7. **SQL Injection Prevention**: Entity Framework Core uses parameterized queries
8. **XSS Prevention**: All user-generated content is sanitized before display


