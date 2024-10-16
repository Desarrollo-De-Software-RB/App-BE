using System;
using Volo.Abp.Domain.Entities;

public class TrackedSeries : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public DateTime LastUpdated { get; set; } // Fecha de última actualización
}