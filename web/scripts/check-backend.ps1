# Script to check if backend services are running
# This verifies connectivity to the API Gateway and all microservices

Write-Host "Checking The Dish Backend Services..." -ForegroundColor Cyan
Write-Host ""

$services = @(
    @{ Name = "API Gateway"; Url = "http://localhost:5000/swagger/index.html" }
    @{ Name = "User Service"; Url = "http://localhost:5001/swagger/index.html" }
    @{ Name = "Place Service"; Url = "http://localhost:5002/swagger/index.html" }
    @{ Name = "Review Service"; Url = "http://localhost:5003/swagger/index.html" }
)

$allRunning = $true

foreach ($service in $services) {
    try {
        $response = Invoke-WebRequest -Uri $service.Url -Method Get -TimeoutSec 2 -UseBasicParsing -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            Write-Host "OK $($service.Name) is running" -ForegroundColor Green
        } else {
            Write-Host "X $($service.Name) returned status $($response.StatusCode)" -ForegroundColor Yellow
            $allRunning = $false
        }
    } catch {
        Write-Host "X $($service.Name) is not accessible" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Gray
        $allRunning = $false
    }
}

Write-Host ""
if ($allRunning) {
    Write-Host "All backend services are running!" -ForegroundColor Green
    Write-Host ""
    Write-Host "API Gateway: http://localhost:5000/api/v1" -ForegroundColor Cyan
    Write-Host "Swagger UI:  http://localhost:5000/swagger" -ForegroundColor Cyan
} else {
    Write-Host "Some services are not running. Please start them using:" -ForegroundColor Yellow
    Write-Host "  cd ..\backend" -ForegroundColor White
    Write-Host "  .\scripts\start-services.ps1" -ForegroundColor White
    Write-Host ""
    Write-Host "Or manually start Docker containers first:" -ForegroundColor Yellow
    Write-Host "  cd ..\backend" -ForegroundColor White
    Write-Host "  docker compose up -d" -ForegroundColor White
}

Write-Host ""
