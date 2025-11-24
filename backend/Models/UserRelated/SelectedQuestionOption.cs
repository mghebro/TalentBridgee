using TalentBridge.Common.Entities;
using TalentBridge.Models.Testing;

namespace TalentBridge.Models.UserRelated;

public class SelectedQuestionOption : BaseEntity
{
    public int SubmissionAnswerId { get; set; }
    public SubmissionAnswer SubmissionAnswer { get; set; }
    public int QuestionOptionId { get; set; }
    public QuestionOption QuestionOption { get; set; }
}
