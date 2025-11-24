# Phase 6 Test Orchestrator
# Runs all Phase 6 error scenario tests

param(
    [string]$OutputFile = "PHASE6_TEST_RESULTS.md",
    [string]$BaseUrl = "http://localhost:5000/api/v1",
    [string]$WebAppUrl = "http://localhost:3000",
    [switch]$SkipServiceStop = $false
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
    
    $totalTests = ($script:TestResults | ForEach-Object { 
        if ($_.Output -is [array]) {
            ($_.Output | Where-Object { $_ -match "\[OK\]|\[WARN\]|\[FAIL\]" }).Count
        } else {
            0
        }
    } | Measure-Object -Sum).Sum
    
    $passedTests = ($script:TestResults | ForEach-Object {
        if ($_.Output -is [array]) {
            ($_.Output | Where-Object { $_ -match "\[OK\]" }).Count
        } else {
            0
        }
    } | Measure-Object -Sum).Sum
    
    $warnTests = ($script:TestResults | ForEach-Object {
        if ($_.Output -is [array]) {
            ($_.Output | Where-Object { $_ -match "\[WARN\]" }).Count
        } else {
            0
        }
    } | Measure-Object -Sum).Sum
    
    $failedTests = ($script:TestResults | ForEach-Object {
        if ($_.Output -is [array]) {
            ($_.Output | Where-Object { $_ -match "\[FAIL\]" }).Count
        } else {
            0
        }
    } | Measure-Object -Sum).Sum
    
    $report = @"
# Phase 6: Error Scenario Testing - Test Results

**Test Execution Date**: $($script:TestStartTime.ToString("yyyy-MM-dd HH:mm:ss"))  
**Total Duration**: $([math]::Round(((Get-Date) - $script:TestStartTime).TotalSeconds, 2)) seconds  
**Base URL**: $BaseUrl  
**Web App URL**: $WebAppUrl  
**Overall Status**: $(if ($script:AllTestsPassed -and $failedTests -eq 0) { "[OK] PASSED" } else { "[FAIL] FAILED" })

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

## Test Statistics

- **Total Tests**: $totalTests
- **Passed**: $passedTests
- **Warnings**: $warnTests
- **Failed**: $failedTests

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

        if ($result.Output -is [array]) {
            $report += "**Output**:`n``````\n$($result.Output -join "`n")\n``````\n\n"
        } elseif ($result.Output) {
            $report += "**Output**: $($result.Output)`n`n"
        }
    }

    $report += @"

---

## Success Criteria

### Service Unavailable Tests
- [ ] API Gateway stop test: Error handling works correctly
- [ ] User Service stop test: Error handling works correctly
- [ ] Place Service stop test: Error handling works correctly
- [ ] Review Service stop test: Error handling works correctly
- [ ] Web app displays user-friendly error messages

### Invalid API Response Tests
- [ ] 404 Not Found: Proper error handling
- [ ] 400 Bad Request: Validation errors displayed
- [ ] 401 Unauthorized: Authentication errors handled
- [ ] 500 Server Error: Graceful error handling

### Network Error Tests
- [ ] Timeout errors: User-friendly messages
- [ ] Connection refused: Proper error handling
- [ ] Invalid request body: Validation errors

---

## Error Handling Verification

### Expected Behavior

1. **Service Unavailable**:
   - API calls should fail gracefully
   - Web app should show error messages (not crash)
   - Error messages should be user-friendly
   - No stack traces exposed to users

2. **Invalid Responses**:
   - 404: "The requested resource was not found."
   - 400: "Invalid request. Please check your input and try again."
   - 401: "Your session has expired. Please log in again." (for protected routes)
   - 500: "Server error. Please try again later."

3. **Network Errors**:
   - Timeout: "Request timed out. Please check your connection and try again."
   - Network Error: "Network error. Please check your internet connection."
   - Connection Error: "Unable to connect to server. Please try again later."

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
        $report += "2. Verify error messages are user-friendly`n"
        $report += "3. Check that services are properly restored after tests`n"
        $report += "4. Ensure web app handles errors gracefully`n"
    } else {
        $report += "[OK] All error scenario tests passed!`n`n"
        $report += "**Findings**:`n"
        $report += "- Error handling works correctly`n"
        $report += "- User-friendly error messages displayed`n"
        $report += "- Services properly restored after tests`n"
        $report += "- Web app handles errors gracefully`n"
    }

    $report += @"

---

## Next Steps

Once Phase 6 is complete:
- Proceed to Phase 7: Documentation & Reporting
- Review all test results
- Update documentation with findings
- Create final comprehensive test report

---

*Report generated by Phase 6 Test Orchestrator*
"@

    # Write report to file
    $reportPath = Join-Path $backendDir "tests\$OutputFile"
    $report | Out-File -FilePath $reportPath -Encoding UTF8
    
    Write-Host ""
    Write-Host "Report saved to: $reportPath" -ForegroundColor Green
    return $reportPath
}

# Main execution
Write-Header "Phase 6: Error Scenario Testing - Test Orchestrator"

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

# Test API Gateway connectivity
try {
    $response = Invoke-WebRequest -Uri "$BaseUrl/places/search?page=1&pageSize=1" -Method Get -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
    Write-Host "[OK] API Gateway is accessible" -ForegroundColor Green
} catch {
    Write-Host "[WARN] API Gateway may not be accessible: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Please ensure backend services are running" -ForegroundColor Yellow
}

# Test web application connectivity
try {
    $response = Invoke-WebRequest -Uri "$WebAppUrl/" -Method Get -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
    Write-Host "[OK] Web application is accessible" -ForegroundColor Green
} catch {
    Write-Host "[WARN] Web application may not be accessible: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Please ensure web application is running: `npm run dev` in web directory" -ForegroundColor Yellow
}

# Run tests
Write-Host ""
Write-Host "Starting test execution..." -ForegroundColor Yellow
Write-Host ""

if ($SkipServiceStop) {
    Write-Host "[INFO] Service stop tests will be skipped (SkipServiceStop flag set)" -ForegroundColor Yellow
    Write-Host ""
}

# Run error scenario tests
$errorTestScript = Join-Path $scriptDir "test-error-scenarios.ps1"
if (Test-Path $errorTestScript) {
    $testParams = @{
        BaseUrl = $BaseUrl
        WebAppUrl = $WebAppUrl
    }
    if ($SkipServiceStop) {
        $testParams.SkipServiceStop = $true
    }
    Invoke-TestScript -ScriptPath $errorTestScript -Description "Error Scenario Tests" -Category "Error Handling" -Parameters $testParams
} else {
    Write-Host "[FAIL] Error scenario test script not found: $errorTestScript" -ForegroundColor Red
    $script:AllTestsPassed = $false
}

# Generate report
Write-Header "Generating Test Report"
$reportPath = Generate-Report -OutputPath $OutputFile

# Final summary
Write-Header "Test Execution Summary"

$totalTests = ($script:TestResults | ForEach-Object { 
    if ($_.Output -is [array]) {
        ($_.Output | Where-Object { $_ -match "\[OK\]|\[WARN\]|\[FAIL\]" }).Count
    } else {
        0
    }
} | Measure-Object -Sum).Sum

$passedTests = ($script:TestResults | ForEach-Object {
    if ($_.Output -is [array]) {
        ($_.Output | Where-Object { $_ -match "\[OK\]" }).Count
    } else {
        0
    }
} | Measure-Object -Sum).Sum

$warnTests = ($script:TestResults | ForEach-Object {
    if ($_.Output -is [array]) {
        ($_.Output | Where-Object { $_ -match "\[WARN\]" }).Count
    } else {
        0
    }
} | Measure-Object -Sum).Sum

$failedTests = ($script:TestResults | ForEach-Object {
    if ($_.Output -is [array]) {
        ($_.Output | Where-Object { $_ -match "\[FAIL\]" }).Count
    } else {
        0
    }
} | Measure-Object -Sum).Sum

Write-Host "Total Tests: $totalTests" -ForegroundColor Cyan
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Warnings: $warnTests" -ForegroundColor Yellow
Write-Host "Failed: $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($script:AllTestsPassed -and $failedTests -eq 0) {
    Write-Host "[OK] Phase 6 error scenario tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next step: Proceed to Phase 7: Documentation & Reporting" -ForegroundColor Cyan
    exit 0
} else {
    Write-Host "[FAIL] Some Phase 6 tests failed. Please review the report: $reportPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please review the issues before proceeding." -ForegroundColor Yellow
    exit 1
}

