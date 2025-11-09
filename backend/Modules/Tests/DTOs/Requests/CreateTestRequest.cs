using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Requests;

public class CreateTestRequest
{
    public int OrganizationId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Profession { get; set; }
    public int DurationMinutes { get; set; }
    public decimal PassingScore { get; set; }
    public TEST_DIFFICULTY Difficulty { get; set; }
}