using TalentBridge.Enums.OrganizationTypes;

namespace TalentBridge.Modules.Organizations.DTOs.Requests;

public class OrganizationFilterRequest
{
        public string? Search { get; set; }         
        public TYPES? Type { get; set; }           
        public string? Location { get; set; }       
        public bool? IsActive { get; set; }         
        
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        
        public string? SortBy { get; set; } = "CreatedDate";
        public string? SortOrder { get; set; } = "desc";
}