using TalentBridge.Common.Entities;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.Auth;
public class PasswordVerification : BaseEntity
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public int AttemptCount { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
