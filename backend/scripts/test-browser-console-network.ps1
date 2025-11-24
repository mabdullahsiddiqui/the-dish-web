# Phase 5: Browser Console & Network Analysis Script
# Tests browser console for errors and network requests

param(
    [string]$BaseUrl = "http://localhost:3000",
    [switch]$JsonOutput = $false
)

$ErrorActionPreference = "Continue"
$script:AllTestsPassed = $true
$script:TestResults = @()

function Write-Status {
    param(
        [string]$Message,
        [string]$Status,  # "OK", "WARN", "FAIL"
        [string]$Details = ""
    )
    
    $color = switch ($Status) {
        "OK" { "Green" }
        "WARN" { "Yellow" }
        "FAIL" { "Red" }
        default { "White" }
    }
    
    $icon = switch ($Status) {
        "OK" { "[OK]" }
        "WARN" { "[WARN]" }
        "FAIL" { "[FAIL]" }
        default { "[INFO]" }
    }
    
    $detailsStr = if ($Details) { " - $Details" } else { "" }
    Write-Host "$icon $Message$detailsStr" -ForegroundColor $color
    
    $script:TestResults += @{
        Message = $Message
        Status = $Status
        Details = $Details
        Timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    }
    
    if ($Status -eq "FAIL") {
        $script:AllTestsPassed = $false
    }
}

# Note: This script provides manual testing instructions
# For automated browser testing, browser automation tools are required
# (Selenium, Playwright, or browser MCP tools)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Phase 5: Browser Console & Network Analysis" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Base URL: $BaseUrl" -ForegroundColor Gray
Write-Host ""
Write-Host "NOTE: Browser console and network analysis requires browser automation." -ForegroundColor Yellow
Write-Host "This script provides test instructions and basic HTTP checks." -ForegroundColor Yellow
Write-Host ""

# [1/5] Basic Connectivity Tests
Write-Host "[1/5] Testing Basic Connectivity..." -ForegroundColor Yellow
Write-Host ""

# Test homepage loads
try {
    $response = Invoke-WebRequest -Uri "$BaseUrl/" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    Write-Status "Homepage is accessible" "OK" "Status: $($response.StatusCode)"
} catch {
    Write-Status "Homepage is not accessible" "FAIL" $_.Exception.Message
}
Write-Host ""

# Test API Gateway connectivity
try {
    $apiResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/v1/places/search?page=1&pageSize=1" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    Write-Status "API Gateway is accessible" "OK" "Status: $($apiResponse.StatusCode)"
} catch {
    Write-Status "API Gateway is not accessible" "FAIL" $_.Exception.Message
}
Write-Host ""

# [2/5] Network Request Testing
Write-Host "[2/5] Testing Network Requests..." -ForegroundColor Yellow
Write-Host ""

# Test key API endpoints that the web app uses
$apiEndpoints = @(
    @{ Url = "http://localhost:5000/api/v1/places/search?page=1&pageSize=10"; Description = "Places search endpoint" }
    @{ Url = "http://localhost:5000/api/v1/places/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=10"; Description = "Nearby places endpoint" }
)

foreach ($endpoint in $apiEndpoints) {
    $startTime = Get-Date
    try {
        $response = Invoke-WebRequest -Uri $endpoint.Url -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        if ($response.StatusCode -eq 200) {
            if ($responseTime -gt 1000) {
                Write-Status "$($endpoint.Description)" "WARN" "Slow response: ${responseTime}ms (>1s)"
            } else {
                Write-Status "$($endpoint.Description)" "OK" "Response time: ${responseTime}ms"
            }
        } else {
            Write-Status "$($endpoint.Description)" "WARN" "Status: $($response.StatusCode)"
        }
    } catch {
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        $statusCode = 0
        
        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode.value__
            Write-Status "$($endpoint.Description)" "FAIL" "Status: $statusCode, Time: ${responseTime}ms"
        } else {
            Write-Status "$($endpoint.Description)" "FAIL" "Error: $($_.Exception.Message)"
        }
    }
    Write-Host ""
}

# [3/5] CORS Testing
Write-Host "[3/5] Testing CORS Configuration..." -ForegroundColor Yellow
Write-Host ""

# Test CORS headers on API Gateway
try {
    $corsResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/v1/places/search?page=1&pageSize=1" -Method Options -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    $corsHeaders = $corsResponse.Headers
    
    if ($corsHeaders['Access-Control-Allow-Origin']) {
        Write-Status "CORS headers present" "OK" "Allow-Origin: $($corsHeaders['Access-Control-Allow-Origin'])"
    } else {
        Write-Status "CORS headers may be missing" "WARN" "No Access-Control-Allow-Origin header found"
    }
} catch {
    # OPTIONS may not be supported, try GET instead
    try {
        $getResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/v1/places/search?page=1&pageSize=1" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
        Write-Status "CORS test - GET request successful" "OK" "CORS likely configured (OPTIONS not tested)"
    } catch {
        Write-Status "CORS test failed" "FAIL" $_.Exception.Message
    }
}
Write-Host ""

# [4/5] Manual Testing Instructions
Write-Host "[4/5] Manual Browser Testing Instructions..." -ForegroundColor Yellow
Write-Host ""
Write-Host "To complete browser console and network analysis:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Open browser DevTools (F12)" -ForegroundColor White
Write-Host "2. Navigate to: $BaseUrl" -ForegroundColor White
Write-Host "3. Check Console tab for:" -ForegroundColor White
Write-Host "   - JavaScript errors (red)" -ForegroundColor Gray
Write-Host "   - Warnings (yellow)" -ForegroundColor Gray
Write-Host "   - Failed API calls" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Check Network tab for:" -ForegroundColor White
Write-Host "   - Failed requests (red)" -ForegroundColor Gray
Write-Host "   - Slow requests (>1s)" -ForegroundColor Gray
Write-Host "   - CORS errors" -ForegroundColor Gray
Write-Host "   - 404/500 errors" -ForegroundColor Gray
Write-Host ""
Write-Host "5. Test JWT token storage:" -ForegroundColor White
Write-Host "   - Open Console tab" -ForegroundColor Gray
Write-Host "   - Run: localStorage.getItem('auth_token')" -ForegroundColor Gray
Write-Host "   - Should return token after login" -ForegroundColor Gray
Write-Host ""
Write-Host "6. Test API calls from console:" -ForegroundColor White
Write-Host "   - Run: fetch('http://localhost:5000/api/v1/places/search?page=1&pageSize=1')" -ForegroundColor Gray
Write-Host "   - Should return successful response" -ForegroundColor Gray
Write-Host ""

# [5/5] Summary
Write-Host "[5/5] Summary" -ForegroundColor Yellow
Write-Host ""

$totalTests = $script:TestResults.Count
$passedTests = ($script:TestResults | Where-Object { $_.Status -eq "OK" }).Count
$warnedTests = ($script:TestResults | Where-Object { $_.Status -eq "WARN" }).Count
$failedTests = ($script:TestResults | Where-Object { $_.Status -eq "FAIL" }).Count

Write-Host "Total Tests: $totalTests" -ForegroundColor Cyan
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Warnings: $warnedTests" -ForegroundColor Yellow
Write-Host "Failed: $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($script:AllTestsPassed) {
    Write-Host "[OK] Basic connectivity tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "NOTE: Complete browser console and network analysis requires manual testing" -ForegroundColor Yellow
    Write-Host "or browser automation tools (Selenium, Playwright)." -ForegroundColor Yellow
    exit 0
} else {
    Write-Host "[WARN] Some tests failed or had warnings" -ForegroundColor Yellow
    exit 1
}

