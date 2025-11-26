using TalentBridge.Modules.UserProfile.Services;

namespace TalentBridge.Modules.UserProfile;

public static class UserProfileExtensions
{
    public static IServiceCollection AddUserProfileModule(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        return services;
    }
}

