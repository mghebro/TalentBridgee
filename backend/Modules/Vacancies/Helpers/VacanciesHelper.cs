using AutoMapper;
using TalentBridge.Models;
using TalentBridge.Models.Recruitment;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Vacancies.DTOs.Requests;
using TalentBridge.Modules.Vacancies.DTOs.Responses;

namespace TalentBridge.Modules.Vacancies.Helpers;

public class VacanciesHelper : Profile
{
     public VacanciesHelper()
     {
          CreateMap<CreateVacancyRequest, Vacancy>().ReverseMap();
          CreateMap<UpdateVacancyRequest, Vacancy>().ReverseMap();
          CreateMap<Vacancy, VacancyList>().ReverseMap();
          CreateMap<Vacancy, VacancyDetails>().ReverseMap();
          CreateMap<Test, VacancyTestDto>().ReverseMap();
     }
}