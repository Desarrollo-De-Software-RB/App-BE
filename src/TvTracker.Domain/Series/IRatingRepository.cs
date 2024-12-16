using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace TvTracker.Series
{
    public interface IRatingRepository : IRepository<Rating, int>
    {
        Task<List<Rating>> GetRatingsByUserAsync(Guid userId);
        Task<Rating?> GetRatingByUserAndSerieAsync(Guid userId, int serieId);
    }

}
