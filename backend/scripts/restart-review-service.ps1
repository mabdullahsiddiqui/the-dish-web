# Script to restart Review Service
# This kills any existing process on port 5003 and starts a fresh instance

Write-Host "Restarting Review Service..." -ForegroundColor Cyan

# Find and kill process on port 5003
$connection = Get-NetTCPConnection -LocalPort 5003 -ErrorAction SilentlyContinue
if ($connection) {
    $processId = $connection.OwningProcess
    Write-Host "Found process $processId on port 5003, stopping it..." -ForegroundColor Yellow
    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Write-Host "Process stopped." -ForegroundColor Green
} else {
    Write-Host "No process found on port 5003." -ForegroundColor Green
}

# Start Review Service
$reviewServicePath = "$PSScriptRoot\..\src\Services\TheDish.Review.API"
if (Test-Path $reviewServicePath) {
    Write-Host "Starting Review Service..." -ForegroundColor Cyan
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$reviewServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5003"
    Write-Host "Review Service started on http://localhost:5003" -ForegroundColor Green
    Write-Host "Swagger UI: http://localhost:5003/swagger" -ForegroundColor Green
} else {
    Write-Host "Review Service not found at: $reviewServicePath" -ForegroundColor Red
}

Write-Host "`nPress any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")


