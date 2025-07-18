using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Asp.Versioning;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [EnableCors("DevCorsPolicy")]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImagesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Serve uploaded images with proper CORS headers
        /// Alternative to static file serving for CORS compliance
        /// </summary>
        /// <param name="filename">Image filename</param>
        /// <returns>Image file with CORS headers</returns>
        [HttpGet("{filename}")]
        public async Task<IActionResult> GetImage(string filename)
        {
            try
            {
                // Validate filename
                if (string.IsNullOrEmpty(filename) || filename.Contains("..") || filename.Contains("/") || filename.Contains("\\"))
                {
                    return BadRequest("Invalid filename");
                }

                // Construct file path
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsPath, filename);

                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Image not found");
                }

                // Get file info
                var fileInfo = new FileInfo(filePath);
                var contentType = GetContentType(fileInfo.Extension);

                // Read file
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Return file with proper content type and CORS headers
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error serving image: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle OPTIONS requests for CORS preflight
        /// </summary>
        /// <param name="filename">Image filename</param>
        /// <returns>OK response with CORS headers</returns>
        [HttpOptions("{filename}")]
        public IActionResult OptionsImage(string filename)
        {
            return Ok();
        }

        /// <summary>
        /// Handle HEAD requests for image accessibility checks
        /// </summary>
        /// <param name="filename">Image filename</param>
        /// <returns>Head response for accessibility check</returns>
        [HttpHead("{filename}")]
        public IActionResult HeadImage(string filename)
        {
            try
            {
                // Validate filename
                if (string.IsNullOrEmpty(filename) || filename.Contains("..") || filename.Contains("/") || filename.Contains("\\"))
                {
                    return BadRequest();
                }

                // Construct file path
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsPath, filename);

                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                // Get file info
                var fileInfo = new FileInfo(filePath);
                var contentType = GetContentType(fileInfo.Extension);

                // Set headers without body
                Response.Headers.Add("Content-Type", contentType);
                Response.Headers.Add("Content-Length", fileInfo.Length.ToString());

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get MIME content type for file extension
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns>MIME content type</returns>
        private static string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }
    }
} 