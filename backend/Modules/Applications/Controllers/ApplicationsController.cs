using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Enums.Auth;
using TalentBridge.Modules.Applications.DTOs.Requests;
using TalentBridge.Modules.Applications.Services;
using TalentBridge.Common.Controllers;
using TalentBridge.Modules.Tests.DTOs.Requests;

namespace TalentBridge.Modules.Applications.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : BaseApiController
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(
        IApplicationService applicationService,
        IHttpContextAccessor httpContextAccessor,
        DataContext context) : base(httpContextAccessor, context)
    {
        _applicationService = applicationService;
    }

    [HttpPost]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
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

        var result = await _applicationService.CreateApplicationAsync(request, currentUserResponse.Data);

        if (result.Status != StatusCodes.Status201Created)
        {
            return StatusCode(result.Status, result);
        }

        return StatusCode(result.Status, result);
    }

    [HttpGet("my-applications")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> GetMyApplications()
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }
        var result = await _applicationService.GetApplicationsByUserIdAsync(currentUserResponse.Data);
        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, result);
        }

        return StatusCode(result.Status, result);
    }

    [HttpGet("/api/vacancies/{vacancyId}/applications")]
    [Authorize(Roles = $"{nameof(ROLES.HR_MANAGER)},{nameof(ROLES.ORGANIZATION_ADMIN)}")]
    public async Task<IActionResult> GetApplicationsForVacancy(int vacancyId)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _applicationService.GetApplicationsByVacancyIdAsync(vacancyId, currentUserResponse.Data);

        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, result);
        }

        return StatusCode(result.Status, result);
    }

    [HttpGet("my-organization-applications")]
    [Authorize(Roles = $"{nameof(ROLES.HR_MANAGER)},{nameof(ROLES.ORGANIZATION_ADMIN)}")]
    public async Task<IActionResult> GetAllApplicationsForOrganization()
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _applicationService.GetAllApplicationsForOrganizationAsync(currentUserResponse.Data);

        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, result);
        }

        return StatusCode(result.Status, result);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = $"{nameof(ROLES.HR_MANAGER)},{nameof(ROLES.ORGANIZATION_ADMIN)}")]
    public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] UpdateApplicationStatusRequest request)
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

        var result = await _applicationService.UpdateApplicationStatusAsync(id, request, currentUserResponse.Data);

        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, result);
        }

        return StatusCode(result.Status, result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetApplicationById(int id)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _applicationService.GetApplicationByIdAsync(id, currentUserResponse.Data);

        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, result);
        }

        return StatusCode(result.Status, result);
    }

    [HttpPost("{id}/notes")]
    [Authorize(Roles = $"{nameof(ROLES.HR_MANAGER)},{nameof(ROLES.ORGANIZATION_ADMIN)}")]
    public async Task<IActionResult> AddReviewNote(int id, [FromBody] AddReviewNoteRequest request)
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

        var result = await _applicationService.AddReviewNoteAsync(id, request, currentUserResponse.Data);

        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, result);
        }

        return StatusCode(result.Status, result);
    }

    [HttpGet("{id}/test")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> GetTestForApplication(int id)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, new { currentUserResponse.Message });
        }

        var result = await _applicationService.GetTestForApplicationAsync(id, currentUserResponse.Data);

        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, new { result.Message });
        }

        return Ok(result.Data);
    }

    [HttpPost("{id}/submit")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> SubmitTest(int id, [FromBody] SubmitTestRequest request)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, new { currentUserResponse.Message });
        }

        var result = await _applicationService.SubmitTestAsync(id, currentUserResponse.Data, request);

        if (result.Status != StatusCodes.Status200OK)
        {
            return StatusCode(result.Status, new { result.Message });
        }

        return Ok(result.Data);
    }
}
