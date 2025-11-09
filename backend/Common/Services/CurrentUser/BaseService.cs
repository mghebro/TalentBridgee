using Microsoft.EntityFrameworkCore;
using TalentBridge.Data;
using TalentBridge.Enums.Auth;

namespace TalentBridge.Common.Services.CurrentUser;

public class BaseService
{
    protected readonly DataContext _context;

    public BaseService(DataContext context)
    {
        _context = context;
    }

    protected async Task<bool> UserCanManageOrganizationAsync(int userId, int organizationId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user?.Role == ROLES.ORGANIZATION_ADMIN)
        {
            return true;
        }

        var isOrgHR = await _context.HrManagers
            .AnyAsync(hr => hr.UserId == userId && 
                hr.OrganizationId == organizationId && 
                hr.IsActive);
        return isOrgHR;
    }
}