using TalentBridge.Enums.OrganizationTypes;

namespace TalentBridge.Modules.Organizations.DTOs.Responses;

public class OrganizationDetails
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? LogoUrl { get; set; }  
    public string? Website { get; set; }
        
    public TYPES Type { get; set; }
    public string TypeName => Type.ToString();
    public int ExactTypeValue { get; set; }
    public string? ExactTypeName { get; set; }
        
    public OrganizationStatistics? Statistics { get; set; }
        
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; }
}