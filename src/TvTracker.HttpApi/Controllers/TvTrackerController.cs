using TvTracker.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace TvTracker.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TvTrackerController : AbpControllerBase
{
    protected TvTrackerController()
    {
        LocalizationResource = typeof(TvTrackerResource);
    }
}
