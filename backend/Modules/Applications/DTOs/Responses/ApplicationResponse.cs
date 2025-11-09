using TalentBridge.Enums.Recruitment;
using TalentBridge.Models.Roles;

namespace TalentBridge.Modules.Applications.DTOs.Responses;

public class ApplicationResponse
{
    public int Id { get; set; }
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public APPLICATION_STATUS Status { get; set; }
    public string? CoverLetter { get; set; }
    public DateTime AppliedAt { get; set; }
}
