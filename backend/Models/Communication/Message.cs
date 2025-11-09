using TalentBridge.Common.Entities;
using TalentBridge.Models.Roles;
namespace TalentBridge.Models.Communication;
public class Message : BaseEntity
{
    public int SenderId { get; set; }
    public User Sender { get; set; }
    
    public int ReceiverId { get; set; }
    public User Receiver { get; set; }
    
    public int? ApplicationId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public int? ParentMessageId { get; set; }
}