using TalentBridge.Modules.Auth.DTOs.Responses;
using TalentBridge.Modules.Auth.DTOs.Requests;
using TalentBridge.Models.Roles;
using TalentBridge.Models.Auth;
using AutoMapper;

namespace TalentBridge.Modules.Auth.Helpers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<EmailVerification, User>().ReverseMap();
        CreateMap<PasswordVerification, User>().ReverseMap();
        CreateMap<LoginRequest, User>().ReverseMap();
        CreateMap<RegisterRequest, User>().ReverseMap();
        CreateMap<UpdateUserRequest , User>().ReverseMap();

       

        CreateMap<User, LoginResponse>().ReverseMap(); 



    }
}
