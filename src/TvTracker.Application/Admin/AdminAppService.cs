using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TvTracker.Permissions;

namespace TvTracker.Admin;

[Authorize(TvTrackerPermissions.AdminOptions)]
public class AdminAppService : TvTrackerAppService, IAdminAppService
{
    public Task<string> GetDeveloperOptionsAsync()
    {
        return Task.FromResult("Welcome to Developer Options!");
    }
}
