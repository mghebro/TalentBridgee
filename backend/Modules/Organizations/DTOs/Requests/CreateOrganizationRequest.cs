using TalentBridge.Enums.OrganizationTypes;

namespace TalentBridge.Modules.Organizations.DTOs.Requests;

public class CreateOrganizationRequest
{
    public string Name { get; set; }
    public TYPES Type { get; set; }
    public string Address { get; set; }
    public string ContactEmail { get; set; }
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public string Website { get; set; }
}
