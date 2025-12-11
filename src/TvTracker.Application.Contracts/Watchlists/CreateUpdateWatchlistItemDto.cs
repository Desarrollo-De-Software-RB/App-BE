using System;
using System.ComponentModel.DataAnnotations;

namespace TvTracker.Watchlists
{
    public class CreateUpdateWatchlistItemDto
    {
        [Required]
        public string ImdbId { get; set; }

        [Required]
        public WatchlistStatus Status { get; set; }
    }
}
