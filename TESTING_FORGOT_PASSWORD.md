# Testing Guide: Forgot Password Feature

## ✅ Implementation Complete

All features have been implemented:
- ✅ Facebook UI commented out
- ✅ Forgot password backend (commands, handlers, email service)
- ✅ Forgot password frontend (forms, pages, API integration)
- ✅ Database migration applied

---

## 🚀 Quick Start Testing

### Prerequisites

1. **Backend Services Running:**
   - User Service (port 5001)
   - API Gateway (port 5000)
   - PostgreSQL database

2. **Frontend Running:**
   - Web App (port 3000)

### Test Flow

#### Step 1: Access Login Page
1. Navigate to: http://localhost:3000/login
2. **Verify:** Facebook login button is NOT visible
3. **Verify:** Google login button IS visible

#### Step 2: Request Password Reset
1. Click "Forgot password?" link below password field
2. You should be redirected to: http://localhost:3000/forgot-password
3. Enter a valid email address (must be registered in the system)
4. Click "Send Reset Code"
5. **Check User Service Console:** You should see a log message like:
   ```
   Password reset code for user@example.com: 123456
   ```
6. **Verify:** Success message appears: "If the email exists, a password reset code has been sent."

#### Step 3: Reset Password
1. Navigate to: http://localhost:3000/reset-password
   - Optional: Add `?email=your@email.com` to pre-fill email
2. Enter:
   - **Email:** The email you used in step 2
   - **Reset Code:** The 6-digit code from User Service console
   - **New Password:** At least 8 characters with:
     - One uppercase letter
     - One lowercase letter
     - One number
     - One special character
   - **Confirm Password:** Must match new password
3. Click "Reset Password"
4. **Verify:** Success message appears
5. **Verify:** Redirected to login page

#### Step 4: Test Login with New Password
1. On login page, enter:
   - Email: The email you used
   - Password: Your new password
2. Click "Sign In"
3. **Verify:** Login successful

---

## 🧪 Additional Test Cases

### Test Invalid Code
1. Request password reset
2. Use an incorrect code (e.g., "000000")
3. **Expected:** Error message: "Invalid or expired reset code"

### Test Expired Code
1. Request password reset
2. Wait 16+ minutes (code expires after 15 minutes)
3. Try to use the code
4. **Expected:** Error message: "Invalid or expired reset code"

### Test Invalid Email
1. Go to forgot password page
2. Enter a non-existent email
3. **Expected:** Still shows success message (security: prevents email enumeration)

### Test Weak Password
1. Go to reset password page
2. Enter valid code but weak password (e.g., "12345678")
3. **Expected:** Validation error showing password requirements

### Test Password Mismatch
1. Go to reset password page
2. Enter different values for "New Password" and "Confirm Password"
3. **Expected:** Validation error: "Passwords do not match"

### Test Social Login Only Account
1. Try to reset password for an account that only uses Google login
2. **Expected:** Code is not sent (account has no password to reset)

---

## 🔍 Verification Checklist

### UI/UX
- [ ] Facebook login button is NOT visible on login/register pages
- [ ] Google login button IS visible and works
- [ ] "Forgot password?" link appears below password field on login page
- [ ] Forgot password page uses 3D UI components (GlassCard, Button3D)
- [ ] Reset password page uses 3D UI components
- [ ] Forms have proper validation and error messages
- [ ] Loading states work correctly
- [ ] Success/error messages are clear and user-friendly

### Functionality
- [ ] Can request password reset code
- [ ] Code appears in User Service console
- [ ] Code is exactly 6 digits
- [ ] Can reset password with valid code
- [ ] Invalid codes are rejected
- [ ] Expired codes are rejected
- [ ] Password validation works (strength requirements)
- [ ] Password confirmation matching works
- [ ] Success redirects to login page
- [ ] Can login with new password

### Security
- [ ] Email enumeration prevention (always returns success)
- [ ] Code expires after 15 minutes
- [ ] Code is cryptographically secure (not predictable)
- [ ] Password is properly hashed
- [ ] Reset code is cleared after successful reset

---

## 🐛 Troubleshooting

### Code Not Appearing in Console
- **Check:** User Service is running and logs are visible
- **Check:** Email exists in database
- **Check:** Account is not social-login-only

### Migration Not Applied
- **Solution:** Restart User Service (EnsureCreated() will apply in development)
- **Or:** Run `dotnet ef database update` manually

### API Errors
- **Check:** API Gateway is running (port 5000)
- **Check:** User Service is running (port 5001)
- **Check:** Routes are configured in `ocelot.json`
- **Check:** CORS is enabled

### Frontend Errors
- **Check:** Web app is running (port 3000)
- **Check:** API base URL is correct in `.env.local`
- **Check:** Browser console for errors

---

## 📝 Notes

- **Email Service:** Currently logs to console. In production, integrate with SMTP/SendGrid/AWS SES
- **Code Expiry:** 15 minutes from generation
- **Rate Limiting:** 10 requests/minute for password reset endpoints
- **Security:** Always returns success for forgot password to prevent email enumeration

---

## ✅ Success Criteria

All features are working correctly when:
1. ✅ Facebook UI is hidden
2. ✅ Google login still works
3. ✅ Forgot password flow works end-to-end
4. ✅ Code generation and validation work
5. ✅ Password reset works with valid code
6. ✅ Invalid/expired codes are rejected
7. ✅ UI is consistent with 3D design system

