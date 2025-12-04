using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using TvTracker.Series;
using Volo.Abp;

namespace TvTracker.Watchlists
{
    [RemoteService(Name = "Watchlist")]
    public class WatchlistAppServices : ApplicationService, IWatchlistAppServices
    {
        private readonly IRepository<WatchlistItem, Guid> _watchlistItemRepository;
        private readonly IRepository<TvTracker.Series.Serie, int> _serieRepository;
        private readonly ISeriesApiService _seriesApiService;

        public WatchlistAppServices(
            IRepository<WatchlistItem, Guid> watchlistItemRepository,
            IRepository<TvTracker.Series.Serie, int> serieRepository,
            ISeriesApiService seriesApiService)
        {
            _watchlistItemRepository = watchlistItemRepository;
            _serieRepository = serieRepository;
            _seriesApiService = seriesApiService;
        }

        public async Task<List<WatchlistItemDto>> GetListAsync()
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                return new List<WatchlistItemDto>();
            }

            var items = await _watchlistItemRepository.GetListAsync(x => x.UserId == userId);
            var dtos = new List<WatchlistItemDto>();

            foreach (var item in items)
            {
                var serie = await _serieRepository.GetAsync(item.SerieId);
                dtos.Add(new WatchlistItemDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    SerieId = item.SerieId,
                    Status = item.Status,
                    Serie = ObjectMapper.Map<TvTracker.Series.Serie, SerieDto>(serie)
                });
            }

            return dtos;
        }

        public async Task<WatchlistItemDto> CreateAsync(CreateUpdateWatchlistItemDto input)
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                throw new Volo.Abp.UserFriendlyException("User not logged in");
            }

            // 1. Find Serie by ImdbId (It should exist as per user workflow)
            var serie = await _serieRepository.FirstOrDefaultAsync(s => s.IMDBID == input.ImdbId);

            // 2. Fallback: If not found locally, fetch from OMDB and save (Safety net)
            if (serie == null)
            {
                var fullSerie = await _seriesApiService.GetSerieDetailsAsync(input.ImdbId);
                if (fullSerie == null)
                {
                    throw new Volo.Abp.UserFriendlyException("Serie not found in external API");
                }
                serie = await _serieRepository.InsertAsync(fullSerie, true);
            }

            // 3. Check if already in watchlist using the internal ID and UserID
            var existingItem = await _watchlistItemRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.SerieId == serie.Id);
            if (existingItem != null)
            {
                throw new Volo.Abp.UserFriendlyException("Serie already in watchlist");
            }

            var item = new WatchlistItem
            {
                UserId = userId.Value,
                SerieId = serie.Id,
                Status = input.Status
            };

            await _watchlistItemRepository.InsertAsync(item);

            return new WatchlistItemDto
            {
                Id = item.Id,
                UserId = item.UserId,
                SerieId = item.SerieId,
                Status = item.Status,
                Serie = ObjectMapper.Map<TvTracker.Series.Serie, SerieDto>(serie)
            };
        }

        public async Task RemoveItemAsync(string imdbId)
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                return;
            }

            var serie = await _serieRepository.FirstOrDefaultAsync(s => s.IMDBID == imdbId);
            if (serie == null) return;

            var item = await _watchlistItemRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.SerieId == serie.Id);
            if (item != null)
            {
                await _watchlistItemRepository.DeleteAsync(item);
            }
        }

        public async Task UpdateAsync(CreateUpdateWatchlistItemDto input)
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                return;
            }

            var serie = await _serieRepository.FirstOrDefaultAsync(s => s.IMDBID == input.ImdbId);
            if (serie == null) return;

            var item = await _watchlistItemRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.SerieId == serie.Id);
            if (item != null)
            {
                item.Status = input.Status;
                await _watchlistItemRepository.UpdateAsync(item);
            }
        }
    }
}
