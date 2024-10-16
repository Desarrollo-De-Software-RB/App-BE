using System.Threading.Tasks;

namespace TvTracker.Data;

public interface ITvTrackerDbSchemaMigrator
{
    Task MigrateAsync();
}
