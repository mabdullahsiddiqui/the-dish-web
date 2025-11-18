# Script to start all backend services
# Prerequisites: Docker containers must be running (docker compose up -d) OR PostgreSQL must be running locally

Write-Host "Starting The Dish Backend Services..." -ForegroundColor Cyan
Write-Host ""

# Check for Docker
$dockerAvailable = $false
try {
    $null = docker --version 2>$null
    $dockerAvailable = $true
    Write-Host "[OK] Docker is available" -ForegroundColor Green
} catch {
    Write-Host "[X] Docker is not available (not in PATH)" -ForegroundColor Yellow
}

# Check if PostgreSQL is running
$postgresRunning = $false
try {
    $connection = New-Object System.Net.Sockets.TcpClient
    $connection.Connect("localhost", 5432)
    $connection.Close()
    $postgresRunning = $true
    Write-Host "[OK] PostgreSQL is running on port 5432" -ForegroundColor Green
} catch {
    Write-Host "[X] PostgreSQL is NOT running on port 5432" -ForegroundColor Red
}

# Check if User Service exists
$userServiceExists = Test-Path "$PSScriptRoot\..\src\Services\TheDish.User.API"

if (-not $postgresRunning) {
    Write-Host "`n[WARNING] PostgreSQL is not running!" -ForegroundColor Red
    Write-Host "You need to start PostgreSQL before running the services." -ForegroundColor Yellow
    Write-Host ""
    if ($dockerAvailable) {
        Write-Host "Option 1: Start Docker containers (recommended):" -ForegroundColor Cyan
        Write-Host "  cd .." -ForegroundColor White
        Write-Host "  docker compose up -d" -ForegroundColor White
        Write-Host ""
    }
    Write-Host "Option 2: Install and start PostgreSQL locally:" -ForegroundColor Cyan
    Write-Host "  - Install PostgreSQL from https://www.postgresql.org/download/" -ForegroundColor White
    Write-Host "  - Create database 'thedish' with user 'thedish' and password 'thedish_dev_password'" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to continue anyway (services will fail to connect)..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    Write-Host ""
}

Write-Host "`nServices will be available at:" -ForegroundColor Yellow
if ($userServiceExists) {
    Write-Host "  User Service:     http://localhost:5001" -ForegroundColor White
}
Write-Host "  Place Service:    http://localhost:5002" -ForegroundColor White
Write-Host "  Review Service:   http://localhost:5003" -ForegroundColor White
Write-Host "  API Gateway:      http://localhost:5000" -ForegroundColor White
Write-Host "`nSwagger UI:" -ForegroundColor Yellow
if ($userServiceExists) {
    Write-Host "  User Service:     http://localhost:5001/swagger" -ForegroundColor White
}
Write-Host "  Place Service:    http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  Review Service:   http://localhost:5003/swagger" -ForegroundColor White
Write-Host "  API Gateway:      http://localhost:5000/swagger" -ForegroundColor White

Write-Host "`nStarting services in separate windows..." -ForegroundColor Cyan

# Start User Service (if it exists)
if ($userServiceExists) {
    $userServicePath = "$PSScriptRoot\..\src\Services\TheDish.User.API"
    if (Test-Path $userServicePath) {
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$userServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5001"
        Write-Host "  [OK] Started User Service" -ForegroundColor Green
    }
} else {
    Write-Host "  - Skipped User Service (not found)" -ForegroundColor Gray
}

# Start Place Service
$placeServicePath = "$PSScriptRoot\..\src\Services\TheDish.Place.API"
if (Test-Path $placeServicePath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$placeServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5002"
    Write-Host "  [OK] Started Place Service" -ForegroundColor Green
} else {
    Write-Host "  [X] Place Service not found at: $placeServicePath" -ForegroundColor Red
}

# Start Review Service
$reviewServicePath = "$PSScriptRoot\..\src\Services\TheDish.Review.API"
if (Test-Path $reviewServicePath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$reviewServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5003"
    Write-Host "  [OK] Started Review Service" -ForegroundColor Green
} else {
    Write-Host "  [X] Review Service not found at: $reviewServicePath" -ForegroundColor Red
}

# Start API Gateway
$gatewayPath = "$PSScriptRoot\..\src\TheDish.ApiGateway"
if (Test-Path $gatewayPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$gatewayPath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5000"
    Write-Host "  [OK] Started API Gateway" -ForegroundColor Green
} else {
    Write-Host "  [X] API Gateway not found at: $gatewayPath" -ForegroundColor Red
}

Write-Host "`nAll available services started!" -ForegroundColor Green
Write-Host "Check the service windows for any errors." -ForegroundColor Yellow
Write-Host "Press any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")


