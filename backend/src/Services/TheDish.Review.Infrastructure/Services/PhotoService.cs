using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Infrastructure.Services;

public class PhotoService : IPhotoService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _baseUrl;

    public PhotoService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWS:S3:BucketName"] ?? "thedish-review-photos";
        _baseUrl = configuration["AWS:S3:BaseUrl"] ?? $"https://{_bucketName}.s3.amazonaws.com";
    }

    public async Task<string> UploadPhotoAsync(Stream photoStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var key = $"reviews/{Guid.NewGuid()}/{fileName}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = photoStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return $"{_baseUrl}/{key}";
    }

    public async Task<string> GeneratePresignedUrlAsync(string key, int expirationMinutes = 60, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<bool> DeletePhotoAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}











