# Database Connectivity Test Script
# Tests PostgreSQL connection, PostGIS extension, and all service database contexts

param(
    [string]$DbHost = "localhost",
    [int]$Port = 5432,
    [string]$User = "thedish",
    [string]$Password = "thedish_dev_password"
)

$ErrorActionPreference = "Continue"
$script:AllTestsPassed = $true
$script:TestResults = @()

function Write-Status {
    param(
        [string]$Message,
        [string]$Status,  # "OK", "WARN", "FAIL"
        [int]$ResponseTime = 0
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
    Write-Host "$icon $Message$timeStr" -ForegroundColor $color
    
    $script:TestResults += @{
        Message = $Message
        Status = $Status
        ResponseTime = $ResponseTime
        Timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    }
    
    if ($Status -eq "FAIL") {
        $script:AllTestsPassed = $false
    }
}

function Test-PostgreSQLQuery {
    param(
        [string]$Database,
        [string]$Query,
        [string]$Description
    )
    
    $startTime = Get-Date
    try {
        $env:PGPASSWORD = $Password
        $result = & psql -h $DbHost -p $Port -U $User -d $Database -c $Query -t 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            $endTime = Get-Date
            $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
            Write-Status "$Description" "OK" $responseTime
            return $true, $result
        } else {
            Write-Status "$Description - Error: $result" "FAIL"
            return $false, $result
        }
    } catch {
        # Fallback: Try TCP connection test if psql is not available
        if ($_.Exception.Message -match "psql.*not recognized") {
            try {
                $tcpClient = New-Object System.Net.Sockets.TcpClient
                $tcpClient.Connect($DbHost, 5432)
                $connected = $tcpClient.Connected
                $tcpClient.Close()
                
                if ($connected) {
                    $endTime = Get-Date
                    $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
                    Write-Status "$Description (TCP check - psql not available)" "WARN" $responseTime
                    return $true, "TCP connection successful"
                } else {
                    Write-Status "$Description - TCP connection failed" "FAIL"
                    return $false, $null
                }
            } catch {
                Write-Status "$Description - Exception: $($_.Exception.Message)" "FAIL"
                return $false, $null
            }
        } else {
            Write-Status "$Description - Exception: $($_.Exception.Message)" "FAIL"
            return $false, $null
        }
    } finally {
        Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    }
}

function Test-DatabaseContext {
    param(
        [string]$ContextName,
        [string]$Database,
        [string]$ConnectionStringKey
    )
    
    Write-Host ""
    Write-Host "Testing $ContextName..." -ForegroundColor Cyan
    
    # Test basic connection
    $success, $result = Test-PostgreSQLQuery -Database $Database -Query "SELECT 1;" -Description "${ContextName}: Basic connection"
    
    if ($success) {
        # Test table count
        $tableQuery = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';"
        $success, $tableCount = Test-PostgreSQLQuery -Database $Database -Query $tableQuery -Description "${ContextName}: Schema access"
        
        if ($success -and $tableCount) {
            $tableCount = $tableCount.Trim()
            Write-Host "   Tables found: $tableCount" -ForegroundColor Gray
        }
    }
    
    return $success
}

# Main execution
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Connectivity Tests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test PostgreSQL version
Write-Host "PostgreSQL Server Information:" -ForegroundColor Yellow
$success, $version = Test-PostgreSQLQuery -Database "postgres" -Query "SELECT version();" -Description "PostgreSQL Version"
if ($success -and $version) {
    $version = $version.Trim()
    Write-Host "   $version" -ForegroundColor Gray
}

# Test PostGIS extension
Write-Host ""
Write-Host "PostGIS Extension:" -ForegroundColor Yellow
$success, $postgisVersion = Test-PostgreSQLQuery -Database "thedish" -Query "SELECT PostGIS_version();" -Description "PostGIS Extension"
if ($success -and $postgisVersion) {
    $postgisVersion = $postgisVersion.Trim()
    Write-Host "   PostGIS Version: $postgisVersion" -ForegroundColor Gray
} else {
    Write-Status "PostGIS extension not available in 'thedish' database" "WARN"
    # Try to check if extension exists
    $success, $extCheck = Test-PostgreSQLQuery -Database "thedish" -Query "SELECT EXISTS(SELECT 1 FROM pg_extension WHERE extname = 'postgis');" -Description "PostGIS Extension Check"
}

# Test each service database context
Write-Host ""
Write-Host "Service Database Contexts:" -ForegroundColor Yellow

# User Service Database
$userDbSuccess = Test-DatabaseContext -ContextName "UserDbContext" -Database "thedish_users" -ConnectionStringKey "UserDb"

# Place Service Database
$placeDbSuccess = Test-DatabaseContext -ContextName "PlaceDbContext" -Database "thedish" -ConnectionStringKey "PlaceDb"

# Review Service Database
$reviewDbSuccess = Test-DatabaseContext -ContextName "ReviewDbContext" -Database "thedish" -ConnectionStringKey "ReviewDb"

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$totalTests = 3  # User, Place, Review contexts
$userDbPassed = if ($userDbSuccess) { 1 } else { 0 }
$placeDbPassed = if ($placeDbSuccess) { 1 } else { 0 }
$reviewDbPassed = if ($reviewDbSuccess) { 1 } else { 0 }
$passedTests = $userDbPassed + $placeDbPassed + $reviewDbPassed

if ($script:AllTestsPassed -and $passedTests -eq $totalTests) {
    Write-Host "[OK] All database connectivity tests passed ($passedTests/$totalTests)" -ForegroundColor Green
    exit 0
} else {
    Write-Host "[WARN] Some database connectivity tests failed ($passedTests/$totalTests passed)" -ForegroundColor Yellow
    exit 1
}


