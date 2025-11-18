# Testing Guide for The Dish - Phase 1

## Test Project Structure

The test projects are organized following the same structure as the source projects:

```
backend/tests/
├── TheDish.Place.Domain.Tests/
│   └── Entities/
│       └── PlaceTests.cs
├── TheDish.Place.Application.Tests/
│   ├── Commands/
│   │   └── CreatePlaceCommandHandlerTests.cs
│   └── Queries/
│       └── GetNearbyPlacesQueryHandlerTests.cs
├── TheDish.Review.Domain.Tests/
│   ├── Entities/
│   │   └── ReviewTests.cs
│   └── Services/
│       └── GpsVerificationServiceTests.cs
├── TheDish.Review.Application.Tests/
│   └── Commands/
│       ├── CreateReviewCommandHandlerTests.cs
│       └── MarkReviewHelpfulCommandHandlerTests.cs
├── TheDish.Place.API.Tests/
│   └── Controllers/
│       └── PlacesControllerTests.cs
└── TheDish.Review.API.Tests/
    └── Controllers/
        └── ReviewsControllerTests.cs
```

## Running Tests

### Run All Tests
```bash
cd backend
dotnet test
```

### Run Specific Test Project
```bash
dotnet test tests/TheDish.Place.Domain.Tests/TheDish.Place.Domain.Tests.csproj
```

### Run with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Run with Verbose Output
```bash
dotnet test --verbosity normal
```

## Test Categories

### Unit Tests
- **Domain Tests**: Test domain entities, value objects, and business logic
- **Application Tests**: Test command/query handlers with mocked dependencies

### Integration Tests
- **API Tests**: Test controllers with in-memory test server
- **Database Tests**: Test repository implementations with test database

## Test Coverage Goals

- **Domain Layer**: 95%+ coverage
- **Application Layer**: 85%+ coverage
- **Infrastructure Layer**: 80%+ coverage
- **API Layer**: 70%+ coverage

## Key Test Scenarios

### Place Service Tests
1. ✅ Place entity validation (name, address, coordinates, price range)
2. ✅ Place business logic (claim, verify, update rating)
3. ✅ CreatePlaceCommand handler
4. ✅ GetNearbyPlacesQuery handler
5. ✅ PlacesController endpoints

### Review Service Tests
1. ✅ Review entity validation (rating, text)
2. ✅ GPS verification service (200m proximity check)
3. ✅ CreateReviewCommand handler with GPS verification
4. ✅ MarkReviewHelpfulCommand handler
5. ✅ ReviewsController endpoints

## Next Steps

1. **Fix Compilation Issues**: Some test files may need adjustments to match actual implementation
2. **Add Integration Tests**: Create tests that use actual database connections
3. **Add API Integration Tests**: Use WebApplicationFactory for end-to-end API testing
4. **Add Test Data Builders**: Create builder pattern for test data setup
5. **Add Test Fixtures**: Create shared test fixtures for common scenarios

## Notes

- Tests use **xUnit** as the testing framework
- **Moq** is used for mocking dependencies
- **FluentAssertions** is used for readable assertions
- Some tests may need to be updated to match the actual implementation signatures








