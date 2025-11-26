using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Modules.UserProfile.DTOs.Requests;
using TalentBridge.Modules.UserProfile.DTOs.Responses;

namespace TalentBridge.Modules.UserProfile.Services;

public interface IProfileService
{
    Task<ServiceResult<ProfileResponse>> GetProfileAsync(int userId);
    Task<ServiceResult<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<ServiceResult<string>> UploadCVAsync(int userId, IFormFile file);
    Task<ServiceResult<string>> UploadProfilePictureAsync(int userId, IFormFile file);
    Task<ServiceResult<bool>> DeleteCVAsync(int userId);
    Task<ServiceResult<bool>> DeleteProfilePictureAsync(int userId);
    
    Task<ServiceResult<EducationDto>> AddEducationAsync(int userId, AddEducationRequest request);
    Task<ServiceResult<EducationDto>> UpdateEducationAsync(int userId, int educationId, AddEducationRequest request);
    Task<ServiceResult<bool>> DeleteEducationAsync(int userId, int educationId);
    
    Task<ServiceResult<ExperienceDto>> AddExperienceAsync(int userId, AddExperienceRequest request);
    Task<ServiceResult<ExperienceDto>> UpdateExperienceAsync(int userId, int experienceId, AddExperienceRequest request);
    Task<ServiceResult<bool>> DeleteExperienceAsync(int userId, int experienceId);
}

