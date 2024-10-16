using Volo.Abp.Modularity;

namespace TvTracker;

[DependsOn(
    typeof(TvTrackerDomainModule),
    typeof(TvTrackerTestBaseModule)
)]
public class TvTrackerDomainTestModule : AbpModule
{

}
