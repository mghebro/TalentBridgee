using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Models.Recruitment;
using TalentBridge.Enums;
using TalentBridge.Enums.Recruitment;
using TalentBridge.Models;
using TalentBridge.Modules.Applications.DTOs.Requests;
using TalentBridge.Modules.Applications.DTOs.Responses;

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
        var vacancy = await _context.Vacancies.FindAsync(request.VacancyId);
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

        var application = new Application
        {
            VacancyId = request.VacancyId,
            UserId = userId,
            CoverLetter = request.CoverLetter,
            AppliedAt = DateTime.UtcNow,
            Status = APPLICATION_STATUS.UnderReview 
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<ApplicationResponse>(application);

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
}
