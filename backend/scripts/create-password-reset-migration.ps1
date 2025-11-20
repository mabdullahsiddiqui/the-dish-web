# Script to create database migration for password reset fields
# IMPORTANT: Stop the User Service before running this script

Write-Host "`n=== Creating Password Reset Migration ===" -ForegroundColor Cyan
Write-Host "`n⚠️ IMPORTANT: Stop the User Service before running this script!" -ForegroundColor Yellow
Write-Host "   The service must be stopped to avoid file locking issues." -ForegroundColor Yellow
Write-Host ""

$userServicePath = "$PSScriptRoot\..\src\Services\TheDish.User.API"
$infrastructurePath = "$PSScriptRoot\..\src\Services\TheDish.User.Infrastructure"

if (Test-Path $userServicePath) {
    Write-Host "Creating migration..." -ForegroundColor Yellow
    cd $userServicePath
    
    dotnet ef migrations add AddPasswordResetFields `
        --project $infrastructurePath `
        --startup-project . `
        --context UserDbContext
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✅ Migration created successfully!" -ForegroundColor Green
        Write-Host "`nNext steps:" -ForegroundColor Cyan
        Write-Host "  1. Review the migration file in:" -ForegroundColor White
        Write-Host "     $infrastructurePath\Migrations" -ForegroundColor Gray
        Write-Host "  2. Apply the migration (database will be updated automatically on next User Service start)" -ForegroundColor White
        Write-Host "     OR run: dotnet ef database update --project $infrastructurePath --startup-project ." -ForegroundColor Gray
    } else {
        Write-Host "`n❌ Migration creation failed. Make sure User Service is stopped." -ForegroundColor Red
    }
} else {
    Write-Host "❌ User Service project not found at: $userServicePath" -ForegroundColor Red
    exit 1
}

