using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TvTracker.Watchlists
{
    public interface IWatchlistAppService : IApplicationService
    {
        Task AddSerieAsync(int serieId);
    }
}
