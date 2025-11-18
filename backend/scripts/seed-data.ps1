# Script to seed test data into the database
# Prerequisites: Docker containers must be running and migrations applied

Write-Host "🌱 The Dish - Data Seeder" -ForegroundColor Cyan
Write-Host "========================`n" -ForegroundColor Cyan

# Check if Docker is running
$dockerRunning = $false
try {
    $null = docker ps 2>$null
    $dockerRunning = $true
    Write-Host "[OK] Docker is running" -ForegroundColor Green
} catch {
    Write-Host "[X] Docker is not running" -ForegroundColor Red
    Write-Host "Please start Docker containers first:" -ForegroundColor Yellow
    Write-Host "  docker compose up -d" -ForegroundColor White
    exit 1
}

# Check if PostgreSQL is accessible
$postgresRunning = $false
try {
    $connection = New-Object System.Net.Sockets.TcpClient
    $connection.Connect("localhost", 5432)
    $connection.Close()
    $postgresRunning = $true
    Write-Host "[OK] PostgreSQL is accessible" -ForegroundColor Green
} catch {
    Write-Host "[X] PostgreSQL is not accessible on port 5432" -ForegroundColor Red
    Write-Host "Please ensure Docker containers are running:" -ForegroundColor Yellow
    Write-Host "  docker compose up -d" -ForegroundColor White
    exit 1
}

# Navigate to seeder project
$seederPath = "$PSScriptRoot\..\src\Tools\TheDish.DataSeeder"

if (-not (Test-Path $seederPath)) {
    Write-Host "[X] DataSeeder project not found at: $seederPath" -ForegroundColor Red
    Write-Host "Please ensure the project exists." -ForegroundColor Yellow
    exit 1
}

Write-Host "`nRunning data seeder..." -ForegroundColor Yellow
Write-Host ""

# Run the seeder
cd $seederPath
dotnet run

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Data seeding completed successfully!" -ForegroundColor Green
    Write-Host "`nYou can now:" -ForegroundColor Cyan
    Write-Host "  1. Search for restaurants in the web app" -ForegroundColor White
    Write-Host "  2. View place details" -ForegroundColor White
    Write-Host "  3. Create reviews for these places" -ForegroundColor White
} else {
    Write-Host "`n❌ Data seeding failed!" -ForegroundColor Red
    Write-Host "Check the error messages above." -ForegroundColor Yellow
    exit $LASTEXITCODE
}




