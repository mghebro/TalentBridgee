namespace TalentBridge.Modules.Vacancies.DTOs.Responses;

public class VacancyAnalytics
{
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; }
    
    public int TotalViews { get; set; }
    public int TotalApplications { get; set; }
    public int TestsCompleted { get; set; }
    public int Hired { get; set; }
    
    public decimal ViewToApplicationRate { get; set; }
    public decimal ApplicationToTestRate { get; set; }
    
    public decimal AverageTestScore { get; set; }
    public decimal AverageTimeToHire { get; set; } // in days
    
    public List<VacancyTimelineDto> Timeline { get; set; }
}