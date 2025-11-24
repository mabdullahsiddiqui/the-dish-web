# Phase 2 Test Orchestrator
# Runs all Phase 2 tests and generates comprehensive test report

param(
    [string]$OutputFile = "PHASE2_TEST_RESULTS.md",
    [switch]$SkipDocker = $false,
    [switch]$SkipServices = $false
)

$ErrorActionPreference = "Continue"
$script:TestStartTime = Get-Date
$script:TestResults = @()
$script:AllTestsPassed = $true

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Split-Path -Parent $scriptDir
$rootDir = Split-Path -Parent $backendDir
$webScriptsDir = Join-Path $rootDir "web\scripts"

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
        [string]$Category
    )
    
    Write-Header "Running: $Description"
    
    $testStart = Get-Date
    try {
        $output = & $ScriptPath 2>&1
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
# Phase 2: Backend Service Health Checks - Test Results

**Test Execution Date**: $($script:TestStartTime.ToString("yyyy-MM-dd HH:mm:ss"))  
**Total Duration**: $([math]::Round(((Get-Date) - $script:TestStartTime).TotalSeconds, 2)) seconds  
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

### Docker Containers
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Docker" -and $_.Success }) { "x" } else { " " })] All required containers running
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Docker" -and $_.Success }) { "x" } else { " " })] All containers show "healthy" status

### Microservices
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Services" -and $_.Success }) { "x" } else { " " })] All services respond to health checks within 2 seconds
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Services" -and $_.Success }) { "x" } else { " " })] Health endpoints return HTTP 200

### Database
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Database" -and $_.Success }) { "x" } else { " " })] PostgreSQL accepts connections
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Database" -and $_.Success }) { "x" } else { " " })] PostGIS extension available
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Database" -and $_.Success }) { "x" } else { " " })] All service databases accessible

### Redis
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Redis" -and $_.Success }) { "x" } else { " " })] Redis accepts connections
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Redis" -and $_.Success }) { "x" } else { " " })] PING command responds
- [$(if ($script:TestResults | Where-Object { $_.Category -eq "Redis" -and $_.Success }) { "x" } else { " " })] SET/GET operations succeed

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
        $report += "2. Check Docker containers: `docker ps`\n"
        $report += "3. Verify services are running: Check service logs`n"
        $report += "4. Test database connectivity manually: `psql -h localhost -U thedish -d thedish`\n"
        $report += "5. Test Redis connectivity manually: `redis-cli -h localhost -p 6379 PING`\n"
    } else {
        $report += "[OK] All tests passed successfully! Proceed to Phase 3: API Endpoint Testing.`n"
    }

    $report += @"

---

## Next Steps

Once Phase 2 is complete and all services are verified healthy:
- Proceed to Phase 3: API Endpoint Testing
- Use healthy services to test actual API functionality
- Verify end-to-end request flows through API Gateway

---

*Report generated by Phase 2 Test Orchestrator*
"@

    # Write report to file
    $reportPath = Join-Path $backendDir "tests\$OutputFile"
    $report | Out-File -FilePath $reportPath -Encoding UTF8
    
    Write-Host ""
    Write-Host "Report saved to: $reportPath" -ForegroundColor Green
    return $reportPath
}

# Main execution
Write-Header "Phase 2: Backend Service Health Checks - Test Orchestrator"

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

# Check if Docker is running
if (-not $SkipDocker) {
    try {
        $null = docker ps 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[WARN] Docker may not be running. Some tests may fail." -ForegroundColor Yellow
        }
    } catch {
        Write-Host "[WARN] Docker command not available. Some tests may fail." -ForegroundColor Yellow
    }
}

# Run tests in sequence
Write-Host ""
Write-Host "Starting test execution..." -ForegroundColor Yellow
Write-Host ""

# 1. Comprehensive health check (includes Docker and Services)
if (-not $SkipDocker -and -not $SkipServices) {
    $comprehensiveScript = Join-Path $webScriptsDir "check-backend-comprehensive.ps1"
    if (Test-Path $comprehensiveScript) {
        Invoke-TestScript -ScriptPath $comprehensiveScript -Description "Comprehensive Health Checks" -Category "Comprehensive"
    } else {
        Write-Host "[WARN] Comprehensive health check script not found: $comprehensiveScript" -ForegroundColor Yellow
    }
}

# 2. Database connectivity tests
$dbScript = Join-Path $scriptDir "test-database-connectivity.ps1"
if (Test-Path $dbScript) {
    Invoke-TestScript -ScriptPath $dbScript -Description "Database Connectivity Tests" -Category "Database"
} else {
        Write-Host "[WARN] Database connectivity test script not found: $dbScript" -ForegroundColor Yellow
}

# 3. Redis connectivity tests
$redisScript = Join-Path $scriptDir "test-redis-connectivity.ps1"
if (Test-Path $redisScript) {
    Invoke-TestScript -ScriptPath $redisScript -Description "Redis Connectivity Tests" -Category "Redis"
} else {
        Write-Host "[WARN] Redis connectivity test script not found: $redisScript" -ForegroundColor Yellow
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
    Write-Host "[OK] All Phase 2 tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next step: Proceed to Phase 3: API Endpoint Testing" -ForegroundColor Cyan
    exit 0
} else {
    Write-Host "[FAIL] Some Phase 2 tests failed. Please review the report: $reportPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please fix the issues before proceeding to Phase 3." -ForegroundColor Yellow
    exit 1
}


