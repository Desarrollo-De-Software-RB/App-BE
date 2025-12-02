using TvTracker.Localization;
using Volo.Abp.Application.Services;

namespace TvTracker;

/* Inherit your application services from this class.
 */
public abstract class TvTrackerAppService : ApplicationService
{
    protected TvTrackerAppService()
    {
        LocalizationResource = typeof(TvTrackerResource);
    }
}
