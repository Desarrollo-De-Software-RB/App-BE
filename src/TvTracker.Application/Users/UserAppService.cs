using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
}
