using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Responses;

public class TestAssignmentResponse
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int TestId { get; set; }
    public string TestTitle { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public TEST_ASSIGNMENT_STATUS Status { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}