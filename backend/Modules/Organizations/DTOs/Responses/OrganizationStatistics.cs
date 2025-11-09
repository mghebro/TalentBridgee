namespace TalentBridge.Modules.Organizations.DTOs.Responses;

public class OrganizationStatistics
{
    public int TotalVacancies { get; set; }
    public int ActiveVacancies { get; set; }
    public int ClosedVacancies { get; set; }
        
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int ReviewedApplications { get; set; }
        
    public int TotalTests { get; set; }
    public int CompletedTests { get; set; }
    public decimal AverageTestScore { get; set; }
        
    public int ScheduledInterviews { get; set; }
    public int CompletedInterviews { get; set; }
        
    public int TotalHires { get; set; }
    public decimal AverageTimeToHire { get; set; } 
        
    public DateTime? LastVacancyPosted { get; set; }
    public DateTime? LastApplicationReceived { get; set; }
}