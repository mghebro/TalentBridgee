using TalentBridge.Enums.Recruitment;

namespace TalentBridge.Modules.Vacancies.DTOs.Responses;

public class VacancyDetails
{
    public int Id { get; set; }
    
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public string OrganizationLogo { get; set; }
    public string OrganizationWebsite { get; set; }
    
    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }
    public string Responsibilities { get; set; }
    
    public string Profession { get; set; }
    public string Industry { get; set; }
    public EMPLOYMENT_TYPE EmploymentType { get; set; }
    public string EmploymentTypeName => EmploymentType.ToString();
    public EXPERIENCE_LEVEL ExperienceLevel { get; set; }
    public string ExperienceLevelName => ExperienceLevel.ToString();
    
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string SalaryCurrency { get; set; }
    
    public string Location { get; set; }
    public bool IsRemote { get; set; }
    
    public VACANCY_STATUS Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime ApplicationDeadline { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public VacancyStatistics Statistics { get; set; }
    
    public List<VacancyTestDto> Tests { get; set; } = new List<VacancyTestDto>();
    
    public bool HasApplied { get; set; }
}