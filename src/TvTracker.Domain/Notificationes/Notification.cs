using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace TvTracker.Notificationes
{
    public class Notification : CreationAuditedEntity<Guid>
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public NotificationType Type { get; set; }
        public string? RelatedEntityId { get; set; } // Can be SerieId (int) or logic for other entities
        public bool IsRead { get; set; }
        public Guid UserId { get; set; }
    }
}