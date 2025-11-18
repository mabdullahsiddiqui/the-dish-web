# Testing Without Docker - Alternative Setup

## Current Situation

Docker is not installed on your system. The backend services require:
- PostgreSQL (database)
- Redis (caching)
- RabbitMQ (message queue)
- Elasticsearch (search)

## Option 1: Install Docker Desktop (Recommended)

### Why Docker?
- Easiest way to run all required services
- Consistent environment
- No manual configuration needed

### Installation Steps

1. **Download Docker Desktop for Windows**
   - Visit: https://www.docker.com/products/docker-desktop/
   - Download Docker Desktop for Windows
   - Run the installer

2. **Install and Start Docker**
   - Follow the installation wizard
   - Restart your computer if prompted
   - Start Docker Desktop from Start menu
   - Wait for Docker to start (whale icon in system tray)

3. **Verify Installation**
   ```powershell
   docker --version
   docker compose version
   ```

4. **Start Backend Services**
   ```powershell
   cd ..\backend
   docker compose up -d
   ```

### Time Required: ~15-20 minutes

---

## Option 2: Install Services Locally (Advanced)

If you prefer not to use Docker, you can install services manually:

### Required Services

1. **PostgreSQL** (Required)
   - Download: https://www.postgresql.org/download/windows/
   - Install with default settings
   - Create database: `thedish`
   - User: `thedish`, Password: `thedish_dev_password`
   - Port: `5432`

2. **Redis** (Optional - for caching)
   - Download: https://github.com/microsoftarchive/redis/releases
   - Or use: https://redis.io/download

3. **RabbitMQ** (Optional - for messaging)
   - Requires Erlang first: https://www.erlang.org/downloads
   - Then RabbitMQ: https://www.rabbitmq.com/download.html

4. **Elasticsearch** (Optional - for search)
   - Download: https://www.elastic.co/downloads/elasticsearch

### Configuration

Update connection strings in backend `appsettings.Development.json` files.

**Time Required: ~1-2 hours**

---

## Option 3: Test Frontend Only (Limited Testing)

You can test the frontend UI without backend, but functionality will be limited:

### What Works
- ✅ UI components render
- ✅ Navigation works
- ✅ Form validation
- ✅ Responsive design
- ✅ Client-side features

### What Doesn't Work
- ❌ User registration/login
- ❌ Search functionality
- ❌ Review submission
- ❌ Data loading
- ❌ API calls

### How to Test Frontend Only

1. **Start Web App**
   ```powershell
   cd web
   npm run dev
   ```

2. **Test UI Elements**
   - Navigate between pages
   - Check responsive design
   - Test form validation
   - Verify UI components

3. **Expected Behavior**
   - Pages load but show "no data" or errors
   - Forms validate but can't submit
   - API calls fail (expected)

---

## Option 4: Use Mock Data (Development)

For frontend development, you could:

1. Create mock API responses
2. Use MSW (Mock Service Worker) to intercept API calls
3. Test UI with fake data

This requires additional setup but allows full frontend testing.

---

## Recommendation

**Install Docker Desktop** - It's the fastest and easiest way to get all services running.

### Quick Docker Installation

1. Download: https://www.docker.com/products/docker-desktop/
2. Install (follow wizard)
3. Restart computer
4. Start Docker Desktop
5. Run: `cd ..\backend && docker compose up -d`

**Total time: ~20 minutes**

---

## After Docker Installation

Once Docker is installed:

1. **Start Docker Desktop**
2. **Start Services**
   ```powershell
   cd ..\backend
   docker compose up -d
   ```
3. **Verify Services**
   ```powershell
   docker ps
   ```
   Should show 6 containers running

4. **Start Backend APIs**
   ```powershell
   .\scripts\start-services.ps1
   ```

5. **Start Web App**
   ```powershell
   cd web
   npm run dev
   ```

6. **Test!**
   - Open http://localhost:3000
   - Check backend health widget
   - Start testing!

---

## Need Help?

- **Docker Issues**: Check Docker Desktop is running (whale icon in system tray)
- **Port Conflicts**: Make sure ports 5000-5003, 5432, 6379 are available
- **Service Errors**: Check Docker logs: `docker compose logs`

---

**Next Step**: Install Docker Desktop, then follow `START_TESTING.md`

