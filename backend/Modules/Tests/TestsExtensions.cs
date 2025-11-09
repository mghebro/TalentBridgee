using Microsoft.Extensions.DependencyInjection;
using TalentBridge.Modules.Tests.Services;

namespace TalentBridge.Modules.Tests;

public static class TestsExtensions
{
    public static IServiceCollection AddTestsModule(this IServiceCollection services)
    {
        services.AddScoped<ITestService, TestService>();
        return services;
    }
}