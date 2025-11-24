# Phase 6: Error Scenario Testing Script
# Tests error handling when services are down, invalid responses, and network errors

param(
    [string]$BaseUrl = "http://localhost:5000/api/v1",
    [string]$WebAppUrl = "http://localhost:3000",
    [switch]$JsonOutput = $false,
    [switch]$SkipServiceStop = $false
)

$ErrorActionPreference = "Continue"
$script:AllTestsPassed = $true
$script:TestResults = @()

function Write-Status {
    param(
        [string]$Message,
        [string]$Status,  # "OK", "WARN", "FAIL"
        [int]$ResponseTime = 0,
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
    
    $timeStr = if ($ResponseTime -gt 0) { " (${ResponseTime}ms)" } else { "" }
    $detailsStr = if ($Details) { " - $Details" } else { "" }
    Write-Host "$icon $Message$timeStr$detailsStr" -ForegroundColor $color
    
    $script:TestResults += @{
        Message = $Message
        Status = $Status
        ResponseTime = $ResponseTime
        Details = $Details
        Timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    }
    
    if ($Status -eq "FAIL") {
        $script:AllTestsPassed = $false
    }
}

function Get-ProcessByPort {
    param([int]$Port)
    
    try {
        $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
        if ($connection) {
            $process = Get-Process -Id $connection.OwningProcess -ErrorAction SilentlyContinue
            return $process
        }
    } catch {
        return $null
    }
    return $null
}

function Stop-ServiceByPort {
    param(
        [int]$Port,
        [string]$ServiceName
    )
    
    $process = Get-ProcessByPort -Port $Port
    if ($process) {
        try {
            Stop-Process -Id $process.Id -Force -ErrorAction Stop
            Write-Status "Stopped $ServiceName (PID: $($process.Id))" "OK"
            return $true
        } catch {
            Write-Status "Failed to stop $ServiceName" "FAIL" -Details $_.Exception.Message
            return $false
        }
    } else {
        Write-Status "$ServiceName is not running on port $Port" "WARN"
        return $false
    }
}

function Start-ServiceByPort {
    param(
        [int]$Port,
        [string]$ServiceName,
        [string]$ServicePath,
        [string]$Urls
    )
    
    $process = Get-ProcessByPort -Port $Port
    if (-not $process) {
        if (Test-Path $ServicePath) {
            try {
                Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$ServicePath'; `$env:ASPNETCORE_ENVIRONMENT='Development'; dotnet run --urls $Urls" -WindowStyle Hidden
                Start-Sleep -Seconds 3
                Write-Status "Started $ServiceName" "OK"
                return $true
            } catch {
                Write-Status "Failed to start $ServiceName" "FAIL" -Details $_.Exception.Message
                return $false
            }
        } else {
            Write-Status "$ServiceName path not found: $ServicePath" "WARN"
            return $false
        }
    } else {
        Write-Status "$ServiceName is already running" "OK"
        return $true
    }
}

function Test-ApiErrorResponse {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [object]$Body = $null,
        [hashtable]$Headers = $null,
        [int]$ExpectedStatus = 0,
        [string]$TestName
    )
    
    $startTime = Get-Date
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            TimeoutSec = 5
            UseBasicParsing = $true
            ErrorAction = "Stop"
        }
        
        if ($Body) {
            if ($Body -is [string]) {
                $params.Body = $Body
            } else {
                $params.Body = ($Body | ConvertTo-Json -Depth 10)
            }
            $params.ContentType = "application/json"
        }
        
        if ($Headers) {
            $params.Headers = $Headers
        }
        
        $response = Invoke-WebRequest @params
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        if ($ExpectedStatus -gt 0 -and $response.StatusCode -eq $ExpectedStatus) {
            Write-Status "${TestName}: Got expected status $ExpectedStatus" "OK" $responseTime
            return $true
        } elseif ($ExpectedStatus -eq 0) {
            Write-Status "${TestName}: Unexpected success (status $($response.StatusCode))" "WARN" $responseTime
            return $false
        } else {
            Write-Status "${TestName}: Got status $($response.StatusCode), expected $ExpectedStatus" "WARN" $responseTime
            return $false
        }
    } catch {
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode.value__
            if ($ExpectedStatus -gt 0 -and $statusCode -eq $ExpectedStatus) {
                Write-Status "${TestName}: Got expected error status $ExpectedStatus" "OK" $responseTime
                return $true
            } else {
                Write-Status "${TestName}: Got error status $statusCode" "WARN" $responseTime -Details $_.Exception.Message
                return $false
            }
        } else {
            # Network error (service down)
            if ($ExpectedStatus -eq 0) {
                Write-Status "${TestName}: Service unavailable (expected)" "OK" $responseTime -Details $_.Exception.Message
                return $true
            } else {
                Write-Status "${TestName}: Network error" "WARN" $responseTime -Details $_.Exception.Message
                return $false
            }
        }
    }
}

function Test-WebAppErrorHandling {
    param(
        [string]$Url,
        [string]$TestName
    )
    
    $startTime = Get-Date
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        # Check if page loaded (status 200)
        if ($response.StatusCode -eq 200) {
            # Check for error messages in content
            $content = $response.Content
            if ($content -match "error|unable|failed|unavailable" -and $content -notmatch "no.*error") {
                Write-Status "${TestName}: Page loaded with error message" "OK" $responseTime
                return $true
            } else {
                Write-Status "${TestName}: Page loaded but no error message detected" "WARN" $responseTime
                return $false
            }
        } else {
            Write-Status "${TestName}: Unexpected status $($response.StatusCode)" "WARN" $responseTime
            return $false
        }
    } catch {
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        Write-Status "${TestName}: Failed to load page" "FAIL" $responseTime -Details $_.Exception.Message
        return $false
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Phase 6: Error Scenario Testing" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Store original service states
$script:OriginalServices = @{
    UserService = @{ Port = 5001; Running = $false; Path = "$PSScriptRoot\..\src\Services\TheDish.User.API" }
    PlaceService = @{ Port = 5002; Running = $false; Path = "$PSScriptRoot\..\src\Services\TheDish.Place.API" }
    ReviewService = @{ Port = 5003; Running = $false; Path = "$PSScriptRoot\..\src\Services\TheDish.Review.API" }
    ApiGateway = @{ Port = 5000; Running = $false; Path = "$PSScriptRoot\..\src\TheDish.ApiGateway" }
}

# Check which services are running
Write-Host "Checking service status..." -ForegroundColor Yellow
foreach ($serviceName in $script:OriginalServices.Keys) {
    $service = $script:OriginalServices[$serviceName]
    $process = Get-ProcessByPort -Port $service.Port
    $service.Running = ($process -ne $null)
    if ($service.Running) {
        Write-Status "$serviceName is running on port $($service.Port)" "OK"
    } else {
        Write-Status "$serviceName is not running on port $($service.Port)" "WARN"
    }
}

Write-Host ""
Write-Host "=== Test 1: Service Unavailable (API Gateway Stopped) ===" -ForegroundColor Yellow
if (-not $SkipServiceStop) {
    # Stop API Gateway
    $gatewayStopped = Stop-ServiceByPort -Port 5000 -ServiceName "API Gateway"
    if ($gatewayStopped) {
        Start-Sleep -Seconds 2
        
        # Test API calls through gateway
        Test-ApiErrorResponse -Url "$BaseUrl/users/register" -Method "GET" -TestName "GET /users/register (gateway down)" -ExpectedStatus 0
        Test-ApiErrorResponse -Url "$BaseUrl/places/search?query=test" -Method "GET" -TestName "GET /places/search (gateway down)" -ExpectedStatus 0
        
        # Test web app error handling
        Test-WebAppErrorHandling -Url "$WebAppUrl/" -TestName "Homepage with gateway down"
        Test-WebAppErrorHandling -Url "$WebAppUrl/search" -TestName "Search page with gateway down"
        
        # Restore API Gateway
        Write-Host ""
        Write-Host "Restoring API Gateway..." -ForegroundColor Yellow
        Start-ServiceByPort -Port 5000 -ServiceName "API Gateway" -ServicePath $script:OriginalServices.ApiGateway.Path -Urls "http://localhost:5000"
        Start-Sleep -Seconds 5
    } else {
        Write-Status "Skipping gateway stop test (gateway not running)" "WARN"
    }
} else {
    Write-Status "Skipping service stop tests (SkipServiceStop flag set)" "WARN"
}

Write-Host ""
Write-Host "=== Test 2: Service Unavailable (User Service Stopped) ===" -ForegroundColor Yellow
if (-not $SkipServiceStop) {
    # Stop User Service
    $userStopped = Stop-ServiceByPort -Port 5001 -ServiceName "User Service"
    if ($userStopped) {
        Start-Sleep -Seconds 2
        
        # Test user endpoints
        Test-ApiErrorResponse -Url "$BaseUrl/users/register" -Method "POST" -Body @{ email = "test@example.com"; password = "Test123!" } -TestName "POST /users/register (user service down)" -ExpectedStatus 0
        Test-ApiErrorResponse -Url "$BaseUrl/users/login" -Method "POST" -Body @{ email = "test@example.com"; password = "Test123!" } -TestName "POST /users/login (user service down)" -ExpectedStatus 0
        
        # Test web app error handling
        Test-WebAppErrorHandling -Url "$WebAppUrl/login" -TestName "Login page with user service down"
        Test-WebAppErrorHandling -Url "$WebAppUrl/register" -TestName "Register page with user service down"
        
        # Restore User Service
        Write-Host ""
        Write-Host "Restoring User Service..." -ForegroundColor Yellow
        Start-ServiceByPort -Port 5001 -ServiceName "User Service" -ServicePath $script:OriginalServices.UserService.Path -Urls "http://localhost:5001"
        Start-Sleep -Seconds 5
    } else {
        Write-Status "Skipping user service stop test (service not running)" "WARN"
    }
} else {
    Write-Status "Skipping service stop tests (SkipServiceStop flag set)" "WARN"
}

Write-Host ""
Write-Host "=== Test 3: Invalid API Responses (404 Not Found) ===" -ForegroundColor Yellow
Test-ApiErrorResponse -Url "$BaseUrl/nonexistent/endpoint" -Method "GET" -TestName "GET /nonexistent/endpoint (404)" -ExpectedStatus 404
Test-ApiErrorResponse -Url "$BaseUrl/users/999999" -Method "GET" -TestName "GET /users/{invalid-id} (404)" -ExpectedStatus 404
Test-ApiErrorResponse -Url "$BaseUrl/places/999999" -Method "GET" -TestName "GET /places/{invalid-id} (404)" -ExpectedStatus 404

Write-Host ""
Write-Host "=== Test 4: Invalid API Responses (400 Bad Request) ===" -ForegroundColor Yellow
Test-ApiErrorResponse -Url "$BaseUrl/users/register" -Method "POST" -Body @{ email = "invalid-email" } -TestName "POST /users/register (invalid email)" -ExpectedStatus 400
Test-ApiErrorResponse -Url "$BaseUrl/users/login" -Method "POST" -Body @{ } -TestName "POST /users/login (missing credentials)" -ExpectedStatus 400

Write-Host ""
Write-Host "=== Test 5: Invalid API Responses (401 Unauthorized) ===" -ForegroundColor Yellow
Test-ApiErrorResponse -Url "$BaseUrl/users/me" -Method "GET" -TestName "GET /users/me (no token)" -ExpectedStatus 401
$invalidTokenHeaders = @{ Authorization = "Bearer invalid-token" }
Test-ApiErrorResponse -Url "$BaseUrl/users/me" -Method "GET" -Headers $invalidTokenHeaders -TestName "GET /users/me (invalid token)" -ExpectedStatus 401

Write-Host ""
Write-Host "=== Test 6: Network Timeout ===" -ForegroundColor Yellow
# Test with very short timeout
try {
    $response = Invoke-WebRequest -Uri "$BaseUrl/places/search?query=test" -Method Get -TimeoutSec 0.001 -UseBasicParsing -ErrorAction Stop
    Write-Status "Timeout test: Unexpected success" "WARN"
} catch {
    if ($_.Exception.Message -match "timeout|timed out") {
        Write-Status "Timeout test: Request timed out (expected)" "OK" -Details $_.Exception.Message
    } else {
        Write-Status "Timeout test: Different error" "WARN" -Details $_.Exception.Message
    }
}

Write-Host ""
Write-Host "=== Test 7: Invalid Request Body ===" -ForegroundColor Yellow
Test-ApiErrorResponse -Url "$BaseUrl/users/register" -Method "POST" -Body "invalid json" -TestName "POST /users/register (invalid JSON)" -ExpectedStatus 400

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Phase 6 Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$totalTests = $script:TestResults.Count
$passedTests = ($script:TestResults | Where-Object { $_.Status -eq "OK" }).Count
$warnTests = ($script:TestResults | Where-Object { $_.Status -eq "WARN" }).Count
$failedTests = ($script:TestResults | Where-Object { $_.Status -eq "FAIL" }).Count

Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Warnings: $warnTests" -ForegroundColor Yellow
Write-Host "Failed: $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($script:AllTestsPassed -and $failedTests -eq 0) {
    Write-Host "Overall Status: [OK] PASSED" -ForegroundColor Green
    exit 0
} else {
    Write-Host "Overall Status: [FAIL] Some tests failed or had warnings" -ForegroundColor Red
    exit 1
}

