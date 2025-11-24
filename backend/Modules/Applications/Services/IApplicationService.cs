using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Modules.Applications.DTOs.Requests;
using TalentBridge.Modules.Applications.DTOs.Responses;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.DTOs.Responses;

namespace TalentBridge.Modules.Applications.Services;

public interface IApplicationService
{
    Task<ApiResponse<ApplicationResponse>> CreateApplicationAsync(CreateApplicationRequest request, int userId);
    Task<ApiResponse<List<ApplicationResponse>>> GetApplicationsByUserIdAsync(int userId);
    Task<ApiResponse<List<ApplicationResponse>>> GetApplicationsByVacancyIdAsync(int vacancyId, int hrManagerUserId);
    Task<ApiResponse<ApplicationResponse>> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusRequest request, int hrManagerUserId);
    Task<ApiResponse<ApplicationResponse>> GetApplicationByIdAsync(int applicationId, int userId);
    Task<ApiResponse<ApplicationResponse>> AddReviewNoteAsync(int applicationId, AddReviewNoteRequest request, int hrManagerUserId);
    Task<ApiResponse<TestForApplicationResponse>> GetTestForApplicationAsync(int applicationId, int userId);
    Task<ApiResponse<TestSubmissionResponse>> SubmitTestAsync(int applicationId, int userId, SubmitTestRequest request);
}
