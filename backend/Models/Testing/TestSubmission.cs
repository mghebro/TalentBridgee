using TalentBridge.Common.Entities;

namespace TalentBridge.Models.Testing;

public class TestSubmission : BaseEntity
{
    public int TestAssignmentId { get; set; }
    public TestAssignment TestAssignment { get; set; }
    
    public int UserId { get; set; }
    public int TestId { get; set; }
    public Test Test { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? SubmittedAt { get; set; }
    
    public decimal? TotalPointsEarned { get; set; }
    public decimal? PercentageScore { get; set; }
    public bool? IsPassed { get; set; }
    
    public bool IsAutoGraded { get; set; }
    public bool RequiresManualReview { get; set; }
    public int? ReviewedBy { get; set; }
    public int? ReviewedByHRManagerId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? Feedback { get; set; }
    
    public List<SubmissionAnswer>? Answers { get; set; }
}