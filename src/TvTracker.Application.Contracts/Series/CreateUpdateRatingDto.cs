using System;
using System.ComponentModel.DataAnnotations;

namespace TvTracker.Series
{
    public class CreateUpdateRatingDto
    {
        [Required]
        public int SerieId { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public string? Comment { get; set; }
    }
}
