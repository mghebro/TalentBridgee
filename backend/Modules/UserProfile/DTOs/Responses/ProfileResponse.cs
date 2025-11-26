using TalentBridge.Enums.Auth;

namespace TalentBridge.Modules.UserProfile.DTOs.Responses;

public class ProfileResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public string? CVPdfUrl { get; set; }
    public GENDER? Gender { get; set; }
    
    public List<EducationDto> Educations { get; set; } = new();
    public List<ExperienceDto> Experiences { get; set; } = new();
}

public class EducationDto
{
    public int Id { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}

public class ExperienceDto
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}

