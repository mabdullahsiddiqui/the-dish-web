# AWS S3 Setup Guide for The Dish

## Option 1: AWS Console (Recommended for Quick Setup)

### Step 1: Create S3 Bucket

1. **Go to AWS Console:**
   - Visit: https://console.aws.amazon.com/s3/
   - Sign in with your AWS account

2. **Create Bucket:**
   - Click **"Create bucket"**
   - **Bucket name:** `thedish-review-photos` (must be globally unique, add suffix if needed)
   - **AWS Region:** `us-east-1` (or your preferred region)
   - **Object Ownership:** ACLs disabled (recommended)
   - **Block Public Access settings:**
     - ⚠️ **UNCHECK** "Block all public access" (we need public read for photos)
     - Acknowledge the warning
   - Click **"Create bucket"**

### Step 2: Configure Bucket Policy (Public Read Access)

1. **Navigate to your bucket:**
   - Click on `thedish-review-photos` in the bucket list

2. **Go to Permissions tab:**
   - Click the **"Permissions"** tab

3. **Edit Bucket Policy:**
   - Scroll down to **"Bucket policy"**
   - Click **"Edit"**
   - Paste this policy (replace `thedish-review-photos` if you used a different name):

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "PublicReadGetObject",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::thedish-review-photos/*"
    }
  ]
}
```

   - Click **"Save changes"**

### Step 3: Configure CORS (Optional but Recommended)

1. **In the Permissions tab:**
   - Scroll to **"Cross-origin resource sharing (CORS)"**
   - Click **"Edit"**
   - Paste this configuration:

```json
[
  {
    "AllowedHeaders": ["*"],
    "AllowedMethods": ["GET", "PUT", "POST", "HEAD"],
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:5000"],
    "ExposeHeaders": ["ETag"],
    "MaxAgeSeconds": 3000
  }
]
```

   - Click **"Save changes"**

### Step 4: Create IAM User for Application Access

1. **Go to IAM Console:**
   - Visit: https://console.aws.amazon.com/iam/
   - Click **"Users"** in the left sidebar
   - Click **"Create user"**

2. **User Details:**
   - **User name:** `thedish-s3-uploader`
   - Click **"Next"**

3. **Set Permissions:**
   - Select **"Attach policies directly"**
   - Search for and select: **"AmazonS3FullAccess"** (or create a custom policy for better security)
   - Click **"Next"**
   - Click **"Create user"**

4. **Create Access Keys:**
   - Click on the newly created user
   - Go to **"Security credentials"** tab
   - Scroll to **"Access keys"**
   - Click **"Create access key"**
   - Select **"Application running outside AWS"**
   - Click **"Next"**
   - Add description: "The Dish Photo Upload"
   - Click **"Create access key"**
   - **⚠️ IMPORTANT:** Copy both:
     - **Access key ID**
     - **Secret access key**
   - Store them securely (you won't see the secret again!)

### Step 5: Configure Your Application

1. **Update Backend Configuration:**

Edit `backend/src/Services/TheDish.Review.API/appsettings.json`:

```json
{
  "AWS": {
    "Profile": "default",
    "Region": "us-east-1",
    "S3": {
      "BucketName": "thedish-review-photos",
      "Region": "us-east-1"
    }
  }
}
```

2. **Set Environment Variables (Windows):**

**Option A: User Environment Variables (Recommended for Development)**
```powershell
# Open PowerShell as Administrator
[System.Environment]::SetEnvironmentVariable('AWS_ACCESS_KEY_ID', 'YOUR_ACCESS_KEY_ID', 'User')
[System.Environment]::SetEnvironmentVariable('AWS_SECRET_ACCESS_KEY', 'YOUR_SECRET_ACCESS_KEY', 'User')
[System.Environment]::SetEnvironmentVariable('AWS_REGION', 'us-east-1', 'User')
```

**Option B: Create `.aws/credentials` file:**
```powershell
# Create AWS directory
New-Item -ItemType Directory -Force -Path "$env:USERPROFILE\.aws"

# Create credentials file
@"
[default]
aws_access_key_id = YOUR_ACCESS_KEY_ID
aws_secret_access_key = YOUR_SECRET_ACCESS_KEY
region = us-east-1
"@ | Out-File -FilePath "$env:USERPROFILE\.aws\credentials" -Encoding ASCII
```

**Option C: For Development Only - appsettings.Development.json:**
```json
{
  "AWS": {
    "Profile": "default",
    "Region": "us-east-1"
  }
}
```
Then use credentials file from Option B.

---

## Option 2: Install AWS CLI (For Future Use)

### Windows Installation:

1. **Download AWS CLI:**
   - Visit: https://aws.amazon.com/cli/
   - Download the Windows installer (MSI)

2. **Install:**
   - Run the installer
   - Follow the installation wizard
   - Restart your terminal after installation

3. **Configure AWS CLI:**
```bash
aws configure
# Enter:
# - AWS Access Key ID
# - AWS Secret Access Key
# - Default region: us-east-1
# - Default output format: json
```

4. **Create Bucket (after CLI is installed):**
```bash
aws s3 mb s3://thedish-review-photos --region us-east-1
```

---

## Option 3: Use LocalStack for Development (No AWS Account Needed)

If you want to test locally without AWS:

1. **Add to docker-compose.yml:**
```yaml
  localstack:
    image: localstack/localstack:latest
    ports:
      - "4566:4566"
    environment:
      - SERVICES=s3
      - DEBUG=1
      - DATA_DIR=/tmp/localstack/data
    volumes:
      - "./localstack:/tmp/localstack"
```

2. **Update appsettings.Development.json:**
```json
{
  "AWS": {
    "ServiceURL": "http://localhost:4566",
    "S3": {
      "BucketName": "thedish-review-photos",
      "Region": "us-east-1",
      "ForcePathStyle": true
    }
  }
}
```

3. **Create bucket in LocalStack:**
```bash
aws --endpoint-url=http://localhost:4566 s3 mb s3://thedish-review-photos
```

---

## Testing Your Setup

### 1. Test Backend Upload Endpoint

**Using PowerShell:**
```powershell
# Create a test image
$testImage = "C:\path\to\test-image.jpg"

# Upload via API
$uri = "http://localhost:5003/api/v1/photos/upload"
$headers = @{
    "Authorization" = "Bearer YOUR_JWT_TOKEN"
}

$form = @{
    file = Get-Item -Path $testImage
}

Invoke-RestMethod -Uri $uri -Method Post -Headers $headers -Form $form
```

### 2. Test from Web App

1. Start backend services:
```powershell
cd backend
.\scripts\start-services.ps1
```

2. Start web app:
```powershell
cd web
npm run dev
```

3. Navigate to: http://localhost:3000
4. Go to a place detail page
5. Click "Write Review"
6. Upload a photo
7. Submit the review

### 3. Verify in S3

1. Go to AWS S3 Console
2. Open your bucket
3. You should see uploaded files with GUID prefixes

---

## Security Best Practices

### For Production:

1. **Use CloudFront instead of direct S3 URLs:**
   - Better performance (CDN)
   - Can use signed URLs for temporary access
   - Hides S3 bucket name

2. **Restrict IAM User Permissions:**

Instead of `AmazonS3FullAccess`, create a custom policy:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:GetObject",
        "s3:DeleteObject"
      ],
      "Resource": "arn:aws:s3:::thedish-review-photos/*"
    },
    {
      "Effect": "Allow",
      "Action": "s3:ListBucket",
      "Resource": "arn:aws:s3:::thedish-review-photos"
    }
  ]
}
```

3. **Enable S3 Versioning:**
   - Protects against accidental deletion
   - Can recover previous versions

4. **Set up Lifecycle Policies:**
   - Archive old photos to Glacier
   - Delete temporary uploads after 24 hours

5. **Enable Server-Side Encryption:**
   - In bucket properties, enable default encryption
   - Use AWS-managed keys (SSE-S3)

---

## Troubleshooting

### "Access Denied" Error:
- Check IAM user has correct permissions
- Verify bucket policy allows public read
- Check AWS credentials are set correctly

### "Bucket Not Found" Error:
- Verify bucket name in appsettings.json matches actual bucket
- Check region is correct

### Photos Not Displaying:
- Check bucket policy allows public GetObject
- Verify CORS configuration
- Check browser console for errors

### Upload Fails:
- Check file size (must be < 10MB)
- Verify file is an image type
- Check AWS credentials are valid
- Review backend logs for errors

---

## Quick Start Checklist

- [ ] Create AWS account (if needed)
- [ ] Create S3 bucket via AWS Console
- [ ] Set bucket policy for public read
- [ ] Configure CORS
- [ ] Create IAM user
- [ ] Generate access keys
- [ ] Set environment variables or credentials file
- [ ] Update appsettings.json with bucket name
- [ ] Test upload endpoint
- [ ] Test from web app
- [ ] Verify photos in S3

---

## Cost Estimation

**AWS S3 Pricing (us-east-1):**
- Storage: $0.023 per GB/month
- PUT requests: $0.005 per 1,000 requests
- GET requests: $0.0004 per 1,000 requests

**Example for 1,000 reviews with 3 photos each (avg 2MB):**
- Storage: 6GB × $0.023 = $0.14/month
- Uploads: 3,000 × $0.005/1000 = $0.015
- Views (10k/month): 10,000 × $0.0004/1000 = $0.004

**Total: ~$0.16/month** (very affordable!)

---

## Next Steps After Setup

1. **Test the complete flow**
2. **Consider CloudFront for production**
3. **Set up monitoring (CloudWatch)**
4. **Implement image optimization (resize, compress)**
5. **Add photo moderation workflow**

---

**Need Help?** 
- AWS S3 Documentation: https://docs.aws.amazon.com/s3/
- AWS Free Tier: https://aws.amazon.com/free/
