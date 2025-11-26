using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Responses;

public class QuestionResponse
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public QUESTION_TYPE QuestionType { get; set; }
    public decimal Points { get; set; }
    public int OrderNumber { get; set; }
    public int TimeLimitSeconds { get; set; }
    public List<QuestionOptionResponse> Options { get; set; } = new List<QuestionOptionResponse>();
}

public class QuestionOptionResponse
{
    public int Id { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}