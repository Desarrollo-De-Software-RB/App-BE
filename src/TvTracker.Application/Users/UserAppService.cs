using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using TvTracker.Permissions;

namespace TvTracker.Users;

[Authorize]
public class UserAppService : TvTrackerAppService, IUserAppService
{
    private readonly IRepository<IdentityUser, Guid> _userRepository;
    private readonly IdentityUserManager _userManager;

    public UserAppService(IRepository<IdentityUser, Guid> userRepository, IdentityUserManager userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task<List<UserDto>> GetListAsync()
    {
        var users = await _userRepository.GetListAsync();
        
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            ProfilePicture = u.GetProperty<string>("ProfilePicture")
        }).ToList();
    }

    [Authorize(TvTrackerPermissions.AdminOptions)]
    public async Task<UserFullDto> GetAsync(Guid id)
    {
        var user = await _userManager.GetByIdAsync(id);
        
        return new UserFullDto
        {
            Id = user.Id,
            UserName = user.UserName,
            ProfilePicture = user.GetProperty<string>("ProfilePicture"),
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CreationTime = user.CreationTime
        };
    }

    [Authorize(TvTrackerPermissions.AdminOptions)]
    public async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        var user = new IdentityUser(
            GuidGenerator.Create(),
            input.UserName,
            input.Email,
            CurrentTenant.Id
        );

        user.Name = input.Name;
        user.Surname = input.Surname;
        user.SetPhoneNumber(input.PhoneNumber, false);
        user.SetIsActive(input.IsActive);
        
        if (!string.IsNullOrEmpty(input.ProfilePicture))
        {
            user.SetProperty("ProfilePicture", input.ProfilePicture);
        }

        CheckErrors(await _userManager.CreateAsync(user, input.Password));

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            ProfilePicture = user.GetProperty<string>("ProfilePicture")
        };
    }

    [Authorize(TvTrackerPermissions.AdminOptions)]
    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input)
    {
        var user = await _userManager.GetByIdAsync(id);

        CheckErrors(await _userManager.SetUserNameAsync(user, input.UserName));
        CheckErrors(await _userManager.SetEmailAsync(user, input.Email));
        user.Name = input.Name;
        user.Surname = input.Surname;
        CheckErrors(await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber));
        user.SetIsActive(input.IsActive);

        user.SetProperty("ProfilePicture", input.ProfilePicture);

        CheckErrors(await _userManager.UpdateAsync(user));

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            ProfilePicture = user.GetProperty<string>("ProfilePicture")
        };
    }

    [Authorize(TvTrackerPermissions.AdminOptions)]
    public async Task DeleteAsync(Guid id)
    {
        var user = await _userManager.GetByIdAsync(id);
        CheckErrors(await _userManager.DeleteAsync(user));
    }

    private static void CheckErrors(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new Volo.Abp.UserFriendlyException(result.Errors.First().Description);
        }
    }
}
