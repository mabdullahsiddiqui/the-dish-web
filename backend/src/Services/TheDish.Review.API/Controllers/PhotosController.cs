using Microsoft.AspNetCore.Mvc;
using TheDish.Common.Application.Interfaces;

namespace TheDish.Review.API.Controllers
{
    [ApiController]
    [Route("api/v1/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<PhotosController> _logger;

        public PhotosController(IFileStorageService fileStorageService, ILogger<PhotosController> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Basic validation
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Only image files are allowed.");

            if (file.Length > 10 * 1024 * 1024) // 10MB limit
                return BadRequest("File size exceeds 10MB limit.");

            try
            {
                using var stream = file.OpenReadStream();
                var url = await _fileStorageService.UploadFileAsync(stream, file.FileName, file.ContentType);
                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading photo");
                return StatusCode(500, "Internal server error uploading photo.");
            }
        }
    }
}
