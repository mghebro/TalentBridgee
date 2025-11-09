using TalentBridge.Enums.Auth;

namespace TalentBridge.Modules.Auth.DTOs.Responses;
public class LoginResponse
{
    public int Id { get; set; }
    public ROLES Role { get; set; }
    public string Email { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;

}