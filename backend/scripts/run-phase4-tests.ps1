# Phase 4 Test Orchestrator
# Runs all Phase 4 web application tests and generates comprehensive test report

param(
    [string]$OutputFile = "PHASE4_TEST_RESULTS.md",
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
# Phase 4: Web Application Testing - Test Results

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

### Page Load Testing
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Page Load" -and $_.Success }) { "x" } else { " " })] Homepage loads without errors
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Page Load" -and $_.Success }) { "x" } else { " " })] Login page loads
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Page Load" -and $_.Success }) { "x" } else { " " })] Register page loads
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Page Load" -and $_.Success }) { "x" } else { " " })] Search page loads

### Navigation Testing
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Navigation" -and $_.Success }) { "x" } else { " " })] Routes navigate correctly
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Navigation" -and $_.Success }) { "x" } else { " " })] Invalid routes return 404

### Authentication UI
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Authentication" -and $_.Success }) { "x" } else { " " })] Login form elements present
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Authentication" -and $_.Success }) { "x" } else { " " })] Register form elements present

### Search Functionality
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Search" -and $_.Success }) { "x" } else { " " })] Search page loads with query
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Search" -and $_.Success }) { "x" } else { " " })] Search results display

### Place Detail Pages
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Place Detail" -and $_.Success }) { "x" } else { " " })] Place detail pages load
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Place Detail" -and $_.Success }) { "x" } else { " " })] Map component renders
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Place Detail" -and $_.Success }) { "x" } else { " " })] Reviews section exists

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
        $report += "2. Verify web application is running: `npm run dev` in web directory`n"
        $report += "3. Check browser console for JavaScript errors`n"
        $report += "4. Test pages manually in browser`n"
    } else {
        $report += "[OK] All tests passed successfully! Proceed to Phase 5: Browser Console & Network Analysis.`n"
    }

    $report += @"

---

## Next Steps

Once Phase 4 is complete and all web pages are verified:
- Proceed to Phase 5: Browser Console & Network Analysis
- Test browser console for errors
- Analyze network requests
- Verify API integration

---

*Report generated by Phase 4 Test Orchestrator*
"@

    # Write report to file
    $reportPath = Join-Path $backendDir "tests\$OutputFile"
    $report | Out-File -FilePath $reportPath -Encoding UTF8
    
    Write-Host ""
    Write-Host "Report saved to: $reportPath" -ForegroundColor Green
    return $reportPath
}

# Main execution
Write-Header "Phase 4: Web Application Testing - Test Orchestrator"

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

# Run web application tests
$webTestScript = Join-Path $scriptDir "test-web-application.ps1"
if (Test-Path $webTestScript) {
    Invoke-TestScript -ScriptPath $webTestScript -Description "Web Application Tests" -Category "Web Application" -Parameters @{ BaseUrl = $BaseUrl }
} else {
    Write-Host "[WARN] Web application test script not found: $webTestScript" -ForegroundColor Yellow
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

if ($script:AllTestsPassed) {
    Write-Host "[OK] All Phase 4 tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next step: Proceed to Phase 5: Browser Console & Network Analysis" -ForegroundColor Cyan
    exit 0
} else {
    Write-Host "[FAIL] Some Phase 4 tests failed. Please review the report: $reportPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please fix the issues before proceeding to Phase 5." -ForegroundColor Yellow
    exit 1
}

