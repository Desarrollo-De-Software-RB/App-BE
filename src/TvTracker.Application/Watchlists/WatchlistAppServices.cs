using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvTracker.Series;
using TvTracker.Watchlists;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;

using TvTracker.Notificationes;

namespace TvTracker.Watchlists
{
    [RemoteService(Name = "Watchlist")]
    public class WatchlistAppServices : ApplicationService, IWatchlistAppServices
    {
        private readonly IRepository<WatchlistItem, Guid> _watchlistItemRepository;
        private readonly IRepository<TvTracker.Series.Serie, int> _serieRepository;
        private readonly ISeriesApiService _seriesApiService;
        private readonly NotificationManager _notificationManager;

        public WatchlistAppServices(
            IRepository<WatchlistItem, Guid> watchlistItemRepository,
            IRepository<TvTracker.Series.Serie, int> serieRepository,
            ISeriesApiService seriesApiService,
            NotificationManager notificationManager)
        {
            _watchlistItemRepository = watchlistItemRepository;
            _serieRepository = serieRepository;
            _seriesApiService = seriesApiService;
            _notificationManager = notificationManager;
        }

        public async Task<List<WatchlistItemDto>> GetListAsync()
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                return new List<WatchlistItemDto>();
            }

            var query = await _watchlistItemRepository.GetQueryableAsync();
            var items = query.Where(x => x.UserId == userId).ToList();
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
            var serieQuery = await _serieRepository.GetQueryableAsync();
            var serie = serieQuery.FirstOrDefault(s => s.IMDBID == input.ImdbId);

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
            var watchlistQuery = await _watchlistItemRepository.GetQueryableAsync();
            var existingItem = watchlistQuery.FirstOrDefault(x => x.UserId == userId && x.SerieId == serie.Id);
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

            await _notificationManager.CreateAsync(
                userId.Value,
                "Watchlist Update",
                $"You added {serie.Title} to your watchlist as {input.Status}.",
                NotificationType.UserActivity,
                serie.Id.ToString());

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

            var serieQuery = await _serieRepository.GetQueryableAsync();
            var serie = serieQuery.FirstOrDefault(s => s.IMDBID == imdbId);
            if (serie == null) return;

            var watchlistQuery = await _watchlistItemRepository.GetQueryableAsync();
            var item = watchlistQuery.FirstOrDefault(x => x.UserId == userId && x.SerieId == serie.Id);
            if (item != null)
            {
                await _watchlistItemRepository.DeleteAsync(item);

                await _notificationManager.CreateAsync(
                    userId.Value,
                    "Watchlist Update",
                    $"You removed {serie.Title} from your watchlist.",
                    NotificationType.UserActivity,
                    serie.Id.ToString());
            }
        }

        public async Task UpdateAsync(CreateUpdateWatchlistItemDto input)
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                return;
            }

            var serieQuery = await _serieRepository.GetQueryableAsync();
            var serie = serieQuery.FirstOrDefault(s => s.IMDBID == input.ImdbId);
            if (serie == null) return;

            var watchlistQuery = await _watchlistItemRepository.GetQueryableAsync();
            var item = watchlistQuery.FirstOrDefault(x => x.UserId == userId && x.SerieId == serie.Id);
            if (item != null)
            {
                item.Status = input.Status;
                await _watchlistItemRepository.UpdateAsync(item);

                string message = $"You marked {serie.Title} as {input.Status}.";
                
                if (input.Status == WatchlistStatus.Completed)
                    message = $"¡Congratulations! You completed {serie.Title}.";
                else if (input.Status == WatchlistStatus.Watching)
                    message = $"You started following {serie.Title}.";
                else if (input.Status == WatchlistStatus.Dropped)
                    message = $"You dropped {serie.Title}.";
                else if (input.Status == WatchlistStatus.Pending)
                    message = $"You marked {serie.Title} as pending.";

                await _notificationManager.CreateAsync(
                    userId.Value,
                    "Watchlist Update",
                    message,
                    NotificationType.UserActivity,
                    serie.Id.ToString());
            }
        }
    }
}
