
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Data;
using TalentBridge.Models.Recruitment;
using TalentBridge.Modules.Vacancies.DTOs.Requests;
using TalentBridge.Modules.Vacancies.DTOs.Responses;
using System.Linq.Expressions;
using TalentBridge.Enums.Recruitment;
using TalentBridge.Models.Roles;

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

        // This is a placeholder implementation. A real implementation would involve more complex queries.
        var analytics = new VacancyAnalytics
        {
            VacancyId = vacancy.Id,
            VacancyTitle = vacancy.Title,
            TotalViews = vacancy.ViewCount,
            TotalApplications = await _context.Applications.CountAsync(a => a.VacancyId == id),
            // Placeholder values for other properties
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
}
