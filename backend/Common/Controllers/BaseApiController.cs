using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;

namespace TalentBridge.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly DataContext _context;

    public BaseApiController(IHttpContextAccessor httpContextAccessor, DataContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    protected async Task<ApiResponse<int>> GetCurrentUserIdAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return new ApiResponse<int>
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = "User ID not found in token",
                Data = 0
            };
        }

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return new ApiResponse<int>
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = "Invalid user ID format",
                Data = 0
            };
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return new ApiResponse<int>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "User not found",
                Data = 0
            };
        }

        return new ApiResponse<int>
        {
            Status = StatusCodes.Status200OK,
            Message = "User authenticated successfully",
            Data = userId
        };
    }
}