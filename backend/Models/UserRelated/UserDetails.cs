using System.ComponentModel.DataAnnotations.Schema;
using TalentBridge.Common.Entities;
using TalentBridge.Enums;
using TalentBridge.Enums.Auth;
using TalentBridge.Models.Roles;

namespace TalentBridge.Models.UserRelated;
    
public class UserDetails : BaseEntity
{
    public int UserId { get; set; }
    public string PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public GENDER Gender { get; set; }
    public string? Bio { get; set; }
    public string? CVPdfUrl { get; set; }
    public string? Skills { get; set; }

   
    public User User { get; set; }
    public List<Education> Educations { get; set; } = new List<Education>();
    public List<Experience> Experiences { get; set; } = new List<Experience>();
}

