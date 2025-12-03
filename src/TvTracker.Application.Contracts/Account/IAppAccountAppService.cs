using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace TvTracker.Account;

public interface IAppAccountAppService : IApplicationService
{
    Task<IdentityUserDto> RegisterAsync(AppRegisterDto input);
    Task<IdentityUserDto> UpdateProfileAsync(AppUpdateProfileDto input);
}
