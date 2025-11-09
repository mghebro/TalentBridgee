using TalentBridge.Common.DTOs.Responses;

namespace TalentBridge.Common.Services
{
    public interface IFileService
    {
        Task<ServiceResult<string>> UploadAsync(
            IFormFile file,
            string folder,
            string[] allowedExtensions);

        Task<bool> DeleteAsync(string filePath);
    }
}