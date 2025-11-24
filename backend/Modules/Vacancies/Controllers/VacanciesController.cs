using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Enums.Auth;
using TalentBridge.Modules.Vacancies.DTOs.Requests;
using TalentBridge.Modules.Vacancies.Services;
using TalentBridge.Common.Controllers;

namespace TalentBridge.Modules.Vacancies.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacanciesController : BaseApiController
{
    private readonly IVacancyService _vacancyService;

    public VacanciesController(
        IVacancyService vacancyService,
        IHttpContextAccessor httpContextAccessor,
        DataContext context) : base(httpContextAccessor, context)
    {
        _vacancyService = vacancyService;
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid request data" });
        }

        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _vacancyService.CreateVacancyAsync(request, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetVacancyById),
            new { id = result.Data.Id },
            result
        );
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetVacancies([FromQuery] VacancyFilterRequest request)
    {
        var result = await _vacancyService.GetVacanciesAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVacancyById(int id)
    {
        int? userId = null;
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var result = await _vacancyService.GetVacancyByIdAsync(id, userId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> UpdateVacancy(int id, [FromBody] UpdateVacancyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid request data" });
        }

        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _vacancyService.UpdateVacancyAsync(id, request, currentUserResponse.Data);

        if (!result.Success)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(result);
            }
            if (result.Errors.Any(e => e.Contains("permission")))
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> DeleteVacancy(int id)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _vacancyService.DeleteVacancyAsync(id, currentUserResponse.Data);

        if (!result.Success)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(result);
            }
            if (result.Errors.Any(e => e.Contains("permission")))
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}/analytics")]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> GetVacancyAnalytics(int id)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _vacancyService.GetVacancyAnalyticsAsync(id, currentUserResponse.Data);

        if (!result.Success)
        {
            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(result);
            }
            if (result.Errors.Any(e => e.Contains("permission")))
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }
    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetVacaniesForOrganization(int organizationId){
        var vacancies = await _vacancyService.GetVacanciesByOrganizationAsync(organizationId);
        return Ok(vacancies);
    }
    
    [HttpPost("{id}/apply")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> Apply(int id)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _vacancyService.ApplyAsync(id, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/assign-test/{testId}")]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> AssignTestToVacancy(int id, int testId)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _vacancyService.AssignTestToVacancyAsync(id, testId, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}