using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TvTracker.Data;
using Volo.Abp.DependencyInjection;

namespace TvTracker.EntityFrameworkCore;

public class EntityFrameworkCoreTvTrackerDbSchemaMigrator
    : ITvTrackerDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreTvTrackerDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the TvTrackerDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<TvTrackerDbContext>()
            .Database
            .MigrateAsync();
    }
}
