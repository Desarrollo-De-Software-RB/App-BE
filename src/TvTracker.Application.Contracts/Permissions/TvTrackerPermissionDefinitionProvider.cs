using TvTracker.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace TvTracker.Permissions;

public class TvTrackerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TvTrackerPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(TvTrackerPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TvTrackerResource>(name);
    }
}
