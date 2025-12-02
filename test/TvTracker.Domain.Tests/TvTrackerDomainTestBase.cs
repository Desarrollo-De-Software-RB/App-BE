using Volo.Abp.Modularity;

namespace TvTracker;

/* Inherit from this class for your domain layer tests. */
public abstract class TvTrackerDomainTestBase<TStartupModule> : TvTrackerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
