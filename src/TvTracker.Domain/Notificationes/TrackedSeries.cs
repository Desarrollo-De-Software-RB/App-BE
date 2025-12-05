using System;
using Volo.Abp.Domain.Entities;

namespace TvTracker.Notificationes
{
    public class TrackedSeries : Entity<Guid>
    {
        public Guid SeriesId { get; set; } // Agregar SeriesId
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; } // Fecha de última actualización
    }
}
