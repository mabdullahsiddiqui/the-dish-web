# Script to apply EF Core migrations to the database
# Prerequisites: Docker containers must be running (docker compose up -d)

Write-Host "Applying Place Service migrations..." -ForegroundColor Cyan
dotnet ef database update `
    --project src/Services/TheDish.Place.Infrastructure/TheDish.Place.Infrastructure.csproj `
    --startup-project src/Services/TheDish.Place.API/TheDish.Place.API.csproj `
    --context PlaceDbContext

if ($LASTEXITCODE -eq 0) {
    Write-Host "Place Service migrations applied successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to apply Place Service migrations" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nApplying Review Service migrations..." -ForegroundColor Cyan
dotnet ef database update `
    --project src/Services/TheDish.Review.Infrastructure/TheDish.Review.Infrastructure.csproj `
    --startup-project src/Services/TheDish.Review.API/TheDish.Review.API.csproj `
    --context ReviewDbContext

if ($LASTEXITCODE -eq 0) {
    Write-Host "Review Service migrations applied successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to apply Review Service migrations" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nAll migrations applied successfully!" -ForegroundColor Green











