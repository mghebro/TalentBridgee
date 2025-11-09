using TalentBridge.Modules.Vacancies.Services;

namespace TalentBridge.Modules.Vacancies;

public static class VacanciesExtensions
{
    public static IServiceCollection AddVacanciesModule(this IServiceCollection services)
    {
        services.AddScoped<IVacancyService, VacancyService>();
        
        return services;
    }
}