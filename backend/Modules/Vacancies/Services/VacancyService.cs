
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Models.Recruitment;
using TalentBridge.Modules.Vacancies.DTOs.Requests;
using TalentBridge.Modules.Vacancies.DTOs.Responses;
using System.Linq.Expressions;
using TalentBridge.Enums.Recruitment;
using TalentBridge.Enums.Testing;
using TalentBridge.Models.Roles;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Applications.DTOs.Responses;

namespace TalentBridge.Modules.Vacancies.Services;

public class VacancyService : IVacancyService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public VacancyService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<VacancyDetails>> CreateVacancyAsync(CreateVacancyRequest dto, int userId)
    {
        var hrManager = await _context.HrManagers.FirstOrDefaultAsync(hr => hr.UserId == userId && hr.OrganizationId == dto.OrganizationId);
        if (hrManager == null)
            return ServiceResult<VacancyDetails>.FailureResult("User is not authorized to create a vacancy for this organization.");

        var vacancy = _mapper.Map<Vacancy>(dto);
        vacancy.CreatedByHRManagerId = hrManager.Id;

        if (vacancy.Status == VACANCY_STATUS.Active)
            vacancy.PublishedAt = DateTime.UtcNow;

        await _context.Vacancies.AddAsync(vacancy);
        await _context.SaveChangesAsync();

        var vacancyDetails = _mapper.Map<VacancyDetails>(vacancy);
        return ServiceResult<VacancyDetails>.SuccessResult(vacancyDetails, "Vacancy created successfully.");
    }

    public async Task<ServiceResult<PaginatedResult<VacancyList>>> GetVacanciesAsync(VacancyFilterRequest request)
    {
        var query = _context.Vacancies.AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(v => v.Title.Contains(request.Search) || v.Description.Contains(request.Search));
        if (request.OrganizationId.HasValue)
            query = query.Where(v => v.OrganizationId == request.OrganizationId.Value);
        if (!string.IsNullOrEmpty(request.Profession))
            query = query.Where(v => v.Profession == request.Profession);
        if (request.EmploymentType.HasValue)
            query = query.Where(v => v.EmploymentType == request.EmploymentType.Value);
        if (request.ExperienceLevel.HasValue)
            query = query.Where(v => v.ExperienceLevel == request.ExperienceLevel.Value);
        if (request.IsRemote.HasValue)
            query = query.Where(v => v.IsRemote == request.IsRemote.Value);
        if (request.SalaryMin.HasValue)
            query = query.Where(v => v.SalaryMin >= request.SalaryMin.Value);
        if (request.SalaryMax.HasValue)
            query = query.Where(v => v.SalaryMax <= request.SalaryMax.Value);

        var totalCount = await query.CountAsync();
        
        var vacancies = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).Where(v => v.IsDeleted == false).ToListAsync();
        
        var vacancyList = _mapper.Map<List<VacancyList>>(vacancies);

        var paginatedResult = new PaginatedResult<VacancyList>
        {
            Items = vacancyList,
            TotalItems = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
        return ServiceResult<PaginatedResult<VacancyList>>.SuccessResult(paginatedResult);
    }

    public async Task<ServiceResult<VacancyDetails>> GetVacancyByIdAsync(int id, int? userId = null)
    {
        var vacancy = await _context.Vacancies
            .Include(v => v.Organization)
            .Include(v => v.CreatedByHRManager)
            .ThenInclude(hr => hr.User)
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

        if (vacancy == null)
            return ServiceResult<VacancyDetails>.FailureResult("Vacancy not found.");

        var vacancyDetails = _mapper.Map<VacancyDetails>(vacancy);
        return ServiceResult<VacancyDetails>.SuccessResult(vacancyDetails);
    }

    public async Task<ServiceResult<VacancyDetails>> UpdateVacancyAsync(int id, UpdateVacancyRequest dto, int userId)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
            return ServiceResult<VacancyDetails>.FailureResult("Vacancy not found.");

        var hrManager = await _context.HrManagers.FirstOrDefaultAsync(hr => hr.UserId == userId && hr.OrganizationId == vacancy.OrganizationId);
        if (hrManager == null)
            return ServiceResult<VacancyDetails>.FailureResult("User is not authorized to update this vacancy.");

        _mapper.Map(dto, vacancy);
        await _context.SaveChangesAsync();

        var vacancyDetails = _mapper.Map<VacancyDetails>(vacancy);
        return ServiceResult<VacancyDetails>.SuccessResult(vacancyDetails, "Vacancy updated successfully.");
    }

    public async Task<ServiceResult<string>> DeleteVacancyAsync(int id, int userId)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy == null || vacancy.IsDeleted)
            return ServiceResult<string>.FailureResult("Vacancy not found.");

        var hrManager = await _context.HrManagers.FirstOrDefaultAsync(hr => hr.UserId == userId && hr.OrganizationId == vacancy.OrganizationId);
        if (hrManager == null)
            return ServiceResult<string>.FailureResult("User is not authorized to delete this vacancy.");

        vacancy.IsDeleted = true;
        vacancy.Status = VACANCY_STATUS.Closed;
        vacancy.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<string>.SuccessResult(null, "Vacancy deleted successfully.");
    }

    public async Task<ServiceResult<VacancyAnalytics>> GetVacancyAnalyticsAsync(int id, int userId)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy == null)
            return ServiceResult<VacancyAnalytics>.FailureResult("Vacancy not found.");

        var hrManager = await _context.HrManagers.FirstOrDefaultAsync(hr => hr.UserId == userId && hr.OrganizationId == vacancy.OrganizationId);
        if (hrManager == null)
            return ServiceResult<VacancyAnalytics>.FailureResult("User is not authorized to view analytics for this vacancy.");

        var analytics = new VacancyAnalytics
        {
            VacancyId = vacancy.Id,
            VacancyTitle = vacancy.Title,
            TotalViews = vacancy.ViewCount,
            TotalApplications = await _context.Applications.CountAsync(a => a.VacancyId == id),
            TestsCompleted = 0,
            Hired = 0,
            ViewToApplicationRate = 0,
            ApplicationToTestRate = 0,

            AverageTestScore = 0,
            AverageTimeToHire = 0,
            Timeline = new List<VacancyTimelineDto>()
        };

        return ServiceResult<VacancyAnalytics>.SuccessResult(analytics);
    }
    public async Task <List<VacancyLookUp>> GetVacanciesByOrganizationAsync(int OrganizationId){
        return await _context.Vacancies.Where(v=> v.OrganizationId == OrganizationId).Select(v => new VacancyLookUp{
            Id = v.Id,
            Title = v.Title
        }).ToListAsync();
    }

    public async Task<ServiceResult<ApplicationResponse>> ApplyAsync(int vacancyId, int userId)
    {
        var vacancy = await _context.Vacancies.Include(v => v.Test)
            .FirstOrDefaultAsync(v => v.Id == vacancyId && v.Status == VACANCY_STATUS.Active && !v.IsDeleted);

        if (vacancy == null)
            return ServiceResult<ApplicationResponse>.FailureResult("Vacancy not found or is not active.");

        var alreadyApplied = await _context.Applications.AnyAsync(a => a.VacancyId == vacancyId && a.UserId == userId);
        if (alreadyApplied)
            return ServiceResult<ApplicationResponse>.FailureResult("You have already applied for this vacancy.");

        var application = new Application
        {
            VacancyId = vacancyId,
            UserId = userId,
            AppliedAt = DateTime.UtcNow,
            Status = vacancy.TestId.HasValue ? APPLICATION_STATUS.TestAssigned : APPLICATION_STATUS.UnderReview,
        };
        _context.Applications.Add(application);

        if (vacancy.TestId.HasValue)
        {
            var testAssignment = new TestAssignment
            {
                Application = application, 
                TestId = vacancy.TestId.Value,
                AssignedBy = userId,
                AssignedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), 
                Status = TEST_ASSIGNMENT_STATUS.Assigned,
                AccessToken = Guid.NewGuid().ToString() 
            };
            _context.TestAssignments.Add(testAssignment);
        }

        await _context.SaveChangesAsync();

        var applicationResponse = _mapper.Map<ApplicationResponse>(application);
        return ServiceResult<ApplicationResponse>.SuccessResult(applicationResponse, "Successfully applied for the vacancy.");
    }

    public async Task<ServiceResult<VacancyDetails>> AssignTestToVacancyAsync(int vacancyId, int testId, int userId)
    {
        var vacancy = await _context.Vacancies
            .Include(v => v.Applications) 
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
            
        if (vacancy == null)
            return ServiceResult<VacancyDetails>.FailureResult("Vacancy not found.");

        var test = await _context.Tests.FindAsync(testId);
        if (test == null)
            return ServiceResult<VacancyDetails>.FailureResult("Test not found.");
            
        var hrManager = await _context.HrManagers.FirstOrDefaultAsync(hr => hr.UserId == userId && hr.OrganizationId == vacancy.OrganizationId);
        if (hrManager == null)
            return ServiceResult<VacancyDetails>.FailureResult("User is not authorized to assign a test to this vacancy.");

        if (vacancy.OrganizationId != test.OrganizationId)
            return ServiceResult<VacancyDetails>.FailureResult("Vacancy and test do not belong to the same organization.");
        
        if (vacancy.TestId.HasValue && vacancy.TestId != testId)
        {
            return ServiceResult<VacancyDetails>.FailureResult("A different test is already assigned. Please remove it first.");
        }

        vacancy.TestId = testId;

        var applicationsToUpdate = vacancy.Applications
            .Where(app => app.Status == APPLICATION_STATUS.UnderReview || app.Status == APPLICATION_STATUS.Submitted)
            .ToList();

        foreach (var app in applicationsToUpdate)
        {
            var existingAssignment = await _context.TestAssignments.AnyAsync(ta => ta.ApplicationId == app.Id);
            if (!existingAssignment)
            {
                app.Status = APPLICATION_STATUS.TestAssigned;
                var testAssignment = new TestAssignment
                {
                    ApplicationId = app.Id,
                    TestId = testId,
                    AssignedBy = userId,
                    AssignedByHRManagerId = hrManager.Id,
                    AssignedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    Status = TEST_ASSIGNMENT_STATUS.Assigned,
                    AccessToken = Guid.NewGuid().ToString()
                };
                _context.TestAssignments.Add(testAssignment);
            }
        }

        await _context.SaveChangesAsync();

        var vacancyDetails = _mapper.Map<VacancyDetails>(vacancy);
        return ServiceResult<VacancyDetails>.SuccessResult(vacancyDetails, "Test assigned successfully.");
    }
}
