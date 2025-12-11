using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TvTracker.Watchlists
{
    public class WatchlistItem : FullAuditedEntity<Guid>
    {
        public Guid UserId { get; set; }
        public int SerieId { get; set; }
        public WatchlistStatus Status { get; set; }
    }
}
