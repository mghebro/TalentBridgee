using TalentBridge.Enums.Recruitment;

namespace TalentBridge.Modules.Vacancies.DTOs.Requests;

public class UpdateVacancyRequest
{
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
    public string SalaryCurrency { get; set; }
    
    public string Location { get; set; }
    public bool IsRemote { get; set; }
    
    public VACANCY_STATUS Status { get; set; }
    public DateTime ApplicationDeadline { get; set; }
}