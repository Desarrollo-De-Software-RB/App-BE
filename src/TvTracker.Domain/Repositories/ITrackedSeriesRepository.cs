using System;
using Volo.Abp.Domain.Repositories;

public interface ITrackedSeriesRepository : IRepository<TrackedSeries, Guid>
{
}