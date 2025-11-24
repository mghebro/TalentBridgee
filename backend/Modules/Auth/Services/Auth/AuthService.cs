using TalentBridge.Modules.Auth.DTOs.Responses;
using TalentBridge.Modules.Auth.DTOs.Requests;
using TalentBridge.Modules.Auth.Validations;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Common.Services.Token;
using System.Security.Claims;
using TalentBridge.Models.Roles;
using TalentBridge.Models.Auth;
using TalentBridge.Data;
using AutoMapper;
using TalentBridge.Common.SMTP;
using TalentBridge.Modules.Organizations.Services; 
using TalentBridge.Enums.Auth;
using TalentBridge.Models;

namespace TalentBridge.Modules.Auth.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IJWTService _jwtService;
    private readonly RegisterValidator _registerValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoginValidator _loginValidator;
    private readonly IOrganizationServices _organizationServices; 

    public AuthService(IMapper mapper, DataContext context, IJWTService jwtService, RegisterValidator registerValidator,
        IHttpContextAccessor httpContextAccessor, LoginValidator loginValidator, IOrganizationServices organizationServices) 
    {
        _mapper = mapper;
        _context = context;
        _jwtService = jwtService;
        _registerValidator = registerValidator;
        _httpContextAccessor = httpContextAccessor;
        _loginValidator = loginValidator;
        _organizationServices = organizationServices; 
    }

 public async Task<ApiResponse<bool>> Register(RegisterRequest request)
{
    var result = _registerValidator.Validate(request);
    if (!result.IsValid)
    {
        var errorMessages = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
        return new ApiResponse<bool>
        {
            Data = false,
            Status = StatusCodes.Status400BadRequest,
            Message = errorMessages
        };
    }

    var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
    if (userExists != null)
    {
        return new ApiResponse<bool>
        {
            Data = false,
            Status = StatusCodes.Status409Conflict,
            Message = "User already exists"
        };
    }

    var user = new User
    {
        Email = request.Email,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        CreatedAt = DateTime.UtcNow,
        IsVerified = false,
    };

    user.Role = request.DesiredRole switch
    {
        "USER" => ROLES.USER,
        "HR_MANAGER" => ROLES.HR_MANAGER,
        "ORGANIZATION_ADMIN" => ROLES.ORGANIZATION_ADMIN,
        _ => throw new ArgumentException("Invalid role specified.")
    };

    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();

    switch (user.Role)
    {
        case ROLES.HR_MANAGER:
            if (!request.OrganizationId.HasValue)
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "OrganizationId is required for HR Manager registration."
                };
            }

            var existingOrganization = await _context.Organizations.FindAsync(request.OrganizationId.Value);
            if (existingOrganization == null)
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "Organization not found."
                };
            }

            var hrManager = new HRManager
            {
                UserId = user.Id,
                OrganizationId = request.OrganizationId.Value,
                Position = "HR Manager",
                HiredDate = DateTime.UtcNow,
                IsActive = true
            };

            await _context.HrManagers.AddAsync(hrManager);
            break;

        case ROLES.ORGANIZATION_ADMIN:
            if (request.OrganizationDetails == null)
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Organization details are required for Organization Admin registration."
                };
            }

            var newOrganization = new Organization  
            {
                UserId = user.Id,
                Name = request.OrganizationDetails.Name,
                Type = request.OrganizationDetails.Type,
                Address = request.OrganizationDetails.Address,
                ContactEmail = request.OrganizationDetails.ContactEmail,
                Website = request.OrganizationDetails.Website,
                Description = request.OrganizationDetails.Description,
                PhoneNumber = request.OrganizationDetails.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Organizations.AddAsync(newOrganization);
            await _context.SaveChangesAsync();

            var orgAdminHrManager = new HRManager
            {
                UserId = user.Id,
                OrganizationId = newOrganization.Id,
                Position = "Organization Admin",
                HiredDate = DateTime.UtcNow,
                IsActive = true
            };

            await _context.HrManagers.AddAsync(orgAdminHrManager);
            break;

        case ROLES.USER:
            break;
    }

    await _context.SaveChangesAsync();

    string verificationCode = await SMTPService.SendVerificationCodeAsync(user.Email!, user.FirstName);

    var emailVerification = new EmailVerification
    {
        UserId = user.Id,
        Code = verificationCode
    };

    await _context.EmailVerifications.AddAsync(emailVerification);
    await _context.SaveChangesAsync();

    return new ApiResponse<bool>
    {
        Data = true,
        Status = StatusCodes.Status200OK,
        Message = "User registered successfully. Please check your email for verification code."
    };
}

    public async Task<ApiResponse<UserToken>> VerifyEmail(VerifyEmailRequest verify)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == verify.Email);

        if (user == null)
        {
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };
        }

        if (user.IsVerified == true)
        {
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "User is already verified.",
                Status = StatusCodes.Status400BadRequest,
            };
        }

        var emailVerification = _context.EmailVerifications
            .Where(e => e.UserId == user.Id)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefault();

        if (emailVerification == null || emailVerification.Code != verify.Code)
        {
            if (emailVerification != null)
            {
                emailVerification.AttemptCount++;
                await _context.SaveChangesAsync();
            }
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "Wrong or already used verification code",
                Status = StatusCodes.Status400BadRequest,
            };
        }

        if (emailVerification.ExpiresAt < DateTime.UtcNow)
        {
            emailVerification.AttemptCount++;
            await _context.SaveChangesAsync();
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "Verification code has expired.",
                Status = StatusCodes.Status400BadRequest,
            };
        }

        if (emailVerification.AttemptCount >= 3)
        {
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "Too many failed attempts. Please request a new verification code.",
                Status = StatusCodes.Status400BadRequest,
            };
        }

        user.IsVerified = true;
        _context.EmailVerifications.Remove(emailVerification);

        await _context.SaveChangesAsync();

        return new ApiResponse<UserToken>
        {
            Data = null,
            Message = "User verified successfully",
            Status = StatusCodes.Status200OK,
        };
    }

    public async Task<ApiResponse<UserToken>> Login(LoginRequest login)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == login.Email);

        if (user == null)
        {
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "Invalid credentials",
                Status = StatusCodes.Status401Unauthorized,
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
        {
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "Invalid credentials",
                Status = StatusCodes.Status401Unauthorized,
            };
        }

        if (!user.IsVerified)
        {
            return new ApiResponse<UserToken>
            {
                Data = null,
                Message = "User Not Verified",
                Status = StatusCodes.Status403Forbidden,
            };
        }

        var userToken = _jwtService.GetUserToken(user);
        return new ApiResponse<UserToken>
        {
            Data = userToken,
            Status = StatusCodes.Status200OK,
            Message = "Login successful"
        };
    }

    public async Task<ApiResponse<bool>> UpdateUser(UpdateUserRequest request)
    {
        var currentUser = _httpContextAccessor.HttpContext?.User;
        if (currentUser == null)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "User not authenticated"
            };
        }

        var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                          currentUser.FindFirst("sub")?.Value ??
                          currentUser.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "Invalid or missing user identification"
            };
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status404NotFound,
                Message = "User not found"
            };
        }

        try
        {
            _mapper.Map(request, user);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = StatusCodes.Status200OK,
                Message = "User updated successfully"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status500InternalServerError,
                Message = $"Failed to update user: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteUser()
    {
        var currentUser = _httpContextAccessor.HttpContext?.User;

        if (currentUser == null)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "User not authenticated"
            };
        }

        var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                          currentUser.FindFirst("sub")?.Value ??
                          currentUser.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status401Unauthorized,
                Message = "Invalid or missing user identification"
            };
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status404NotFound,
                Message = "User not found"
            };
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Data = true,
            Status = StatusCodes.Status200OK,
            Message = "User deleted successfully"
        };
    }

    public async Task<ApiResponse<bool>> SendEmailVerification(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            var response = new ApiResponse<bool>
            {
                Data = false,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };

            return response;
        }

        else
        {
            if (!user.IsVerified)
            {
                string newCode = SMTPService.SendVerificationCode(user.Email, user.FirstName);

                var emailVerification = await _context.EmailVerifications
                    .FirstOrDefaultAsync(e => e.UserId == user.Id);

                if (emailVerification != null)
                {
                    emailVerification.Code = newCode;
                }
                else
                {
                    emailVerification = new EmailVerification
                    {
                        UserId = user.Id,
                        Code = newCode,
                    };
                    await _context.EmailVerifications.AddAsync(emailVerification);
                }

                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Data = true,
                    Status = StatusCodes.Status200OK,
                    Message = "Resend Code Sent Successfully",
                };
            }
            else
            {
                var response = new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Something went wrong , try again",
                    Status = StatusCodes.Status400BadRequest,
                };

                return response;
            }
        }
    }

    public async Task<ApiResponse<LoginResponse>> GetCurrentUser()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return new ApiResponse<LoginResponse>
            {
                Data = null,
                Message = "User ID not found ",
                Status = StatusCodes.Status401Unauthorized
            };
        }

        var userId = int.Parse(userIdClaim);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new ApiResponse<LoginResponse>
            {
                Data = null,
                Message = "User not found",
                Status = StatusCodes.Status404NotFound
            };
        }

        var userDto = _mapper.Map<LoginResponse>(user);

        return new ApiResponse<LoginResponse>
        {
            Data = userDto,
            Message = "User retrieved successfully",
            Status = StatusCodes.Status200OK
        };
    }

    public async Task<ApiResponse<bool>> SendResetPasswordLink(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            var notFoundresponse = new ApiResponse<bool>
            {
                Data = false,
                Message = "User Not Found",
                Status = StatusCodes.Status404NotFound,
            };
            return notFoundresponse;
        }

        PasswordVerification verification = new PasswordVerification() { UserId = user.Id };

        var resetLink = $"http://localhost:4200/auth/reset-password/new-password?token={verification.Token}";


        await _context.PasswordVerifications.AddAsync(verification);

        await _context.SaveChangesAsync();

        await SMTPService.SendPasswordResetLinkAsync(user.Email!, user.FirstName, resetLink);

        var response = new ApiResponse<bool>
        {
            Data = true,
            Status = StatusCodes.Status200OK,
            Message = "Password reset link sent successfully",
        };
        return response;
    }

    public async Task<ApiResponse<bool>> ResetPassword(ResetPasswordTokenRequest req)
    {
        var verification = await _context.PasswordVerifications
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Token == req.Token);

        if (verification == null)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Message = "Invalid or expired reset token",
                Status = StatusCodes.Status400BadRequest,
            };
        }

        verification.AttemptCount++;

        if (verification.ExpiresAt < DateTime.UtcNow)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status400BadRequest,
                Message = "Reset token has expired",
            };
        }

        if (verification.AttemptCount >= 3)
        {
            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status400BadRequest,
                Message = "Too many failed attempts. Please request a new reset link.",
            };
        }

        string HashedPassword = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);

        verification.User.Password = HashedPassword;
        _context.PasswordVerifications.Remove(verification);
        await _context.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Data = true,
            Status = StatusCodes.Status200OK,
            Message = "Password reset successfully",
        };
    }

    public async Task<ApiResponse<bool>> ChangePassword(int userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
        {

            return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status400BadRequest,
                Message = "Invalid old password"

            };
        }

        if (request.NewPassword != request.ConfirmPassword)
        { return new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status400BadRequest,
                Message = "Passwords Does not match"

            };
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return new ApiResponse<bool>
        {
            Data = true,
            Status = StatusCodes.Status200OK,
            Message = "Password changed successfully"
        };
    }
}



