namespace TalentBridge.Modules.Organizations.DTOs.Requests;

public class UpdateOrganizationRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public bool DeleteLogo { get; set; }
        
        
}