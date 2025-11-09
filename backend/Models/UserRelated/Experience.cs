using TalentBridge.Common.Entities;

namespace TalentBridge.Models.UserRelated;
    
public class Experience : BaseEntity
{
    public string Company { get; set; }
    public string Role { get; set; }
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public int? UserDetailsId { get; set; }
}