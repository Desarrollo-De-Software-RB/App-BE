using System;
using Volo.Abp.Domain.Repositories;

namespace TvTracker.Notificationes
{
    public interface INotificationRepository : IRepository<Notification, Guid>
    {
    }
}