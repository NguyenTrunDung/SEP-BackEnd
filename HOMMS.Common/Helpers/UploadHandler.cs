using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HOMMS.Common.Helpers
{
    public static class UploadHandler
    {
        private static readonly List<string> ValidExtensions = new()
        {
            ".jpg", ".jpeg", ".png", ".gif"
        };

        private const long MaxSize = 50 * 1024 * 1024; // 50MB

        public static async Task<string> SaveImageAsync(IFormFile file, string webRootPath, string folderName = "uploads")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or not selected.");

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!ValidExtensions.Contains(extension))
                throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed: {string.Join(", ", ValidExtensions)}");

            if (file.Length > MaxSize)
                throw new ArgumentException("Maximum file size exceeded (50MB).");

            string fileName = $"{Guid.NewGuid()}{extension}";
            string folderPath = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{fileName}";
        }
    }
}