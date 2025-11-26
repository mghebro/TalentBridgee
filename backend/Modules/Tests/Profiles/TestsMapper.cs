using AutoMapper;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.DTOs.Responses;

namespace TalentBridge.Modules.Tests.Profiles;

public class TestsMapper : AutoMapper.Profile
{
    public TestsMapper()
    {
        CreateMap<CreateTestRequest, Test>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Questions, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Test, TestDetailsResponse>();

        CreateMap<CreateQuestionRequest, Question>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Test, opt => opt.Ignore())
            .ForMember(dest => dest.Options, opt => opt.Ignore());

        CreateMap<Question, QuestionResponse>();

        CreateMap<CreateQuestionOptionRequest, QuestionOption>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OptionText, opt => opt.Ignore());

        CreateMap<QuestionOption, QuestionOptionResponse>();

        CreateMap<TestSubmission, TestSubmissionResponse>();

        CreateMap<SubmissionAnswer, SubmissionAnswerResponse>();
    
        CreateMap<TestAssignment, TestAssignmentResponse>();
    }
}