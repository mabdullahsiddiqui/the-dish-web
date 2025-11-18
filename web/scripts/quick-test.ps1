# Quick Test Script - Verifies basic functionality
# This script helps you quickly verify the application is working

Write-Host "=== The Dish - Quick Test Script ===" -ForegroundColor Cyan
Write-Host ""

# Check if backend services are running
Write-Host "1. Checking backend services..." -ForegroundColor Yellow
$backendCheck = & "$PSScriptRoot\check-backend.ps1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠ Backend services check failed" -ForegroundColor Red
} else {
    Write-Host "✓ Backend services check passed" -ForegroundColor Green
}

Write-Host ""
Write-Host "2. Testing API connectivity..." -ForegroundColor Yellow

$apiBaseUrl = "http://localhost:5000/api/v1"

# Test API Gateway health
try {
    $response = Invoke-WebRequest -Uri "$apiBaseUrl/health" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ API Gateway is accessible" -ForegroundColor Green
    }
} catch {
    Write-Host "✗ API Gateway health check failed" -ForegroundColor Red
    Write-Host "  Note: Health endpoint may not exist, but API Gateway should still work" -ForegroundColor Gray
}

Write-Host ""
Write-Host "3. Testing web application..." -ForegroundColor Yellow

# Check if web app is running
try {
    $webResponse = Invoke-WebRequest -Uri "http://localhost:3000" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    if ($webResponse.StatusCode -eq 200) {
        Write-Host "✓ Web application is running" -ForegroundColor Green
    }
} catch {
    Write-Host "✗ Web application is not running" -ForegroundColor Red
    Write-Host "  Start it with: npm run dev" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Test Summary ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Open http://localhost:3000 in your browser" -ForegroundColor White
Write-Host "2. Check the backend health widget (bottom-right)" -ForegroundColor White
Write-Host "3. Follow the testing checklist: TESTING_CHECKLIST.md" -ForegroundColor White
Write-Host "4. Test user registration and login" -ForegroundColor White
Write-Host "5. Test search functionality" -ForegroundColor White
Write-Host "6. Test review submission" -ForegroundColor White
Write-Host ""
Write-Host "For detailed testing guide, see: INTEGRATION_TESTING.md" -ForegroundColor Cyan
Write-Host ""

