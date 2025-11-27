using System;
using TalentBridge.Common.Entities;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.Auth;
public class PasswordVerification : BaseEntity
{
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(1);
    public int AttemptCount { get; set; } = 0;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
