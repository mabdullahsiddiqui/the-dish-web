# Script to restart all backend services
# This kills existing processes and starts fresh instances

Write-Host "`n=== Restarting All The Dish Services ===" -ForegroundColor Cyan
Write-Host ""

# Function to kill process on a port
function Stop-ProcessOnPort {
    param([int]$Port)
    $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    if ($connection) {
        $processId = $connection.OwningProcess
        $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "  Stopping process on port $Port (PID: $processId)..." -ForegroundColor Yellow
            Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
            Start-Sleep -Seconds 1
        }
    }
}

# Stop all services
Write-Host "Stopping existing services..." -ForegroundColor Yellow
Stop-ProcessOnPort -Port 5001  # User Service
Stop-ProcessOnPort -Port 5002  # Place Service
Stop-ProcessOnPort -Port 5003  # Review Service
Stop-ProcessOnPort -Port 5000  # API Gateway
Write-Host "  All services stopped." -ForegroundColor Green
Write-Host ""

# Wait a moment
Start-Sleep -Seconds 2

# Check for Docker
$dockerAvailable = $false
try {
    $null = docker --version 2>$null
    $dockerAvailable = $true
    Write-Host "[OK] Docker is available" -ForegroundColor Green
} catch {
    Write-Host "[X] Docker is not available" -ForegroundColor Yellow
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
    if ($dockerAvailable) {
        Write-Host "  Starting Docker containers..." -ForegroundColor Yellow
        docker compose up -d
        Start-Sleep -Seconds 3
    }
}

Write-Host "`nStarting services..." -ForegroundColor Cyan

# Start User Service
$userServicePath = "$PSScriptRoot\..\src\Services\TheDish.User.API"
if (Test-Path $userServicePath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$userServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5001"
    Write-Host "  [OK] Started User Service on http://localhost:5001" -ForegroundColor Green
} else {
    Write-Host "  [X] User Service not found" -ForegroundColor Red
}

# Start Place Service
$placeServicePath = "$PSScriptRoot\..\src\Services\TheDish.Place.API"
if (Test-Path $placeServicePath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$placeServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5002"
    Write-Host "  [OK] Started Place Service on http://localhost:5002" -ForegroundColor Green
} else {
    Write-Host "  [X] Place Service not found" -ForegroundColor Red
}

# Start Review Service
$reviewServicePath = "$PSScriptRoot\..\src\Services\TheDish.Review.API"
if (Test-Path $reviewServicePath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$reviewServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5003"
    Write-Host "  [OK] Started Review Service on http://localhost:5003" -ForegroundColor Green
} else {
    Write-Host "  [X] Review Service not found" -ForegroundColor Red
}

# Start API Gateway
$gatewayPath = "$PSScriptRoot\..\src\TheDish.ApiGateway"
if (Test-Path $gatewayPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$gatewayPath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5000"
    Write-Host "  [OK] Started API Gateway on http://localhost:5000" -ForegroundColor Green
} else {
    Write-Host "  [X] API Gateway not found" -ForegroundColor Red
}

Write-Host "`n=== Services Started ===" -ForegroundColor Green
Write-Host "`nService URLs:" -ForegroundColor Cyan
Write-Host "  User Service:     http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  Place Service:    http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  Review Service:   http://localhost:5003/swagger" -ForegroundColor White
Write-Host "  API Gateway:      http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  Web App:          http://localhost:3000" -ForegroundColor White

Write-Host "`nWaiting for services to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host "`n✅ All services restarted!" -ForegroundColor Green
Write-Host "`nNext: Start the web app with 'npm run dev' in the web/ directory" -ForegroundColor Cyan
Write-Host "Press any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

