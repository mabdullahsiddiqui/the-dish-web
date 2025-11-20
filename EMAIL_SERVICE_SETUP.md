# Email Service Setup Guide

## 📧 Email Provider Options

The application supports multiple email providers. Choose the one that best fits your needs.

---

## 🆓 AWS SES (Recommended for Free Tier)

### Pricing:
- **Free Tier**: 3,000 emails/month for the first 12 months
- **After Free Tier**: $0.10 per 1,000 emails
- **Very cost-effective** for low to medium volume
- **Note**: Additional free tier of 62,000 emails/month available when sending from EC2 instances

### Setup Steps:

1. **Install AWS SDK Package:**
   ```powershell
   cd backend\src\Services\TheDish.User.Infrastructure
   dotnet add package AWSSDK.SimpleEmail
   ```

2. **Configure AWS Credentials:**
   - Option A: AWS IAM User with SES permissions
     - Create IAM user with `AmazonSESFullAccess` policy
     - Generate Access Key ID and Secret Access Key
   - Option B: Use AWS CLI credentials (`~/.aws/credentials`)
   - Option C: Environment variables:
     - `AWS_ACCESS_KEY_ID`
     - `AWS_SECRET_ACCESS_KEY`
     - `AWS_REGION`

3. **Verify Email Address/Domain in SES:**
   - Go to AWS SES Console
   - Verify sender email address or domain
   - Move out of sandbox mode (for production)

4. **Update `appsettings.json`:**
   ```json
   "Email": {
     "Provider": "SES",
     "AwsSes": {
       "Region": "us-east-1",
       "FromEmail": "noreply@yourdomain.com",
       "FromName": "The Dish"
     }
   }
   ```

5. **Update EmailService.cs:**
   - Uncomment and complete the AWS SES implementation
   - Add using statements for AWS SDK

---

## 📬 SMTP (Gmail, Outlook, Custom SMTP)

### Pricing:
- **Gmail**: Free (up to 500 emails/day with App Password)
- **Outlook/Hotmail**: Free (up to 300 emails/day)
- **Custom SMTP**: Varies by provider

### Setup Steps:

1. **For Gmail:**
   - Enable 2-Factor Authentication
   - Generate App Password: https://myaccount.google.com/apppasswords
   - Use App Password (not regular password)

2. **Update `appsettings.json`:**
   ```json
   "Email": {
     "Provider": "SMTP",
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "your-email@gmail.com",
       "Password": "your-app-password",
       "FromEmail": "your-email@gmail.com",
       "FromName": "The Dish",
       "EnableSsl": true
     }
   }
   ```

3. **For Outlook:**
   ```json
   "Smtp": {
     "Host": "smtp-mail.outlook.com",
     "Port": 587,
     "Username": "your-email@outlook.com",
     "Password": "your-password",
     "FromEmail": "your-email@outlook.com",
     "FromName": "The Dish",
     "EnableSsl": true
   }
   ```

---

## 📧 SendGrid

### Pricing:
- **Free Tier**: 100 emails/day forever
- **Essentials Plan**: $19.95/month for 50,000 emails
- **Good for**: Small to medium volume

### Setup Steps:

1. **Create SendGrid Account:**
   - Sign up at https://sendgrid.com
   - Verify your email

2. **Generate API Key:**
   - Go to Settings → API Keys
   - Create API Key with "Mail Send" permissions
   - Copy the API key (shown only once!)

3. **Verify Sender:**
   - Go to Settings → Sender Authentication
   - Verify single sender or domain

4. **Update `appsettings.json`:**
   ```json
   "Email": {
     "Provider": "SendGrid",
     "SendGrid": {
       "ApiKey": "SG.your-api-key-here",
       "FromEmail": "noreply@yourdomain.com",
       "FromName": "The Dish"
     }
   }
   ```

5. **Install SendGrid Package (Optional - for better integration):**
   ```powershell
   cd backend\src\Services\TheDish.User.Infrastructure
   dotnet add package SendGrid
   ```

---

## 🖥️ Console (Development/Testing)

### Default Mode:
- Logs password reset codes to console
- No email sent
- Perfect for development and testing

### Configuration:
```json
"Email": {
  "Provider": "Console"
}
```

---

## 🔧 Configuration Priority

1. **Development**: Uses `appsettings.Development.json`
2. **Production**: Uses `appsettings.json`
3. **Environment Variables**: Can override settings

### Using Environment Variables:
```powershell
$env:Email__Provider = "SMTP"
$env:Email__Smtp__Host = "smtp.gmail.com"
$env:Email__Smtp__Username = "your-email@gmail.com"
$env:Email__Smtp__Password = "your-app-password"
```

---

## ✅ Testing Email Service

1. **Set Provider** in `appsettings.Development.json`
2. **Configure credentials** for chosen provider
3. **Restart User Service**
4. **Test forgot password flow:**
   - Go to http://localhost:3000/forgot-password
   - Enter email
   - Check email inbox (or console if using Console provider)

---

## 🔒 Security Best Practices

1. **Never commit credentials** to Git
2. **Use environment variables** or Azure Key Vault / AWS Secrets Manager in production
3. **Use App Passwords** for Gmail (not regular passwords)
4. **Verify sender** in SES/SendGrid before production
5. **Rate limit** email sending to prevent abuse

---

## 📊 Comparison

| Provider | Free Tier | Best For | Setup Difficulty |
|----------|----------|----------|------------------|
| **AWS SES** | 3K/month (12 months) | Low to medium volume, AWS users | Medium |
| **SendGrid** | 100/day (forever) | Small to medium volume | Easy |
| **Gmail SMTP** | 500/day | Development, small scale | Easy |
| **Console** | Unlimited | Development, testing | Easiest |

---

## 🚀 Quick Start (AWS SES)

If you want to use AWS SES (recommended for free tier):

1. Install package:
   ```powershell
   cd backend\src\Services\TheDish.User.Infrastructure
   dotnet add package AWSSDK.SimpleEmail
   ```

2. Configure in `appsettings.Development.json`:
   ```json
   "Email": {
     "Provider": "SES",
     "AwsSes": {
       "Region": "us-east-1",
       "FromEmail": "noreply@yourdomain.com",
       "FromName": "The Dish"
     }
   }
   ```

3. Set AWS credentials (via environment variables or AWS CLI)

4. Complete the AWS SES implementation in `EmailService.cs`

---

## 📝 Notes

- **Current Implementation**: EmailService supports SMTP and SendGrid via HTTP API
- **AWS SES**: Requires AWSSDK.SimpleEmail package and implementation completion
- **Fallback**: If email sending fails, code is logged to console
- **Error Handling**: All errors are logged, service continues to work

