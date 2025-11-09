using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Common.Services;

namespace TalentBridge.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<ServiceResult<string>> UploadAsync(
            IFormFile file,
            string folder,
            string[] allowedExtensions)
        {
            try
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    return ServiceResult<string>.FailureResult("Invalid file type");

                var uploadPath = Path.Combine(_env.WebRootPath, folder);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine(folder, fileName).Replace("\\", "/");
                return ServiceResult<string>.SuccessResult(relativePath);
            }
            catch
            {
                return ServiceResult<string>.FailureResult("File upload failed");
            }
        }

        public Task<bool> DeleteAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, filePath);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
