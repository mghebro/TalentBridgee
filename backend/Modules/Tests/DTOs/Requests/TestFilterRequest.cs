using TalentBridge.Enums.Testing;

namespace TalentBridge.Modules.Tests.DTOs.Requests;

public class TestFilterRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public int? OrganizationId { get; set; }
    public TEST_DIFFICULTY? Difficulty { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}