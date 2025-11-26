using TalentBridge.Common.Entities;

namespace TalentBridge.Models.UserRelated;
    
public class Education : BaseEntity
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public int UserDetailsId { get; set; }
    public UserDetails UserDetails { get; set; } = null!;
}