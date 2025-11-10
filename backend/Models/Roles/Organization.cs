using System.ComponentModel.DataAnnotations.Schema;
using TalentBridge.Common.Entities;
using TalentBridge.Enums.OrganizationTypes;
using TalentBridge.Models.Recruitment;

namespace TalentBridge.Models.Roles;

public class Organization : BaseEntity
{ public string Name { get; set; }
    public string Address { get; set; }
    public string ContactEmail { get; set; }
    public string PhoneNumber { get; set; }
    public TYPES Type { get; set; }
    public byte[]? Logo { get; set; }
    public string? Website { get; set; }
    public string Description { get; set; } 
    public bool IsActive { get; set; }
    public int UserId { get; set; }
    public bool IsDeleted { get; set; }
    public List<Vacancy>? Vacancies { get; set; }
    
    public List<HRManager>? HRManagers { get; set; }

    public DateTime? DeletedAt { get; set; }
    
  
}
