namespace TalentBridge.Modules.Tests.DTOs.Requests;

public class SubmitTestRequest
{
    public List<SubmitAnswerRequest> Answers { get; set; } = new List<SubmitAnswerRequest>();
}

public class SubmitAnswerRequest
{
    public int QuestionId { get; set; }
    public List<int> SelectedOptionIds { get; set; } = new List<int>(); 
    public string? AnswerText { get; set; } 
    public int TimeSpentSeconds { get; set; }
}