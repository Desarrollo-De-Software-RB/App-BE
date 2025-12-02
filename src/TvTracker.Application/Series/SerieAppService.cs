using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TvTracker.Series
{
    public class SerieAppService : CrudAppService<Serie, SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>, ISerieAppService
    {
        private readonly ISeriesApiService _seriesApiService;
        public SerieAppService(IRepository<Serie, int> repository, ISeriesApiService seriesapiService) : base(repository)
        {
            _seriesApiService = seriesapiService;
        }

        public async Task<ICollection<SerieDto>> SearchAsync(string? title, string? genre, string? type = null)
        {
            // 1. Search in API by title (lightweight)
            var apiResults = await _seriesApiService.SearchByTitleAsync(title, type);

            // 2. Load existing series from DB once (to avoid async queries in loop)
            var queryable = await Repository.GetQueryableAsync();
            var imdbIds = apiResults.Select(s => s.IMDBID).ToList();
            
            // Note: In a real DB, this translates to WHERE IMDBID IN (...)
            var existingSeries = queryable.Where(s => imdbIds.Contains(s.IMDBID)).ToList();
            var existingImdbIds = new HashSet<string>(existingSeries.Select(s => s.IMDBID));

            // 3. Sync results to local DB
            foreach (var apiSerie in apiResults)
            {
                if (!existingImdbIds.Contains(apiSerie.IMDBID))
                {
                    // Fetch full details and save
                    var fullSerie = await _seriesApiService.GetSerieDetailsAsync(apiSerie.IMDBID);
                    if (fullSerie != null)
                    {
                        var insertedSerie = await Repository.InsertAsync(fullSerie, true);
                        existingSeries.Add(insertedSerie);
                    }
                }
            }

            // 4. Query local results with filters (Title AND Genre)
            // We filter in memory since we have the relevant subset (or we could query DB again if needed, but this is efficient for search results)
            var localResults = existingSeries
                .Where(s => (title == null || s.Title.Contains(title, StringComparison.OrdinalIgnoreCase)))
                .WhereIf(!string.IsNullOrEmpty(genre), s => s.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase))
                .WhereIf(!string.IsNullOrEmpty(type), s => s.Type == type)
                .ToList();
            
            return ObjectMapper.Map<ICollection<Serie>, ICollection<SerieDto>>(localResults);
        }

        public async Task<SerieDto> GetByImdbIdAsync(string imdbId)
        {
            var queryable = await Repository.GetQueryableAsync();
            // Using synchronous FirstOrDefault to avoid async extension method mocking issues in unit tests
            var serie = queryable.FirstOrDefault(s => s.IMDBID == imdbId);
            
            if (serie == null)
            {
                // If not in DB, fetch from API and save
                var fullSerie = await _seriesApiService.GetSerieDetailsAsync(imdbId);
                if (fullSerie != null)
                {
                    serie = await Repository.InsertAsync(fullSerie, true);
                }
            }

            return ObjectMapper.Map<Serie, SerieDto>(serie);
        }
    }
}