using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.DTOs.Responses;
using System.Threading.Tasks;

namespace TalentBridge.Modules.Tests.Services;

public interface ITestService
{
    Task<ServiceResult<TestDetailsResponse>> CreateTestAsync(CreateTestRequest request, int userId);
    Task<ServiceResult<QuestionResponse>> AddQuestionToTestAsync(int testId, CreateQuestionRequest request, int userId);
    Task<ServiceResult<TestSubmissionResponse>> StartTestSubmissionAsync(int testAssignmentId, int userId);
    Task<ServiceResult<TestSubmissionResponse>> SubmitTestSubmissionAsync(int testSubmissionId, SubmitTestRequest request, int userId);
    Task<ServiceResult<SubmissionAnswerResponse>> SubmitAnswerAsync(int assignmentId, int questionId, SubmitAnswerRequest request, int userId);
    Task<ServiceResult<SubmissionAnswerResponse>> HandleQuestionTimeoutAsync(int assignmentId, int questionId, int userId);
    Task<ServiceResult<TestDetailsResponse>> GetTestByIdAsync(int testId);
    Task<ServiceResult<TestSubmissionResponse>> GetTestSubmissionResultAsync(int testSubmissionId, int userId);
    Task<ServiceResult<PaginatedResult<TestListResponse>>> GetTestsAsync(TestFilterRequest request);
    Task<ServiceResult<TestAssignmentResponse>> AssignTestAsync(CreateTestAssignmentRequest request, int userId);
    Task<ServiceResult<SubmissionAnswerResponse>> GradeSubmissionAnswerAsync(int submissionAnswerId, GradeAnswerRequest request, int hrManagerUserId);
}