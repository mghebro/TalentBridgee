using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Enums.Testing;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.DTOs.Responses;

namespace TalentBridge.Modules.Tests.Services;

public class TestService : ITestService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TestService> _logger;

    public TestService(DataContext context, IMapper mapper, ILogger<TestService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<SubmissionAnswerResponse>> SubmitAnswerAsync(int assignmentId, int questionId, SubmitAnswerRequest request, int userId)
    {
        var assignment = await UserCanAccessTestAssignment(assignmentId, userId);
        if (assignment == null)
        {
            return ServiceResult<SubmissionAnswerResponse>.FailureResult("Test assignment not found or access denied.");
        }

        var submission = await _context.TestSubmissions
            .FirstOrDefaultAsync(s => s.TestAssignmentId == assignmentId && s.UserId == userId);
        if (submission == null)
        {
            return ServiceResult<SubmissionAnswerResponse>.FailureResult("Test submission not started.");
        }
        
        var existingAnswer = await _context.SubmissionAnswers
            .AnyAsync(sa => sa.TestSubmissionId == submission.Id && sa.QuestionId == questionId);

        if (existingAnswer)
        {
            _logger.LogWarning("User {UserId} attempted to re-submit answer for question {QuestionId} in assignment {AssignmentId}. Action blocked.", userId, questionId, assignmentId);
            return ServiceResult<SubmissionAnswerResponse>.FailureResult("This question has already been answered or timed out and is locked.");
        }

        var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == questionId);
        if (question == null)
        {
            return ServiceResult<SubmissionAnswerResponse>.FailureResult("Question not found.");
        }

        var newAnswer = new SubmissionAnswer
        {
            TestSubmissionId = submission.Id,
            QuestionId = questionId,
            AnsweredAt = DateTime.UtcNow,
            TimeSpentSeconds = request.TimeSpentSeconds,
            IsTimedOut = false, 
            AnswerText = request.AnswerText,
        };

        bool isCorrect = false;
        if (question.QuestionType == QUESTION_TYPE.MultipleChoice && request.SelectedOptionIds.Any())
        {
            var correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet();
            var selectedOptionIds = request.SelectedOptionIds.ToHashSet();

            isCorrect = correctOptionIds.SetEquals(selectedOptionIds);

            foreach (var optionId in request.SelectedOptionIds)
            {
                newAnswer.SelectedOptions.Add(new Models.UserRelated.SelectedQuestionOption { QuestionOptionId = optionId });
            }
        }

        newAnswer.IsCorrect = isCorrect;
        newAnswer.PointsAwarded = isCorrect ? question.Points : 0;
        
        _context.SubmissionAnswers.Add(newAnswer);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<SubmissionAnswerResponse>(newAnswer);
        return ServiceResult<SubmissionAnswerResponse>.SuccessResult(response, "Answer submitted successfully.");
    }
    

    public async Task<ServiceResult<SubmissionAnswerResponse>> HandleQuestionTimeoutAsync(int assignmentId, int questionId, int userId)
    {
        var assignment = await UserCanAccessTestAssignment(assignmentId, userId);
        if (assignment == null)
        {
            return ServiceResult<SubmissionAnswerResponse>.FailureResult("Test assignment not found or access denied.");
        }

        var submission = await _context.TestSubmissions
            .FirstOrDefaultAsync(s => s.TestAssignmentId == assignmentId && s.UserId == userId);

        if (submission == null)
        {
            return ServiceResult<SubmissionAnswerResponse>.FailureResult("Test submission not started.");
        }

        var existingAnswer = await _context.SubmissionAnswers
            .FirstOrDefaultAsync(sa => sa.TestSubmissionId == submission.Id && sa.QuestionId == questionId);

        if (existingAnswer != null)
        {
            _logger.LogInformation("Timeout for question {QuestionId} in assignment {AssignmentId} received, but an answer already exists. No action taken.", questionId, assignmentId);
            var existingResponse = _mapper.Map<SubmissionAnswerResponse>(existingAnswer);
            return ServiceResult<SubmissionAnswerResponse>.SuccessResult(existingResponse, "Question already processed.");
        }

        var question = await _context.Questions.FindAsync(questionId);
        if (question == null)
        {
            return ServiceResult<SubmissionAnswerResponse>.FailureResult("Question not found.");
        }

        var timedOutAnswer = new SubmissionAnswer
        {
            TestSubmissionId = submission.Id,
            QuestionId = questionId,
            AnsweredAt = DateTime.UtcNow,
            TimeSpentSeconds = question.TimeLimitSeconds,
            IsTimedOut = true,
            IsCorrect = false,
            PointsAwarded = 0,
            AnswerText = "Timed Out"
        };
        
        _context.SubmissionAnswers.Add(timedOutAnswer);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} timed out on question {QuestionId} in assignment {AssignmentId}. A 'wrong' answer was recorded.", userId, questionId, assignmentId);

        var response = _mapper.Map<SubmissionAnswerResponse>(timedOutAnswer);
        return ServiceResult<SubmissionAnswerResponse>.SuccessResult(response, "Question timed out and was marked as incorrect.");
    }
    
    private async Task<TestAssignment?> UserCanAccessTestAssignment(int assignmentId, int userId)
    {
        return await _context.TestAssignments
            .FirstOrDefaultAsync(ta => ta.Id == assignmentId && ta.Application.UserId == userId && ta.Status == TEST_ASSIGNMENT_STATUS.InProgress);
    }
    
    
    
    public async Task<ServiceResult<TestDetailsResponse>> CreateTestAsync(CreateTestRequest request, int userId)
    {
        var organization = await _context.Organizations.FindAsync(request.OrganizationId);
        if (organization == null) 
            return ServiceResult<TestDetailsResponse>.FailureResult("Organization not found.");

        if (organization.UserId != userId) 
            return ServiceResult<TestDetailsResponse>.FailureResult("Access denied.");

        var vacancy = await _context.Vacancies.FindAsync(request.VacancyId);
        if (vacancy == null)
            return ServiceResult<TestDetailsResponse>.FailureResult("Vacancy not found.");

        var test = _mapper.Map<Test>(request);
        test.VacancyId = request.VacancyId;
        test.Industry = "prosta industria ra";
        test.TotalPoints = 0;

        if (request.Questions != null && request.Questions.Any())
        {
            test.Questions = new List<Question>();
            var questionOrder = 1;

            foreach (var questionRequest in request.Questions)
            {
                var question = new Question
                {
                    QuestionText = questionRequest.Text,
                    QuestionType = MapQuestionType(questionRequest.Type),
                    Points = questionRequest.Points,
                    OrderNumber = questionOrder++,
                    TimeLimitSeconds = questionRequest.TimeLimitSeconds ?? 0,
                    IsRequired = true,
                    Options = new List<QuestionOption>()
                };

                if (questionRequest.Options != null && questionRequest.Options.Any())
                {
                    var optionOrder = 1;
                    foreach (var optionRequest in questionRequest.Options)
                    {
                        question.Options.Add(new QuestionOption
                        {
                            OptionText = optionRequest.Text,
                            IsCorrect = optionRequest.IsCorrect,
                            OrderNumber = optionOrder++
                        });
                    }
                }

                test.TotalPoints += question.Points;
                test.Questions.Add(question);
            }
        }

        _context.Tests.Add(test);

        await _context.SaveChangesAsync();

        vacancy.TestId = test.Id;

        await _context.SaveChangesAsync();

        var response = _mapper.Map<TestDetailsResponse>(test);
        return ServiceResult<TestDetailsResponse>.SuccessResult(response, "Test created successfully.");
    }

    public async Task<ServiceResult<QuestionResponse>> AddQuestionToTestAsync(int testId, CreateQuestionRequest request, int userId)
    {
        var test = await _context.Tests.FindAsync(testId);
        if (test == null) return ServiceResult<QuestionResponse>.FailureResult("Test not found.");
        var organization = await _context.Organizations.FindAsync(test.OrganizationId);
        if (organization.UserId != userId) return ServiceResult<QuestionResponse>.FailureResult("Access denied.");

        var question = _mapper.Map<Question>(request);
        question.TestId = testId;
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<QuestionResponse>(question);
        return ServiceResult<QuestionResponse>.SuccessResult(response, "Question added successfully.");
    }

    public async Task<ServiceResult<TestSubmissionResponse>> StartTestSubmissionAsync(int testAssignmentId, int userId)
    {
        var assignment = await _context.TestAssignments
            .FirstOrDefaultAsync(ta => ta.Id == testAssignmentId && ta.Application.UserId == userId);

        if (assignment == null) return ServiceResult<TestSubmissionResponse>.FailureResult("Test assignment not found.");
        if (assignment.Status != TEST_ASSIGNMENT_STATUS.Assigned) return ServiceResult<TestSubmissionResponse>.FailureResult("Test already started or completed.");

        assignment.Status = TEST_ASSIGNMENT_STATUS.InProgress;
        
        var submission = new TestSubmission
        {
            TestAssignmentId = testAssignmentId,
            UserId = userId,
            TestId = assignment.TestId,
            StartTime = DateTime.UtcNow,
        };
        _context.TestSubmissions.Add(submission);
        
        await _context.SaveChangesAsync();
        var response = _mapper.Map<TestSubmissionResponse>(submission);
        return ServiceResult<TestSubmissionResponse>.SuccessResult(response, "Test started successfully.");
    }

    public async Task<ServiceResult<TestSubmissionResponse>> SubmitTestSubmissionAsync(int testSubmissionId, SubmitTestRequest request, int userId)
    {
        var submission = await _context.TestSubmissions.FindAsync(testSubmissionId);
        if (submission == null || submission.UserId != userId) return ServiceResult<TestSubmissionResponse>.FailureResult("Submission not found.");

        submission.EndTime = DateTime.UtcNow;
        submission.SubmittedAt = DateTime.UtcNow;

        var assignment = await _context.TestAssignments.FindAsync(submission.TestAssignmentId);
        if(assignment != null) assignment.Status = TEST_ASSIGNMENT_STATUS.Completed;

        await _context.SaveChangesAsync();
        
        var response = _mapper.Map<TestSubmissionResponse>(submission);
        return ServiceResult<TestSubmissionResponse>.SuccessResult(response, "Test submitted successfully.");
    }
    
    public async Task<ServiceResult<TestDetailsResponse>> GetTestByIdAsync(int testId)
    {
        var test = await _context.Tests.Include(t => t.Questions).ThenInclude(q => q.Options).FirstOrDefaultAsync(t => t.Id == testId);
        if (test == null) return ServiceResult<TestDetailsResponse>.FailureResult("Test not found.");
        var response = _mapper.Map<TestDetailsResponse>(test);
        return ServiceResult<TestDetailsResponse>.SuccessResult(response);
    }
    
    public async Task<ServiceResult<TestSubmissionResponse>> GetTestSubmissionResultAsync(int testSubmissionId, int userId)
    {
        var submission = await _context.TestSubmissions.Include(s => s.Answers).FirstOrDefaultAsync(s => s.Id == testSubmissionId && s.UserId == userId);
        if (submission == null) return ServiceResult<TestSubmissionResponse>.FailureResult("Submission not found.");
        var response = _mapper.Map<TestSubmissionResponse>(submission);
        return ServiceResult<TestSubmissionResponse>.SuccessResult(response);
    }

    public async Task<ServiceResult<PaginatedResult<TestListResponse>>> GetTestsAsync(TestFilterRequest request)
    {
        var query = _context.Tests.AsQueryable();
        var totalItems = await query.CountAsync();
        var tests = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        var mappedTests = _mapper.Map<List<TestListResponse>>(tests);
        var result = new PaginatedResult<TestListResponse> { Items = mappedTests, TotalItems = totalItems, Page = request.Page, PageSize = request.PageSize };
        return ServiceResult<PaginatedResult<TestListResponse>>.SuccessResult(result);
    }
    
    public async Task<ServiceResult<TestAssignmentResponse>> AssignTestAsync(CreateTestAssignmentRequest request, int userId)
    {
        var application = await _context.Applications.FindAsync(request.ApplicationId);
        if (application == null) return ServiceResult<TestAssignmentResponse>.FailureResult("Application not found.");
        
        var assignment = _mapper.Map<TestAssignment>(request);
        assignment.AssignedBy = userId;
        assignment.AssignedAt = DateTime.UtcNow;
        assignment.Status = TEST_ASSIGNMENT_STATUS.Assigned;
        
        _context.TestAssignments.Add(assignment);
        await _context.SaveChangesAsync();
        
        var response = _mapper.Map<TestAssignmentResponse>(assignment);
        return ServiceResult<TestAssignmentResponse>.SuccessResult(response, "Test assigned.");
    }

    public async Task<ServiceResult<SubmissionAnswerResponse>> GradeSubmissionAnswerAsync(int submissionAnswerId, GradeAnswerRequest request, int hrManagerUserId)
    {
        var submissionAnswer = await _context.SubmissionAnswers.FindAsync(submissionAnswerId);
        if (submissionAnswer == null) return ServiceResult<SubmissionAnswerResponse>.FailureResult("Answer not found.");

        submissionAnswer.PointsAwarded = request.PointsAwarded;
        submissionAnswer.GraderComments = request.GraderComments;
        submissionAnswer.IsCorrect = request.PointsAwarded > 0; 
        
        await _context.SaveChangesAsync();
        
        var testSubmission = await _context.TestSubmissions.Include(ts => ts.Answers).FirstOrDefaultAsync(ts => ts.Id == submissionAnswer.TestSubmissionId);
        if(testSubmission != null)
        {
            testSubmission.TotalPointsEarned = testSubmission.Answers.Sum(a => a.PointsAwarded);
        }
        await _context.SaveChangesAsync();

        var response = _mapper.Map<SubmissionAnswerResponse>(submissionAnswer);
        return ServiceResult<SubmissionAnswerResponse>.SuccessResult(response, "Answer graded.");
    }

    private static QUESTION_TYPE MapQuestionType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return QUESTION_TYPE.MultipleChoice;
        }

        return type.Trim().ToUpperInvariant() switch
        {
            "SINGLE_CHOICE" => QUESTION_TYPE.MultipleChoice,
            "MULTIPLE_CHOICE" => QUESTION_TYPE.MultipleChoice,
            "TRUE_FALSE" => QUESTION_TYPE.TrueFalse,
            "SHORT_ANSWER" => QUESTION_TYPE.ShortAnswer,
            "ESSAY" => QUESTION_TYPE.Essay,
            "CODING" => QUESTION_TYPE.Coding,
            _ => QUESTION_TYPE.MultipleChoice
        };
    }
    
}