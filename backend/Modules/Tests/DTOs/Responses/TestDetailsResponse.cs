using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Responses;

public class TestDetailsResponse
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Profession { get; set; }
    public int DurationMinutes { get; set; }
    public decimal PassingScore { get; set; }
    public decimal TotalPoints { get; set; }
    public TEST_DIFFICULTY Difficulty { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuestionResponse> Questions { get; set; } = new List<QuestionResponse>();
}

public class TestListResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string OrganizationName { get; set; }
    public string Profession { get; set; }
    public TEST_DIFFICULTY Difficulty { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalPoints { get; set; }
    public int QuestionCount { get; set; }
}
