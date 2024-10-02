using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TvTracker.Series
{
    public class OmdbService : ISeriesApiService
    {
        public async Task<ICollection<SerieDto>> GetSeriesAsync(string title, string gender)
        {
            SerieDto[] series = new SerieDto[]
            {
                new SerieDto
                {
                    Title = "Breaking Bad",
                },
                new SerieDto
                {
                    Title = "The Mandalorian",
                }
            };
            return await Task.FromResult(series);


        }
    }
}