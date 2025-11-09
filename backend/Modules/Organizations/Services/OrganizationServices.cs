using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Common.Services;
using TalentBridge.Common.Services.CurrentUser;
using TalentBridge.Data;
using TalentBridge.Enums;
using TalentBridge.Enums.OrganizationTypes;
using TalentBridge.Models.Roles;
using TalentBridge.Modules.Organizations.DTOs.Requests;
using TalentBridge.Modules.Organizations.DTOs.Responses;
using TalentBridge.Enums.Testing;
using TalentBridge.Enums.Recruitment;

namespace TalentBridge.Modules.Organizations.Services;


public class OrganizationServices : BaseService, IOrganizationServices
{
    private readonly IMapper _mapper;
    private readonly IFileService _fileService; 

    public OrganizationServices(
        IMapper mapper, 
        DataContext context,
        IFileService fileService
     ) : base(context)
    {
        _mapper = mapper;
        _fileService = fileService;
    }

    
    public async Task<ServiceResult<OrganizationDetails>> CreateOrganizationAsync(
        CreateOrganizationRequest dto, 
        IFormFile logoFile, 
        int createdByUserId)
    {
        var user = await _context.Users.FindAsync(createdByUserId);
        if (user == null)
            return ServiceResult<OrganizationDetails>.FailureResult("User not found");

        var existingOrg = await _context.Organizations
            .AnyAsync(o => o.Name.ToLower() == dto.Name.ToLower());

        if (existingOrg)
            return ServiceResult<OrganizationDetails>.FailureResult("Organization with this name already exists");

        Organization organization = dto.Type switch
        {
            TYPES.BUSINESS_COMPANY => _mapper.Map<BusinessOrganization>(dto),
            TYPES.EDUCATION => _mapper.Map<EducationOrganization>(dto),
            TYPES.HEALTHCARE => _mapper.Map<HealthcareOrganization>(dto),
            TYPES.NON_GOV => _mapper.Map<NGOOrganization>(dto),
            TYPES.GOV => _mapper.Map<GOVOrganization>(dto),
            TYPES.OTHERS_ASSOCIATIONS => _mapper.Map<OtherOrganization>(dto),
            _ => throw new InvalidOperationException("Unknown organization type")
        };

        if (logoFile != null)
        {
            var logoResult = await ValidateAndProcessLogoAsync(logoFile);
            if (!logoResult.Success)
                return ServiceResult<OrganizationDetails>.FailureResult(logoResult.Errors);
            
            organization.Logo = logoResult.Data;
        }

        organization.UserId = createdByUserId;
        organization.IsActive = true;

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        var organizationDetails = await GetOrganizationDetailsWithStats(organization.Id);


        return ServiceResult<OrganizationDetails>.SuccessResult(
            organizationDetails,
            "Organization created successfully"
        );
    }


    public async Task<ServiceResult<PaginatedResult<OrganizationList>>> GetOrganizationsAsync(
        OrganizationFilterRequest request)
    {
        var query = _context.Organizations.AsQueryable();

    
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(o => 
                o.Name.ToLower().Contains(searchTerm) || 
                o.Description.ToLower().Contains(searchTerm) ||
                o.Address.ToLower().Contains(searchTerm)
            );
        }

        if (request.Type.HasValue)
        {
            query = query.Where(o => o.Type == request.Type.Value);
        }
        

        if (request.IsActive.HasValue)
        {
            query = query.Where(o => o.IsActive == request.IsActive.Value);
        }

   
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder?.ToLower() == "asc" 
                ? query.OrderBy(o => o.Name) 
                : query.OrderByDescending(o => o.Name),
            "createddate" => request.SortOrder?.ToLower() == "asc" 
                ? query.OrderBy(o => o.CreatedAt) 
                : query.OrderByDescending(o => o.CreatedAt),
            _ => query.OrderByDescending(o => o.CreatedAt)
        };

        var totalItems = await query.CountAsync();

    
        var organizationDtos = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<OrganizationList>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var paginatedResult = new PaginatedResult<OrganizationList>
        {
            Items = organizationDtos,
            TotalItems = totalItems,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ServiceResult<PaginatedResult<OrganizationList>>.SuccessResult(paginatedResult);
    }
       
    public async Task<ServiceResult<OrganizationDetails>> GetOrganizationByIdAsync(int id)
    {
        var organizationDetails = await GetOrganizationDetailsWithStats(id);

        if (organizationDetails == null)
        {
            return ServiceResult<OrganizationDetails>.FailureResult("Organization not found");
        }

        return ServiceResult<OrganizationDetails>.SuccessResult(organizationDetails);
    }
    
    public async Task<ServiceResult<OrganizationDetails>> UpdateOrganizationAsync(
        int id, 
        UpdateOrganizationRequest dto, 
        IFormFile? logoFile, 
        int userId)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == id);

        if (organization == null)
            return ServiceResult<OrganizationDetails>.FailureResult("Organization not found");

        if (!await UserCanManageOrganizationAsync(userId, id))
            return ServiceResult<OrganizationDetails>.FailureResult("You don't have permission to update this organization");

        if (!string.Equals(dto.Name, organization.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await _context.Organizations.AnyAsync(o => o.Name.ToLower() == dto.Name.ToLower() && o.Id != id))
            {
                return ServiceResult<OrganizationDetails>.FailureResult("Organization with this name already exists");
            }
        }

        if (logoFile != null)
        {
            var logoResult = await ValidateAndProcessLogoAsync(logoFile);
            if (!logoResult.Success)
                return ServiceResult<OrganizationDetails>.FailureResult(logoResult.Errors);
            
            organization.Logo = logoResult.Data;
        }
        else if (dto.DeleteLogo)
        {
            organization.Logo = null;
        }

        _mapper.Map(dto, organization);
        organization.UpdatedAt = DateTime.UtcNow;

        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync();

        var organizationDetails = await GetOrganizationDetailsWithStats(id);

        return ServiceResult<OrganizationDetails>.SuccessResult(
            organizationDetails,
            "Organization updated successfully"
        );
    }
    
    public async Task<ServiceResult<string>> DeleteOrganizationAsync(int id, int userId)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == id);

        if (organization == null)
            return ServiceResult<string>.FailureResult("Organization not found");
        
        if (!await UserCanManageOrganizationAsync(userId, id))
            return ServiceResult<string>.FailureResult("You don't have permission to delete this organization");

        if (await _context.Vacancies.AnyAsync(v => v.OrganizationId == id && v.Status == VACANCY_STATUS.Active))
            return ServiceResult<string>.FailureResult("Cannot delete organization with active vacancies");

        organization.IsActive = false;
        organization.IsDeleted = true;
        organization.DeletedAt = DateTime.UtcNow;

        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync();

        return ServiceResult<string>.SuccessResult("Organization deleted successfully", "Organization has been successfully deleted");
    }

    public async Task<ServiceResult<List<OrganizationList>>> GetOrganizationsForCurrentUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return ServiceResult<List<OrganizationList>>.FailureResult("User not found");

        var organizations = await _context.Organizations
            .Include(o => o.HRManagers)
            .Where(o => o.HRManagers.Any(hr => hr.UserId == userId))
            .ProjectTo<OrganizationList>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return ServiceResult<List<OrganizationList>>.SuccessResult(organizations);
    }

    private async Task<bool> UserCanManageOrganizationAsync(int userId, int organizationId)
    {
        var organization = await _context.Organizations
            .AsNoTracking()
            .Include(o => o.HRManagers)
            .FirstOrDefaultAsync(o => o.Id == organizationId);

        if (organization == null)
        {
            return false;
        }

        return organization.UserId == userId || 
               organization.HRManagers.Any(hr => hr.UserId == userId);
    }


    private async Task<OrganizationDetails?> GetOrganizationDetailsWithStats(int organizationId)
    {
        var details = await _context.Organizations
            .Where(o => o.Id == organizationId)
            .Select(o => new 
            {
                Organization = o, 
                Stats = new OrganizationStatistics
                {
                    TotalVacancies = o.Vacancies.Count(),
                    ActiveVacancies = o.Vacancies.Count(v => v.Status == VACANCY_STATUS.Active),
                    ClosedVacancies = o.Vacancies.Count(v => v.Status == VACANCY_STATUS.Closed),
                    TotalApplications = o.Vacancies.SelectMany(v => v.Applications).Count(),
                    PendingApplications = o.Vacancies.SelectMany(v => v.Applications).Count(a => a.Status == APPLICATION_STATUS.Submitted),
                    ReviewedApplications = o.Vacancies.SelectMany(v => v.Applications).Count(a => a.Status == APPLICATION_STATUS.UnderReview),
                    TotalHires = o.Vacancies.SelectMany(v => v.Applications).Count(a => a.Status == APPLICATION_STATUS.Hired),
                    TotalTests = o.Vacancies.SelectMany(v => v.Tests).Count(),
                    AverageTestScore = o.Vacancies
                        .SelectMany(v => v.Tests)
                        .SelectMany(t => _context.TestAssignments.Where(ta => ta.TestId == t.Id))
                        .Where(ts => ts.Status == TEST_ASSIGNMENT_STATUS.Completed)
                        .Select(ts => _context.TestSubmissions.FirstOrDefault(s => s.TestAssignmentId == ts.Id))
                        .Where(s => s != null && s.PercentageScore.HasValue)
                        .Average(s => (decimal?)s.PercentageScore.Value) ?? 0, 
                    LastVacancyPosted = o.Vacancies.OrderByDescending(v => v.CreatedAt).Select(v => (DateTime?)v.CreatedAt).FirstOrDefault(),
                    LastApplicationReceived = o.Vacancies.SelectMany(a => a.Applications).OrderByDescending(a => a.CreatedAt).Select(a => (DateTime?)a.CreatedAt).FirstOrDefault()
                }
            })
            .FirstOrDefaultAsync();

        if (details == null) return null;

        var organizationDetails = _mapper.Map<OrganizationDetails>(details.Organization);
        organizationDetails.Statistics = details.Stats;

        var hiredApplications = await _context.Applications
            .Where(a => a.Vacancy.OrganizationId == organizationId && a.Status == APPLICATION_STATUS.Hired && a.HiredAt.HasValue)
            .Select(a => new { a.AppliedAt, a.HiredAt })
            .ToListAsync();

        if (hiredApplications.Any())
        {
            organizationDetails.Statistics.AverageTimeToHire = (decimal)hiredApplications.Average(a => (a.HiredAt.Value - a.AppliedAt).TotalDays);
        }

        return organizationDetails;
    }



  
    private async Task<ServiceResult<byte[]>> ValidateAndProcessLogoAsync(IFormFile logoFile)
    {
        const int maxFileSize = 2 * 1024 * 1024; 
        if (logoFile.Length > maxFileSize)
            return ServiceResult<byte[]>.FailureResult("Logo file is too large (max 2MB)");

        var ext = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
        if (!allowedExtensions.Contains(ext))
            return ServiceResult<byte[]>.FailureResult("Invalid logo file type. Allowed types: .jpg, .jpeg, .png, .svg");
        
        using var memoryStream = new MemoryStream();
        await logoFile.CopyToAsync(memoryStream);
        return ServiceResult<byte[]>.SuccessResult(memoryStream.ToArray());
    }
    

}