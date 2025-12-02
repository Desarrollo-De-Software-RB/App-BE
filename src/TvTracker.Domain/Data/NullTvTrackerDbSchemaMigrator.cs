using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TvTracker.Data;

/* This is used if database provider does't define
 * ITvTrackerDbSchemaMigrator implementation.
 */
public class NullTvTrackerDbSchemaMigrator : ITvTrackerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
