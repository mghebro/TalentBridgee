using TalentBridge.Common.Entities;

namespace TalentBridge.Models.UserRelated;
    
public class Experience : BaseEntity
{
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public int UserDetailsId { get; set; }
    public UserDetails UserDetails { get; set; } = null!;
}