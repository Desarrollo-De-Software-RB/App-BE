using System;

namespace TvTracker.Users;

public class UserFullDto : UserDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
}
