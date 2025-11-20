# Script to restart User Service
# This kills any existing process on port 5001 and starts a fresh instance

Write-Host "Restarting User Service..." -ForegroundColor Cyan

# Find and kill process on port 5001
$connection = Get-NetTCPConnection -LocalPort 5001 -ErrorAction SilentlyContinue
if ($connection) {
    $processId = $connection.OwningProcess
    Write-Host "Found process $processId on port 5001, stopping it..." -ForegroundColor Yellow
    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Write-Host "Process stopped." -ForegroundColor Green
} else {
    Write-Host "No process found on port 5001." -ForegroundColor Green
}

# Start User Service
$userServicePath = "$PSScriptRoot\..\src\Services\TheDish.User.API"
if (Test-Path $userServicePath) {
    Write-Host "Starting User Service..." -ForegroundColor Cyan
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$userServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls http://localhost:5001"
    Write-Host "User Service started on http://localhost:5001" -ForegroundColor Green
    Write-Host "Swagger UI: http://localhost:5001/swagger" -ForegroundColor Green
} else {
    Write-Host "User Service not found at: $userServicePath" -ForegroundColor Red
}

Write-Host "`nPress any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

