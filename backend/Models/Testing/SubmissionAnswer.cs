using TalentBridge.Common.Entities;
namespace TalentBridge.Models.Testing;

public class SubmissionAnswer : BaseEntity
{
    public int TestSubmissionId { get; set; }
    public TestSubmission TestSubmission { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    
    public int? SelectedOptionId { get; set; }
    public QuestionOption? SelectedOption { get; set; }
    
    public string? AnswerText { get; set; }
    public decimal? PointsAwarded { get; set; }
    public bool? IsCorrect { get; set; }
    public string? GraderComments { get; set; }
    public DateTime AnsweredAt { get; set; }
    public int TimeSpentSeconds { get; set; }
}