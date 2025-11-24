using AutoMapper;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Tests.DTOs.Responses;

namespace TalentBridge.Modules.Tests.Profiles
{
    public class TestApplicationProfile : Profile
    {
        public TestApplicationProfile()
        {
            CreateMap<Test, TestForApplicationResponse>();
            CreateMap<Question, QuestionForUserResponse>();
            CreateMap<QuestionOption, QuestionOptionForUserResponse>();
        }
    }
}
