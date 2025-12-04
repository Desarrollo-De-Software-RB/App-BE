using System;
using Volo.Abp.Application.Dtos;

namespace TvTracker.Users;

public class UserDto : EntityDto<Guid>
{
    public string UserName { get; set; }
    public string? ProfilePicture { get; set; }
}
