using TalentBridge.Common.Entities;

namespace TalentBridge.Models.UserRelated;
    
public class Education : BaseEntity
{
    public string School { get; set; }
    public string Degree { get; set; }
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public int? UserDetailsId { get; set; }
}