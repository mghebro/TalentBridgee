using AutoMapper;
using TalentBridge.Models.Roles;
using TalentBridge.Models.Roles;
using TalentBridge.Modules.Organizations.DTOs.Requests;
using TalentBridge.Modules.Organizations.DTOs.Responses;

namespace TalentBridge.Modules.Organizations;

public class OrganizationMapper : AutoMapper.Profile
{
    public OrganizationMapper()
    {
        CreateMap<Organization, OrganizationList>();
        CreateMap<Organization, OrganizationDetails>();
        CreateMap<Organization, OrganizationStatistics>();
        CreateMap<CreateOrganizationRequest, Organization>();
        CreateMap<UpdateOrganizationRequest, Organization>();
        
    
    }
}