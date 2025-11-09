using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Requests;

public class CreateQuestionRequest
{
    public string QuestionText { get; set; }
    public QUESTION_TYPE QuestionType { get; set; }
    public decimal Points { get; set; }
    public List<CreateQuestionOptionRequest> Options { get; set; } = new List<CreateQuestionOptionRequest>();
}

public class CreateQuestionOptionRequest
{
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}