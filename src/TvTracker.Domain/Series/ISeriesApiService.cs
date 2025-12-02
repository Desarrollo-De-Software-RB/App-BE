using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvTracker.Series
{
    public interface ISeriesApiService
    {
        Task<ICollection<Serie>> SearchByTitleAsync(string title, string? type = null);
        Task<Serie> GetSerieDetailsAsync(string imdbId);
    }
}