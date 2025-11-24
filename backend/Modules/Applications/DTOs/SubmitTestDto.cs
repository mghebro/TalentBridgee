using System.Collections.Generic;

namespace TalentBridge.Modules.Applications.DTOs
{
    public class SubmitTestDto
    {
        public List<AnswerDto> Answers { get; set; }
    }

    public class AnswerDto
    {
        public int QuestionId { get; set; }
        public List<int> SelectedOptionIds { get; set; } = new List<int>();
        public string AnswerText { get; set; }
    }
}
