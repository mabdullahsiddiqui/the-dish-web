<!-- 5656f218-28ce-4ae0-8c3d-bed2c124d1fb d6a6818e-852c-41db-92de-c495c74eec26 -->
# Extend DataSeeder to Include Users and Reviews

## Overview

Extend `backend/src/Tools/TheDish.DataSeeder/Program.cs` to seed test users and 10-20 reviews per place. The seeder will create users first, then generate reviews for all existing places.

## Implementation Steps

### 1. Update Project References

**File**: `backend/src/Tools/TheDish.DataSeeder/TheDish.DataSeeder.csproj`

- Add project references to:
- `TheDish.User.Infrastructure`
- `TheDish.User.Domain`
- `TheDish.Review.Infrastructure`
- `TheDish.Review.Domain`

### 2. Extend Program.cs

**File**: `backend/src/Tools/TheDish.DataSeeder/Program.cs`

**Changes**:

- Add using statements for User and Review namespaces
- Create `UserDbContext` alongside `PlaceDbContext`
- Create 15-20 test users with realistic names and emails
- For each place (10 places), create 10-20 reviews with:
- Random ratings (1-5, weighted toward 3-5)
- Realistic review text (varied lengths and styles)
- GPS verification (70% verified, 30% not verified)
- Check-in locations near the place (within 0.01 degrees)
- Dietary accuracy data (for places with dietary tags)
- Helpfulness counts (0-50 helpful, 0-10 not helpful)
- Varied creation dates (spread over last 6 months)
- Ensure unique constraint: one review per user per place
- Update user review counts after creating reviews

**Review Data Structure**:

- Rating distribution: 20% 5-star, 30% 4-star, 30% 3-star, 15% 2-star, 5% 1-star
- Review text: Mix of short (50-100 chars) and longer (200-500 chars) reviews
- GPS verification: 70% verified with coordinates near place location
- Dietary accuracy: Only for places with dietary tags (Halal, Vegan, etc.)
- Helpfulness: Random distribution, some reviews with 0 votes

### 3. Add BCrypt Package

**File**: `backend/src/Tools/TheDish.DataSeeder/TheDish.DataSeeder.csproj`

- Add `BCrypt.Net-Next` package reference for password hashing

### 4. Sample Review Text Templates

Create varied, realistic review text samples like:

- "Great food and service! Will definitely come back."
- "The atmosphere was nice but the food was just okay."
- "Amazing experience! The staff was friendly and the food was delicious."
- "Not worth the price. Food was cold and service was slow."
- Longer detailed reviews with specific dish mentions

## Files to Modify

1. `backend/src/Tools/TheDish.DataSeeder/TheDish.DataSeeder.csproj` - Add project references and BCrypt package
2. `backend/src/Tools/TheDish.DataSeeder/Program.cs` - Extend with user and review seeding logic

## Expected Output

- 15-20 test users created
- 100-200 total reviews (10-20 per place × 10 places)
- Reviews distributed across all 10 seeded places
- Realistic data including GPS verification, dietary accuracy, and helpfulness counts