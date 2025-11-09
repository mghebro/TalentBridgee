namespace TalentBridge.Modules.Vacancies.DTOs.Responses;

public class VacancyStatistics
{
    public int ViewCount { get; set; }
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int ReviewedApplications { get; set; }
    public int ShortlistedApplications { get; set; }
    public int RejectedApplications { get; set; }
    
    public int TestsAssigned { get; set; }
    public int TestsCompleted { get; set; }
    public decimal AverageTestScore { get; set; }
    
    public int InterviewsScheduled { get; set; }
    public int InterviewsCompleted { get; set; }
    
    public int DaysActive { get; set; }
    public int DaysRemaining { get; set; }
}