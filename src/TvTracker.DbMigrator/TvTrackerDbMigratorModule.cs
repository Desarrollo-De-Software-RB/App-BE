using TvTracker.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace TvTracker.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TvTrackerEntityFrameworkCoreModule),
    typeof(TvTrackerApplicationContractsModule)
)]
public class TvTrackerDbMigratorModule : AbpModule
{
}
