using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TvTracker.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class TvTrackerDbContextFactory : IDesignTimeDbContextFactory<TvTrackerDbContext>
{
    public TvTrackerDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        TvTrackerEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<TvTrackerDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new TvTrackerDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TvTracker.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
