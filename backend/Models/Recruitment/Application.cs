using TalentBridge.Common.Entities;
using TalentBridge.Enums.Recruitment;
using TalentBridge.Models.Roles;
using TalentBridge.Models.Testing;

namespace TalentBridge.Models.Recruitment;

public class Application : BaseEntity
{
    public int VacancyId { get; set; }
    public Vacancy Vacancy { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public APPLICATION_STATUS Status { get; set; }
    public string? CoverLetter { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? HiredAt { get; set; }
    
    public int? ReviewedBy { get; set; }
    public int? ReviewedByHRManagerId { get; set; }
    public string? ReviewNotes { get; set; }
    public string? RejectionReason { get; set; }
    
    public List<ApplicationTimeline>? Timelines { get; set; }
    public TestAssignment? TestAssignment { get; set; }
}
