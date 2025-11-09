using System.ComponentModel.DataAnnotations.Schema;
using TalentBridge.Common.Entities;
using TalentBridge.Enums.OrganizationTypes;
using TalentBridge.Models.Recruitment;

namespace TalentBridge.Models.Roles;

public abstract class Organization : BaseEntity
{ public string Name { get; set; }
    public string Address { get; set; }
    public string ContactEmail { get; set; }
    public string PhoneNumber { get; set; }
    public TYPES Type { get; set; }
    public int ExactTypeValue { get; set; }
    public byte[]? Logo { get; set; }
    public string? Website { get; set; }
    public string Description { get; set; } 
    public bool IsActive { get; set; }
    public int UserId { get; set; }
    public bool IsDeleted { get; set; }
    public List<Vacancy>? Vacancies { get; set; }
    
    public List<HRManager>? HRManagers { get; set; }

    public DateTime? DeletedAt { get; set; }
    
    [NotMapped]
    public object ExactType
    {
        get => Type switch
        {
            TYPES.BUSINESS_COMPANY => (BUSINESS_COMPANY)ExactTypeValue,
            TYPES.EDUCATION => (EDUCATION)ExactTypeValue,
            TYPES.HEALTHCARE => (HEALTHCARE)ExactTypeValue,
            TYPES.NON_GOV => (NON_GOV)ExactTypeValue,
            TYPES.GOV => (GOV)ExactTypeValue,
            TYPES.OTHERS_ASSOCIATIONS => (OTHERS_ASSOCIATIONS)ExactTypeValue,
            _ => ExactTypeValue
        };
        set => ExactTypeValue = Convert.ToInt32(value);
    }
}
