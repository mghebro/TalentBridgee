using TalentBridge.Common.Entities;
using TalentBridge.Enums.Auth;
using TalentBridge.Models.UserRelated;

namespace TalentBridge.Models.Roles;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string? AppleId { get; set; }
    public string Password { get; set; }
    public string? GoogleId { get; set; }
    public ROLES Role { get; set; } = ROLES.USER;
    public bool IsVerified { get; set; } = false;
    public string AuthProvider { get; set; } = "Email";
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public UserDetails? Details { get; set; }
}