using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TvTracker.Series
{
    public interface IRatingAppService : IApplicationService
    {
        Task<List<RatingDto>> GetSeriesRatingsAsync(int serieId);
        Task RateSeriesAsync(CreateUpdateRatingDto input);
    }
}
