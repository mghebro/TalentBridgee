using TalentBridge.Enums.Recruitment;

namespace TalentBridge.Modules.Vacancies.DTOs.Requests;

public class CreateVacancyRequest
{
    public int OrganizationId { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }
    public string Responsibilities { get; set; }
    
    public string Profession { get; set; }
    public string Industry { get; set; }
    public EMPLOYMENT_TYPE EmploymentType { get; set; }
    public EXPERIENCE_LEVEL ExperienceLevel { get; set; }
    
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string SalaryCurrency { get; set; } = "GEL";
    
    public string Location { get; set; }
    public bool IsRemote { get; set; } = false;
    
    public VACANCY_STATUS Status { get; set; } = VACANCY_STATUS.Active;
    public DateTime ApplicationDeadline { get; set; }
}