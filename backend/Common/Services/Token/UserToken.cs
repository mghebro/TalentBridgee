namespace TalentBridge.Common.Services.Token;
public class UserToken
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string RefreshToken { get; set; }
}
