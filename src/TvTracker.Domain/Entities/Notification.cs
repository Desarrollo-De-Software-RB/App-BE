using System;
using Volo.Abp.Domain.Entities;

public class Notification : Entity<Guid>
{
    public string Message { get; set; }
    public string Method { get; set; } // Push, Email, etc.
    public Guid UserId { get; set; }
}
