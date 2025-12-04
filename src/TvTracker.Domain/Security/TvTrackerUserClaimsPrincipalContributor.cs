using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Identity;
using Volo.Abp.Data;
using OpenIddict.Abstractions;

namespace TvTracker.Security;

public class TvTrackerUserClaimsPrincipalContributor : IAbpClaimsPrincipalContributor, ITransientDependency
{
    private readonly IdentityUserManager _identityUserManager;

    public TvTrackerUserClaimsPrincipalContributor(IdentityUserManager identityUserManager)
    {
        _identityUserManager = identityUserManager;
    }

    public async Task ContributeAsync(AbpClaimsPrincipalContributorContext context)
    {
        var identity = context.ClaimsPrincipal.Identities.FirstOrDefault();
        if (identity == null)
        {
            return;
        }

        var userIdClaim = context.ClaimsPrincipal.FindFirst(AbpClaimTypes.UserId);
        var userId = userIdClaim != null ? (Guid?)Guid.Parse(userIdClaim.Value) : null;
        if (userId == null)
        {
            return;
        }

        var user = await _identityUserManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            return;
        }

        var profilePicture = user.GetProperty<string>("ProfilePicture");
        if (!string.IsNullOrEmpty(profilePicture))
        {
            var claim = new Claim("picture", profilePicture);
            claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
            identity.AddClaim(claim);
        }
    }
}
