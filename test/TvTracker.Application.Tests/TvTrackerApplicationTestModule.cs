using Volo.Abp.Modularity;

namespace TvTracker;

[DependsOn(
    typeof(TvTrackerApplicationModule),
    typeof(TvTrackerDomainTestModule)
)]
public class TvTrackerApplicationTestModule : AbpModule
{

}
