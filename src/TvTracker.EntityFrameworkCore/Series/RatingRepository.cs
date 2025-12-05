using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvTracker.EntityFrameworkCore;
using TvTracker.Series;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace TvTracker.Series
{
    public class RatingRepository : EfCoreRepository<TvTrackerDbContext, Rating, int>, IRatingRepository
    {
        public RatingRepository(IDbContextProvider<TvTrackerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Rating>> GetRatingsByUserAsync(Guid userId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<Rating?> GetRatingByUserAndSerieAsync(Guid userId, int serieId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(r => r.UserId == userId && r.SerieId == serieId);
        }

        public async Task<List<Rating>> GetRatingsBySerieAsync(int serieId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(r => r.SerieId == serieId).ToListAsync();
        }
    }
}
