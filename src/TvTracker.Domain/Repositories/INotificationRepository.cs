using System;
using Volo.Abp.Domain.Repositories;

public interface INotificationRepository : IRepository<Notification, Guid>
{
}