using TalentBridge.Enums.OrganizationTypes;

namespace TalentBridge.Modules.Organizations.DTOs.Responses;

public class OrganizationList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string ContactEmail { get; set; }
    public string PhoneNumber { get; set; }
    public string Logo { get; set; }
    public string Website { get; set; }
        
    public TYPES Type { get; set; }
    public string TypeName => Type.ToString();
    public int ExactTypeValue { get; set; }
    public string ExactTypeName { get; set; }
        
    public int ActiveVacancies { get; set; }
    public int TotalApplications { get; set; }
        
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}