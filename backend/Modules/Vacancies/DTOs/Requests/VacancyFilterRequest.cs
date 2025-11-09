using TalentBridge.Enums.Recruitment;

namespace TalentBridge.Modules.Vacancies.DTOs.Requests;

public class VacancyFilterRequest
{
    public string? Search { get; set; }
    public int? OrganizationId { get; set; }
    public string? Profession { get; set; }
    public string? Industry { get; set; }
    public EMPLOYMENT_TYPE? EmploymentType { get; set; }
    public EXPERIENCE_LEVEL? ExperienceLevel { get; set; }
    public VACANCY_STATUS? Status { get; set; }
    public string? Location { get; set; }
    public bool? IsRemote { get; set; }
    
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    
    public DateTime? PublishedFrom { get; set; }
    public DateTime? PublishedTo { get; set; }
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    public string? SortBy { get; set; } = "PublishedAt";
    public string? SortOrder { get; set; } = "desc";
}