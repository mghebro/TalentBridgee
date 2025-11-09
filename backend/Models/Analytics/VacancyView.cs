using TalentBridge.Common.Entities;

namespace TalentBridge.Models.Analytics;
    
public class VacancyView : BaseEntity
{
    public int VacancyId { get; set; }
    public int? UserId { get; set; }
    public DateTime ViewedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}