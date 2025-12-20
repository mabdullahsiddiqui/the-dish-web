# AI Review Analysis Service

This service processes restaurant reviews using AI sentiment analysis and generates tags for enhanced search capabilities.

## Features

- **Sentiment Analysis**: Analyzes review text to determine sentiment (positive, neutral, negative)
- **Tag Generation**: Automatically generates relevant tags like "delicious", "family-friendly", "noisy", "bad-service", etc.
- **Elasticsearch Integration**: Indexes tags in Elasticsearch for fast search queries
- **RabbitMQ Consumer**: Processes review events asynchronously

## Architecture

- **Domain**: Entities and domain models
- **Application**: Interfaces and DTOs
- **Infrastructure**: ML.NET integration, RabbitMQ consumer, Elasticsearch client
- **API**: Health check endpoints

## Configuration

Update `appsettings.json`:

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "thedish",
    "Password": "thedish_dev_password"
  },
  "Elasticsearch": {
    "Url": "http://localhost:9200"
  }
}
```

## Running the Service

### Docker Compose

The service is included in `docker-compose.yml` and will start automatically:

```bash
docker-compose up -d
```

### Manual Run

```bash
cd backend/src/Services/TheDish.AI.ReviewAnalysis.API
dotnet run
```

## Health Check

```bash
curl http://localhost:5004/health
```

## Testing

Test data is provided in `TestData/FakeReviews.json` with 10 sample reviews covering:
- 3 positive reviews
- 3 negative reviews
- 2 neutral reviews
- 2 mixed sentiment reviews

## API Endpoints

- `GET /health` - Health check endpoint

## Tag Categories

### Positive Tags
- delicious, family-friendly, great-service, cozy, romantic, affordable, authentic, fresh, generous-portions

### Negative Tags
- bad-service, noisy, overpriced, slow-service, dirty, rude-staff, poor-quality, limited-menu

### Neutral/Contextual Tags
- busy, casual, formal, outdoor-seating, parking-available, reservations-required








