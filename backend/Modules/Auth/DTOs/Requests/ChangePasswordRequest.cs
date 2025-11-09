namespace TalentBridge.Modules.Auth.DTOs.Requests;

public class ChangePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}