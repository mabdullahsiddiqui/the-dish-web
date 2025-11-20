# Restart Web App

## Quick Restart

1. **Stop the current web app**:
   - Find the terminal running `npm run dev`
   - Press `Ctrl+C` to stop it

2. **Start the web app**:
   ```powershell
   cd web
   npm run dev
   ```

## Verify Everything is Running

After restarting, check:

1. **Backend Services** (should all be running):
   - User Service: http://localhost:5001/swagger
   - Place Service: http://localhost:5002/swagger
   - Review Service: http://localhost:5003/swagger
   - API Gateway: http://localhost:5000/swagger

2. **Web App**:
   - Frontend: http://localhost:3000
   - Registration: http://localhost:3000/register
   - Login: http://localhost:3000/login

3. **Test Google Login**:
   - Go to http://localhost:3000/register
   - Click "Sign in with Google"
   - Should work now! ✅

