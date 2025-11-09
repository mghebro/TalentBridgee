namespace TalentBridge.Modules.Auth.DTOs.Requests;

public class VerifyEmailRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}