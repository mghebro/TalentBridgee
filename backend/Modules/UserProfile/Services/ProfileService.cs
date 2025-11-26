using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Common.Services.CurrentUser;
using TalentBridge.Data;
using TalentBridge.Models.UserRelated;
using TalentBridge.Modules.UserProfile.DTOs.Requests;
using TalentBridge.Modules.UserProfile.DTOs.Responses;

namespace TalentBridge.Modules.UserProfile.Services;

public class ProfileService : BaseService, IProfileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadsFolder;

    public ProfileService(DataContext context, IWebHostEnvironment environment) : base(context)
    {
        _environment = environment;
        _uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        
        Directory.CreateDirectory(Path.Combine(_uploadsFolder, "cv"));
        Directory.CreateDirectory(Path.Combine(_uploadsFolder, "avatars"));
        Directory.CreateDirectory(Path.Combine(_uploadsFolder, "organizations"));
    }

    public async Task<ServiceResult<ProfileResponse>> GetProfileAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Details)
                .ThenInclude(d => d.Educations)
            .Include(u => u.Details)
                .ThenInclude(d => d.Experiences)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return ServiceResult<ProfileResponse>.FailureResult("User not found");

        var response = new ProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            IsVerified = user.IsVerified,
            CreatedAt = user.CreatedAt,
            PhoneNumber = user.Details?.PhoneNumber,
            ProfilePictureUrl = user.Details?.ProfilePictureUrl,
            Bio = user.Details?.Bio,
            Skills = user.Details?.Skills,
            CVPdfUrl = user.Details?.CVPdfUrl,
            Gender = user.Details?.Gender,
            Educations = user.Details?.Educations?.Select(e => new EducationDto
            {
                Id = e.Id,
                Institution = e.Institution,
                Degree = e.Degree,
                FieldOfStudy = e.FieldOfStudy,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrent = e.IsCurrent,
                Description = e.Description
            }).ToList() ?? new List<EducationDto>(),
            Experiences = user.Details?.Experiences?.Select(e => new ExperienceDto
            {
                Id = e.Id,
                Company = e.Company,
                Position = e.Position,
                Location = e.Location,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrent = e.IsCurrent,
                Description = e.Description
            }).ToList() ?? new List<ExperienceDto>()
        };

        return ServiceResult<ProfileResponse>.SuccessResult(response);
    }

    public async Task<ServiceResult<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Details)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return ServiceResult<ProfileResponse>.FailureResult("User not found");

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;
        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (user.Details == null)
        {
            user.Details = new UserDetails
            {
                UserId = userId,
                PhoneNumber = request.PhoneNumber ?? string.Empty
            };
            _context.UserDetails.Add(user.Details);
        }

        if (request.PhoneNumber != null)
            user.Details.PhoneNumber = request.PhoneNumber;
        if (request.Bio != null)
            user.Details.Bio = request.Bio;
        if (request.Skills != null)
            user.Details.Skills = request.Skills;
        if (request.Gender.HasValue)
            user.Details.Gender = request.Gender.Value;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GetProfileAsync(userId);
    }

    public async Task<ServiceResult<string>> UploadCVAsync(int userId, IFormFile file)
    {
        var user = await _context.Users
            .Include(u => u.Details)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return ServiceResult<string>.FailureResult("User not found");

        var allowedExtensions = new[] { ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return ServiceResult<string>.FailureResult("Only PDF files are allowed for CV");

        const int maxSize = 5 * 1024 * 1024; 
        if (file.Length > maxSize)
            return ServiceResult<string>.FailureResult("CV file size must be less than 5MB");

        if (user.Details?.CVPdfUrl != null)
        {
            var oldPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), 
                user.Details.CVPdfUrl.TrimStart('/'));
            if (File.Exists(oldPath))
                File.Delete(oldPath);
        }

        var fileName = $"cv_{userId}_{DateTime.UtcNow.Ticks}{extension}";
        var filePath = Path.Combine(_uploadsFolder, "cv", fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/cv/{fileName}";

        if (user.Details == null)
        {
            user.Details = new UserDetails
            {
                UserId = userId,
                PhoneNumber = string.Empty,
                CVPdfUrl = relativePath
            };
            _context.UserDetails.Add(user.Details);
        }
        else
        {
            user.Details.CVPdfUrl = relativePath;
        }

        await _context.SaveChangesAsync();

        return ServiceResult<string>.SuccessResult(relativePath, "CV uploaded successfully");
    }

    public async Task<ServiceResult<string>> UploadProfilePictureAsync(int userId, IFormFile file)
    {
        var user = await _context.Users
            .Include(u => u.Details)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return ServiceResult<string>.FailureResult("User not found");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return ServiceResult<string>.FailureResult("Only JPG, PNG, and WebP images are allowed");

        const int maxSize = 2 * 1024 * 1024; 
        if (file.Length > maxSize)
            return ServiceResult<string>.FailureResult("Image size must be less than 2MB");

        if (user.Details?.ProfilePictureUrl != null)
        {
            var oldPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), 
                user.Details.ProfilePictureUrl.TrimStart('/'));
            if (File.Exists(oldPath))
                File.Delete(oldPath);
        }

        var fileName = $"avatar_{userId}_{DateTime.UtcNow.Ticks}{extension}";
        var filePath = Path.Combine(_uploadsFolder, "avatars", fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/avatars/{fileName}";

        if (user.Details == null)
        {
            user.Details = new UserDetails
            {
                UserId = userId,
                PhoneNumber = string.Empty,
                ProfilePictureUrl = relativePath
            };
            _context.UserDetails.Add(user.Details);
        }
        else
        {
            user.Details.ProfilePictureUrl = relativePath;
        }

        await _context.SaveChangesAsync();

        return ServiceResult<string>.SuccessResult(relativePath, "Profile picture uploaded successfully");
    }

    public async Task<ServiceResult<bool>> DeleteCVAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Details)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Details?.CVPdfUrl == null)
            return ServiceResult<bool>.FailureResult("No CV found to delete");

        var filePath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), 
            user.Details.CVPdfUrl.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        user.Details.CVPdfUrl = null;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true, "CV deleted successfully");
    }

    public async Task<ServiceResult<bool>> DeleteProfilePictureAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Details)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Details?.ProfilePictureUrl == null)
            return ServiceResult<bool>.FailureResult("No profile picture found to delete");

        var filePath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), 
            user.Details.ProfilePictureUrl.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        user.Details.ProfilePictureUrl = null;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true, "Profile picture deleted successfully");
    }

    public async Task<ServiceResult<EducationDto>> AddEducationAsync(int userId, AddEducationRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Details)
                .ThenInclude(d => d.Educations)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return ServiceResult<EducationDto>.FailureResult("User not found");

        if (user.Details == null)
        {
            user.Details = new UserDetails
            {
                UserId = userId,
                PhoneNumber = string.Empty
            };
            _context.UserDetails.Add(user.Details);
            await _context.SaveChangesAsync();
        }

        var education = new Education
        {
            UserDetailsId = user.Details.Id,
            Institution = request.Institution,
            Degree = request.Degree,
            FieldOfStudy = request.FieldOfStudy,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Educations.Add(education);
        await _context.SaveChangesAsync();

        return ServiceResult<EducationDto>.SuccessResult(new EducationDto
        {
            Id = education.Id,
            Institution = education.Institution,
            Degree = education.Degree,
            FieldOfStudy = education.FieldOfStudy,
            StartDate = education.StartDate,
            EndDate = education.EndDate,
            IsCurrent = education.IsCurrent,
            Description = education.Description
        }, "Education added successfully");
    }

    public async Task<ServiceResult<EducationDto>> UpdateEducationAsync(int userId, int educationId, AddEducationRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Details)
                .ThenInclude(d => d.Educations)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Details == null)
            return ServiceResult<EducationDto>.FailureResult("User not found");

        var education = user.Details.Educations.FirstOrDefault(e => e.Id == educationId);
        if (education == null)
            return ServiceResult<EducationDto>.FailureResult("Education not found");

        education.Institution = request.Institution;
        education.Degree = request.Degree;
        education.FieldOfStudy = request.FieldOfStudy;
        education.StartDate = request.StartDate;
        education.EndDate = request.EndDate;
        education.IsCurrent = request.IsCurrent;
        education.Description = request.Description;
        education.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ServiceResult<EducationDto>.SuccessResult(new EducationDto
        {
            Id = education.Id,
            Institution = education.Institution,
            Degree = education.Degree,
            FieldOfStudy = education.FieldOfStudy,
            StartDate = education.StartDate,
            EndDate = education.EndDate,
            IsCurrent = education.IsCurrent,
            Description = education.Description
        }, "Education updated successfully");
    }

    public async Task<ServiceResult<bool>> DeleteEducationAsync(int userId, int educationId)
    {
        var user = await _context.Users
            .Include(u => u.Details)
                .ThenInclude(d => d.Educations)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Details == null)
            return ServiceResult<bool>.FailureResult("User not found");

        var education = user.Details.Educations.FirstOrDefault(e => e.Id == educationId);
        if (education == null)
            return ServiceResult<bool>.FailureResult("Education not found");

        _context.Educations.Remove(education);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true, "Education deleted successfully");
    }

    public async Task<ServiceResult<ExperienceDto>> AddExperienceAsync(int userId, AddExperienceRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Details)
                .ThenInclude(d => d.Experiences)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return ServiceResult<ExperienceDto>.FailureResult("User not found");

        if (user.Details == null)
        {
            user.Details = new UserDetails
            {
                UserId = userId,
                PhoneNumber = string.Empty
            };
            _context.UserDetails.Add(user.Details);
            await _context.SaveChangesAsync();
        }

        var experience = new Experience
        {
            UserDetailsId = user.Details.Id,
            Company = request.Company,
            Position = request.Position,
            Location = request.Location,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Experiences.Add(experience);
        await _context.SaveChangesAsync();

        return ServiceResult<ExperienceDto>.SuccessResult(new ExperienceDto
        {
            Id = experience.Id,
            Company = experience.Company,
            Position = experience.Position,
            Location = experience.Location,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsCurrent = experience.IsCurrent,
            Description = experience.Description
        }, "Experience added successfully");
    }

    public async Task<ServiceResult<ExperienceDto>> UpdateExperienceAsync(int userId, int experienceId, AddExperienceRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Details)
                .ThenInclude(d => d.Experiences)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Details == null)
            return ServiceResult<ExperienceDto>.FailureResult("User not found");

        var experience = user.Details.Experiences.FirstOrDefault(e => e.Id == experienceId);
        if (experience == null)
            return ServiceResult<ExperienceDto>.FailureResult("Experience not found");

        experience.Company = request.Company;
        experience.Position = request.Position;
        experience.Location = request.Location;
        experience.StartDate = request.StartDate;
        experience.EndDate = request.EndDate;
        experience.IsCurrent = request.IsCurrent;
        experience.Description = request.Description;
        experience.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ServiceResult<ExperienceDto>.SuccessResult(new ExperienceDto
        {
            Id = experience.Id,
            Company = experience.Company,
            Position = experience.Position,
            Location = experience.Location,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsCurrent = experience.IsCurrent,
            Description = experience.Description
        }, "Experience updated successfully");
    }

    public async Task<ServiceResult<bool>> DeleteExperienceAsync(int userId, int experienceId)
    {
        var user = await _context.Users
            .Include(u => u.Details)
                .ThenInclude(d => d.Experiences)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Details == null)
            return ServiceResult<bool>.FailureResult("User not found");

        var experience = user.Details.Experiences.FirstOrDefault(e => e.Id == experienceId);
        if (experience == null)
            return ServiceResult<bool>.FailureResult("Experience not found");

        _context.Experiences.Remove(experience);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true, "Experience deleted successfully");
    }
}

