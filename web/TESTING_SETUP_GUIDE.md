# Testing Setup Guide - Step by Step

## Current Status: Docker Not Installed

You need Docker to run the backend services. Here's how to proceed:

## Quick Decision Tree

```
Do you want to install Docker?
├─ YES → Follow "Option 1: Install Docker" below
└─ NO  → Choose:
    ├─ Option 2: Install services manually (advanced)
    ├─ Option 3: Test frontend only (limited)
    └─ Option 4: Use mock data (development)
```

## Option 1: Install Docker Desktop (Easiest - Recommended)

### Step 1: Download Docker Desktop

1. Go to: https://www.docker.com/products/docker-desktop/
2. Click "Download for Windows"
3. Save the installer

### Step 2: Install Docker Desktop

1. Run the installer (Docker Desktop Installer.exe)
2. Follow the installation wizard
3. **Important**: Check "Use WSL 2 instead of Hyper-V" if prompted
4. Click "Ok" when installation completes
5. **Restart your computer** if prompted

### Step 3: Start Docker Desktop

1. After restart, find Docker Desktop in Start menu
2. Launch Docker Desktop
3. Wait for Docker to start (whale icon appears in system tray)
4. Docker may ask to accept terms - accept them

### Step 4: Verify Docker

Open PowerShell and run:

```powershell
docker --version
```

You should see something like: `Docker version 24.x.x`

### Step 5: Start Backend Services

```powershell
# Navigate to backend directory
cd ..\backend

# Start all Docker containers
docker compose up -d
```

**Expected Output**: 
```
[+] Running 6/6
 ✔ Container thedish-postgres    Started
 ✔ Container thedish-redis       Started
 ✔ Container thedish-elasticsearch Started
 ✔ Container thedish-rabbitmq     Started
 ✔ Container thedish-pgadmin      Started
 ✔ Container thedish-redis-commander Started
```

### Step 6: Verify Containers

```powershell
docker ps
```

Should show 6 containers running.

### Step 7: Start Backend APIs

```powershell
# Still in backend directory
.\scripts\start-services.ps1
```

This opens 4 PowerShell windows (one for each service).

### Step 8: Verify Backend

```powershell
# From web directory
cd web
.\scripts\check-backend.ps1
```

All services should show as "running".

### Step 9: Start Web App

```powershell
# Still in web directory
npm run dev
```

### Step 10: Test!

1. Open browser: http://localhost:3000
2. Check backend health widget (bottom-right)
3. Start testing!

---

## Troubleshooting Docker Installation

### Issue: "Docker Desktop won't start"

**Solutions**:
- Make sure virtualization is enabled in BIOS
- Check Windows features: Enable "Virtual Machine Platform" and "Windows Subsystem for Linux"
- Restart computer

### Issue: "WSL 2 installation is incomplete"

**Solution**:
1. Open PowerShell as Administrator
2. Run: `wsl --install`
3. Restart computer
4. Try Docker Desktop again

### Issue: "Port already in use"

**Solutions**:
- Check what's using the port: `netstat -ano | findstr :5000`
- Stop the conflicting service
- Or change ports in docker-compose.yml

### Issue: "Docker daemon not running"

**Solution**:
- Make sure Docker Desktop is running (check system tray)
- Right-click Docker icon → "Start"

---

## Alternative: Test Without Backend

If you can't install Docker right now, you can still test the frontend:

### Frontend-Only Testing

```powershell
# Start web app
cd web
npm run dev
```

**What you can test**:
- ✅ Page navigation
- ✅ UI components
- ✅ Form validation
- ✅ Responsive design
- ✅ Client-side features

**What won't work**:
- ❌ API calls (will fail)
- ❌ Data loading (will show errors)
- ❌ User authentication
- ❌ Search functionality

**Note**: You'll see errors in browser console - this is expected without backend.

---

## Next Steps

1. **If Docker installed**: Follow `START_TESTING.md`
2. **If no Docker**: Test frontend UI only, or install Docker later
3. **Need help**: Check `TESTING_WITHOUT_DOCKER.md` for more options

---

**Recommendation**: Install Docker Desktop - it's the fastest path to full testing! 🐳

