using TalentBridge.Modules.Auth.Services.Token;
using TalentBridge.Modules.Auth.DTOs.Requests;
using TalentBridge.Modules.Auth.Services.Auth;
using TalentBridge.Modules.Auth.Validations;
using TalentBridge.Common.Services.Token;
using FluentValidation;
using TalentBridge.Modules.Organizations.Services;

namespace TalentBridge.Modules.Organizations;

public static class OrganizationsExtensionss
{
    public static IServiceCollection AddORGModule(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationServices, OrganizationServices>();
        
        return services;
    }
}