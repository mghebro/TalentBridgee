namespace TalentBridge.Modules.Vacancies.DTOs.Responses;

public class VacancyTestDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Duration { get; set; }
    public int QuestionCount { get; set; }
    public bool IsActive { get; set; }
}