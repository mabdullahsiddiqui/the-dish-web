namespace TheDish.Place.Application.Interfaces;

public interface IPhotoService
{
    Task<string> UploadPhotoAsync(Stream photoStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<string> GeneratePresignedUrlAsync(string key, int expirationMinutes = 60, CancellationToken cancellationToken = default);
    Task<bool> DeletePhotoAsync(string key, CancellationToken cancellationToken = default);
}








