using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace TvTracker.Account;

public class AppAccountAppService : TvTrackerAppService, IAppAccountAppService
{
    private readonly IdentityUserManager _userManager;

    public AppAccountAppService(IdentityUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityUserDto> RegisterAsync(AppRegisterDto input)
    {
        var user = new IdentityUser(
            GuidGenerator.Create(),
            input.UserName,
            input.EmailAddress,
            CurrentTenant.Id
        );

        user.Name = input.Name;
        user.Surname = input.Surname;
        
        // Set extra property
        user.SetProperty("ProfilePicture", input.ProfilePicture);

        (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    public async Task<IdentityUserDto> UpdateProfileAsync(AppUpdateProfileDto input)
    {
        if (CurrentUser.Id == null)
        {
             throw new UnauthorizedAccessException();
        }
        var user = await _userManager.GetByIdAsync(CurrentUser.Id.Value);

        (await _userManager.SetUserNameAsync(user, input.UserName)).CheckErrors();
        (await _userManager.SetEmailAsync(user, input.Email)).CheckErrors();
        user.Name = input.Name;
        user.Surname = input.Surname;
        (await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
        
        user.SetProperty("ProfilePicture", input.ProfilePicture);

        (await _userManager.UpdateAsync(user)).CheckErrors();

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }
}
