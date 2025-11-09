using Microsoft.AspNetCore.Authorization;
using TalentBridge.Modules.Auth.DTOs.Requests;
using TalentBridge.Modules.Auth.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using TalentBridge.Common.Controllers;
using TalentBridge.Data;

namespace TalentBridge.Modules.Auth.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor, DataContext context) : base(httpContextAccessor, context)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var User = await _authService.Register(request);
            return StatusCode(User.Status, User);

        }
        catch
        (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult> Verify(VerifyEmailRequest verify)
    {
        try
        {
            var VerifiedUser = await _authService.VerifyEmail(verify);
            return StatusCode(VerifiedUser.Status, VerifiedUser);
        }
        catch
        (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("send-resend-verification-code")]
    public async Task<ActionResult> SendResendCode(string userEmail)
    {
        try
        {
            var sendResendcode = await _authService.SendEmailVerification(userEmail);
            return StatusCode(sendResendcode.Status, sendResendcode);
        }
        catch
        (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [HttpPost("send-reset-password-link")]
    public async Task<ActionResult> SendResetPasswordLink([FromQuery] string userEmail)
    {
        try
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var sendResetCode = await _authService.SendResetPasswordLink(userEmail);
            return StatusCode(sendResetCode.Status, sendResetCode);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("reset-password")]
    public async Task<ActionResult> ResetPassword( ResetPasswordTokenRequest request)
    {
        try
        {
            var resetPassword = await _authService.ResetPassword(request);
            return StatusCode(resetPassword.Status, resetPassword); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest login)
    {
        try
        {
            var userlogin = await _authService.Login(login);
            return StatusCode(userlogin.Status, userlogin);
        }
        catch
        (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [Authorize]
    [HttpGet("get-current-user")]
    public async Task<ActionResult> GetCurrentUser()
    {
        try
        {
            var currentuser = await _authService.GetCurrentUser();
            return StatusCode(currentuser.Status, currentuser);
        }
        catch
        (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("update-user")]
    public async Task<ActionResult> UpdateUser(UpdateUserRequest request)
    {
        try
        {
            var user = await _authService.UpdateUser(request);
            return StatusCode(user.Status, user);
        }
        catch
        (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("delete-user")]
    public async Task<ActionResult> DeleteUser()
    {
        try
        {
            var user = await _authService.DeleteUser();
            return StatusCode(user.Status, user);
        }
        catch
        (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}