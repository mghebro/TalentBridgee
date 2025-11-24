using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Requests;

public class CreateTestRequest
{
    public int VacancyId { get; set; }
    public int OrganizationId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Profession { get; set; }
    public int DurationMinutes { get; set; }
    public decimal PassingScore { get; set; }
    public TEST_DIFFICULTY Difficulty { get; set; }
    public List<CreateQuestionInTestRequest>? Questions { get; set; }
}

public class CreateQuestionInTestRequest
{
    public string Text { get; set; }
    public string Type { get; set; }
    public decimal Points { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public List<CreateQuestionOptionInTestRequest>? Options { get; set; }
}

public class CreateQuestionOptionInTestRequest
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}