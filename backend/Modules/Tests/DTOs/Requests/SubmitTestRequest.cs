namespace TalentBridge.Modules.Tests.DTOs.Requests;

public class SubmitTestRequest
{
    public List<SubmitAnswerRequest> Answers { get; set; } = new List<SubmitAnswerRequest>();
}

public class SubmitAnswerRequest
{
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; } // For MultipleChoice
    public string AnswerText { get; set; } // For ShortAnswer, Essay, Coding
}