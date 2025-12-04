using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TvTracker.Admin;

public interface IAdminAppService : IApplicationService
{
    Task<string> GetDeveloperOptionsAsync();
}
