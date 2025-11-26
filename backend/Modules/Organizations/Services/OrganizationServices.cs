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
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadsFolder;

    public OrganizationServices(
        IMapper mapper, 
        DataContext context,
        IWebHostEnvironment environment
     ) : base(context)
    {
        _mapper = mapper;
        _environment = environment;
        _uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "organizations");
        Directory.CreateDirectory(_uploadsFolder);
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

        Organization organization = _mapper.Map<Organization>(dto);
        
        

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
            .Take(request.PageSize).Where(o => o.IsActive == true)
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
    var organization = await _context.Organizations
        .FirstOrDefaultAsync(o => o.Id == organizationId);

    if (organization == null)
        return null;

    var vacancies = await _context.Vacancies
        .Where(v => v.OrganizationId == organizationId)
        .ToListAsync();

    var applications = await _context.Applications
        .Where(a => vacancies.Select(v => v.Id).Contains(a.VacancyId))
        .ToListAsync();

   
    var stats = new OrganizationStatistics
    {
        TotalVacancies = vacancies.Count,
        ActiveVacancies = vacancies.Count(v => v.Status == VACANCY_STATUS.Active),
        ClosedVacancies = vacancies.Count(v => v.Status == VACANCY_STATUS.Closed),
        TotalApplications = applications.Count,
        PendingApplications = applications.Count(a => a.Status == APPLICATION_STATUS.Submitted),
        ReviewedApplications = applications.Count(a => a.Status == APPLICATION_STATUS.UnderReview),
        TotalHires = applications.Count(a => a.Status == APPLICATION_STATUS.Hired),
        LastVacancyPosted = vacancies
            .OrderByDescending(v => v.CreatedAt)
            .Select(v => (DateTime?)v.CreatedAt)
            .FirstOrDefault(),
        LastApplicationReceived = applications
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => (DateTime?)a.CreatedAt)
            .FirstOrDefault()
    };

    var organizationDetails = _mapper.Map<OrganizationDetails>(organization);
    organizationDetails.Statistics = stats;

    var hiredApplications = applications
        .Where(a => a.Status == APPLICATION_STATUS.Hired && a.HiredAt.HasValue)
        .ToList();

    if (hiredApplications.Any())
    {
        organizationDetails.Statistics.AverageTimeToHire = (decimal)hiredApplications
            .Average(a => (a.HiredAt!.Value - a.AppliedAt).TotalDays);
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

    public async Task<ServiceResult<string>> UploadOrganizationLogoAsync(int organizationId, IFormFile file, int userId)
    {
        var organization = await _context.Organizations.FindAsync(organizationId);
        if (organization == null)
            return ServiceResult<string>.FailureResult("Organization not found");

        if (!await UserCanManageOrganizationAsync(userId, organizationId))
            return ServiceResult<string>.FailureResult("You don't have permission to update this organization");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return ServiceResult<string>.FailureResult("Only JPG, PNG, WebP, and SVG images are allowed");

        const int maxSize = 2 * 1024 * 1024; 
        if (file.Length > maxSize)
            return ServiceResult<string>.FailureResult("Image size must be less than 2MB");

        if (!string.IsNullOrEmpty(organization.LogoUrl))
        {
            var oldPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), 
                organization.LogoUrl.TrimStart('/'));
            if (File.Exists(oldPath))
                File.Delete(oldPath);
        }

        var fileName = $"org_{organizationId}_{DateTime.UtcNow.Ticks}{extension}";
        var filePath = Path.Combine(_uploadsFolder, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/organizations/{fileName}";
        organization.LogoUrl = relativePath;
        organization.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ServiceResult<string>.SuccessResult(relativePath, "Organization logo uploaded successfully");
    }

    public async Task<ServiceResult<bool>> DeleteOrganizationLogoAsync(int organizationId, int userId)
    {
        var organization = await _context.Organizations.FindAsync(organizationId);
        if (organization == null)
            return ServiceResult<bool>.FailureResult("Organization not found");

        if (!await UserCanManageOrganizationAsync(userId, organizationId))
            return ServiceResult<bool>.FailureResult("You don't have permission to update this organization");

        if (string.IsNullOrEmpty(organization.LogoUrl))
            return ServiceResult<bool>.FailureResult("No logo found to delete");

        var filePath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), 
            organization.LogoUrl.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        organization.LogoUrl = null;
        organization.Logo = null;
        organization.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true, "Organization logo deleted successfully");
    }
}