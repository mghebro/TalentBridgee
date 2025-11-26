using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentBridge.Common.Controllers;
using TalentBridge.Data;
using TalentBridge.Enums.Auth;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.Services;

namespace TalentBridge.Modules.Tests.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestsController : BaseApiController
{
    private readonly ITestService _testService;

    public TestsController(
        ITestService testService,
        IHttpContextAccessor httpContextAccessor,
        DataContext context) : base(httpContextAccessor, context)
    {
        _testService = testService;
    }

    #region New Per-Question Endpoints

    [HttpPost("assignments/{assignmentId:int}/questions/{questionId:int}/submit")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> SubmitAnswer(int assignmentId, int questionId, [FromBody] SubmitAnswerRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse.Message);
        }

        var result = await _testService.SubmitAnswerAsync(assignmentId, questionId, request, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
    
    [HttpPost("assignments/{assignmentId:int}/questions/{questionId:int}/timeout")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> HandleQuestionTimeout(int assignmentId, int questionId)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse.Message);
        }

        var result = await _testService.HandleQuestionTimeoutAsync(assignmentId, questionId, currentUserResponse.Data);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    #endregion

    #region Existing Endpoints
    
    [HttpPost]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> CreateTest([FromBody] CreateTestRequest request)
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

        var result = await _testService.CreateTestAsync(request, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetTestById),
            new { id = result.Data.Id },
            result
        );
    }

    [HttpPost("{testId}/questions")]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> AddQuestionToTest(int testId, [FromBody] CreateQuestionRequest request)
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

        var result = await _testService.AddQuestionToTestAsync(testId, request, currentUserResponse.Data);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("assign")]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> AssignTest([FromBody] CreateTestAssignmentRequest request)
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

        var result = await _testService.AssignTestAsync(request, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }


        return Ok(result);
    }

    [HttpPost("assignments/{testAssignmentId}/start")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> StartTestSubmission(int testAssignmentId)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _testService.StartTestSubmissionAsync(testAssignmentId, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }


        return Ok(result);
    }

    [HttpPost("submissions/{testSubmissionId}/submit")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> SubmitTestSubmission(int testSubmissionId, [FromBody] SubmitTestRequest request)
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

        var result = await _testService.SubmitTestSubmissionAsync(testSubmissionId, request, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestById(int id)
    {
        var result = await _testService.GetTestByIdAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTests([FromQuery] TestFilterRequest request)
    {
        var result = await _testService.GetTestsAsync(request);
        return Ok(result);
    }

    [HttpGet("my-assignments")]
    [Authorize(Roles = nameof(ROLES.USER))]
    public async Task<IActionResult> GetMyAssignedTests()
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _testService.GetAssignedTestsForUserAsync(currentUserResponse.Data);
        return Ok(result);
    }

    [HttpGet("my-tests")]
    [Authorize(Roles = $"{nameof(ROLES.ORGANIZATION_ADMIN)},{nameof(ROLES.HR_MANAGER)}")]
    public async Task<IActionResult> GetMyCreatedTests()
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _testService.GetTestsCreatedByUserAsync(currentUserResponse.Data);
        return Ok(result);
    }

    [HttpGet("submissions/{testSubmissionId}/result")]
    [Authorize] 
    public async Task<IActionResult> GetTestSubmissionResult(int testSubmissionId)
    {
        var currentUserResponse = await GetCurrentUserIdAsync();
        if (currentUserResponse.Status != StatusCodes.Status200OK)
        {
            return StatusCode(currentUserResponse.Status, currentUserResponse);
        }

        var result = await _testService.GetTestSubmissionResultAsync(testSubmissionId, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }


        return Ok(result);
    }

    [HttpPut("/api/submission-answers/{id}/grade")]
    [Authorize(Roles = $"{nameof(ROLES.HR_MANAGER)},{nameof(ROLES.ORGANIZATION_ADMIN)}")]
    public async Task<IActionResult> GradeSubmissionAnswer(int id, [FromBody] GradeAnswerRequest request)
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

        var result = await _testService.GradeSubmissionAnswerAsync(id, request, currentUserResponse.Data);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
    
    #endregion
}