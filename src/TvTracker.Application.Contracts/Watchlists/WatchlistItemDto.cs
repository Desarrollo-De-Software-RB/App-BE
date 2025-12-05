using System;
using TvTracker.Series;
using Volo.Abp.Application.Dtos;

namespace TvTracker.Watchlists
{
    public class WatchlistItemDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public int SerieId { get; set; }
        public SerieDto Serie { get; set; }
        public WatchlistStatus Status { get; set; }
    }
}
