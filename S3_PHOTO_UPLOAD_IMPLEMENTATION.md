# S3 Photo Upload Implementation - Complete

**Date:** November 27, 2025  
**Status:** ✅ IMPLEMENTED

## Overview
This document outlines the complete implementation of AWS S3 photo upload functionality for "The Dish" restaurant review application.

---

## Backend Implementation

### 1. Common Infrastructure Layer

#### **Files Created:**

**`TheDish.Common.Application/Interfaces/IFileStorageService.cs`**
- Interface for file storage operations
- Methods: `UploadFileAsync`, `DeleteFileAsync`, `GetFileUrl`

**`TheDish.Common.Infrastructure/Configuration/AwsS3Settings.cs`**
- Configuration class for S3 settings
- Properties: `BucketName`, `Region`

**`TheDish.Common.Infrastructure/Services/AwsS3Service.cs`**
- Implementation of `IFileStorageService`
- Uses AWS SDK for S3 operations
- Generates unique file keys with GUID prefixes
- Returns public S3 URLs

#### **Files Modified:**

**`TheDish.Common.Infrastructure/Extensions/ServiceCollectionExtensions.cs`**
- Added AWS S3 service registration
- Configured `AwsS3Settings` from configuration
- Registered `IAmazonS3` client
- Registered `IFileStorageService` implementation

**`TheDish.Common.Infrastructure/TheDish.Common.Infrastructure.csproj`**
- Added NuGet packages:
  - `AWSSDK.S3`
  - `AWSSDK.Extensions.NETCore.Setup`

---

### 2. Review Service

#### **API Controller:**

**`TheDish.Review.API/Controllers/PhotosController.cs`** (NEW)
- Endpoint: `POST /api/v1/photos/upload`
- Accepts `multipart/form-data`
- Validates:
  - File type (images only)
  - File size (10MB limit)
- Returns: `{ "url": "https://..." }`

#### **Configuration:**

**`TheDish.Review.API/appsettings.json`**
- Added AWS S3 configuration:
```json
"AWS": {
  "S3": {
    "BucketName": "thedish-review-photos",
    "Region": "us-east-1",
    "BaseUrl": "https://thedish-review-photos.s3.amazonaws.com"
  }
}
```

#### **Domain Entities:**

**`TheDish.Review.Domain/Entities/Review.cs`**
- Added `AddPhoto` method to create `ReviewPhoto` entities
- Maintains sync between `Photos` collection and `PhotoUrls` list

**`TheDish.Review.Domain/Entities/ReviewPhoto.cs`**
- Existing entity for photo metadata
- Properties: `Url`, `ThumbnailUrl`, `Caption`, `UploadedBy`, `UploadedAt`

#### **Application Layer:**

**`TheDish.Review.Application/Commands/CreateReviewCommand.cs`**
- Added `PhotoUrls` property (List<string>)

**`TheDish.Review.Application/Commands/CreateReviewCommandHandler.cs`**
- Added logic to process `PhotoUrls` from request
- Creates `ReviewPhoto` entities for each URL
- Maintains backward compatibility with existing reviews

---

## Frontend Implementation

### 1. API Client

**`web/lib/api/photos.ts`** (NEW)
- `photosApi.upload(file: File)` method
- Sends `multipart/form-data` to `/api/v1/photos/upload`
- Returns `{ url: string }`

### 2. React Hooks

**`web/hooks/usePhotos.ts`** (NEW)
- `useUploadPhoto()` mutation hook
- Handles photo upload with error handling
- Integrates with React Query

### 3. Types

**`web/types/review.ts`**
- Added `photoUrls?: string[]` to `CreateReviewRequest`

### 4. Components

**`web/components/features/reviews/photo-upload.tsx`** (EXISTING)
- Already implemented with:
  - Drag & drop support
  - Multiple file selection
  - File validation (type, size)
  - Preview with thumbnails
  - Remove functionality

**`web/components/reviews/ReviewSubmissionForm.tsx`** (UPDATED)
- Integrated `PhotoUpload` component
- Replaced manual photo handling
- Upload flow:
  1. User selects photos via `PhotoUpload`
  2. On submit, uploads each photo to S3
  3. Collects uploaded URLs
  4. Submits review with `photoUrls` array
- Shows loading state during upload
- Disables submit button while uploading

---

## Upload Flow

### End-to-End Process:

1. **User Interaction:**
   - User drags/drops or selects photos in `PhotoUpload` component
   - Photos stored in local state as `File[]` objects
   - Preview shown immediately

2. **Form Submission:**
   - User clicks "Submit Review"
   - `onSubmit` handler triggered

3. **Photo Upload:**
   ```typescript
   const uploadedUrls: string[] = [];
   if (photos.length > 0) {
     const uploadPromises = photos.map(photo => uploadPhoto.mutateAsync(photo));
     const results = await Promise.all(uploadPromises);
     results.forEach(res => {
       if (res.success && res.data?.url) uploadedUrls.push(res.data.url);
     });
   }
   ```

4. **Backend Processing:**
   - Each file sent to `POST /api/v1/photos/upload`
   - `PhotosController` validates file
   - `AwsS3Service.UploadFileAsync()` uploads to S3
   - Returns S3 URL

5. **Review Creation:**
   - Frontend sends `CreateReviewRequest` with `photoUrls`
   - Backend creates review with photos
   - `ReviewPhoto` entities created and linked

---

## Configuration Requirements

### AWS Setup:

1. **Create S3 Bucket:**
   ```bash
   aws s3 mb s3://thedish-review-photos --region us-east-1
   ```

2. **Configure Bucket Policy** (for public read access):
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

3. **Configure CORS** (if needed for direct uploads):
   ```json
   [
     {
       "AllowedHeaders": ["*"],
       "AllowedMethods": ["GET", "PUT", "POST"],
       "AllowedOrigins": ["http://localhost:3000"],
       "ExposeHeaders": []
     }
   ]
   ```

4. **AWS Credentials:**
   - Set environment variables or use AWS credentials file:
     - `AWS_ACCESS_KEY_ID`
     - `AWS_SECRET_ACCESS_KEY`
     - `AWS_REGION`

### Application Configuration:

**Backend (`appsettings.json`):**
```json
"AWS": {
  "S3": {
    "BucketName": "thedish-review-photos",
    "Region": "us-east-1"
  }
}
```

**Frontend (`.env.local`):**
```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000/api/v1
```

---

## Testing Checklist

### Backend:
- [ ] AWS credentials configured
- [ ] S3 bucket created and accessible
- [ ] `POST /api/v1/photos/upload` endpoint responds
- [ ] File validation works (type, size)
- [ ] Photos uploaded to S3 successfully
- [ ] Review creation with photos works

### Frontend:
- [ ] PhotoUpload component renders
- [ ] Drag & drop works
- [ ] File selection works
- [ ] Preview displays correctly
- [ ] Remove photo works
- [ ] Upload progress shown
- [ ] Review submission with photos succeeds
- [ ] Photos display in review after submission

### Integration:
- [ ] End-to-end flow: select → upload → submit → display
- [ ] Error handling for upload failures
- [ ] Multiple photos upload correctly
- [ ] Large files rejected appropriately

---

## Security Considerations

1. **File Validation:**
   - ✅ Content type validation (images only)
   - ✅ File size limit (10MB)
   - ⚠️ Consider: Image content scanning for inappropriate content

2. **Access Control:**
   - ✅ Authentication required for upload endpoint
   - ⚠️ Consider: Rate limiting on upload endpoint
   - ⚠️ Consider: User quota limits

3. **S3 Security:**
   - ⚠️ Review bucket policy (currently public read)
   - ⚠️ Consider: CloudFront for CDN + signed URLs
   - ⚠️ Consider: Encryption at rest

---

## Future Enhancements

1. **Image Processing:**
   - Thumbnail generation
   - Image compression/optimization
   - Automatic resizing

2. **Advanced Features:**
   - Photo captions
   - Photo ordering/primary photo
   - Photo moderation workflow
   - Photo deletion

3. **Performance:**
   - Client-side image compression before upload
   - Progress indicators for individual photos
   - Retry logic for failed uploads

4. **Storage:**
   - CloudFront CDN integration
   - Lifecycle policies for old photos
   - Cost optimization

---

## API Reference

### Upload Photo
```http
POST /api/v1/photos/upload
Content-Type: multipart/form-data
Authorization: Bearer {token}

file: <binary>
```

**Response:**
```json
{
  "success": true,
  "data": {
    "url": "https://thedish-review-photos.s3.amazonaws.com/abc123_photo.jpg"
  }
}
```

**Errors:**
- `400`: No file uploaded / Invalid file type / File too large
- `401`: Unauthorized
- `500`: Upload failed

---

## Files Summary

### Created:
- `backend/src/Common/TheDish.Common.Application/Interfaces/IFileStorageService.cs`
- `backend/src/Common/TheDish.Common.Infrastructure/Configuration/AwsS3Settings.cs`
- `backend/src/Common/TheDish.Common.Infrastructure/Services/AwsS3Service.cs`
- `backend/src/Services/TheDish.Review.API/Controllers/PhotosController.cs`
- `web/lib/api/photos.ts`
- `web/hooks/usePhotos.ts`

### Modified:
- `backend/src/Common/TheDish.Common.Infrastructure/Extensions/ServiceCollectionExtensions.cs`
- `backend/src/Common/TheDish.Common.Infrastructure/TheDish.Common.Infrastructure.csproj`
- `backend/src/Services/TheDish.Review.API/appsettings.json`
- `backend/src/Services/TheDish.Review.Domain/Entities/Review.cs`
- `backend/src/Services/TheDish.Review.Application/Commands/CreateReviewCommand.cs`
- `backend/src/Services/TheDish.Review.Application/Commands/CreateReviewCommandHandler.cs`
- `web/types/review.ts`
- `web/components/reviews/ReviewSubmissionForm.tsx`

---

## Next Steps

1. **Configure AWS:**
   - Create S3 bucket
   - Set up IAM credentials
   - Configure bucket policy

2. **Test Backend:**
   ```bash
   cd backend
   dotnet build
   dotnet run --project src/Services/TheDish.Review.API
   ```

3. **Test Frontend:**
   ```bash
   cd web
   npm install
   npm run dev
   ```

4. **Verify Integration:**
   - Navigate to a place detail page
   - Click "Write Review"
   - Upload photos
   - Submit review
   - Verify photos appear in review

---

**Implementation Status:** ✅ COMPLETE  
**Ready for Testing:** YES  
**AWS Configuration Required:** YES
