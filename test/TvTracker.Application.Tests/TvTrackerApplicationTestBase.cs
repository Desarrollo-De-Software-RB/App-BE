using Volo.Abp.Modularity;

namespace TvTracker;

public abstract class TvTrackerApplicationTestBase<TStartupModule> : TvTrackerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
