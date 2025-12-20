using System.IO;
using System.Threading.Tasks;

namespace TheDish.Common.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteFileAsync(string fileKey);
        string GetFileUrl(string fileKey);
    }
}
