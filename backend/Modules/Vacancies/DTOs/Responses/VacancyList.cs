using TalentBridge.Enums.Recruitment;

namespace TalentBridge.Modules.Vacancies.DTOs.Responses;

public class VacancyList
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public string OrganizationLogo { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string Profession { get; set; }
    public string Industry { get; set; }
    
    public EMPLOYMENT_TYPE EmploymentType { get; set; }
    public string EmploymentTypeName => EmploymentType.ToString();
    public EXPERIENCE_LEVEL ExperienceLevel { get; set; }
    public string ExperienceLevelName => ExperienceLevel.ToString();
    
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string SalaryCurrency { get; set; }
    public string SalaryRange => FormatSalaryRange();
    
    public string Location { get; set; }
    public bool IsRemote { get; set; }
    
    public VACANCY_STATUS Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime ApplicationDeadline { get; set; }
    public DateTime? PublishedAt { get; set; }
    
    public int ViewCount { get; set; }
    public int ApplicationCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    private string FormatSalaryRange()
    {
        if (SalaryMin.HasValue && SalaryMax.HasValue)
            return $"{SalaryMin:N0} - {SalaryMax:N0} {SalaryCurrency}";
        if (SalaryMin.HasValue)
            return $"From {SalaryMin:N0} {SalaryCurrency}";
        if (SalaryMax.HasValue)
            return $"Up to {SalaryMax:N0} {SalaryCurrency}";
        return "Negotiable";
    }
}