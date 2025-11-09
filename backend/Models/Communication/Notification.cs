
using TalentBridge.Common.Entities;
using TalentBridge.Enums;
using TalentBridge.Enums.Communication;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.Communication;

    

public class Notification : BaseEntity
{

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public NOTIFICATION_TYPE Type { get; set; }    

    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    public int? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }

    public string? ActionUrl { get; set; }

}