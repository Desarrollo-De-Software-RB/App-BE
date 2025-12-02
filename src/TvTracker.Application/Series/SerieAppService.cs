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

            // 2. Sync results to local DB
            foreach (var apiSerie in apiResults)
            {
                var existingSerie = await Repository.FirstOrDefaultAsync(s => s.IMDBID == apiSerie.IMDBID);
                if (existingSerie == null)
                {
                    // Fetch full details and save
                    var fullSerie = await _seriesApiService.GetSerieDetailsAsync(apiSerie.IMDBID);
                    if (fullSerie != null)
                    {
                        await Repository.InsertAsync(fullSerie, true);
                    }
                }
            }

            // 3. Query local DB with filters (Title AND Genre)
            // We use Contains for title to match what API returned (roughly) and exact/contains for genre
            var query = await Repository.GetQueryableAsync();
            var localResults = query
                .Where(s => s.Title.Contains(title))
                .WhereIf(!string.IsNullOrEmpty(genre), s => s.Genre.Contains(genre))
                .WhereIf(!string.IsNullOrEmpty(type), s => s.Type == type)
                .ToList();

            // If local results are empty but we had API results, it might be due to strict title matching or genre filtering.
            // However, the requirement is to use local DB for genre filtering.
            
            return ObjectMapper.Map<ICollection<Serie>, ICollection<SerieDto>>(localResults);
        }

        public async Task<SerieDto> GetByImdbIdAsync(string imdbId)
        {
            var serie = await Repository.FirstOrDefaultAsync(s => s.IMDBID == imdbId);
            
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