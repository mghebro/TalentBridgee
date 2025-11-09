using TalentBridge.Modules.Auth.DTOs.Responses;
using TalentBridge.Modules.Auth.DTOs.Requests;
using TalentBridge.Common.Services.Token;
using TalentBridge.Common.DTOs.Responses;

namespace TalentBridge.Modules.Auth.Services.Auth;

public interface IAuthService
{
    Task<ApiResponse<bool>> Register(RegisterRequest request);
    Task<ApiResponse<UserToken>> VerifyEmail(VerifyEmailRequest verify);
    Task<ApiResponse<UserToken>> Login(LoginRequest login);
    Task<ApiResponse<bool>> UpdateUser(UpdateUserRequest request);
    Task<ApiResponse<bool>> DeleteUser();
    Task<ApiResponse<bool>> SendEmailVerification(string email);
    Task<ApiResponse<LoginResponse>> GetCurrentUser();
    Task<ApiResponse<bool>> SendResetPasswordLink(string email);
    Task<ApiResponse<bool>> ResetPassword(ResetPasswordTokenRequest request);
    Task<ApiResponse<bool>> ChangePassword(int userId,  ChangePasswordRequest request);
}