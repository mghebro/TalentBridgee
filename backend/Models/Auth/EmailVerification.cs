using TalentBridge.Common.Entities;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.Auth;

public class EmailVerification : BaseEntity
{
        public string Code { get; set; } = null!;

        public int AttemptCount { get; set; }

        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }

