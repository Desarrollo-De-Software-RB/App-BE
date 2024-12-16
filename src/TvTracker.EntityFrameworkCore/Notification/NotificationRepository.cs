using System;
using TvTracker.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace TvTracker.Notificationes
{
    public class NotificationRepository : EfCoreRepository<TvTrackerDbContext, Notification, Guid>, INotificationRepository
    {
        public NotificationRepository(IDbContextProvider<TvTrackerDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}