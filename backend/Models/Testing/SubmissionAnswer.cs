using TalentBridge.Common.Entities;
using System.Collections.Generic;
using TalentBridge.Models.UserRelated;

namespace TalentBridge.Models.Testing;

public class SubmissionAnswer : BaseEntity
{
    public int TestSubmissionId { get; set; }
    public TestSubmission TestSubmission { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    
    public ICollection<SelectedQuestionOption> SelectedOptions { get; set; } = new List<SelectedQuestionOption>();
    
    public string? AnswerText { get; set; }
    public decimal? PointsAwarded { get; set; }
    public bool? IsCorrect { get; set; }
    public bool IsTimedOut { get; set; } = false;
    public string? GraderComments { get; set; }
    public DateTime AnsweredAt { get; set; }
    public int TimeSpentSeconds { get; set; }
}