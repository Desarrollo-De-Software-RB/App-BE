using System;
using Volo.Abp.Domain.Repositories;

namespace TvTracker.Notificationes
{
    public interface ITrackedSeriesRepository : IRepository<TrackedSeries, Guid>
    {
    }
}