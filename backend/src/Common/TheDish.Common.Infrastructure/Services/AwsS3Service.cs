using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TheDish.Common.Application.Interfaces;
using TheDish.Common.Infrastructure.Configuration;

namespace TheDish.Common.Infrastructure.Services
{
    public class AwsS3Service : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsS3Settings _settings;

        public AwsS3Service(IAmazonS3 s3Client, IOptions<AwsS3Settings> settings)
        {
            _s3Client = s3Client;
            _settings = settings.Value;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var key = $"{Guid.NewGuid()}_{fileName}";
            
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType
                // Note: We are not setting CannedACL to PublicRead because S3 buckets often block public ACLs by default.
                // We assume the bucket policy allows public read or we use CloudFront.
                // For this MVP, we'll assume the bucket is configured to allow public access to objects or we return a presigned URL (though presigned is temporary).
                // Let's stick to returning the direct URL and assume public bucket policy for now.
            };

            await _s3Client.PutObjectAsync(request);

            // Construct the URL. This assumes standard S3 URL format.
            // If using a custom domain or CloudFront, this would need to change.
            return $"https://{_settings.BucketName}.s3.amazonaws.com/{key}";
        }

        public async Task DeleteFileAsync(string fileKey)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = fileKey
            };

            await _s3Client.DeleteObjectAsync(request);
        }

        public string GetFileUrl(string fileKey)
        {
            // If the fileKey is already a full URL, return it.
            if (fileKey.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return fileKey;

            return $"https://{_settings.BucketName}.s3.amazonaws.com/{fileKey}";
        }
    }
}
