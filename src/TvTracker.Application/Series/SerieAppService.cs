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

        public async Task<ICollection<SerieDto>> SearchAsync(string? title, string? genre)
        {
            var series = await _seriesApiService.GetSeriesAsync(title, genre);
            return ObjectMapper.Map<ICollection<Serie>, ICollection<SerieDto>>(series);
        }
    }
}