using TalentBridge.Common.Entities;
using TalentBridge.Enums.Recruitment;

namespace TalentBridge.Models.Recruitment;

public class ApplicationTimeline : BaseEntity
{
    public int ApplicationId { get; set; }
    public Application Application { get; set; }
    
    public APPLICATION_STATUS Status { get; set; }
    public int ChangedBy { get; set; }
    public int ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Notes { get; set; }
}