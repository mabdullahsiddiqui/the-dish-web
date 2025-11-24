# Phase 3: API Endpoint Testing Script
# Tests all API endpoints through the API Gateway

param(
    [string]$BaseUrl = "http://localhost:5000/api/v1",
    [switch]$JsonOutput = $false
)

$ErrorActionPreference = "Continue"
$script:AllTestsPassed = $true
$script:TestResults = @()
$script:TestUser = $null
$script:TestToken = $null
$script:TestPlaceId = $null
$script:TestReviewId = $null

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

function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Endpoint,
        [hashtable]$Body = $null,
        [string]$Token = $null,
        [int]$ExpectedStatus = 200,
        [string]$Description
    )
    
    $startTime = Get-Date
    $url = "$BaseUrl/$Endpoint"
    
    try {
        $headers = @{
            "Content-Type" = "application/json"
        }
        
        if ($Token) {
            $headers["Authorization"] = "Bearer $Token"
        }
        
        $params = @{
            Uri = $url
            Method = $Method
            Headers = $headers
            TimeoutSec = 10
            UseBasicParsing = $true
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-WebRequest @params -ErrorAction Stop
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        $responseBody = $null
        try {
            $responseBody = $response.Content | ConvertFrom-Json
        } catch {
            $responseBody = $response.Content
        }
        
        if ($response.StatusCode -eq $ExpectedStatus) {
            Write-Status "$Description" "OK" $responseTime
            return $true, $responseBody, $response.StatusCode
        } else {
            Write-Status "$Description - Expected $ExpectedStatus, got $($response.StatusCode)" "WARN" $responseTime
            return $false, $responseBody, $response.StatusCode
        }
    } catch {
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        $statusCode = 0
        
        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode.value__
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $responseBody = $reader.ReadToEnd() | ConvertFrom-Json -ErrorAction SilentlyContinue
            
            if ($statusCode -eq $ExpectedStatus) {
                Write-Status "$Description (expected error)" "OK" $responseTime
                return $true, $responseBody, $statusCode
            } else {
                Write-Status "$Description - Error: $($_.Exception.Message) (Status: $statusCode)" "FAIL" $responseTime
                return $false, $responseBody, $statusCode
            }
        } else {
            Write-Status "$Description - Exception: $($_.Exception.Message)" "FAIL" $responseTime
            return $false, $null, 0
        }
    }
}

# Main execution
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Phase 3: API Endpoint Testing" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Base URL: $BaseUrl" -ForegroundColor Gray
Write-Host ""

# [1/6] Test User Service Endpoints
Write-Host "[1/6] Testing User Service Endpoints..." -ForegroundColor Yellow
Write-Host ""

# Test User Registration
$testEmail = "test_$(Get-Date -Format 'yyyyMMddHHmmss')@test.com"
$registerBody = @{
    email = $testEmail
    password = "Test123!"
    firstName = "Test"
    lastName = "User"
}

$success, $registerResponse, $statusCode = Invoke-ApiRequest -Method "POST" -Endpoint "users/register" -Body $registerBody -Description "POST /users/register - User registration"

if ($success -and $registerResponse -and $registerResponse.success) {
    $script:TestUser = $registerResponse.data.user
    $script:TestToken = $registerResponse.data.token
    Write-Host "   User ID: $($script:TestUser.id)" -ForegroundColor Gray
    Write-Host "   Token received: $($script:TestToken.Substring(0, [Math]::Min(20, $script:TestToken.Length)))..." -ForegroundColor Gray
} else {
    Write-Host "   [WARN] Registration failed, some tests may be skipped" -ForegroundColor Yellow
}

Write-Host ""

# Test User Login with valid credentials
if ($script:TestUser) {
    $loginBody = @{
        email = $testEmail
        password = "Test123!"
    }
    
    $success, $loginResponse, $statusCode = Invoke-ApiRequest -Method "POST" -Endpoint "users/login" -Body $loginBody -Description "POST /users/login - Valid credentials"
    
    if ($success -and $loginResponse -and $loginResponse.success) {
        $script:TestToken = $loginResponse.data.token
    }
    Write-Host ""
}

# Test User Login with invalid credentials
$invalidLoginBody = @{
    email = "invalid@test.com"
    password = "WrongPassword123!"
}

$success, $invalidLoginResponse, $statusCode = Invoke-ApiRequest -Method "POST" -Endpoint "users/login" -Body $invalidLoginBody -ExpectedStatus 401 -Description "POST /users/login - Invalid credentials (should fail)"

Write-Host ""

# Test Get User by ID
if ($script:TestUser -and $script:TestUser.id) {
    $success, $userResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "users/$($script:TestUser.id)" -Token $script:TestToken -Description "GET /users/{id} - Get user profile"
    Write-Host ""
}

# Test Forgot Password
$forgotPasswordBody = @{
    email = $testEmail
}

$success, $forgotPasswordResponse, $statusCode = Invoke-ApiRequest -Method "POST" -Endpoint "users/forgot-password" -Body $forgotPasswordBody -Description "POST /users/forgot-password - Password reset request"

Write-Host ""

# [2/6] Test Place Service Endpoints
Write-Host "[2/6] Testing Place Service Endpoints..." -ForegroundColor Yellow
Write-Host ""

# Test Place Search
$success, $searchResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "places/search?searchTerm=pizza&page=1&pageSize=10" -Description "GET /places/search - Search places"

if ($success -and $searchResponse -and $searchResponse.data -and $searchResponse.data.items) {
    $places = $searchResponse.data.items
    Write-Host "   Found $($places.Count) places" -ForegroundColor Gray
    if ($places.Count -gt 0) {
        $script:TestPlaceId = $places[0].id
        Write-Host "   Using place ID: $script:TestPlaceId" -ForegroundColor Gray
    }
}
Write-Host ""

# Test Nearby Places
$success, $nearbyResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "places/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=10" -Description "GET /places/nearby - Get nearby places"

if ($success -and $nearbyResponse -and $nearbyResponse.data) {
    $nearbyPlaces = $nearbyResponse.data
    Write-Host "   Found $($nearbyPlaces.Count) nearby places" -ForegroundColor Gray
    if ($nearbyPlaces.Count -gt 0 -and -not $script:TestPlaceId) {
        $script:TestPlaceId = $nearbyPlaces[0].id
        Write-Host "   Using place ID: $script:TestPlaceId" -ForegroundColor Gray
    }
}
Write-Host ""

# Test Get Place by ID
if ($script:TestPlaceId) {
    $success, $placeResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "places/$script:TestPlaceId" -Description "GET /places/{id} - Get place details"
    Write-Host ""
} else {
    Write-Status "GET /places/{id} - Skipped (no place ID available)" "WARN"
    Write-Host ""
}

# Test Get Place with Invalid ID
$success, $invalidPlaceResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "places/00000000-0000-0000-0000-000000000000" -ExpectedStatus 404 -Description "GET /places/{id} - Invalid ID (should return 404)"

Write-Host ""

# [3/6] Test Review Service Endpoints
Write-Host "[3/6] Testing Review Service Endpoints..." -ForegroundColor Yellow
Write-Host ""

# Test Get Reviews by Place
if ($script:TestPlaceId) {
    # Try without query parameters first
    $success, $reviewsByPlaceResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "reviews/place/$script:TestPlaceId" -Description "GET /reviews/place/{placeId} - Get reviews by place"
    
    # If that fails with 400, try with query parameters
    if (-not $success -and $statusCode -eq 400) {
        $success, $reviewsByPlaceResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "reviews/place/$script:TestPlaceId?page=1&pageSize=10" -Description "GET /reviews/place/{placeId} - Get reviews by place (with pagination)"
    }
    
    if ($success -and $reviewsByPlaceResponse) {
        if ($reviewsByPlaceResponse.data) {
            $reviews = if ($reviewsByPlaceResponse.data.items) { $reviewsByPlaceResponse.data.items } else { $reviewsByPlaceResponse.data }
            if ($reviews) {
                Write-Host "   Found $($reviews.Count) reviews" -ForegroundColor Gray
                if ($reviews.Count -gt 0) {
                    $script:TestReviewId = $reviews[0].id
                    Write-Host "   Using review ID: $script:TestReviewId" -ForegroundColor Gray
                }
            }
        }
    }
    Write-Host ""
} else {
    Write-Status "GET /reviews/place/{placeId} - Skipped (no place ID available)" "WARN"
    Write-Host ""
}

# Test Get Review by ID
if ($script:TestReviewId) {
    $success, $reviewResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "reviews/$script:TestReviewId" -Description "GET /reviews/{id} - Get review details"
    Write-Host ""
} else {
    Write-Status "GET /reviews/{id} - Skipped (no review ID available)" "WARN"
    Write-Host ""
}

# Test Get Reviews by User
if ($script:TestUser -and $script:TestUser.id) {
    $success, $reviewsByUserResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "reviews/user/$($script:TestUser.id)?page=1&pageSize=10" -Description "GET /reviews/user/{userId} - Get reviews by user"
    Write-Host ""
} else {
    Write-Status "GET /reviews/user/{userId} - Skipped (no user ID available)" "WARN"
    Write-Host ""
}

# Test Get Review with Invalid ID
$success, $invalidReviewResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "reviews/00000000-0000-0000-0000-000000000000" -ExpectedStatus 404 -Description "GET /reviews/{id} - Invalid ID (should return 404)"

Write-Host ""

# [4/6] Test Authentication-Required Endpoints (if token available)
Write-Host "[4/6] Testing Authentication-Required Endpoints..." -ForegroundColor Yellow
Write-Host ""

if ($script:TestToken) {
    # Test Create Place (if authenticated)
    if ($script:TestToken) {
        $createPlaceBody = @{
            name = "Test Restaurant $(Get-Date -Format 'HHmmss')"
            address = "123 Test Street"
            latitude = 40.7128
            longitude = -74.0060
            priceRange = 2
            cuisineTypes = @("Italian", "Pizza")
        }
        
        $success, $createPlaceResponse, $statusCode = Invoke-ApiRequest -Method "POST" -Endpoint "places" -Body $createPlaceBody -Token $script:TestToken -ExpectedStatus 201 -Description "POST /places - Create place (auth required)"
        Write-Host ""
    }
    
    # Test Create Review (if authenticated and place exists)
    if ($script:TestPlaceId) {
        $createReviewBody = @{
            placeId = $script:TestPlaceId
            rating = 5
            text = "Great food! Test review from Phase 3 testing."
            checkInLatitude = 40.7128
            checkInLongitude = -74.0060
            placeLatitude = 40.7128
            placeLongitude = -74.0060
        }
        
        $success, $createReviewResponse, $statusCode = Invoke-ApiRequest -Method "POST" -Endpoint "reviews" -Body $createReviewBody -Token $script:TestToken -ExpectedStatus 201 -Description "POST /reviews - Create review (auth required)"
        Write-Host ""
    }
} else {
    Write-Status "Authentication-required endpoints - Skipped (no token available)" "WARN"
    Write-Host ""
}

# [5/6] Test Error Handling
Write-Host "[5/6] Testing Error Handling..." -ForegroundColor Yellow
Write-Host ""

# Test invalid endpoint
$success, $invalidEndpointResponse, $statusCode = Invoke-ApiRequest -Method "GET" -Endpoint "invalid/endpoint" -ExpectedStatus 404 -Description "GET /invalid/endpoint - Invalid endpoint (should return 404)"

Write-Host ""

# [6/6] Summary
Write-Host "[6/6] Summary" -ForegroundColor Yellow
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
    Write-Host "[OK] All critical API endpoint tests passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "[WARN] Some API endpoint tests failed or had warnings" -ForegroundColor Yellow
    exit 1
}

