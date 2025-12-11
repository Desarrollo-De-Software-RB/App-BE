using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace TvTracker.Watchlists
{
    [RemoteService(Name = "Watchlist")]
    public interface IWatchlistAppServices : IApplicationService
    {
        Task<List<WatchlistItemDto>> GetListAsync();
        Task<WatchlistItemDto> CreateAsync(CreateUpdateWatchlistItemDto input);
        Task RemoveItemAsync(string imdbId);
        Task UpdateAsync(CreateUpdateWatchlistItemDto input);
    }
}
