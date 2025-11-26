
using AutoMapper;
using TalentBridge.Models.Recruitment;
using TalentBridge.Modules.Vacancies.DTOs.Requests;
using TalentBridge.Modules.Vacancies.DTOs.Responses;

namespace TalentBridge.Modules.Vacancies.Profiles;

public class VacancyProfile : AutoMapper.Profile
{
    public VacancyProfile()
    {
        CreateMap<CreateVacancyRequest, Vacancy>();
        CreateMap<UpdateVacancyRequest, Vacancy>();

        CreateMap<Vacancy, VacancyDetails>()
            .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization != null ? src.Organization.Name : string.Empty))
            .ForMember(dest => dest.OrganizationLogo, opt => opt.MapFrom(src => src.Organization != null ? src.Organization.LogoUrl : null))
            .ForMember(dest => dest.OrganizationWebsite, opt => opt.MapFrom(src => src.Organization != null ? src.Organization.Website : null))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByHRManager != null ? src.CreatedByHRManager.UserId : 0))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByHRManager != null && src.CreatedByHRManager.User != null ? src.CreatedByHRManager.User.FirstName : null));

        CreateMap<Vacancy, VacancyList>()
            .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization != null ? src.Organization.Name : string.Empty))
            .ForMember(dest => dest.OrganizationLogo, opt => opt.MapFrom(src => src.Organization != null ? src.Organization.LogoUrl : null))
            .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications != null ? src.Applications.Count : 0));
    }
}
