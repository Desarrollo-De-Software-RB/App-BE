using System;
using TvTracker.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

public class TrackedSeriesRepository : EfCoreRepository<TvTrackerDbContext, TrackedSeries, Guid>, ITrackedSeriesRepository
{
    public TrackedSeriesRepository(IDbContextProvider<TvTrackerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}