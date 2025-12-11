using System;
using Volo.Abp.Application.Dtos;

namespace TvTracker.Series
{
    public class RatingDto : EntityDto<int>
    {
        public int SerieId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
