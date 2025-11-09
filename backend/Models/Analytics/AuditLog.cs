using TalentBridge.Common.Entities;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.Analytics;

public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public User User { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}