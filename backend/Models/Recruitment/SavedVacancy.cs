using TalentBridge.Common.Entities;

namespace TalentBridge.Models.Recruitment;

public class SavedVacancy : BaseEntity
{
    public int UserId { get; set; }
    public int VacancyId { get; set; }
    public DateTime SavedAt { get; set; }
    public string? Notes { get; set; }
}