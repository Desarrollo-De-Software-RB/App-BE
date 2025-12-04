using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvTracker.Series;
using TvTracker.WatchLists;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TvTracker.Watchlists
{
    public class WatchlistAppService : ApplicationService, IWatchlistAppService
    {
        private readonly IRepository<Watchlist, int> _watchlistRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUser _currentUser;

        public WatchlistAppService(IRepository<Watchlist, int> watchlistRepository, IRepository<Serie, int> serieRepository, ICurrentUser currentUser)
        {
            _serieRepository = serieRepository;
            _watchlistRepository = watchlistRepository;
            _currentUser = currentUser;
        }

        public async Task AddSerieAsync(int serieId)
        {
            Guid? userId = _currentUser.Id;

            var watchlist = ((List<Watchlist>)await _watchlistRepository.GetListAsync()).FirstOrDefault();

            if (watchlist == null)
            {
                watchlist = new Watchlist();
                await _watchlistRepository.InsertAsync(watchlist);
            }

            var serie = await _serieRepository.GetAsync(serieId);
            watchlist.Series.Add(serie);
            await _watchlistRepository.UpdateAsync(watchlist);
        }
    }
}
