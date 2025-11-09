using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Responses;

public class TestSubmissionResponse
{
    public int Id { get; set; }
    public int TestAssignmentId { get; set; }
    public int UserId { get; set; }
    public int TestId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public decimal? TotalPointsEarned { get; set; }
    public decimal? PercentageScore { get; set; }
    public bool? IsPassed { get; set; }
    public string? Feedback { get; set; }
    public TEST_ASSIGNMENT_STATUS Status { get; set; }
    public List<SubmissionAnswerResponse> Answers { get; set; } = new List<SubmissionAnswerResponse>();
}

public class SubmissionAnswerResponse
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public QUESTION_TYPE QuestionType { get; set; }
    public int? SelectedOptionId { get; set; }
    public string? AnswerText { get; set; }
    public decimal PointsEarned { get; set; }
    public decimal MaxPoints { get; set; }
    public bool IsCorrect { get; set; }
}