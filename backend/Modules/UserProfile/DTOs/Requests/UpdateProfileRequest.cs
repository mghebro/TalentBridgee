using TalentBridge.Enums.Auth;

namespace TalentBridge.Modules.UserProfile.DTOs.Requests;

public class UpdateProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public GENDER? Gender { get; set; }
}

