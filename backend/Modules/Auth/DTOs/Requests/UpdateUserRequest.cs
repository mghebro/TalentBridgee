using TalentBridge.Enums.Auth;

namespace TalentBridge.Modules.Auth.DTOs.Requests;
public class UpdateUserRequest
{
    public string? bio { get; set; }
    public string Avatar { get; set; }
    public GENDER Gender { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? PhoneNumber { get; set; }

}
