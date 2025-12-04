using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TvTracker.Users;

public interface IUserAppService : IApplicationService
{
    Task<List<UserDto>> GetListAsync();
    Task<UserFullDto> GetAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserDto input);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input);
    Task DeleteAsync(Guid id);
}
