using AutoMapper;
using TalentBridge.Enums.Testing;
using TalentBridge.Models;
using TalentBridge.Models.Recruitment;
using TalentBridge.Modules.Applications.DTOs.Responses;

namespace TalentBridge.Modules.Applications.Profiles;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<Application, ApplicationResponse>()
            .ForMember(dest => dest.VacancyTitle, opt => opt.MapFrom(src => src.Vacancy.Title))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.TestAssignmentId, opt => opt.MapFrom(src => src.TestAssignment != null ? (int?)src.TestAssignment.Id : null))
            .ForMember(dest => dest.TestAssignmentStatus, opt => opt.MapFrom(src => src.TestAssignment != null ? (TEST_ASSIGNMENT_STATUS?)src.TestAssignment.Status : null));
    }
}
