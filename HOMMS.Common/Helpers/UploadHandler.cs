using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace HOMMS.Common.Helpers
{
    public static class UploadHandler
    {
        private static readonly List<string> ValidExtensions = new()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        private const long MaxSize = 50 * 1024 * 1024; // 50MB
        private const int BufferSize = 4096; // 4KB buffer for file operations

        /// <summary>
        /// Original SaveImageAsync method for backward compatibility
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <param name="webRootPath">Web root path</param>
        /// <param name="folderName">Folder name (default: uploads)</param>
        /// <returns>Relative path to the saved image</returns>
        public static async Task<string> SaveImageAsync(IFormFile file, string webRootPath, string folderName = "uploads")
        {
            return await SaveImageAsync(file, webRootPath, folderName, null);
        }

        /// <summary>
        /// Enhanced SaveImageAsync with duplicate detection and optimized file handling
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <param name="webRootPath">Web root path</param>
        /// <param name="folderName">Folder name (default: uploads)</param>
        /// <param name="existingImagePath">Existing image path to replace (optional)</param>
        /// <returns>Relative path to the saved image</returns>
        public static async Task<string> SaveImageAsync(IFormFile file, string webRootPath, string folderName = "uploads", string existingImagePath = null)
        {
            // Validate input
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or not selected.");

            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("Web root path cannot be null or empty.");

            // Validate file extension
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!ValidExtensions.Contains(extension))
                throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed: {string.Join(", ", ValidExtensions)}");

            // Validate file size
            if (file.Length > MaxSize)
                throw new ArgumentException($"Maximum file size exceeded. File size: {file.Length / 1024 / 1024}MB, Max allowed: {MaxSize / 1024 / 1024}MB.");

            // Ensure upload directory exists
            string folderPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            try
            {
                // Calculate file hash to detect duplicates
                string fileHash = await CalculateFileHashAsync(file);
                
                // Check for existing file with same hash
                string existingFile = await FindExistingFileByHashAsync(folderPath, fileHash, extension);
                
                if (!string.IsNullOrEmpty(existingFile))
                {
                    // File already exists, return existing path
                    string existingRelativePath = $"/{folderName}/{Path.GetFileName(existingFile)}";
                    
                    // Clean up old image if replacing
                    if (!string.IsNullOrEmpty(existingImagePath))
                    {
                        await DeleteOldImageAsync(webRootPath, existingImagePath, existingRelativePath);
                    }
                    
                    return existingRelativePath;
                }

                // Generate new filename with hash prefix for better organization
                string fileName = $"{fileHash.Substring(0, 8)}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
                string fullPath = Path.Combine(folderPath, fileName);

                // Ensure unique filename (additional safety check)
                int counter = 1;
                string originalFileName = fileName;
                while (File.Exists(fullPath))
                {
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
                    fileName = $"{nameWithoutExt}_{counter}{extension}";
                    fullPath = Path.Combine(folderPath, fileName);
                    counter++;
                }

                // Save the new file with optimized buffer
                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, useAsync: true))
                {
                    await file.CopyToAsync(stream);
                    await stream.FlushAsync();
                }

                // Clean up old image if replacing
                if (!string.IsNullOrEmpty(existingImagePath))
                {
                    string newRelativePath = $"/{folderName}/{fileName}";
                    await DeleteOldImageAsync(webRootPath, existingImagePath, newRelativePath);
                }

                // Return relative path
                return $"/{folderName}/{fileName}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save image: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates SHA256 hash of the uploaded file for duplicate detection
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <returns>File hash as hexadecimal string</returns>
        private static async Task<string> CalculateFileHashAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var sha256 = SHA256.Create();
            
            byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(stream));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        /// <summary>
        /// Finds existing file with the same hash to prevent duplicates
        /// </summary>
        /// <param name="folderPath">Upload folder path</param>
        /// <param name="fileHash">File hash to search for</param>
        /// <param name="extension">File extension</param>
        /// <returns>Path to existing file or null if not found</returns>
        private static async Task<string> FindExistingFileByHashAsync(string folderPath, string fileHash, string extension)
        {
            try
            {
                // Look for files with same hash prefix (first 8 characters)
                string hashPrefix = fileHash.Substring(0, 8);
                var files = Directory.GetFiles(folderPath, $"{hashPrefix}_*{extension}", SearchOption.TopDirectoryOnly);

                foreach (string filePath in files)
                {
                    // Verify full hash match
                    string existingHash = await CalculateExistingFileHashAsync(filePath);
                    if (existingHash.Equals(fileHash, StringComparison.OrdinalIgnoreCase))
                    {
                        return filePath;
                    }
                }
            }
            catch (Exception)
            {
                // If hash checking fails, allow new file creation
            }

            return null;
        }

        /// <summary>
        /// Calculates hash of an existing file on disk
        /// </summary>
        /// <param name="filePath">Path to the existing file</param>
        /// <returns>File hash as hexadecimal string</returns>
        private static async Task<string> CalculateExistingFileHashAsync(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, useAsync: true);
            using var sha256 = SHA256.Create();
            
            byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(stream));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        /// <summary>
        /// Safely deletes old image file when replacing with new one
        /// </summary>
        /// <param name="webRootPath">Web root path</param>
        /// <param name="oldImagePath">Old image relative path</param>
        /// <param name="newImagePath">New image relative path</param>
        /// <returns>Task representing the async operation</returns>
        private static async Task DeleteOldImageAsync(string webRootPath, string oldImagePath, string newImagePath)
        {
            if (string.IsNullOrEmpty(oldImagePath) || oldImagePath.Equals(newImagePath, StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                // Convert relative path to absolute path
                string oldImageFullPath = oldImagePath.StartsWith("/") 
                    ? Path.Combine(webRootPath, oldImagePath.TrimStart('/'))
                    : Path.Combine(webRootPath, oldImagePath);

                if (File.Exists(oldImageFullPath))
                {
                    await Task.Run(() => File.Delete(oldImageFullPath));
                }
            }
            catch (Exception)
            {
                // Ignore errors when deleting old files to prevent breaking the main operation
                // Could log this error if logging is available
            }
        }

        /// <summary>
        /// Validates if a file is a valid image format
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <returns>True if valid image format</returns>
        public static bool IsValidImageFormat(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return ValidExtensions.Contains(extension);
        }

        /// <summary>
        /// Gets the formatted file size string for display
        /// </summary>
        /// <param name="bytes">File size in bytes</param>
        /// <returns>Formatted file size string</returns>
        public static string GetFormattedFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            
            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// Cleans up orphaned image files that are no longer referenced
        /// This method should be called periodically to maintain disk space
        /// </summary>
        /// <param name="webRootPath">Web root path</param>
        /// <param name="folderName">Upload folder name</param>
        /// <param name="referencedImagePaths">List of currently referenced image paths</param>
        /// <returns>Number of files cleaned up</returns>
        public static async Task<int> CleanupOrphanedFilesAsync(string webRootPath, string folderName, IEnumerable<string> referencedImagePaths)
        {
            int cleanedUpCount = 0;
            
            try
            {
                string folderPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(folderPath))
                    return 0;

                var referencedSet = new HashSet<string>(referencedImagePaths.Select(path => 
                    path?.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                ), StringComparer.OrdinalIgnoreCase);

                var allFiles = Directory.GetFiles(folderPath);
                
                foreach (string filePath in allFiles)
                {
                    string relativePath = Path.GetRelativePath(webRootPath, filePath);
                    
                    if (!referencedSet.Contains(relativePath))
                    {
                        await Task.Run(() => File.Delete(filePath));
                        cleanedUpCount++;
                    }
                }
            }
            catch (Exception)
            {
                // Ignore cleanup errors to prevent breaking the application
            }

            return cleanedUpCount;
        }
    }
}