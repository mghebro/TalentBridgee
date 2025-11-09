using TalentBridge.Common.Entities;
using TalentBridge.Enums.Testing;
using TalentBridge.Models.Recruitment;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.Testing;

public class Test : BaseEntity
{
    public int OrganizationId { get; set; }
    public int CreatedBy { get; set; }
    public int CreatedByHRManagerId { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string Profession { get; set; }
    public string Industry { get; set; }
    
    public int DurationMinutes { get; set; }
    public decimal PassingScore { get; set; }
    public decimal TotalPoints { get; set; }
    public TEST_DIFFICULTY Difficulty { get; set; }
    public bool IsActive { get; set; }
    public string? Instructions { get; set; }
    public int VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; }
    public Organization Organization { get; set; }
    public List<Question>? Questions { get; set; }
    
}