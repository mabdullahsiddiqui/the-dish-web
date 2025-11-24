# Comprehensive Backend Health Check Script
# Tests Docker containers, service health endpoints, database, and Redis connectivity

param(
    [switch]$JsonOutput = $false
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
    }
    
    if ($Status -eq "FAIL") {
        $script:AllTestsPassed = $false
    }
}

function Test-DockerContainer {
    param(
        [string]$ContainerName,
        [string]$DisplayName
    )
    
    try {
        $container = docker ps --filter "name=$ContainerName" --format "{{.Names}}|{{.Status}}|{{.Health}}" 2>&1
        if ($LASTEXITCODE -eq 0 -and $container) {
            $parts = $container -split '\|'
            $status = $parts[1]
            $health = $parts[2]
            
            if ($health -match "healthy" -or $status -match "Up") {
                Write-Status "${DisplayName}: $status" "OK"
                return $true
            } else {
                Write-Status "${DisplayName}: $status (Health: $health)" "WARN"
                return $false
            }
        } else {
            Write-Status "${DisplayName}: Not running" "FAIL"
            return $false
        }
    } catch {
        Write-Status "${DisplayName}: Error checking status - $($_.Exception.Message)" "FAIL"
        return $false
    }
}

function Test-HealthEndpoint {
    param(
        [string]$ServiceName,
        [string]$Url,
        [string]$FallbackUrl = $null,
        [int]$TimeoutSeconds = 3
    )
    
    $startTime = Get-Date
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Get -TimeoutSec $TimeoutSeconds -UseBasicParsing -ErrorAction Stop
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        
        if ($response.StatusCode -eq 200) {
            Write-Status "${ServiceName}: $Url" "OK" $responseTime
            return $true
        } else {
            Write-Status "${ServiceName}: Status $($response.StatusCode)" "WARN" $responseTime
            return $false
        }
    } catch {
        # Try fallback URL if provided
        if ($FallbackUrl -and $_.Exception.Message -match "404") {
            try {
                $fallbackResponse = Invoke-WebRequest -Uri $FallbackUrl -Method Get -TimeoutSec $TimeoutSeconds -UseBasicParsing -ErrorAction Stop
                $endTime = Get-Date
                $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
                if ($fallbackResponse.StatusCode -eq 200) {
                    Write-Status "${ServiceName}: $FallbackUrl (fallback)" "OK" $responseTime
                    return $true
                }
            } catch {
                # Fallback also failed, continue to main error
            }
        }
        
        $endTime = Get-Date
        $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
        Write-Status "${ServiceName}: Not accessible - $($_.Exception.Message)" "FAIL" $responseTime
        return $false
    }
}

function Test-PostgreSQLConnection {
    param(
        [string]$DbHost = "localhost",
        [int]$Port = 5432,
        [string]$Database = "thedish",
        [string]$User = "thedish",
        [string]$Password = "thedish_dev_password"
    )
    
    $startTime = Get-Date
    try {
        # Try to connect using psql if available, otherwise use .NET connection test
        $env:PGPASSWORD = $Password
        $testQuery = "SELECT 1;"
        $result = & psql -h $DbHost -p $Port -U $User -d $Database -c $testQuery 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            $endTime = Get-Date
            $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
            Write-Status "PostgreSQL: Connected to $Database" "OK" $responseTime
            
            # Test PostGIS extension
            $postgisQuery = "SELECT PostGIS_version();"
            $postgisResult = & psql -h $DbHost -p $Port -U $User -d $Database -c $postgisQuery 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Status "PostGIS Extension: Available" "OK"
            } else {
                Write-Status "PostGIS Extension: Not available" "WARN"
            }
            
            return $true
        } else {
            Write-Status "PostgreSQL: Connection failed - $result" "FAIL"
            return $false
        }
        } catch {
            # Fallback: Try TCP connection test
            try {
                $tcpClient = New-Object System.Net.Sockets.TcpClient
                $tcpClient.Connect($DbHost, $Port)
                $connected = $tcpClient.Connected
                $tcpClient.Close()
                
                if ($connected) {
                    $endTime = Get-Date
                    $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
                    Write-Status "PostgreSQL: TCP connection successful (basic check, psql not available)" "OK" $responseTime
                    Write-Status "PostgreSQL: Advanced queries require psql or .NET driver" "WARN"
                    return $true
                } else {
                    Write-Status "PostgreSQL: TCP connection failed" "FAIL"
                    return $false
                }
            } catch {
                Write-Status "PostgreSQL: Connection failed - $($_.Exception.Message)" "FAIL"
                return $false
            }
        } finally {
            Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
        }
}

function Test-RedisConnection {
    param(
        [string]$RedisHost = "localhost",
        [int]$Port = 6379
    )
    
    $startTime = Get-Date
    try {
        # Try using redis-cli if available
        $pingResult = & redis-cli -h $RedisHost -p $Port PING 2>&1
        if ($LASTEXITCODE -eq 0 -and $pingResult -match "PONG") {
            $endTime = Get-Date
            $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
            Write-Status "Redis: Connected (PING successful)" "OK" $responseTime
            
            # Test SET/GET operations
            $testKey = "health_check_test_$(Get-Date -Format 'yyyyMMddHHmmss')"
            $setResult = & redis-cli -h $RedisHost -p $Port SET $testKey "test_value" 2>&1
            $getResult = & redis-cli -h $RedisHost -p $Port GET $testKey 2>&1
            $delResult = & redis-cli -h $RedisHost -p $Port DEL $testKey 2>&1
            
            if ($setResult -match "OK" -and $getResult -match "test_value") {
                Write-Status "Redis: SET/GET operations successful" "OK"
            } else {
                Write-Status "Redis: SET/GET operations failed" "WARN"
            }
            
            return $true
        } else {
            Write-Status "Redis: PING failed - $pingResult" "FAIL"
            return $false
        }
    } catch {
        # Fallback: Try TCP connection test
        try {
            $tcpClient = New-Object System.Net.Sockets.TcpClient
            $tcpClient.Connect($RedisHost, $Port)
            $tcpClient.Close()
            $endTime = Get-Date
            $responseTime = [math]::Round(($endTime - $startTime).TotalMilliseconds)
            Write-Status "Redis: TCP connection successful (basic check)" "OK" $responseTime
            Write-Status "Redis: Advanced operations require redis-cli" "WARN"
            return $true
        } catch {
            Write-Status "Redis: Connection failed - $($_.Exception.Message)" "FAIL"
            return $false
        }
    }
}

# Main execution
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Phase 2: Backend Service Health Checks" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# [1/5] Check Docker Containers
Write-Host "[1/5] Checking Docker Containers..." -ForegroundColor Yellow
Write-Host ""

$containers = @(
    @{ Name = "the-dish-postgres"; Display = "PostgreSQL" }
    @{ Name = "the-dish-redis"; Display = "Redis" }
    @{ Name = "the-dish-rabbitmq"; Display = "RabbitMQ" }
    @{ Name = "the-dish-elasticsearch"; Display = "Elasticsearch" }
    @{ Name = "the-dish-pgadmin"; Display = "pgAdmin" }
    @{ Name = "the-dish-redis-commander"; Display = "Redis Commander" }
    @{ Name = "the-dish-ai-review-analysis"; Display = "AI Review Analysis" }
)

$containerResults = @()
foreach ($container in $containers) {
    $result = Test-DockerContainer -ContainerName $container.Name -DisplayName $container.Display
    $containerResults += $result
}

Write-Host ""

# [2/5] Check Service Health Endpoints
Write-Host "[2/5] Checking Service Health Endpoints..." -ForegroundColor Yellow
Write-Host ""

$services = @(
    @{ Name = "API Gateway"; Url = "http://localhost:5000/swagger/index.html"; FallbackUrl = "http://localhost:5000/health" }
    @{ Name = "User Service"; Url = "http://localhost:5001/health" }
    @{ Name = "Place Service"; Url = "http://localhost:5002/health" }
    @{ Name = "Review Service"; Url = "http://localhost:5003/health" }
    @{ Name = "AI Review Analysis"; Url = "http://localhost:5004/health" }
)

$serviceResults = @()
foreach ($service in $services) {
    $fallback = if ($service.FallbackUrl) { $service.FallbackUrl } else { $null }
    $result = Test-HealthEndpoint -ServiceName $service.Name -Url $service.Url -FallbackUrl $fallback
    $serviceResults += $result
}

Write-Host ""

# [3/5] Test Database Connectivity
Write-Host "[3/5] Testing Database Connectivity..." -ForegroundColor Yellow
Write-Host ""

$dbResult = Test-PostgreSQLConnection

Write-Host ""

# [4/5] Test Redis Connectivity
Write-Host "[4/5] Testing Redis Connectivity..." -ForegroundColor Yellow
Write-Host ""

$redisResult = Test-RedisConnection

Write-Host ""

# [5/5] Summary
Write-Host "[5/5] Summary" -ForegroundColor Yellow
Write-Host ""

$totalTests = $containerResults.Count + $serviceResults.Count + 1 + 1  # containers + services + db + redis
$dbTestPassed = if ($dbResult -eq $true) { 1 } else { 0 }
$redisTestPassed = if ($redisResult -eq $true) { 1 } else { 0 }
$passedTests = ($containerResults | Where-Object { $_ -eq $true }).Count + 
               ($serviceResults | Where-Object { $_ -eq $true }).Count + 
               $dbTestPassed + 
               $redisTestPassed

if ($script:AllTestsPassed) {
    Write-Host "[OK] All checks passed ($passedTests/$totalTests)" -ForegroundColor Green
} else {
    Write-Host "[WARN] Some checks failed ($passedTests/$totalTests passed)" -ForegroundColor Yellow
}

Write-Host ""

# JSON output if requested
if ($JsonOutput) {
    $jsonOutput = @{
        timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
        allTestsPassed = $script:AllTestsPassed
        summary = @{
            totalTests = $totalTests
            passedTests = $passedTests
            failedTests = $totalTests - $passedTests
        }
        results = $script:TestResults
    }
    $jsonOutput | ConvertTo-Json -Depth 10
}

# Exit code
if ($script:AllTestsPassed) {
    exit 0
} else {
    exit 1
}


