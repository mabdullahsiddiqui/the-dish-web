# Gmail SMTP Setup Guide

## Quick Setup (5 minutes)

This guide will help you configure Gmail SMTP to send password reset emails from your application.

---

## Prerequisites

- A Gmail account
- 2-Factor Authentication enabled on your Gmail account

---

## Step 1: Enable 2-Factor Authentication

Gmail requires 2-Factor Authentication (2FA) to generate App Passwords.

1. Go to your Google Account: https://myaccount.google.com/
2. Click **Security** in the left sidebar
3. Under **Signing in to Google**, find **2-Step Verification**
4. Click **2-Step Verification** and follow the prompts to enable it
   - You'll need to verify your phone number
   - You may need to verify your account with a code sent to your phone

---

## Step 2: Generate App Password

Once 2FA is enabled, generate an App Password for the application:

1. Go to: https://myaccount.google.com/apppasswords
   - Or navigate: Google Account → Security → 2-Step Verification → App passwords
2. You may be asked to sign in again
3. Under **Select app**, choose **Mail**
4. Under **Select device**, choose **Other (Custom name)**
5. Enter a name like "The Dish Application" and click **Generate**
6. **Copy the 16-character password** (it will look like: `abcd efgh ijkl mnop`)
   - ⚠️ **Important**: You can only see this password once. Copy it now!
   - Remove the spaces when using it (e.g., `abcdefghijklmnop`)

---

## Step 3: Configure Application

Update the `appsettings.Development.json` file with your Gmail credentials:

### File Location:
```
backend/src/Services/TheDish.User.API/appsettings.Development.json
```

### Configuration:

```json
{
  "Email": {
    "Provider": "SMTP",
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "your-email@gmail.com",
      "Password": "abcdefghijklmnop",
      "FromEmail": "your-email@gmail.com",
      "FromName": "The Dish",
      "EnableSsl": true
    }
  }
}
```

### Replace:
- `your-email@gmail.com` → Your actual Gmail address
- `abcdefghijklmnop` → Your 16-character App Password (no spaces)

### Example:
```json
{
  "Email": {
    "Provider": "SMTP",
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "john.doe@gmail.com",
      "Password": "abcd efgh ijkl mnop",
      "FromEmail": "john.doe@gmail.com",
      "FromName": "The Dish",
      "EnableSsl": true
    }
  }
}
```

---

## Step 4: Restart User Service

After updating the configuration:

1. **Stop** the User Service (close the PowerShell window running it)
2. **Start** the User Service again
3. The new email configuration will be loaded

---

## Step 5: Test Email Sending

1. Go to: http://localhost:3000/forgot-password
2. Enter a valid email address (must be registered in the system)
3. Click "Send Reset Code"
4. **Check your email inbox** (and spam folder) for the password reset email
5. The email should contain:
   - Subject: "Reset Your Password - The Dish"
   - A 6-digit code in a styled box
   - Instructions to use the code

---

## Troubleshooting

### "SMTP configuration is incomplete" Error

**Problem**: Missing or empty SMTP settings

**Solution**: 
- Verify all fields in `appsettings.Development.json` are filled
- Check that `Provider` is set to `"SMTP"` (not `"Console"`)
- Ensure `Host`, `Username`, and `Password` are not empty

### "Invalid credentials" or Authentication Failed

**Problem**: Wrong password or username

**Solution**:
- Make sure you're using the **App Password**, not your regular Gmail password
- Verify the App Password has no spaces (remove them if present)
- Check that your Gmail address is correct
- Ensure 2FA is enabled on your Gmail account

### "The SMTP server requires a secure connection" Error

**Problem**: SSL/TLS configuration issue

**Solution**:
- Verify `EnableSsl` is set to `true`
- Check that `Port` is `587` (not `465` or `25`)

### Email Not Received

**Problem**: Email not arriving in inbox

**Solution**:
- Check **Spam/Junk folder**
- Wait a few minutes (Gmail may delay emails)
- Verify the email address is correct
- Check User Service logs for errors
- Try sending to a different email address

### "Less secure app access" Error

**Problem**: Gmail blocking the connection

**Solution**:
- This shouldn't happen with App Passwords
- Make sure you're using an App Password, not your regular password
- Verify 2FA is enabled

---

## Security Best Practices

1. **Never commit credentials to Git**
   - Add `appsettings.Development.json` to `.gitignore` if it contains real credentials
   - Use environment variables for production

2. **Use App Passwords, not regular passwords**
   - App Passwords are more secure
   - Can be revoked individually if compromised

3. **Rotate App Passwords regularly**
   - Generate new App Passwords periodically
   - Revoke old ones you're no longer using

4. **For Production:**
   - Use environment variables or secure configuration management
   - Consider using a dedicated email service (SendGrid, AWS SES) for better deliverability

---

## Gmail Limits

- **Free Gmail**: 500 emails per day
- **Google Workspace**: 2,000 emails per day
- If you exceed limits, emails will be rejected

---

## Alternative: Use Environment Variables

Instead of storing credentials in `appsettings.Development.json`, you can use environment variables:

### Windows PowerShell:
```powershell
$env:Email__Provider = "SMTP"
$env:Email__Smtp__Host = "smtp.gmail.com"
$env:Email__Smtp__Port = "587"
$env:Email__Smtp__Username = "your-email@gmail.com"
$env:Email__Smtp__Password = "your-app-password"
$env:Email__Smtp__FromEmail = "your-email@gmail.com"
$env:Email__Smtp__FromName = "The Dish"
$env:Email__Smtp__EnableSsl = "true"
```

### Linux/Mac:
```bash
export Email__Provider="SMTP"
export Email__Smtp__Host="smtp.gmail.com"
export Email__Smtp__Port="587"
export Email__Smtp__Username="your-email@gmail.com"
export Email__Smtp__Password="your-app-password"
export Email__Smtp__FromEmail="your-email@gmail.com"
export Email__Smtp__FromName="The Dish"
export Email__Smtp__EnableSsl="true"
```

---

## Quick Reference

| Setting | Value |
|---------|-------|
| Host | `smtp.gmail.com` |
| Port | `587` |
| EnableSsl | `true` |
| Username | Your Gmail address |
| Password | 16-character App Password |
| FromEmail | Your Gmail address |
| FromName | "The Dish" |

---

## Need Help?

If you encounter issues:

1. Check User Service logs for detailed error messages
2. Verify all configuration values are correct
3. Test with a different email address
4. Try generating a new App Password
5. Ensure 2FA is enabled on your Gmail account

---

## Success!

Once configured, password reset emails will be sent automatically when users request a password reset. The email will contain a beautiful HTML template with the 6-digit reset code.

