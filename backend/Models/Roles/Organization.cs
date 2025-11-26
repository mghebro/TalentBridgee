using System.ComponentModel.DataAnnotations.Schema;
using TalentBridge.Common.Entities;
using TalentBridge.Enums.OrganizationTypes;
using TalentBridge.Models.Recruitment;

namespace TalentBridge.Models.Roles;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public TYPES Type { get; set; }
    public byte[]? Logo { get; set; }
    public string? LogoUrl { get; set; } 
    public string? Website { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int UserId { get; set; }
    public bool IsDeleted { get; set; }
    public List<Vacancy>? Vacancies { get; set; }
    
    public List<HRManager>? HRManagers { get; set; }

    public DateTime? DeletedAt { get; set; }
}
