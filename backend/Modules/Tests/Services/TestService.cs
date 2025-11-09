using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Common.Services.CurrentUser;
using TalentBridge.Data;
using TalentBridge.Enums.Auth;
using TalentBridge.Enums.Testing;
using TalentBridge.Models;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.DTOs.Responses;

namespace TalentBridge.Modules.Tests.Services;

public class TestService : BaseService, ITestService
{
    private readonly IMapper _mapper;

    public TestService(DataContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<ServiceResult<TestDetailsResponse>> CreateTestAsync(CreateTestRequest request, int userId)
    {
        var organization = await _context.Organizations.FindAsync(request.OrganizationId);
        if (organization == null)
        {
            return ServiceResult<TestDetailsResponse>.FailureResult("Organization not found.");
        }

        if (!await UserCanManageOrganizationAsync(userId, request.OrganizationId))
        {
            return ServiceResult<TestDetailsResponse>.FailureResult("You don't have permission to create tests for this organization.");
        }

        var test = _mapper.Map<Test>(request);
        test.CreatedBy = userId;
        test.CreatedAt = DateTime.UtcNow;
        test.TotalPoints = 0; 

        _context.Tests.Add(test);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<TestDetailsResponse>(test);
        response.OrganizationName = organization.Name;

        return ServiceResult<TestDetailsResponse>.SuccessResult(response, "Test created successfully.");
    }

    public async Task<ServiceResult<QuestionResponse>> AddQuestionToTestAsync(int testId, CreateQuestionRequest request, int userId)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
        {
            return ServiceResult<QuestionResponse>.FailureResult("Test not found.");
        }

        if (!await UserCanManageOrganizationAsync(userId, test.OrganizationId))
        {
            return ServiceResult<QuestionResponse>.FailureResult("You don't have permission to add questions to this test.");
        }

        var question = _mapper.Map<Question>(request);
        question.TestId = testId;
        question.OrderNumber = test.Questions.Any() ? test.Questions.Max(q => q.OrderNumber) + 1 : 1;

        foreach (var optionRequest in request.Options)
        {
            question.Options.Add(_mapper.Map<QuestionOption>(optionRequest));
        }

        _context.Questions.Add(question);
        test.TotalPoints += question.Points;
        await _context.SaveChangesAsync();

        var response = _mapper.Map<QuestionResponse>(question);
        return ServiceResult<QuestionResponse>.SuccessResult(response, "Question added successfully.");
    }

    public async Task<ServiceResult<TestSubmissionResponse>> StartTestSubmissionAsync(int testAssignmentId, int userId)
    {
        var assignment = await _context.TestAssignments
            .Include(ta => ta.Test)
            .Include(ta => ta.Application)
            .FirstOrDefaultAsync(ta => ta.Id == testAssignmentId && ta.Application.UserId == userId);

        if (assignment == null)
        {
            return ServiceResult<TestSubmissionResponse>.FailureResult("Test assignment not found or not assigned to you.");
        }

        if (assignment.Status != TEST_ASSIGNMENT_STATUS.Assigned)
        {
            return ServiceResult<TestSubmissionResponse>.FailureResult("Test has already been started or completed.");
        }

        var submission = new TestSubmission
        {
            TestAssignmentId = testAssignmentId,
            UserId = userId,
            TestId = assignment.TestId,
            StartTime = DateTime.UtcNow,
        };

        _context.TestSubmissions.Add(submission);
        assignment.Status = TEST_ASSIGNMENT_STATUS.InProgress;
        await _context.SaveChangesAsync();

        var response = _mapper.Map<TestSubmissionResponse>(submission);
        return ServiceResult<TestSubmissionResponse>.SuccessResult(response, "Test started successfully.");
    }

    public async Task<ServiceResult<TestSubmissionResponse>> SubmitTestSubmissionAsync(int testSubmissionId, SubmitTestRequest request, int userId)
    {
        var submission = await _context.TestSubmissions
            .Include(s => s.Test)
                .ThenInclude(t => t.Questions)
                    .ThenInclude(q => q.Options)
            .Include(s => s.TestAssignment)
            .FirstOrDefaultAsync(s => s.Id == testSubmissionId && s.UserId == userId);

        if (submission == null)
        {
            return ServiceResult<TestSubmissionResponse>.FailureResult("Test submission not found or not assigned to you.");
        }

        if (submission.TestAssignment.Status != TEST_ASSIGNMENT_STATUS.Assigned)
        {
            return ServiceResult<TestSubmissionResponse>.FailureResult("Test is not in progress or already submitted.");
        }

        submission.EndTime = DateTime.UtcNow;
        submission.SubmittedAt = DateTime.UtcNow;
        submission.TestAssignment.Status = TEST_ASSIGNMENT_STATUS.Completed;

        decimal totalPointsEarned = 0;
        foreach (var answerRequest in request.Answers)
        {
            var question = submission.Test.Questions.FirstOrDefault(q => q.Id == answerRequest.QuestionId);
            if (question == null) continue;

            var submissionAnswer = new SubmissionAnswer
            {
                TestSubmissionId = testSubmissionId,
                QuestionId = question.Id,
            };

            if (question.QuestionType == QUESTION_TYPE.MultipleChoice)
            {
                var selectedOption = question.Options.FirstOrDefault(o => o.Id == answerRequest.SelectedOptionId);
                if (selectedOption != null && selectedOption.IsCorrect)
                {
                    submissionAnswer.PointsAwarded = question.Points;
                    submissionAnswer.IsCorrect = true;
                    totalPointsEarned += question.Points;
                }
                else
                {
                    submissionAnswer.PointsAwarded = 0;
                    submissionAnswer.IsCorrect = false;
                }
                submissionAnswer.SelectedOptionId = answerRequest.SelectedOptionId;
            }
            else if (question.QuestionType == QUESTION_TYPE.TrueFalse)
            {
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                if (correctOption != null && bool.TryParse(answerRequest.AnswerText, out bool userAnswer) && userAnswer == bool.Parse(correctOption.OptionText))
                {
                    submissionAnswer.PointsAwarded = question.Points;
                    submissionAnswer.IsCorrect = true;
                    totalPointsEarned += question.Points;
                }
                else
                {
                    submissionAnswer.PointsAwarded = 0;
                    submissionAnswer.IsCorrect = false;
                }
                submissionAnswer.AnswerText = answerRequest.AnswerText;
            }
            else
            {
                // For other types, just save the answer text for manual review
                submissionAnswer.AnswerText = answerRequest.AnswerText;
                submissionAnswer.PointsAwarded = 0; // Manual grading needed
                submissionAnswer.IsCorrect = false;
            }
            _context.SubmissionAnswers.Add(submissionAnswer);
        }

        submission.TotalPointsEarned = totalPointsEarned;
        submission.PercentageScore = (totalPointsEarned / submission.Test.TotalPoints) * 100;
        submission.IsPassed = submission.PercentageScore >= submission.Test.PassingScore;

        await _context.SaveChangesAsync();

        var response = _mapper.Map<TestSubmissionResponse>(submission);
        response.Answers = _mapper.Map<List<SubmissionAnswerResponse>>(submission.Answers);

        return ServiceResult<TestSubmissionResponse>.SuccessResult(response, "Test submitted successfully.");
    }

    public async Task<ServiceResult<TestDetailsResponse>> GetTestByIdAsync(int testId)
    {
        var test = await _context.Tests
            .Include(t => t.Organization)
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
        {
            return ServiceResult<TestDetailsResponse>.FailureResult("Test not found.");
        }

        var response = _mapper.Map<TestDetailsResponse>(test);
        response.OrganizationName = test.Organization.Name;

        return ServiceResult<TestDetailsResponse>.SuccessResult(response);
    }

    public async Task<ServiceResult<TestSubmissionResponse>> GetTestSubmissionResultAsync(int testSubmissionId, int userId)
    {
        var submission = await _context.TestSubmissions
            .Include(s => s.Test)
                .ThenInclude(t => t.Questions)
                    .ThenInclude(q => q.Options)
            .Include(s => s.Answers)
            .FirstOrDefaultAsync(s => s.Id == testSubmissionId && s.UserId == userId);

        if (submission == null)
        {
            return ServiceResult<TestSubmissionResponse>.FailureResult("Test submission not found or not accessible.");
        }

        var response = _mapper.Map<TestSubmissionResponse>(submission);
        response.Answers = _mapper.Map<List<SubmissionAnswerResponse>>(submission.Answers);

        return ServiceResult<TestSubmissionResponse>.SuccessResult(response);
    }

    public async Task<ServiceResult<PaginatedResult<TestListResponse>>> GetTestsAsync(TestFilterRequest request)
    {
        var query = _context.Tests
            .Include(t => t.Organization)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(t =>
                t.Title.Contains(request.Search) ||
                t.Description.Contains(request.Search) ||
                t.Profession.Contains(request.Search)
            );
        }

        if (request.OrganizationId.HasValue)
        {
            query = query.Where(t => t.OrganizationId == request.OrganizationId.Value);
        }

        if (request.Difficulty.HasValue)
        {
            query = query.Where(t => t.Difficulty == request.Difficulty.Value);
        }

        var totalItems = await query.CountAsync();

        query = request.SortBy?.ToLower() switch
        {
            "title" => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
            "createdat" => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        var tests = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TestListResponse
            {
                Id = t.Id,
                Title = t.Title,
                OrganizationName = t.Organization.Name,
                Profession = t.Profession,
                Difficulty = t.Difficulty,
                DurationMinutes = t.DurationMinutes,
                TotalPoints = t.TotalPoints,
                QuestionCount = t.Questions.Count()
            })
            .ToListAsync();

        var paginatedResult = new PaginatedResult<TestListResponse>
        {
            Items = tests,
            TotalItems = totalItems,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ServiceResult<PaginatedResult<TestListResponse>>.SuccessResult(paginatedResult);
    }

    public async Task<ServiceResult<TestAssignmentResponse>> AssignTestAsync(CreateTestAssignmentRequest request, int userId)
    {
        var application = await _context.Applications
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId);

        if (application == null)
        {
            return ServiceResult<TestAssignmentResponse>.FailureResult("Application not found.");
        }

        var test = await _context.Tests.FindAsync(request.TestId);
        if (test == null)
        {
            return ServiceResult<TestAssignmentResponse>.FailureResult("Test not found.");
        }

        if (!await UserCanManageOrganizationAsync(userId, application.Vacancy.OrganizationId))
        {
            return ServiceResult<TestAssignmentResponse>.FailureResult("You don't have permission to assign tests for this application.");
        }

        var existingAssignment = await _context.TestAssignments
            .AnyAsync(ta => ta.ApplicationId == request.ApplicationId && ta.TestId == request.TestId);
        if (existingAssignment)
        {
            return ServiceResult<TestAssignmentResponse>.FailureResult("This test has already been assigned to this application.");
        }

        var assignment = new TestAssignment
        {
            ApplicationId = request.ApplicationId,
            TestId = request.TestId,
            AssignedBy = userId, 
            AssignedAt = DateTime.UtcNow,
            Status = TEST_ASSIGNMENT_STATUS.Assigned
        };

        _context.TestAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<TestAssignmentResponse>(assignment);
        response.TestTitle = test.Title;
                response.UserName = (await _context.Users.FindAsync(application.UserId))?.FirstName + " " + (await _context.Users.FindAsync(application.UserId))?.LastName;
        
                return ServiceResult<TestAssignmentResponse>.SuccessResult(response, "Test assigned successfully.");
            }
        
            public async Task<ServiceResult<SubmissionAnswerResponse>> GradeSubmissionAnswerAsync(int submissionAnswerId, GradeAnswerRequest request, int hrManagerUserId)
            {
                var submissionAnswer = await _context.SubmissionAnswers
                    .Include(sa => sa.TestSubmission)
                        .ThenInclude(ts => ts.Test)
                            .ThenInclude(t => t.Organization)
                    .Include(sa => sa.Question)
                    .FirstOrDefaultAsync(sa => sa.Id == submissionAnswerId);
        
                if (submissionAnswer == null)
                {
                    return ServiceResult<SubmissionAnswerResponse>.FailureResult("Submission answer not found.");
                }
        
                if (!await UserCanManageOrganizationAsync(hrManagerUserId, submissionAnswer.TestSubmission.Test.OrganizationId))
                {
                    return ServiceResult<SubmissionAnswerResponse>.FailureResult("You don't have permission to grade this answer.");
                }
        
                if (submissionAnswer.Question.QuestionType == QUESTION_TYPE.MultipleChoice || submissionAnswer.Question.QuestionType == QUESTION_TYPE.TrueFalse)
                {
                    return ServiceResult<SubmissionAnswerResponse>.FailureResult("This question type is auto-graded.");
                }
        
                submissionAnswer.PointsAwarded = request.PointsAwarded;
                submissionAnswer.GraderComments = request.GraderComments;
                submissionAnswer.IsCorrect = request.PointsAwarded >= submissionAnswer.Question.Points; 
        
                await _context.SaveChangesAsync();
        
                // Recalculate total score for the submission
                var testSubmission = await _context.TestSubmissions
                    .Include(ts => ts.Answers)
                    .FirstOrDefaultAsync(ts => ts.Id == submissionAnswer.TestSubmissionId);
        
                if (testSubmission != null)
                {
                    testSubmission.TotalPointsEarned = testSubmission.Answers.Sum(a => a.PointsAwarded);
                    testSubmission.PercentageScore = (testSubmission.TotalPointsEarned / testSubmission.Test.TotalPoints) * 100;
                    testSubmission.IsPassed = testSubmission.PercentageScore >= testSubmission.Test.PassingScore;
                    await _context.SaveChangesAsync();
                }
        
                var response = _mapper.Map<SubmissionAnswerResponse>(submissionAnswer);
                return ServiceResult<SubmissionAnswerResponse>.SuccessResult(response, "Answer graded successfully.");
            }
        }