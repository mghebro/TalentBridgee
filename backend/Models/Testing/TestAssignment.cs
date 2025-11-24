
using TalentBridge.Common.Entities;
using TalentBridge.Enums.Testing;
using TalentBridge.Models.Recruitment;
namespace TalentBridge.Models.Testing;

public class TestAssignment : BaseEntity
{
    public int ApplicationId { get; set; }
    public Application Application { get; set; }
    
    public int TestId { get; set; }
    public Test Test { get; set; }
    
    public int AssignedBy { get; set; }
    public int AssignedByHRManagerId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TEST_ASSIGNMENT_STATUS Status { get; set; }
    public string? AccessToken { get; set; }
}
