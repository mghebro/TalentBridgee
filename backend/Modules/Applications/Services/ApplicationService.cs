using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Models.Recruitment;
using TalentBridge.Enums;
using TalentBridge.Enums.Recruitment;
using TalentBridge.Enums.Testing;
using TalentBridge.Models;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Applications.DTOs.Requests;
using TalentBridge.Modules.Applications.DTOs.Responses;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.DTOs.Responses;

namespace TalentBridge.Modules.Applications.Services;

public class ApplicationService : IApplicationService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<ApplicationResponse>> CreateApplicationAsync(CreateApplicationRequest request, int userId)
    {
        var vacancy = await _context.Vacancies
            .Include(v => v.Test)
            .FirstOrDefaultAsync(v => v.Id == request.VacancyId);
        if (vacancy == null)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Vacancy not found."
            };
        }

        var existingApplication = await _context.Applications
            .AnyAsync(a => a.UserId == userId && a.VacancyId == request.VacancyId);
        if (existingApplication)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status409Conflict,
                Message = "You have already applied for this vacancy."
            };
        }

        var initialStatus = vacancy.TestId.HasValue 
            ? APPLICATION_STATUS.TestAssigned 
            : APPLICATION_STATUS.UnderReview;

        var application = new Application
        {
            VacancyId = request.VacancyId,
            UserId = userId,
            CoverLetter = request.CoverLetter,
            AppliedAt = DateTime.UtcNow,
            Status = initialStatus
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        if (vacancy.TestId.HasValue)
        {
            var test = await _context.Tests.FindAsync(vacancy.TestId.Value);
            if (test != null)
            {
                var existingAssignment = await _context.TestAssignments
                    .AnyAsync(ta => ta.ApplicationId == application.Id && ta.TestId == test.Id);
                
                if (!existingAssignment)
                {
                    var testAssignment = new TestAssignment
                    {
                        ApplicationId = application.Id,
                        TestId = test.Id,
                        AssignedBy = 0, 
                        AssignedByHRManagerId = vacancy.CreatedByHRManagerId,
                        AssignedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(7), 
                        Status = TEST_ASSIGNMENT_STATUS.Assigned
                    };

                    _context.TestAssignments.Add(testAssignment);
                    await _context.SaveChangesAsync();
                }
            }
        }

        var applicationWithTest = await _context.Applications
            .Include(a => a.TestAssignment)
            .FirstOrDefaultAsync(a => a.Id == application.Id);

        var response = _mapper.Map<ApplicationResponse>(applicationWithTest ?? application);

        return new ApiResponse<ApplicationResponse>
        {
            Status = StatusCodes.Status201Created,
            Message = "Application created successfully.",
            Data = response
        };
    }

    public async Task<ApiResponse<List<ApplicationResponse>>> GetApplicationsByUserIdAsync(int userId)
    {
        var applications = await _context.Applications
            .Where(a => a.UserId == userId)
            .Include(a => a.Vacancy)
            .Include(a => a.User)
            .Include(a => a.TestAssignment)
            .ToListAsync();
        var response = applications.Select(a => _mapper.Map<ApplicationResponse>(a)).ToList();

        return new ApiResponse<List<ApplicationResponse>>
        {
            Status = StatusCodes.Status200OK,
            Message = "Applications retrieved successfully.",
            Data = response
        };
    }

    public async Task<ApiResponse<List<ApplicationResponse>>> GetApplicationsByVacancyIdAsync(int vacancyId, int hrManagerUserId)
    {
        var hrManager = await _context.HrManagers
            .Include(hrm => hrm.Organization)
            .FirstOrDefaultAsync(hrm => hrm.UserId == hrManagerUserId);

        if (hrManager == null)
        {
            return new ApiResponse<List<ApplicationResponse>>
            {
                Status = StatusCodes.Status403Forbidden,
                Message = "HR Manager not found or unauthorized."
            };
        }

        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == vacancyId && v.OrganizationId == hrManager.OrganizationId);

        if (vacancy == null)
        {
            return new ApiResponse<List<ApplicationResponse>>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Vacancy not found or you do not have permission to view its applications."
            };
        }

        var applications = await _context.Applications
            .Where(a => a.VacancyId == vacancyId)
            .Include(a => a.User)
            .Include(a => a.Vacancy)
            .ToListAsync();

        var response = applications.Select(a => _mapper.Map<ApplicationResponse>(a)).ToList();

        return new ApiResponse<List<ApplicationResponse>>
        {
            Status = StatusCodes.Status200OK,
            Message = "Applications for vacancy retrieved successfully.",
            Data = response
        };
    }

    public async Task<ApiResponse<ApplicationResponse>> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusRequest request, int hrManagerUserId)
    {
        var application = await _context.Applications
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Application not found."
            };
        }

        var hrManager = await _context.HrManagers
            .FirstOrDefaultAsync(hrm => hrm.UserId == hrManagerUserId && hrm.OrganizationId == application.Vacancy.OrganizationId);

        if (hrManager == null)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status403Forbidden,
                Message = "You do not have permission to update this application's status."
            };
        }

        application.Status = request.Status;

        var timelineEntry = new ApplicationTimeline
        {
            ApplicationId = applicationId,
            Status = request.Status,
            ChangedAt = DateTime.UtcNow,
            Notes = $"Status changed by HR Manager (ID: {hrManagerUserId})"
        };
        _context.ApplicationTimelines.Add(timelineEntry);

        await _context.SaveChangesAsync();

        var response = _mapper.Map<ApplicationResponse>(application);

        return new ApiResponse<ApplicationResponse>
        {
            Status = StatusCodes.Status200OK,
            Message = "Application status updated successfully.",
            Data = response
        };
    }

    public async Task<ApiResponse<ApplicationResponse>> AddReviewNoteAsync(int applicationId, AddReviewNoteRequest request, int hrManagerUserId)
    {
        var application = await _context.Applications
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Application not found."
            };
        }

        var hrManager = await _context.HrManagers
            .FirstOrDefaultAsync(hrm => hrm.UserId == hrManagerUserId && hrm.OrganizationId == application.Vacancy.OrganizationId);

        if (hrManager == null)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status403Forbidden,
                Message = "You do not have permission to add a review note to this application."
            };
        }

        application.ReviewNotes = request.Note;

        var timelineEntry = new ApplicationTimeline
        {
            ApplicationId = applicationId,
            Status = application.Status,
            ChangedAt = DateTime.UtcNow,
            Notes = $"Review note added by HR Manager (ID: {hrManagerUserId}): {request.Note}"
        };
        _context.ApplicationTimelines.Add(timelineEntry);

        await _context.SaveChangesAsync();

        var response = _mapper.Map<ApplicationResponse>(application);

        return new ApiResponse<ApplicationResponse>
        {
            Status = StatusCodes.Status200OK,
            Message = "Review note added successfully.",
            Data = response
        };
    }

    public async Task<ApiResponse<ApplicationResponse>> GetApplicationByIdAsync(int applicationId, int userId)
    {
        var application = await _context.Applications
            .Include(a => a.Vacancy)
            .Include(a => a.User)
            .Include(a => a.Timelines)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Application not found."
            };
        }

        var isOwner = application.UserId == userId;
        
        var isHrManager = await _context.HrManagers.AnyAsync(hrm => hrm.UserId == userId && hrm.OrganizationId == application.Vacancy.OrganizationId);

        if (!isOwner && !isHrManager)
        {
            return new ApiResponse<ApplicationResponse>
            {
                Status = StatusCodes.Status403Forbidden,
                Message = "You do not have permission to view this application."
            };
        }

        var response = _mapper.Map<ApplicationResponse>(application);

        return new ApiResponse<ApplicationResponse>
        {
            Status = StatusCodes.Status200OK,
            Message = "Application retrieved successfully.",
            Data = response
        };
    }

    public async Task<ApiResponse<TestForApplicationResponse>> GetTestForApplicationAsync(int applicationId, int userId)
    {
        var application = await _context.Applications
            .Include(a => a.TestAssignment)
                .ThenInclude(ta => ta.Test)
                    .ThenInclude(t => t.Questions)
                        .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
            return new ApiResponse<TestForApplicationResponse> { Status = 404, Message = "Application not found." };

        if (application.UserId != userId)
            return new ApiResponse<TestForApplicationResponse> { Status = 403, Message = "You are not authorized to access this test." };

        if (application.TestAssignment == null || application.TestAssignment.Test == null)
            return new ApiResponse<TestForApplicationResponse> { Status = 404, Message = "Test not found for this application." };

        var submission = await _context.TestSubmissions
            .FirstOrDefaultAsync(ts => ts.TestAssignmentId == application.TestAssignment.Id && ts.UserId == userId);

        switch (application.TestAssignment.Status)
        {
            case TEST_ASSIGNMENT_STATUS.Assigned:
                if (submission == null)
                {
                    submission = new TestSubmission
                    {
                        TestAssignmentId = application.TestAssignment.Id,
                        TestId = application.TestAssignment.TestId,
                        UserId = userId,
                        StartTime = DateTime.UtcNow,
                    };
                    _context.TestSubmissions.Add(submission);
                }
                
                application.TestAssignment.Status = TEST_ASSIGNMENT_STATUS.InProgress;
                application.TestAssignment.StartedAt = DateTime.UtcNow;
                application.Status = APPLICATION_STATUS.TestInProgress; 
                await _context.SaveChangesAsync();
                break;
            case TEST_ASSIGNMENT_STATUS.InProgress:
                if (submission == null)
                {
                    submission = new TestSubmission
                    {
                        TestAssignmentId = application.TestAssignment.Id,
                        TestId = application.TestAssignment.TestId,
                        UserId = userId,
                        StartTime = application.TestAssignment.StartedAt ?? DateTime.UtcNow,
                    };
                    _context.TestSubmissions.Add(submission);
                    await _context.SaveChangesAsync();
                }
                break;
            default:
                return new ApiResponse<TestForApplicationResponse> { Status = 400, Message = "Test cannot be started or resumed in its current state." };
        }

        var testResponse = _mapper.Map<TestForApplicationResponse>(application.TestAssignment.Test);
        testResponse.AssignmentId = application.TestAssignment.Id;
        testResponse.SubmissionId = submission?.Id;
        return new ApiResponse<TestForApplicationResponse> { Status = 200, Data = testResponse };
    }

    public async Task<ApiResponse<TestSubmissionResponse>> SubmitTestAsync(int applicationId, int userId, SubmitTestRequest request)
    {
        var application = await _context.Applications
            .Include(a => a.TestAssignment)
                .ThenInclude(ta => ta.Test)
                    .ThenInclude(t => t.Questions)
                        .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
            return new ApiResponse<TestSubmissionResponse> { Status = 404, Message = "Application not found." };

        if (application.UserId != userId)
            return new ApiResponse<TestSubmissionResponse> { Status = 403, Message = "You are not authorized to submit this test." };

        if (application.TestAssignment == null || application.TestAssignment.Test == null)
            return new ApiResponse<TestSubmissionResponse> { Status = 404, Message = "Test not found for this application." };

        if (application.TestAssignment.Status != TEST_ASSIGNMENT_STATUS.InProgress)
            return new ApiResponse<TestSubmissionResponse> { Status = 400, Message = "Test is not in progress or has already been submitted." };

        var test = application.TestAssignment.Test;
        
        var submission = await _context.TestSubmissions
            .FirstOrDefaultAsync(ts => ts.TestAssignmentId == application.TestAssignment.Id && ts.UserId == userId);
        
        if (submission == null)
        {
            var startTime = application.TestAssignment.StartedAt ?? DateTime.UtcNow;
            
            submission = new TestSubmission
            {
                TestAssignmentId = application.TestAssignment.Id,
                TestId = test.Id,
                UserId = userId,
                StartTime = startTime,
                SubmittedAt = DateTime.UtcNow,
                EndTime = DateTime.UtcNow,
            };
            _context.TestSubmissions.Add(submission);
        }
        else
        {
            submission.SubmittedAt = DateTime.UtcNow;
            submission.EndTime = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync(); 

        var answeredQuestionIds = request.Answers.Select(a => a.QuestionId).ToHashSet();
        var allQuestionIds = test.Questions?.Select(q => q.Id).ToHashSet() ?? new HashSet<int>();
        
        var duplicateAnswers = request.Answers
            .GroupBy(a => a.QuestionId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        
        if (duplicateAnswers.Any())
        {
            return new ApiResponse<TestSubmissionResponse> 
            { 
                Status = 400, 
                Message = $"Duplicate answers found for questions: {string.Join(", ", duplicateAnswers)}" 
            };
        }

        decimal totalScore = 0;
        foreach (var answerRequest in request.Answers)
        {
            var question = test.Questions?.FirstOrDefault(q => q.Id == answerRequest.QuestionId);
            if (question == null) continue;

            bool isCorrect = false;
            decimal pointsAwarded = 0;
            
            if (question.QuestionType == QUESTION_TYPE.MultipleChoice)
            {
                if (question.Options != null && question.Options.Any() && answerRequest.SelectedOptionIds != null && answerRequest.SelectedOptionIds.Any())
                {
                    var correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToHashSet();
                    var submittedOptionIds = answerRequest.SelectedOptionIds.ToHashSet();
                    
                    if (correctOptionIds.Any())
                    {
                        isCorrect = correctOptionIds.SetEquals(submittedOptionIds);
                        if (isCorrect)
                        {
                            pointsAwarded = question.Points;
                            totalScore += question.Points;
                        }
                    }
                }
            }
            else if (question.QuestionType == QUESTION_TYPE.TrueFalse)
            {
                if (question.Options != null && question.Options.Any() && !string.IsNullOrEmpty(answerRequest.AnswerText))
                {
                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                    if (correctOption != null && bool.TryParse(answerRequest.AnswerText, out bool userAnswer))
                    {
                        if (bool.TryParse(correctOption.OptionText, out bool correctAnswer) && userAnswer == correctAnswer)
                        {
                            isCorrect = true;
                            pointsAwarded = question.Points;
                            totalScore += question.Points;
                        }
                    }
                }
            }
            else
            {
                isCorrect = false;
                pointsAwarded = 0;
            }

            var submissionAnswer = new SubmissionAnswer
            {
                TestSubmissionId = submission.Id,
                QuestionId = question.Id,
                AnswerText = answerRequest.AnswerText ?? string.Empty,
                IsCorrect = isCorrect,
                PointsAwarded = pointsAwarded,
                AnsweredAt = DateTime.UtcNow,
                TimeSpentSeconds = 0 
            };
            
            if (answerRequest.SelectedOptionIds != null && answerRequest.SelectedOptionIds.Any())
            {
                submissionAnswer.AnswerText = string.Join(",", answerRequest.SelectedOptionIds);
            }
            
            _context.SubmissionAnswers.Add(submissionAnswer);
        }

        submission.TotalPointsEarned = totalScore;
        
        decimal percentageScore = 0;
        if (test.TotalPoints > 0)
        {
            percentageScore = (totalScore / test.TotalPoints) * 100;
        }
        submission.PercentageScore = percentageScore;
        
        bool isPassed = percentageScore >= test.PassingScore;
        submission.IsPassed = isPassed;
        
        application.TestAssignment.Status = TEST_ASSIGNMENT_STATUS.Completed;
        application.TestAssignment.CompletedAt = DateTime.UtcNow;
        application.Status = isPassed ? APPLICATION_STATUS.TestPassed : APPLICATION_STATUS.TestFailed;

        await _context.SaveChangesAsync();

        var submissionResponse = _mapper.Map<TestSubmissionResponse>(submission);
        return new ApiResponse<TestSubmissionResponse> { Status = 200, Data = submissionResponse, Message = "Test submitted successfully." };
    }
}
