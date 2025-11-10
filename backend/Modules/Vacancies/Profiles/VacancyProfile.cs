
using AutoMapper;
using TalentBridge.Models.Recruitment;
using TalentBridge.Modules.Vacancies.DTOs.Requests;
using TalentBridge.Modules.Vacancies.DTOs.Responses;

namespace TalentBridge.Modules.Vacancies.Profiles;

public class VacancyProfile : Profile
{
    public VacancyProfile()
    {
        CreateMap<CreateVacancyRequest, Vacancy>();
        CreateMap<UpdateVacancyRequest, Vacancy>();

        CreateMap<Vacancy, VacancyDetails>()
            .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization.Name))
            .ForMember(dest => dest.OrganizationLogo, opt => opt.MapFrom(src => src.Organization.Logo))
            .ForMember(dest => dest.OrganizationWebsite, opt => opt.MapFrom(src => src.Organization.Website))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByHRManager.UserId))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByHRManager.User.FirstName));

        CreateMap<Vacancy, VacancyList>()
            .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization.Name))
            .ForMember(dest => dest.OrganizationLogo, opt => opt.MapFrom(src => src.Organization.Logo))
            .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications.Count));
    }
}
