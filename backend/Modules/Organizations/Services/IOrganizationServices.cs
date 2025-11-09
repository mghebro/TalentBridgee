using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Modules.Organizations.DTOs.Requests;
using TalentBridge.Modules.Organizations.DTOs.Responses;

namespace TalentBridge.Modules.Organizations.Services;

public interface IOrganizationServices
{
    Task<ServiceResult<OrganizationDetails>> CreateOrganizationAsync(CreateOrganizationRequest dto,IFormFile logoFile, int createdByUserId);
    Task<ServiceResult<PaginatedResult<OrganizationList>>> GetOrganizationsAsync(OrganizationFilterRequest request);
    Task<ServiceResult<OrganizationDetails>> GetOrganizationByIdAsync(int id);
    Task<ServiceResult<OrganizationDetails>> UpdateOrganizationAsync(int id, UpdateOrganizationRequest dto,IFormFile logoFile, int userId);
    Task<ServiceResult<string>> DeleteOrganizationAsync(int id, int userId);
    Task<ServiceResult<List<OrganizationList>>> GetOrganizationsForCurrentUserAsync(int userId);
}