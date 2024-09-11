using Microsoft.Extensions.Localization;
using TvTracker.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace TvTracker;

[Dependency(ReplaceServices = true)]
public class TvTrackerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<TvTrackerResource> _localizer;

    public TvTrackerBrandingProvider(IStringLocalizer<TvTrackerResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
