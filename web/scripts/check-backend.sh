#!/bin/bash
# Script to check if backend services are running
# This verifies connectivity to the API Gateway and all microservices

echo "Checking The Dish Backend Services..."
echo ""

check_service() {
    local name=$1
    local url=$2
    
    if curl -s -f -o /dev/null -w "%{http_code}" "$url" | grep -q "200"; then
        echo "✓ $name is running"
        return 0
    else
        echo "✗ $name is not accessible"
        return 1
    fi
}

ALL_RUNNING=true

check_service "API Gateway" "http://localhost:5000/swagger/index.html" || ALL_RUNNING=false
check_service "User Service" "http://localhost:5001/swagger/index.html" || ALL_RUNNING=false
check_service "Place Service" "http://localhost:5002/swagger/index.html" || ALL_RUNNING=false
check_service "Review Service" "http://localhost:5003/swagger/index.html" || ALL_RUNNING=false

echo ""
if [ "$ALL_RUNNING" = true ]; then
    echo "All backend services are running!"
    echo ""
    echo "API Gateway: http://localhost:5000/api/v1"
    echo "Swagger UI:  http://localhost:5000/swagger"
else
    echo "Some services are not running. Please start them using:"
    echo "  cd ../backend"
    echo "  ./scripts/start-services.sh"
    echo ""
    echo "Or manually start Docker containers first:"
    echo "  cd ../backend"
    echo "  docker compose up -d"
fi

echo ""
