using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Common.Helpers
{
    public class UploadHandler
    {
        private readonly List<string> validExtentions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };

        private const long size = 50 * 1024 * 1024;


        public async Task<string> Upload(IFormFile file)
        {

            //check file type
            string extention = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!validExtentions.Contains(extention)) return $"Extention is not valid ({string.Join(", ", validExtentions)})";
            //check file size
            if (file.Length > size) return "Maximum file size is 50MB";
            string fileName = Guid.NewGuid().ToString() + extention;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "UploadImg");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
            file.CopyToAsync(stream);
            return fileName;
        }


    }
}
