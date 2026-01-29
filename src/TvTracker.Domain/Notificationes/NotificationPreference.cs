using System;
using Volo.Abp.Domain.Entities;

namespace TvTracker.Notificationes
{
    public class NotificationPreference : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public bool IsEnabled { get; set; }

        protected NotificationPreference() { }

        public NotificationPreference(Guid id, Guid userId, NotificationType type, NotificationChannel channel, bool isEnabled)
            : base(id)
        {
            UserId = userId;
            Type = type;
            Channel = channel;
            IsEnabled = isEnabled;
        }
    }
}
