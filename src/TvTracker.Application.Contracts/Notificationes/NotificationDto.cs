using System;
using Volo.Abp.Application.Dtos;
using TvTracker.Notificationes;

namespace TvTracker.Notificationes
{
    public class NotificationDto : EntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public NotificationType Type { get; set; }
        public string? RelatedEntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class NotificationPreferenceDto
    {
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public bool IsEnabled { get; set; }
    }
}
