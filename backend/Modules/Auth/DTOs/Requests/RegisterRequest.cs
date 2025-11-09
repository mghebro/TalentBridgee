using TalentBridge.Enums.Auth;
using TalentBridge.Modules.Organizations.DTOs.Requests; 

namespace TalentBridge.Modules.Auth.DTOs.Requests;
public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string DesiredRole { get; set; } = "USER"; 
    public int? OrganizationId { get; set; } 
    public CreateOrganizationRequest? OrganizationDetails { get; set; } 
}