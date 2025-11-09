using TalentBridge.Common.Entities;
using TalentBridge.Models.Recruitment;
using TalentBridge.Models.Roles;

using TalentBridge.Models.Testing;

namespace TalentBridge.Models;

public class HRManager : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; }
    
    public string Position { get; set; }
    public string? Department { get; set; }
    public DateTime HiredDate { get; set; }
    public string? Permissions { get; set; } 
    public bool IsActive { get; set; } = true;
    
    public List<Vacancy>? Vacancies { get; set; }
    public List<Test>? Tests { get; set; }
}