using TalentBridge.Modules.Applications.Services;

namespace TalentBridge.Modules.Applications;

public static class ApplicationsExtensions
{
    public static IServiceCollection AddApplicationsModule(this IServiceCollection services)
    {
        services.AddScoped<IApplicationService, ApplicationService>();
        return services;
    }
}
