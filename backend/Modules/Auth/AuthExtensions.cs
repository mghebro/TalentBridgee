using TalentBridge.Modules.Auth.Services.Token;
using TalentBridge.Modules.Auth.DTOs.Requests;
using TalentBridge.Modules.Auth.Services.Auth;
using TalentBridge.Modules.Auth.Validations;
using TalentBridge.Common.Services.Token;
using FluentValidation;

namespace TalentBridge.Modules.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJWTService, JWTService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            options.AddPolicy("HostOnly", policy => policy.RequireRole("Host"));
            options.AddPolicy("Universal", policy => policy.RequireRole("Host, Admin"));
        });


        return services;
    }
}