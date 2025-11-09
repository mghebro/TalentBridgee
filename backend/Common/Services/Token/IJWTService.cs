using TalentBridge.Models.Roles;

namespace TalentBridge.Common.Services.Token;

public interface IJWTService
{
    string GenerateRefreshToken();
    UserToken GetUserToken(User user);

    public string HashToken(string token);
    public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token,
        DateTime expiration);
}