using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace TvTracker.Users;

public class UpdateUserDto
{
    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
    public string UserName { get; set; }

    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength))]
    public string Name { get; set; }

    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxSurnameLength))]
    public string Surname { get; set; }

    [Required]
    [EmailAddress]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    public string Email { get; set; }

    public string? PhoneNumber { get; set; }
    
    public bool IsActive { get; set; }

    public string? ProfilePicture { get; set; }
}
