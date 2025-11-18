# Technical Stack User Documentation

## Overview

This document provides user-facing technical documentation for The Dish platform, explaining how the system works from an end-user perspective.

## Platform Architecture

### What is The Dish?

The Dish is a modern restaurant and hotel review platform that helps you:
- Discover restaurants and hotels based on your location and preferences
- Read and write reviews with GPS verification
- Find places that accommodate your dietary needs
- Build your reputation as a trusted reviewer

### How It Works

The platform consists of:
- **Web Application**: Access via your browser at thedish.com
- **Mobile Application**: Native apps for iOS and Android (coming soon)
- **Backend Services**: Secure microservices that handle all data processing
- **Database**: Your data is securely stored in PostgreSQL databases
- **Cloud Infrastructure**: Hosted on AWS for reliability and scalability

## User Features

### Account Management

**Registration**:
- Create an account with your email and password
- Your password is securely encrypted and never stored in plain text
- You can add your name and dietary preferences during registration

**Login**:
- Secure login using email and password
- Sessions are managed with JWT tokens (expire after 60 minutes)
- "Remember me" option extends session duration

**Profile**:
- View your review history
- See your reputation score and level
- Manage dietary preferences
- Update account information

### Search and Discovery

**Location-Based Search**:
- Find places near your current location
- Search radius is configurable (default: 5km)
- Results are sorted by distance and rating

**Text Search**:
- Search by place name, cuisine type, or dietary tags
- Full-text search across all place information
- Results ranked by relevance

**Filters**:
- Filter by cuisine types (Italian, Mexican, Asian, etc.)
- Filter by dietary tags (Vegetarian, Vegan, Gluten-Free, etc.)
- Filter by price range ($ to $$$$)
- Filter by minimum rating (1-5 stars)
- Filter by distance from your location

### Reviews

**Writing Reviews**:
- Rate places from 1-5 stars
- Write detailed review text
- Upload photos (up to 5 per review)
- Optionally enable GPS verification (must be within 200m of place)
- Provide dietary accuracy feedback

**Reading Reviews**:
- View all reviews for a place
- Sort by date, rating, or helpfulness
- See GPS verification badges
- Vote on review helpfulness
- View review photos

**Review Management**:
- Edit your reviews anytime
- Delete your reviews
- See how many people found your review helpful

### GPS Verification

**What is GPS Verification?**:
- GPS verification confirms you were physically at the place when writing the review
- Reviews with GPS verification are marked with a badge
- GPS-verified reviews are prioritized in search results

**How It Works**:
1. When writing a review, you can enable location sharing
2. The app calculates your distance from the place
3. If you're within 200 meters, your review is marked as GPS verified
4. GPS verification is optional - you can still write reviews without it

**Privacy**:
- Your exact location is never stored
- Only the distance calculation is used for verification
- You can disable location services anytime

### Reputation System

**How Reputation Works**:
- Start with 0 reputation points
- Earn points when others find your reviews helpful
- Lose points when reviews are marked as not helpful
- Reputation cannot go below 0

**Reputation Levels**:
- **Bronze** (0-49 points): New reviewer
- **Silver** (50-149 points): Active reviewer
- **Gold** (150-299 points): Trusted reviewer
- **Platinum** (300-499 points): Expert reviewer
- **Diamond** (500+ points): Top contributor

**Benefits of Higher Reputation**:
- Your reviews appear higher in search results
- Business owners see your reputation level
- Access to exclusive features (coming soon)

### Dietary Preferences

**Setting Preferences**:
- Add dietary restrictions to your profile
- Common tags: Vegetarian, Vegan, Gluten-Free, Dairy-Free, Nut-Free, Halal, Kosher
- Preferences are used to personalize search results

**Dietary Trust Scores**:
- Each place has trust scores for dietary tags
- Trust scores are based on community feedback
- Higher trust scores mean more accurate dietary information
- Trust scores help you find places that truly accommodate your needs

### Place Information

**What You Can See**:
- Place name, address, and location on map
- Contact information (phone, website, email)
- Cuisine types and price range
- Dietary tags and trust scores
- Hours of operation
- Amenities and parking information
- Photos (user-submitted and business owner)
- Menu items (if available)
- Average rating and review count

**Place Actions**:
- Write a review
- Upload photos
- Share place with friends
- Save to favorites (coming soon)
- Report incorrect information

## Technical Details (User-Facing)

### Performance

**Speed**:
- Search results load in under 1 second
- Place detail pages load in under 2 seconds
- Photos are optimized for fast loading
- Results are cached for quick access

**Reliability**:
- 99.9% uptime guarantee
- Automatic failover if a server goes down
- Data is backed up daily
- Multiple data centers for redundancy

### Security

**Data Protection**:
- All data is encrypted in transit (HTTPS)
- Passwords are hashed and never stored in plain text
- JWT tokens expire after 60 minutes
- Rate limiting prevents abuse

**Privacy**:
- Your personal information is never shared with third parties
- You can export your data anytime (GDPR compliant)
- You can delete your account and all associated data
- Location data is only used for GPS verification (not stored)

### Mobile Experience

**Responsive Design**:
- Website works on all devices (desktop, tablet, mobile)
- Touch-optimized interface for mobile devices
- Fast loading on mobile networks
- Offline capability (coming soon)

**Native Apps** (Coming Soon):
- iOS and Android apps
- Push notifications for review responses
- Offline review submission
- Location-based alerts

## API Access (For Developers)

If you're a developer building an integration with The Dish:

**API Endpoints**:
- Base URL: `https://api.thedish.com/api/v1`
- Authentication: JWT tokens in Authorization header
- Rate Limit: 100 requests per minute per API key
- Documentation: Available at `/swagger` endpoint

**Getting Started**:
1. Register for an API key
2. Read the API documentation
3. Start making requests
4. Follow rate limiting guidelines

## Support

**Getting Help**:
- FAQ: Available in the app
- Email Support: support@thedish.com
- In-App Support: Use the help button in the app
- Community Forum: community.thedish.com

**Reporting Issues**:
- Report bugs through the app
- Report inappropriate content
- Report incorrect place information
- Request new features

## Privacy Policy

**Data We Collect**:
- Account information (email, name)
- Reviews and photos you submit
- Search queries (anonymized)
- Usage analytics (anonymized)

**How We Use Data**:
- To provide and improve the service
- To personalize your experience
- To prevent fraud and abuse
- To comply with legal obligations

**Your Rights**:
- Access your data
- Export your data
- Delete your account
- Opt out of analytics

## Terms of Service

**User Responsibilities**:
- Provide accurate information
- Respect other users
- Follow community guidelines
- Report inappropriate content

**Platform Rules**:
- No spam or fake reviews
- No harassment or abuse
- No illegal content
- No copyright infringement

**Consequences**:
- Violations may result in account suspension
- Repeated violations may result in permanent ban
- Legal action may be taken for serious violations

## Updates and Changes

**Platform Updates**:
- New features are added regularly
- Bug fixes are deployed automatically
- Major changes are announced in advance
- You can opt out of non-essential updates

**Notification Preferences**:
- Email notifications for review responses
- Push notifications for important updates
- In-app notifications for new features
- Customize notification settings in your profile

## Accessibility

**Accessibility Features**:
- Screen reader support
- Keyboard navigation
- High contrast mode
- Text size adjustment
- WCAG 2.1 AA compliant

**Getting Help**:
- Accessibility support: accessibility@thedish.com
- Report accessibility issues
- Request accessibility features


