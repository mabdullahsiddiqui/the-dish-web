# Phase 5 Test Orchestrator
# Runs all Phase 5 browser console and network analysis tests

param(
    [string]$OutputFile = "PHASE5_TEST_RESULTS.md",
    [string]$BaseUrl = "http://localhost:3000"
)

$ErrorActionPreference = "Continue"
$script:TestStartTime = Get-Date
$script:TestResults = @()
$script:AllTestsPassed = $true

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Split-Path -Parent $scriptDir

function Write-Header {
    param([string]$Text)
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host $Text -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
}

function Invoke-TestScript {
    param(
        [string]$ScriptPath,
        [string]$Description,
        [string]$Category,
        [hashtable]$Parameters = @{}
    )
    
    Write-Header "Running: $Description"
    
    $testStart = Get-Date
    try {
        $paramString = ""
        foreach ($key in $Parameters.Keys) {
            $value = $Parameters[$key]
            if ($value -is [switch]) {
                if ($value) {
                    $paramString += " -$key"
                }
            } else {
                $paramString += " -$key `"$value`""
            }
        }
        
        $command = "& `"$ScriptPath`"$paramString"
        $output = Invoke-Expression $command 2>&1
        $testEnd = Get-Date
        $duration = [math]::Round(($testEnd - $testStart).TotalSeconds, 2)
        $exitCode = $LASTEXITCODE
        
        $script:TestResults += @{
            Category = $Category
            Description = $Description
            Script = $ScriptPath
            Success = ($exitCode -eq 0)
            Duration = $duration
            Output = $output
            Timestamp = $testStart.ToString("yyyy-MM-dd HH:mm:ss")
        }
        
        if ($exitCode -ne 0) {
            $script:AllTestsPassed = $false
            Write-Host "[FAIL] $Description failed (Exit code: $exitCode)" -ForegroundColor Red
        } else {
            Write-Host "[OK] $Description completed successfully (Duration: ${duration}s)" -ForegroundColor Green
        }
        
        return $exitCode -eq 0
    } catch {
        $testEnd = Get-Date
        $duration = [math]::Round(($testEnd - $testStart).TotalSeconds, 2)
        
        $script:TestResults += @{
            Category = $Category
            Description = $Description
            Script = $ScriptPath
            Success = $false
            Duration = $duration
            Output = $_.Exception.Message
            Timestamp = $testStart.ToString("yyyy-MM-dd HH:mm:ss")
            Error = $_.Exception.Message
        }
        
        $script:AllTestsPassed = $false
        Write-Host "[FAIL] $Description failed with exception: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Generate-Report {
    param([string]$OutputPath)
    
    $report = @"
# Phase 5: Browser Console & Network Analysis - Test Results

**Test Execution Date**: $($script:TestStartTime.ToString("yyyy-MM-dd HH:mm:ss"))  
**Total Duration**: $([math]::Round(((Get-Date) - $script:TestStartTime).TotalSeconds, 2)) seconds  
**Base URL**: $BaseUrl  
**Overall Status**: $(if ($script:AllTestsPassed) { "[OK] PASSED" } else { "[FAIL] FAILED" })

---

## Test Summary

| Category | Description | Status | Duration |
|----------|-------------|--------|----------|
"@

    foreach ($result in $script:TestResults) {
        $status = if ($result.Success) { "[OK] PASS" } else { "[FAIL] FAIL" }
        $report += "`n| $($result.Category) | $($result.Description) | $status | $($result.Duration)s |"
    }

    $report += @"

---

## Detailed Results

"@

    foreach ($result in $script:TestResults) {
        $report += @"
### $($result.Description)

- **Category**: $($result.Category)
- **Script**: `$($result.Script)`
- **Status**: $(if ($result.Success) { "[OK] PASSED" } else { "[FAIL] FAILED" })
- **Duration**: $($result.Duration) seconds
- **Timestamp**: $($result.Timestamp)

"@

        if ($result.Error) {
            $report += "**Error**: $($result.Error)`n`n"
        }

        if ($result.Output) {
            $report += "**Output**:`n``````\n$($result.Output -join "`n")\n``````\n\n"
        }
    }

    $report += @"

---

## Success Criteria

### Browser Console
- [ ] No critical JavaScript errors
- [ ] No failed API calls in console
- [ ] Warnings are acceptable (non-critical)

### Network Requests
- [ ] No failed requests (404, 500 errors)
- [ ] No CORS errors
- [ ] Response times < 1s for most requests
- [ ] API calls succeed

### JWT Token Storage
- [ ] Token stored in localStorage after login
- [ ] Token included in API requests
- [ ] Token refresh works correctly

---

## Manual Testing Instructions

### Browser Console Analysis

1. **Open Browser DevTools**:
   - Press F12 or Right-click → Inspect
   - Navigate to Console tab

2. **Navigate to Application**:
   - Go to: $BaseUrl
   - Wait for page to fully load

3. **Check Console for Errors**:
   - Look for red error messages
   - Check for failed API calls
   - Note any warnings (yellow messages)

4. **Test JWT Token Storage**:
   - Open Console tab
   - Run: `localStorage.getItem('auth_token')`
   - Should return token after login
   - Run: `localStorage.getItem('auth_user')`
   - Should return user data

### Network Analysis

1. **Open Network Tab**:
   - Press F12 → Network tab
   - Refresh page (F5)

2. **Check for Failed Requests**:
   - Filter by "Failed" status
   - Check for 404, 500 errors
   - Note any CORS errors

3. **Check Response Times**:
   - Sort by "Time" column
   - Identify slow requests (>1s)
   - Check API endpoint response times

4. **Test API Calls**:
   - Open Console tab
   - Run: `fetch('http://localhost:5000/api/v1/places/search?page=1&pageSize=1').then(r => r.json()).then(console.log)`
   - Should return successful response

### Common Issues to Check

- **CORS Errors**: Check if API Gateway allows requests from web app origin
- **401 Errors**: Check if JWT token is being sent correctly
- **Slow Requests**: Check backend service performance
- **404 Errors**: Check if API routes are configured correctly

---

## Recommendations

"@

    $failedTests = $script:TestResults | Where-Object { -not $_.Success }
    if ($failedTests) {
        $report += "### Issues Found`n`n"
        foreach ($failed in $failedTests) {
            $report += "- **$($failed.Description)**: Failed - $($failed.Error)`n"
        }
        $report += "`n### Next Steps`n`n"
        $report += "1. Review the detailed output above for each failed test`n"
        $report += "2. Perform manual browser testing as described above`n"
        $report += "3. Check browser console for JavaScript errors`n"
        $report += "4. Check Network tab for failed requests`n"
    } else {
        $report += "[OK] Basic connectivity tests passed!`n"
        $report += "Please perform manual browser testing to complete console and network analysis.`n"
    }

    $report += @"

---

## Next Steps

Once Phase 5 is complete:
- Proceed to Phase 6: Error Scenario Testing
- Test error handling with services down
- Verify graceful error messages

---

*Report generated by Phase 5 Test Orchestrator*
"@

    # Write report to file
    $reportPath = Join-Path $backendDir "tests\$OutputFile"
    $report | Out-File -FilePath $reportPath -Encoding UTF8
    
    Write-Host ""
    Write-Host "Report saved to: $reportPath" -ForegroundColor Green
    return $reportPath
}

# Main execution
Write-Header "Phase 5: Browser Console & Network Analysis - Test Orchestrator"

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

# Test web application connectivity
try {
    $response = Invoke-WebRequest -Uri "$BaseUrl/" -Method Get -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
    Write-Host "[OK] Web application is accessible" -ForegroundColor Green
} catch {
    Write-Host "[WARN] Web application may not be accessible: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Please ensure web application is running: `npm run dev` in web directory" -ForegroundColor Yellow
}

# Run tests
Write-Host ""
Write-Host "Starting test execution..." -ForegroundColor Yellow
Write-Host ""

# Run browser console and network tests
$browserTestScript = Join-Path $scriptDir "test-browser-console-network.ps1"
if (Test-Path $browserTestScript) {
    Invoke-TestScript -ScriptPath $browserTestScript -Description "Browser Console & Network Tests" -Category "Browser Analysis" -Parameters @{ BaseUrl = $BaseUrl }
} else {
    Write-Host "[WARN] Browser console test script not found: $browserTestScript" -ForegroundColor Yellow
}

# Generate report
Write-Header "Generating Test Report"
$reportPath = Generate-Report -OutputPath $OutputFile

# Final summary
Write-Header "Test Execution Summary"
$totalTests = $script:TestResults.Count
$passedTests = ($script:TestResults | Where-Object { $_.Success }).Count
$failedTests = $totalTests - $passedTests

Write-Host "Total Tests: $totalTests" -ForegroundColor Cyan
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "Green" })
Write-Host ""

Write-Host "NOTE: Complete browser console and network analysis requires:" -ForegroundColor Yellow
Write-Host "  - Manual browser testing with DevTools" -ForegroundColor Yellow
Write-Host "  - Or browser automation tools (Selenium, Playwright)" -ForegroundColor Yellow
Write-Host ""

if ($script:AllTestsPassed) {
    Write-Host "[OK] Basic connectivity tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next step: Perform manual browser testing or proceed to Phase 6" -ForegroundColor Cyan
    exit 0
} else {
    Write-Host "[FAIL] Some Phase 5 tests failed. Please review the report: $reportPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please fix the issues before proceeding." -ForegroundColor Yellow
    exit 1
}

