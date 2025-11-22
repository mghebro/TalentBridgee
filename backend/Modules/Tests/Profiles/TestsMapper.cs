using AutoMapper;
using TalentBridge.Models.Testing;
using TalentBridge.Modules.Tests.DTOs.Requests;
using TalentBridge.Modules.Tests.DTOs.Responses;

namespace TalentBridge.Modules.Tests.Profiles;

public class TestsMapper : Profile
{
    public TestsMapper()
    {
        // --- TEST ---
        CreateMap<CreateTestRequest, Test>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Questions, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Test, TestDetailsResponse>();

        // --- QUESTION ---
        CreateMap<CreateQuestionRequest, Question>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Test, opt => opt.Ignore())
            .ForMember(dest => dest.Options, opt => opt.Ignore());

        CreateMap<Question, QuestionResponse>();

        // --- QUESTION OPTION ---
        CreateMap<CreateQuestionOptionRequest, QuestionOption>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OptionText, opt => opt.Ignore());

        CreateMap<QuestionOption, QuestionOptionResponse>();

        // --- TEST SUBMISSION ---
        CreateMap<TestSubmission, TestSubmissionResponse>();

        // --- SUBMISSION ANSWER ---
        CreateMap<SubmissionAnswer, SubmissionAnswerResponse>();

        // --- TEST ASSIGNMENT ---
        CreateMap<TestAssignment, TestAssignmentResponse>();
    }
}