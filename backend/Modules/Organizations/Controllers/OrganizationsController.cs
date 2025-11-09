using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Common.Services.CurrentUser;
using TalentBridge.Data;
using TalentBridge.Enums.Auth;
using TalentBridge.Modules.Organizations.DTOs.Requests;
using TalentBridge.Modules.Organizations.DTOs.Responses;
using TalentBridge.Modules.Organizations.Services;
using TalentBridge.Common.Controllers;

namespace TalentBridge.Modules.Organizations.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : BaseApiController
{
    private readonly IOrganizationServices _organizationServices;
    private readonly ILogger<OrganizationsController> _logger;

    public OrganizationsController(
        IOrganizationServices organizationServices,
        ILogger<OrganizationsController> logger,
        IHttpContextAccessor httpContextAccessor,
        DataContext context) : base(httpContextAccessor, context)
    {
        _organizationServices = organizationServices;
        _logger = logger;
    }
[Authorize(Roles = "${nameof(ROLES.HR_MANAGER)},${nameof(ROLES.ORGANIZATION_ADMIN)}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateOrganization([FromForm] CreateOrganizationRequest request, IFormFile? logo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ServiceResult<OrganizationDetails>.FailureResult("Invalid request data"));
        }

        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, 
                ServiceResult<OrganizationDetails>.FailureResult(currentUserResponse.Message));
        }

        var result = await _organizationServices.CreateOrganizationAsync(request, logo, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetOrganizationById),
            new { id = result.Data.Id },
            result
        );
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrganizations([FromQuery] OrganizationFilterRequest request)
    {
        var result = await _organizationServices.GetOrganizationsAsync(request);
        return Ok(result);
    }

  
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrganizationById(int id)
    {
        var result = await _organizationServices.GetOrganizationByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = nameof(ROLES.ORGANIZATION_ADMIN))]
    [Consumes("multipart/form-data")]
    
    public async Task<IActionResult> UpdateOrganization(int id, [FromForm] UpdateOrganizationRequest request, IFormFile? logo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ServiceResult<OrganizationDetails>.FailureResult("Invalid request data"));
        }

        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, 
                ServiceResult<OrganizationDetails>.FailureResult(currentUserResponse.Message));
        }

        var result = await _organizationServices.UpdateOrganizationAsync(id, request, logo, currentUserResponse.Data);

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
    [Authorize(Roles = nameof(ROLES.ORGANIZATION_ADMIN))]
    [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrganization(int id)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, 
                ServiceResult<string>.FailureResult(currentUserResponse.Message));
        }

        var result = await _organizationServices.DeleteOrganizationAsync(id, currentUserResponse.Data);

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

    [HttpGet("my")]
    [Authorize(Roles = $"{nameof(ROLES.HR_MANAGER)},{nameof(ROLES.ORGANIZATION_ADMIN)}")]
    public async Task<IActionResult> GetOrganizationsForCurrentUser()
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status,
                ServiceResult<string>.FailureResult(currentUserResponse.Message));
        }

        var result = await _organizationServices.GetOrganizationsForCurrentUserAsync(currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}