# Phase 4: Web Application Testing Script
# Tests web application pages, navigation, and functionality

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

function Test-PageLoad {
    param(
        [string]$Url,
        [string]$Description,
        [string[]]$ExpectedContent = @(),
        [int]$ExpectedStatus = 200,
        [int]$TimeoutSeconds = 10
    )
    
    $startTime = Get-Date
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Get -TimeoutSec $TimeoutSeconds -UseBasicParsing -ErrorAction Stop
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        if ($response.StatusCode -eq $ExpectedStatus) {
            $contentChecks = @()
            $allContentFound = $true
            
            foreach ($content in $ExpectedContent) {
                if ($response.Content -match $content) {
                    $contentChecks += "Found: $content"
                } else {
                    $contentChecks += "Missing: $content"
                    $allContentFound = $false
                }
            }
            
            if ($allContentFound -or $ExpectedContent.Count -eq 0) {
                $details = if ($contentChecks.Count -gt 0) { $contentChecks -join "; " } else { "" }
                Write-Status "$Description" "OK" $responseTime $details
                return $true, $response
            } else {
                $details = $contentChecks -join "; "
                Write-Status "$Description - Some expected content missing" "WARN" $responseTime $details
                return $true, $response
            }
        } else {
            Write-Status "$Description - Expected status $ExpectedStatus, got $($response.StatusCode)" "WARN" $responseTime
            return $false, $response
        }
    } catch {
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        $statusCode = 0
        
        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode.value__
            if ($statusCode -eq $ExpectedStatus) {
                Write-Status "$Description (expected status $statusCode)" "OK" $responseTime
                return $true, $null
            } else {
                Write-Status "$Description - Error: $($_.Exception.Message) (Status: $statusCode)" "FAIL" $responseTime
                return $false, $null
            }
        } else {
            Write-Status "$Description - Exception: $($_.Exception.Message)" "FAIL" $responseTime
            return $false, $null
        }
    }
}

function Test-PageContent {
    param(
        [string]$Content,
        [string[]]$RequiredElements,
        [string]$PageName
    )
    
    $foundElements = @()
    $missingElements = @()
    
    foreach ($element in $RequiredElements) {
        if ($Content -match $element) {
            $foundElements += $element
        } else {
            $missingElements += $element
        }
    }
    
    if ($missingElements.Count -eq 0) {
        Write-Status "$PageName - All required elements found" "OK" 0
        return $true
    } else {
        Write-Status "$PageName - Missing elements: $($missingElements -join ', ')" "WARN" 0
        return $false
    }
}

# Main execution
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Phase 4: Web Application Testing" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Base URL: $BaseUrl" -ForegroundColor Gray
Write-Host ""

# [1/6] Page Load Testing
Write-Host "[1/6] Testing Page Loads..." -ForegroundColor Yellow
Write-Host ""

# Test Homepage
$success, $homepageResponse = Test-PageLoad -Url "$BaseUrl/" -Description "Homepage (/) loads" -ExpectedContent @("The Dish", "search", "restaurant")
Write-Host ""

# Test Login Page
$success, $loginResponse = Test-PageLoad -Url "$BaseUrl/login" -Description "Login page (/login) loads" -ExpectedContent @("login", "email", "password")
Write-Host ""

# Test Register Page
$success, $registerResponse = Test-PageLoad -Url "$BaseUrl/register" -Description "Register page (/register) loads" -ExpectedContent @("register", "email", "password")
Write-Host ""

# Test Search Page
$success, $searchResponse = Test-PageLoad -Url "$BaseUrl/search?q=pizza" -Description "Search page (/search?q=pizza) loads" -ExpectedContent @("search", "pizza")
Write-Host ""

# Test Places Page
$success, $placesResponse = Test-PageLoad -Url "$BaseUrl/places" -Description "Places page (/places) loads"
Write-Host ""

# [2/6] Navigation Testing
Write-Host "[2/6] Testing Navigation..." -ForegroundColor Yellow
Write-Host ""

# Test invalid route (should return 404)
$success, $invalidRouteResponse = Test-PageLoad -Url "$BaseUrl/invalid-route-12345" -Description "Invalid route returns 404" -ExpectedStatus 404
Write-Host ""

# Test place detail route format (should load or redirect)
$success, $placeDetailResponse = Test-PageLoad -Url "$BaseUrl/places/test-id" -Description "Place detail route format (/places/{id})"
Write-Host ""

# [3/6] Authentication UI Testing
Write-Host "[3/6] Testing Authentication UI..." -ForegroundColor Yellow
Write-Host ""

if ($loginResponse) {
    Test-PageContent -Content $loginResponse.Content -RequiredElements @("input.*type.*email", "input.*type.*password", "button|submit") -PageName "Login Page"
}
Write-Host ""

if ($registerResponse) {
    Test-PageContent -Content $registerResponse.Content -RequiredElements @("input.*type.*email", "input.*type.*password", "button|submit") -PageName "Register Page"
}
Write-Host ""

# [4/6] Search Functionality Testing
Write-Host "[4/6] Testing Search Functionality..." -ForegroundColor Yellow
Write-Host ""

# Test search with different queries
$searchQueries = @("pizza", "italian", "restaurant")
foreach ($query in $searchQueries) {
    $success, $searchQueryResponse = Test-PageLoad -Url "$BaseUrl/search?q=$query" -Description "Search with query: $query" -ExpectedContent @("search")
    if ($success -and $searchQueryResponse) {
        if ($searchQueryResponse.Content -match $query) {
            Write-Status "Search results contain query term: $query" "OK" 0
        } else {
            Write-Status "Search results may not contain query term: $query" "WARN" 0
        }
    }
    Write-Host ""
}

# [5/6] Place Detail Page Testing
Write-Host "[5/6] Testing Place Detail Pages..." -ForegroundColor Yellow
Write-Host ""

# Try to get a real place ID from the API
try {
    $apiResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/v1/places/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=10" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    $apiData = $apiResponse.Content | ConvertFrom-Json
    
    if ($apiData -and $apiData.data -and $apiData.data.Count -gt 0) {
        $testPlaceId = $apiData.data[0].id
        Write-Host "   Using place ID: $testPlaceId" -ForegroundColor Gray
        
        $success, $placeDetailResponse = Test-PageLoad -Url "$BaseUrl/places/$testPlaceId" -Description "Place detail page (/places/{id}) loads" -ExpectedContent @("place", "review|rating")
        
        if ($success -and $placeDetailResponse) {
            # Check for map component (Leaflet)
            if ($placeDetailResponse.Content -match "leaflet|map") {
                Write-Status "Place detail page - Map component detected" "OK" 0
            } else {
                Write-Status "Place detail page - Map component not detected" "WARN" 0
            }
            
            # Check for reviews section
            if ($placeDetailResponse.Content -match "review|rating") {
                Write-Status "Place detail page - Reviews section detected" "OK" 0
            } else {
                Write-Status "Place detail page - Reviews section not detected" "WARN" 0
            }
        }
    } else {
        Write-Status "Place detail page - No places available for testing" "WARN" 0
    }
} catch {
    Write-Status "Place detail page - Could not fetch place ID from API" "WARN" 0
}
Write-Host ""

# [6/6] Additional Page Testing
Write-Host "[6/6] Testing Additional Pages..." -ForegroundColor Yellow
Write-Host ""

# Test Profile Page (may require authentication)
$success, $profileResponse = Test-PageLoad -Url "$BaseUrl/profile" -Description "Profile page (/profile) loads"
Write-Host ""

# Test Forgot Password Page
$success, $forgotPasswordResponse = Test-PageLoad -Url "$BaseUrl/forgot-password" -Description "Forgot password page (/forgot-password) loads" -ExpectedContent @("forgot|reset", "email")
Write-Host ""

# Test Reset Password Page
$success, $resetPasswordResponse = Test-PageLoad -Url "$BaseUrl/reset-password" -Description "Reset password page (/reset-password) loads"
Write-Host ""

# [7/6] Summary
Write-Host "[7/6] Summary" -ForegroundColor Yellow
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
    Write-Host "[OK] All critical web application tests passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "[WARN] Some web application tests failed or had warnings" -ForegroundColor Yellow
    exit 1
}

