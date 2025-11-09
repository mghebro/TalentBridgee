namespace TalentBridge.Modules.Auth.DTOs.Requests;
public class ResetPasswordTokenRequest
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
