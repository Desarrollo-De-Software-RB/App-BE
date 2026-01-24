using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvTracker.Series;
using TvTracker.Watchlists;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace TvTracker.Notificationes
{
    public class SeriesChangeDetectionService : ISeriesChangeDetectionService, ITransientDependency
    {
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<WatchlistItem, Guid> _watchlistRepository;
        private readonly IRepository<NotificationPreference, Guid> _preferenceRepository;
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly ISeriesApiService _omdbService;
        private readonly NotificationManager _notificationManager;

        public SeriesChangeDetectionService(
            IRepository<Serie, int> serieRepository,
            IRepository<WatchlistItem, Guid> watchlistRepository,
            IRepository<NotificationPreference, Guid> preferenceRepository,
            IRepository<Notification, Guid> notificationRepository,
            ISeriesApiService omdbService,
            NotificationManager notificationManager)
        {
            _serieRepository = serieRepository;
            _watchlistRepository = watchlistRepository;
            _preferenceRepository = preferenceRepository;
            _notificationRepository = notificationRepository;
            _omdbService = omdbService;
            _notificationManager = notificationManager;
        }

        public async Task<List<Notification>> DetectChangesAsync()
        {
            // 1. Get series not updated in the last 24 hours (or never updated)
            // Ideally this should be paginated or limited to prevent timeout, let's limit to 20 for now per run
            // 1. Get series not updated in the last 12 hours (or never updated)
            var cutoffDate = DateTime.UtcNow.AddHours(-12);
            var query = await _serieRepository.GetQueryableAsync();
            
            // Execute query on DB side
            var seriesToCheck = query
                                .Where(s => s.LastOmdbCheck == null || s.LastOmdbCheck < cutoffDate)
                                .Take(20)
                                .ToList();

            var newNotifications = new List<Notification>();

            foreach (var localSerie in seriesToCheck)
            {
                // 2. Fetch from OMDB
                var omdbSerie = await _omdbService.GetSerieDetailsAsync(localSerie.IMDBID);
                if (omdbSerie == null) continue;

                var changes = DetectChanges(localSerie, omdbSerie);

                if (changes.Any())
                {
                    // 3. Update Local Serie
                    UpdateLocalSerie(localSerie, omdbSerie);
                    await _serieRepository.UpdateAsync(localSerie);

                    // 4. Notify Users
                    await GenerateNotificationsForChanges(localSerie, changes, newNotifications);
                }
                else
                {
                    // Update check time even if no changes
                    localSerie.LastOmdbCheck = DateTime.UtcNow;
                    await _serieRepository.UpdateAsync(localSerie);
                }
            }

            return newNotifications;
        }

        private List<(NotificationType Type, string Message)> DetectChanges(Serie local, Serie remote)
        {
            var changes = new List<(NotificationType, string)>();

            // Tolerance for float comparison
            if (Math.Abs(local.IMDBRating - remote.IMDBRating) > 0.2)
            {
                string msg = $"The rating of {local.Title} changed to {remote.IMDBRating} on IMDB.";
                changes.Add((NotificationType.RatingChange, msg));
            }

            // Votes - assuming IMDBVotes is string "1,234,567"
            if (local.IMDBVotes != remote.IMDBVotes)
            {
                // Simple string check for now, logic could be improved to parse int
                if (IsSignificantVoteIncrease(local.IMDBVotes, remote.IMDBVotes))
                {
                     string msg = $"{local.Title} is gaining popularity on IMDB.";
                     changes.Add((NotificationType.VotesChange, msg));
                }
            }

            if (local.Poster != remote.Poster && !string.IsNullOrEmpty(remote.Poster) && remote.Poster != "N/A")
            {
                changes.Add((NotificationType.PosterChange, $"New poster available for {local.Title}."));
            }

            if (local.Plot != remote.Plot && !string.IsNullOrEmpty(remote.Plot))
            {
                changes.Add((NotificationType.PlotChange, $"The synopsis of {local.Title} has been updated."));
            }
            


            if (local.Runtime != remote.Runtime && !string.IsNullOrEmpty(remote.Runtime))
            {
                 changes.Add((NotificationType.RuntimeChange, $"Runtime updated for {local.Title}."));
            }
            
             // Check if status changed (not directly in Serie model but maybe implied or if we had a Status field)
             // For now we don't have a clear "Status" field in Serie like "Ended" vs "Running", 
             // but if Year changed from "2010–" to "2010–2014" that implies ended.
             if (local.Year != remote.Year)
             {
                 changes.Add((NotificationType.StatusChange, $"Status/Year updated for {local.Title}."));
             }

            return changes;
        }

        private bool IsSignificantVoteIncrease(string localVotes, string remoteVotes)
        {
            // Helper to parse "2,000" -> 2000
            long Parse(string s) => long.TryParse(s?.Replace(",", ""), out var v) ? v : 0;
            var oldIdx = Parse(localVotes);
            var newIdx = Parse(remoteVotes);
            // Example threshold: 10% increase or > 1000 votes diff (simplified)
            return newIdx > oldIdx + 1000; 
        }

        private void UpdateLocalSerie(Serie local, Serie remote)
        {
            local.IMDBRating = remote.IMDBRating;
            local.IMDBVotes = remote.IMDBVotes;
            local.Poster = remote.Poster;
            local.Plot = remote.Plot;
            local.Awards = remote.Awards;
            local.Runtime = remote.Runtime;
            local.Year = remote.Year;
            local.LastOmdbCheck = DateTime.UtcNow;
            // Update other fields as needed
        }

        private async Task GenerateNotificationsForChanges(Serie serie, List<(NotificationType Type, string Message)> changes, List<Notification> newNotifications)
        {
            // Get all users watching this serie
            // Assuming WatchlistItem has SerieId and UserId
            var watchItems = await _watchlistRepository.GetListAsync(w => w.SerieId == serie.Id);
            var userIds = watchItems.Select(w => w.UserId).Distinct().ToList();

            if (!userIds.Any()) return;

            if (!userIds.Any()) return;

            // We iterate users and trigger NotificationManager for each.
            // NotificationManager handles preferences (InApp vs Email) internally.
            
            foreach (var userId in userIds)
            {
                foreach (var change in changes)
                {
                    // This call handles DB insertion (if enabled) and Email sending (if enabled)
                    // It uses the new HTML templates we just added.
                    await _notificationManager.CreateAsync(
                        userId, 
                        change.Type.ToString(), 
                        change.Message, 
                        change.Type, 
                        serie.Id.ToString()
                    );
                }
            }
        }


    }
}