using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentBridge.Common.Controllers;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Modules.UserProfile.DTOs.Requests;
using TalentBridge.Modules.UserProfile.DTOs.Responses;
using TalentBridge.Modules.UserProfile.Services;

namespace TalentBridge.Modules.UserProfile.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : BaseApiController
{
    private readonly IProfileService _profileService;

    public ProfileController(
        IProfileService profileService,
        IHttpContextAccessor httpContextAccessor,
        DataContext context) : base(httpContextAccessor, context)
    {
        _profileService = profileService;
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<ProfileResponse>.FailureResult(userIdResult.Message));

        var result = await _profileService.GetProfileAsync(userIdResult.Data);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("update-user")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<ProfileResponse>.FailureResult(userIdResult.Message));

        var result = await _profileService.UpdateProfileAsync(userIdResult.Data, request);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("upload-cv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCV(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ServiceResult<string>.FailureResult("No file uploaded"));

        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<string>.FailureResult(userIdResult.Message));

        var result = await _profileService.UploadCVAsync(userIdResult.Data, file);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("upload-avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ServiceResult<string>.FailureResult("No file uploaded"));

        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<string>.FailureResult(userIdResult.Message));

        var result = await _profileService.UploadProfilePictureAsync(userIdResult.Data, file);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("cv")]
    public async Task<IActionResult> DeleteCV()
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<bool>.FailureResult(userIdResult.Message));

        var result = await _profileService.DeleteCVAsync(userIdResult.Data);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<bool>.FailureResult(userIdResult.Message));

        var result = await _profileService.DeleteProfilePictureAsync(userIdResult.Data);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("education")]
    public async Task<IActionResult> AddEducation([FromBody] AddEducationRequest request)
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<EducationDto>.FailureResult(userIdResult.Message));

        var result = await _profileService.AddEducationAsync(userIdResult.Data, request);
        
        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetProfile), result);
    }

    [HttpPut("education/{educationId}")]
    public async Task<IActionResult> UpdateEducation(int educationId, [FromBody] AddEducationRequest request)
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<EducationDto>.FailureResult(userIdResult.Message));

        var result = await _profileService.UpdateEducationAsync(userIdResult.Data, educationId, request);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("education/{educationId}")]
    public async Task<IActionResult> DeleteEducation(int educationId)
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<bool>.FailureResult(userIdResult.Message));

        var result = await _profileService.DeleteEducationAsync(userIdResult.Data, educationId);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("experience")]
    public async Task<IActionResult> AddExperience([FromBody] AddExperienceRequest request)
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<ExperienceDto>.FailureResult(userIdResult.Message));

        var result = await _profileService.AddExperienceAsync(userIdResult.Data, request);
        
        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetProfile), result);
    }

    [HttpPut("experience/{experienceId}")]
    public async Task<IActionResult> UpdateExperience(int experienceId, [FromBody] AddExperienceRequest request)
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<ExperienceDto>.FailureResult(userIdResult.Message));

        var result = await _profileService.UpdateExperienceAsync(userIdResult.Data, experienceId, request);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("experience/{experienceId}")]
    public async Task<IActionResult> DeleteExperience(int experienceId)
    {
        var userIdResult = await GetCurrentUserIdAsync();
        if (userIdResult.Status != StatusCodes.Status200OK)
            return StatusCode(userIdResult.Status, ServiceResult<bool>.FailureResult(userIdResult.Message));

        var result = await _profileService.DeleteExperienceAsync(userIdResult.Data, experienceId);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}

