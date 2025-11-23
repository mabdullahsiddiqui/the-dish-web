<!-- 0fc6b316-3735-455f-b6da-79e641c5e3d6 b931c43b-127b-46b9-bd1e-38f21010c689 -->
# The Dish - Production Development Plan (Updated with AI Review Analysis)

## Executive Summary

**The Dish** is a next-generation restaurant and hotel review platform with intelligent dietary preference adaptation, community-verified certifications, comprehensive business tools, and **AI-powered review sentiment analysis and tagging**.

**Timeline**: 20 months (84 weeks) to public launch

**Architecture**: .NET Core 8 Microservices + Next.js 14 + Expo React Native + React Admin Panel

**Deployment**: AWS Cloud-Native with Kubernetes

**Target**: 10,000+ users and 500+ business subscribers by Month 12

---

## Technology Stack

### Backend

- **.NET Core 8** - Microservices with Clean Architecture
- **PostgreSQL 15 + PostGIS** - Primary database with geospatial support
- **Redis** - Caching and session management
- **Elasticsearch** - Full-text search with AI-generated tags
- **RabbitMQ** - Event-driven messaging for review processing
- **ML.NET** - Machine learning framework for sentiment analysis
- **Hugging Face Transformers** - Pre-trained sentiment model integration
- **Ocelot** - API Gateway

### Frontend

- **Next.js 14** - Web application (App Router + TypeScript)
- **Expo SDK 50+** - Mobile application (iOS/Android)
- **React + Vite** - Admin panel
- **Tailwind CSS** - Styling
- **Zustand + React Query** - State management

### Infrastructure

- **AWS** - EKS, RDS, ElastiCache, S3, CloudFront
- **Docker + Kubernetes** - Containerization and orchestration
- **Terraform** - Infrastructure as Code
- **GitHub Actions** - CI/CD pipelines

---

## Phase 1: Core Backend Services - MVP (Weeks 5-14)

### 1.3 Review Service (Weeks 11-13)

**Features**:

- Review submission with GPS verification (proximity check)
- Star rating system (1-5 stars)
- Review photo upload (multiple photos)
- Review aggregation (average ratings, counts)
- Helpful votes system
- Review moderation queue
- Review editing with history tracking
- **RabbitMQ event publishing** - Publish review created events to queue

**GPS Verification**:

- Validate user is within 200m of place when submitting
- Store submission coordinates
- Timestamp verification

**RabbitMQ Integration**:

- Publish `ReviewCreatedEvent` message to `review.created` queue after successful review creation
- Event payload includes: ReviewId, PlaceId, UserId, Text, Rating, Timestamp
- Use MassTransit or RabbitMQ.Client for message publishing
- Ensure message is published after database commit (transactional outbox pattern)

**Key Endpoints**:

- `POST /api/v1/reviews`
- `GET /api/v1/reviews/place/{placeId}`
- `PUT /api/v1/reviews/{id}`
- `POST /api/v1/reviews/{id}/vote`
- `GET /api/v1/reviews/moderation/queue`

---

## Phase 1.5: AI Review Analysis Service (Weeks 13-14)

### 1.5.1 AI Service Architecture

**New Microservice**: `TheDish.AI.ReviewAnalysis`

**Clean Architecture Structure**:

- `AI.ReviewAnalysis.Domain` - Entities, value objects, domain events
- `AI.ReviewAnalysis.Application` - Use cases, DTOs, interfaces
- `AI.ReviewAnalysis.Infrastructure` - ML.NET integration, Hugging Face model, Elasticsearch client
- `AI.ReviewAnalysis.API` - Health check endpoints, monitoring

### 1.5.2 ML.NET Integration with Hugging Face Model

**Model Setup**:

- Use Hugging Face model: `cardiffnlp/twitter-roberta-base-sentiment`
- Download and convert model to ONNX format for ML.NET compatibility
- Load model using `MLContext` and `TransformerChain`
- Model outputs: sentiment scores (positive, neutral, negative) + confidence scores

**Tag Generation Logic**:

- Analyze review text for sentiment (positive/neutral/negative)
- Extract keywords and phrases using NLP techniques
- Map sentiment + keywords to predefined tags:
  - **Positive tags**: delicious, family-friendly, great-service, cozy, romantic, affordable, authentic, fresh, generous-portions
  - **Negative tags**: bad-service, noisy, overpriced, slow-service, dirty, rude-staff, poor-quality, limited-menu
  - **Neutral/Contextual tags**: busy, casual, formal, outdoor-seating, parking-available, reservations-required
- Generate confidence scores for each tag (0.0 - 1.0)
- Return top 5-8 tags with confidence > 0.6

**Implementation**:

- Create `SentimentAnalysisService` interface and implementation
- Use ML.NET `PredictionEngine` for inference
- Implement tag mapping rules based on sentiment + keyword extraction
- Cache model in memory for performance (singleton service)
- Handle model loading errors gracefully with fallback

### 1.5.3 RabbitMQ Consumer

**Message Consumption**:

- Subscribe to `review.created` queue
- Consume `ReviewCreatedEvent` messages
- Process review text through AI model
- Generate tags with confidence scores
- Publish `ReviewAnalyzedEvent` with tags to `review.analyzed` queue

**Error Handling**:

- Retry failed analyses (max 3 retries with exponential backoff)
- Dead letter queue for permanently failed messages
- Log all processing errors for monitoring

### 1.5.4 Elasticsearch Tag Indexing

**Index Update**:

- Update existing Places index in Elasticsearch to include:
  - `ai_tags` (keyword array) - Generated tags from reviews
  - `review_sentiment_scores` (nested object) - Per-review sentiment data
  - `aggregated_tags` (nested object) - Tag frequency and confidence aggregation per place

**Indexing Flow**:

1. AI service generates tags for review
2. Update place document in Elasticsearch with new tags
3. Aggregate tags across all reviews for the place
4. Calculate tag frequency and average confidence scores
5. Store in `aggregated_tags` field for fast filtering

**Elasticsearch Mapping**:

```json
{
  "properties": {
    "ai_tags": {
      "type": "keyword"
    },
    "review_sentiment_scores": {
      "type": "nested",
      "properties": {
        "review_id": { "type": "keyword" },
        "tags": { "type": "keyword" },
        "confidence": { "type": "float" },
        "sentiment": { "type": "keyword" }
      }
    },
    "aggregated_tags": {
      "type": "nested",
      "properties": {
        "tag": { "type": "keyword" },
        "frequency": { "type": "integer" },
        "avg_confidence": { "type": "float" }
      }
    }
  }
}
```

### 1.5.5 Docker Containerization

**Dockerfile**:

- Base image: `mcr.microsoft.com/dotnet/aspnet:8.0`
- Copy model files (ONNX format) to container
- No GPU required - CPU inference only
- Multi-stage build for optimization
- Health check endpoint for Kubernetes liveness probes

**Docker Compose**:

- Add AI service to `docker-compose.yml`
- Configure RabbitMQ connection
- Configure Elasticsearch connection
- Set environment variables for model path

### 1.5.6 Testing

**Test Plan**:

- Create 10 fake reviews with varied sentiment:
  - 3 positive reviews (delicious, great-service)
  - 3 negative reviews (bad-service, noisy, overpriced)
  - 2 neutral reviews (casual, busy)
  - 2 mixed sentiment reviews
- Verify tag generation accuracy
- Test RabbitMQ message flow end-to-end
- Verify Elasticsearch indexing
- Performance testing (target: <500ms per review analysis)

**Key Endpoints**:

- `GET /health` - Health check
- `GET /metrics` - Performance metrics (optional)

---

## Phase 2: Web Application - MVP (Weeks 15-24)

### 2.3 Search & Discovery (Weeks 18-20)

**Search Page**:

- Search bar with autocomplete
- Advanced filters:
  - Dietary preferences (adaptive based on user)
  - Cuisine types
  - Price range
  - Distance
  - Rating threshold
  - Amenities
  - **AI-generated tags** (delicious, family-friendly, noisy, bad-service, etc.)
- Map view integration (Leaflet with OpenStreetMap)
- List view with place cards
- Sorting options (distance, rating, price)

**AI Tag Filtering**:

- Filter places by AI-generated tags in Elasticsearch query
- Example query: "Show me 4-star family-friendly spots in Delhi"
- Query Elasticsearch with:
  - Rating filter: `rating >= 4`
  - Tag filter: `aggregated_tags.tag = "family-friendly"`
  - Location filter: `city = "Delhi"`
- Display matching tags prominently in search results

### 2.6 New Endpoint: Review Analysis (Week 24)

**Frontend Integration**:

- Add new API endpoint in Next.js: `GET /api/analyze?reviewId={id}`
- Calls backend endpoint: `GET /api/v1/reviews/{id}/analyze`
- Backend endpoint queries Elasticsearch for review tags
- Returns: tags array, sentiment scores, confidence levels
- Display tags in review detail page
- Show tag confidence indicators (high/medium/low)

**Implementation**:

- Create `ReviewAnalysisController` in Review Service
- Query Elasticsearch for review's AI tags
- Return structured response with tags and metadata
- Cache results in Redis (5-minute TTL)

---

## Phase 7: Search Service & External APIs (Weeks 57-62)

### 7.1 Elasticsearch Integration (Weeks 57-58) - UPDATED

**Index Design**:

- Places index with mappings:
  - Name, description (text fields)
  - Cuisine, dietary tags (keyword fields)
  - Location (geo_point)
  - Rating, price (numeric)
  - Trust scores (nested objects)
  - **AI-generated tags (keyword array)**
  - **Review sentiment scores (nested objects)**
  - **Aggregated tags (nested objects with frequency and confidence)**

**Search Features**:

- Full-text search with relevance scoring
- Faceted search (filters with counts)
- Geospatial search (nearby + text)
- Autocomplete suggestions
- "Did you mean?" spelling corrections
- Search analytics tracking
- **AI tag-based filtering** - Filter places by sentiment-derived tags

**Enhanced Search Queries**:

- Support queries like: "4-star family-friendly spots in Delhi"
- Query structure:
  ```json
  {
    "query": {
      "bool": {
        "must": [
          { "range": { "rating": { "gte": 4 } } },
          { "term": { "aggregated_tags.tag": "family-friendly" } },
          { "match": { "city": "Delhi" } }
        ]
      }
    }
  }
  ```


---

## Updated Architecture Diagram

```
┌─────────────┐
│   Next.js   │
│  Frontend   │
└──────┬──────┘
       │
       │ HTTP/REST
       │
┌──────▼─────────────────────────────────────┐
│         API Gateway (Ocelot)               │
└──────┬────────────────────────────────────┘
       │
       ├──────────┬──────────┬──────────┐
       │          │          │          │
┌──────▼──────┐ ┌─▼──────┐ ┌─▼──────┐ ┌─▼──────────────┐
│ User Service│ │ Place  │ │ Review │ │ AI Review     │
│             │ │Service │ │Service │ │ Analysis       │
│             │ │        │ │        │ │ Service       │
└─────────────┘ └────────┘ └────┬───┘ └───────┬───────┘
                                 │             │
                                 │             │
                    ┌────────────▼─────────────▼────┐
                    │      RabbitMQ                  │
                    │  (review.created queue)        │
                    └────────────┬────────────────────┘
                                 │
                    ┌────────────▼────────────────────┐
                    │   AI Service Consumes           │
                    │   → Analyzes Review Text        │
                    │   → Generates Tags              │
                    │   → Publishes to                │
                    │     review.analyzed queue       │
                    └────────────┬────────────────────┘
                                 │
                    ┌────────────▼────────────────────┐
                    │   Elasticsearch Indexer         │
                    │   → Updates Place Document      │
                    │   → Adds AI Tags                │
                    │   → Aggregates Tag Data         │
                    └─────────────────────────────────┘
                                 │
                    ┌────────────▼────────────────────┐
                    │   Elasticsearch                 │
                    │   (Places Index with AI Tags)   │
                    └─────────────────────────────────┘
```

---

## Implementation Details

### AI Service Dependencies

**NuGet Packages**:

- `Microsoft.ML` - ML.NET core
- `Microsoft.ML.OnnxRuntime` - ONNX model inference
- `MassTransit.RabbitMQ` - Message queue integration
- `NEST` or `Elasticsearch.Net` - Elasticsearch client
- `HuggingFace.Transformers` (if needed for model conversion)

### Model Conversion

- Download Hugging Face model: `cardiffnlp/twitter-roberta-base-sentiment`
- Convert to ONNX format using Hugging Face transformers library or ONNX Runtime
- Store model file in container or S3 bucket
- Load model at service startup

### Tag Mapping Rules

**Positive Sentiment + Keywords**:

- "delicious", "amazing food", "tasty" → `delicious`
- "family", "kids", "children" → `family-friendly`
- "great service", "friendly staff" → `great-service`
- "cozy", "intimate" → `cozy`
- "romantic", "date night" → `romantic`
- "affordable", "cheap", "value" → `affordable`

**Negative Sentiment + Keywords**:

- "bad service", "slow service" → `bad-service`, `slow-service`
- "noisy", "loud" → `noisy`
- "expensive", "overpriced" → `overpriced`
- "dirty", "unclean" → `dirty`
- "rude", "unfriendly" → `rude-staff`

**Neutral/Contextual**:

- "busy", "crowded" → `busy`
- "casual", "informal" → `casual`
- "outdoor", "patio" → `outdoor-seating`
- "parking" → `parking-available`

### Performance Considerations

- Model inference: Target <300ms per review
- RabbitMQ processing: Async, non-blocking
- Elasticsearch updates: Batch updates if possible
- Cache frequently used tags in Redis
- Scale AI service horizontally (stateless design)

### Branch Strategy

- Create feature branch: `ai-review`
- Implement AI service
- Update Review Service to publish RabbitMQ events
- Update Elasticsearch mappings
- Add frontend endpoint
- Test with 10 fake reviews
- Merge to `develop` after review

---

## Updated Success Metrics

### Technical Metrics

**AI Service Performance**:

- Review analysis latency: <500ms (p95)
- Tag generation accuracy: >75% (validated against manual tagging)
- Model inference time: <300ms per review
- RabbitMQ message processing: <1s end-to-end
- Elasticsearch indexing: <200ms per update

### Business Metrics

**AI Feature Adoption**:

- % of reviews with AI tags: >90%
- Search queries using AI tags: Track usage
- User engagement with tag filters: Monitor click-through rates

---

## Risk Mitigation

### Technical Risks

**Risk**: Model accuracy may not meet expectations

**Mitigation**:

- Start with rule-based tag mapping as fallback
- Continuously validate tag accuracy
- Allow manual tag correction/adjustment
- A/B test model performance

**Risk**: Model inference performance at scale

**Mitigation**:

- Use ONNX Runtime for optimized inference
- Implement caching for similar reviews
- Scale AI service horizontally
- Consider model quantization for faster inference

**Risk**: Hugging Face model compatibility with ML.NET

**Mitigation**:

- Convert model to ONNX format (standardized)
- Test model loading and inference early
- Have fallback to simpler sentiment analysis if needed
- Document conversion process thoroughly

---

## Next Steps for AI Implementation

1. **Week 13 Day 1-2**: Set up AI Review Analysis Service project structure
2. **Week 13 Day 3-4**: Download and convert Hugging Face model to ONNX
3. **Week 13 Day 5**: Implement ML.NET model loading and inference
4. **Week 13 Day 6-7**: Implement tag generation logic and mapping rules
5. **Week 14 Day 1-2**: Set up RabbitMQ consumer for review events
6. **Week 14 Day 3**: Update Review Service to publish RabbitMQ events
7. **Week 14 Day 4**: Implement Elasticsearch tag indexing
8. **Week 14 Day 5**: Create Dockerfile and update docker-compose.yml
9. **Week 14 Day 6**: Test with 10 fake reviews, validate tag generation
10. **Week 14 Day 7**: Update Elasticsearch mappings, add frontend endpoint, push to `ai-review` branch

---

**This updated plan integrates AI-powered review analysis seamlessly into the existing architecture, enabling intelligent tag-based search and discovery while maintaining system performance and scalability.**

### To-dos

- [ ] Add RabbitMQ event publishing to Review Service - publish ReviewCreatedEvent after review creation
- [ ] Create AI Review Analysis Service microservice with Clean Architecture (Domain, Application, Infrastructure, API layers)
- [ ] Download and convert Hugging Face model (cardiffnlp/twitter-roberta-base-sentiment) to ONNX format, integrate with ML.NET
- [ ] Implement sentiment analysis and tag generation logic with mapping rules (delicious, family-friendly, noisy, bad-service, etc.)
- [ ] Implement RabbitMQ consumer in AI service to process ReviewCreatedEvent messages
- [ ] Update Elasticsearch Places index to include AI tags, review sentiment scores, and aggregated tags with frequency/confidence
- [ ] Implement Elasticsearch indexing service to update place documents with AI-generated tags after analysis
- [ ] Create Dockerfile for AI service (CPU-only, no GPU), update docker-compose.yml with AI service configuration
- [ ] Add /api/analyze endpoint in Next.js frontend and backend Review Service to query and display AI tags for reviews
- [ ] Update Elasticsearch search queries to support AI tag-based filtering (e.g., 4-star family-friendly spots in Delhi)
- [ ] Create and test with 10 fake reviews covering positive, negative, neutral, and mixed sentiment scenarios
- [ ] Push all changes to 'ai-review' branch and update architecture diagram