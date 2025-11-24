# Redis Connectivity Test Script
# Tests Redis connection and basic operations (PING, SET, GET)

param(
    [string]$RedisHost = "localhost",
    [int]$Port = 6379
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

function Test-RedisCommand {
    param(
        [string]$Command,
        [string]$Description,
        [string]$ExpectedResult = $null
    )
    
    $startTime = Get-Date
    try {
        $result = & redis-cli -h $RedisHost -p $Port $Command 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            $endTime = Get-Date
            $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
            
            if ($ExpectedResult -and $result -notmatch $ExpectedResult) {
                Write-Status "$Description - Unexpected result: $result" "WARN" $responseTime
                return $false, $result
            } else {
                Write-Status "$Description" "OK" $responseTime
                return $true, $result
            }
        } else {
            Write-Status "$Description - Error: $result" "FAIL"
            return $false, $result
        }
    } catch {
        Write-Status "$Description - Exception: $($_.Exception.Message)" "FAIL"
        return $false, $null
    }
}

function Test-RedisTcpConnection {
    param(
        [string]$RedisHost,
        [int]$RedisPort
    )
    
    $startTime = Get-Date
    try {
        $tcpClient = New-Object System.Net.Sockets.TcpClient
        $tcpClient.ReceiveTimeout = 2000
        $tcpClient.SendTimeout = 2000
        $tcpClient.Connect($RedisHost, $RedisPort)
        $connected = $tcpClient.Connected
        $tcpClient.Close()
        
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        if ($connected) {
            Write-Status "Redis: TCP connection successful" "OK" $responseTime
            return $true
        } else {
            Write-Status "Redis: TCP connection failed" "FAIL"
            return $false
        }
    } catch {
        Write-Status "Redis: TCP connection error - $($_.Exception.Message)" "FAIL"
        return $false
    }
}

# Main execution
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Redis Connectivity Tests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if redis-cli is available
$redisCliAvailable = $false
try {
    $null = & redis-cli --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        $redisCliAvailable = $true
        Write-Host "redis-cli detected" -ForegroundColor Gray
    }
} catch {
    Write-Host "redis-cli not found, using TCP connection test only" -ForegroundColor Yellow
}

if ($redisCliAvailable) {
    # Test PING command
    Write-Host ""
    Write-Host "Basic Connectivity:" -ForegroundColor Yellow
    $pingSuccess, $pingResult = Test-RedisCommand -Command "PING" -Description "Redis PING" -ExpectedResult "PONG"
    
    if ($pingSuccess) {
        # Test INFO command
        Write-Host ""
        Write-Host "Server Information:" -ForegroundColor Yellow
        $infoSuccess, $infoResult = Test-RedisCommand -Command "INFO server" -Description "Redis INFO"
        
        if ($infoSuccess -and $infoResult) {
            $redisVersion = ($infoResult | Select-String "redis_version:(\S+)").Matches.Groups[1].Value
            if ($redisVersion) {
                Write-Host "   Redis Version: $redisVersion" -ForegroundColor Gray
            }
        }
        
        # Test SET/GET operations
        Write-Host ""
        Write-Host "Data Operations:" -ForegroundColor Yellow
        $testKey = "health_check_test_$(Get-Date -Format 'yyyyMMddHHmmss')"
        $testValue = "test_value_$(Get-Random)"
        
        $setSuccess, $setResult = Test-RedisCommand -Command "SET $testKey `"$testValue`"" -Description "Redis SET operation" -ExpectedResult "OK"
        
        if ($setSuccess) {
            $getSuccess, $getResult = Test-RedisCommand -Command "GET $testKey" -Description "Redis GET operation" -ExpectedResult $testValue
            
            if ($getSuccess) {
                # Cleanup
                $delSuccess, $delResult = Test-RedisCommand -Command "DEL $testKey" -Description "Redis DEL operation (cleanup)"
            }
        }
        
        # Test persistence (if configured)
        Write-Host ""
        Write-Host "Persistence:" -ForegroundColor Yellow
        $persistenceSuccess, $persistenceResult = Test-RedisCommand -Command "CONFIG GET save" -Description "Redis persistence configuration"
        
        if ($persistenceSuccess -and $persistenceResult) {
            Write-Host "   $persistenceResult" -ForegroundColor Gray
        }
    }
} else {
    # Fallback to TCP connection test
    Write-Host ""
    Write-Host "Basic Connectivity (TCP only):" -ForegroundColor Yellow
    $tcpSuccess = Test-RedisTcpConnection -RedisHost $RedisHost -RedisPort $Port
    
    if ($tcpSuccess) {
        Write-Status "Redis: Basic TCP connection successful (advanced operations require redis-cli)" "WARN"
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($script:AllTestsPassed) {
    Write-Host "[OK] All Redis connectivity tests passed" -ForegroundColor Green
    exit 0
} else {
    Write-Host "[WARN] Some Redis connectivity tests failed" -ForegroundColor Yellow
    exit 1
}


