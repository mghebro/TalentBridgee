using TalentBridge.Enums.Testing;
using System.Collections.Generic;

namespace TalentBridge.Modules.Tests.DTOs.Responses
{
    public class TestForApplicationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int AssignmentId { get; set; }
        public int? SubmissionId { get; set; }
        public List<QuestionForUserResponse> Questions { get; set; } = new List<QuestionForUserResponse>();
    }

    public class QuestionForUserResponse
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public QUESTION_TYPE QuestionType { get; set; }
        public decimal Points { get; set; }
        public int OrderNumber { get; set; }
        public int? TimeLimitSeconds { get; set; }
        public List<QuestionOptionForUserResponse> Options { get; set; } = new List<QuestionOptionForUserResponse>();
    }

    public class QuestionOptionForUserResponse
    {
        public int Id { get; set; }
        public string OptionText { get; set; }
    }
}
