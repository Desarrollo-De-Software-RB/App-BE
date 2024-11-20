using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvTracker.Series
{
    public interface ISeriesApiService
    {
        Task<ICollection<SerieDto>> GetSeriesAsync(string title, string gender);
    }
}