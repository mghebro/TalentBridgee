using TalentBridge.Common.Entities;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.Auth;

public class EmailVerification : BaseEntity
{  public string Code { get; set; }
    public int AttemptCount { get; set; } = 0;
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(15);

    public int UserId { get; set; }
    public User User { get; set; }
    }

