using TalentBridge.Common.DTOs.Responses;
using TalentBridge.Modules.Applications.DTOs.Responses;
using TalentBridge.Modules.Vacancies.DTOs.Requests;
using TalentBridge.Modules.Vacancies.DTOs.Responses;

namespace TalentBridge.Modules.Vacancies.Services;

public interface IVacancyService
{
  
    Task<ServiceResult<VacancyDetails>> CreateVacancyAsync(CreateVacancyRequest dto, int userId);
    
  
    Task<ServiceResult<PaginatedResult<VacancyList>>> GetVacanciesAsync(VacancyFilterRequest request);
    
   
    Task<ServiceResult<VacancyDetails>> GetVacancyByIdAsync(int id, int? userId = null);
    
 
    Task<ServiceResult<VacancyDetails>> UpdateVacancyAsync(int id, UpdateVacancyRequest dto, int userId);
    
   
    Task<ServiceResult<string>> DeleteVacancyAsync(int id, int userId);
    Task<ServiceResult<VacancyAnalytics>> GetVacancyAnalyticsAsync(int id, int userId);
    Task<List<VacancyLookUp>> GetVacanciesByOrganizationAsync(int OrganizationId);

    Task<ServiceResult<ApplicationResponse>> ApplyAsync(int vacancyId, int userId);
    Task<ServiceResult<VacancyDetails>> AssignTestToVacancyAsync(int vacancyId, int testId, int userId);
    Task<ServiceResult<List<VacancyList>>> GetVacanciesForCurrentUserAsync(int userId);
}