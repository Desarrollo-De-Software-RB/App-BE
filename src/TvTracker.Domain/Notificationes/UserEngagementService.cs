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
    public class UserEngagementService : ITransientDependency
    {
        private readonly IRepository<WatchlistItem, Guid> _watchlistRepository;
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<Rating, int> _ratingRepository;
        private readonly NotificationManager _notificationManager;

        public UserEngagementService(
            IRepository<WatchlistItem, Guid> watchlistRepository,
            IRepository<Notification, Guid> notificationRepository,
            IRepository<Serie, int> serieRepository,
            IRepository<Rating, int> ratingRepository,
            NotificationManager notificationManager)
        {
            _watchlistRepository = watchlistRepository;
            _notificationRepository = notificationRepository;
            _serieRepository = serieRepository;
            _ratingRepository = ratingRepository;
            _notificationManager = notificationManager;
        }

        public async Task AnalyzeUserEngagementAsync()
        {
            await CheckInactivityRemindersAsync();
            await CheckPendingRemindersAsync();
            await CheckPersonalizedTrendsAsync();
            await CheckMonthlySummaryAsync();
        }

        private async Task CheckMonthlySummaryAsync()
        {
            // Monthly Summary
            // Calculate start of current month
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Get completed items modified this month
            // Note: This relies on LastModificationTime being the time of completion. 
            // If the user updates a completed item later, it might count again. 
            // For a robust solution we'd need AuditLogs or a History table.
            
            var query = await _watchlistRepository.GetQueryableAsync();
            var completedItems = query
                .Where(w => w.Status == WatchlistStatus.Completed && 
                            w.LastModificationTime >= startOfMonth)
                .ToList();

            // Group by user
            var userGroups = completedItems.GroupBy(w => w.UserId);

            foreach (var group in userGroups)
            {
                var count = group.Count();
                if (count == 0) continue;
                
                string summaryKey = $"Summary-{now.Month}-{now.Year}";
                
                if (await RecentlyNotifiedAsync(group.Key, NotificationType.Reminder, summaryKey, 30))
                    continue;
                
                await _notificationManager.CreateAsync(
                    group.Key,
                    "Monthly Summary",
                    $"You completed {count} series this month.",
                    NotificationType.Reminder,
                    summaryKey); // Storing key to prevent duplicate
            }
        }

        private async Task CheckInactivityRemindersAsync()
        {
            // Inactivity in 'Watching' series (e.g., no updates for 15 days)
            var cutoff = DateTime.UtcNow.AddDays(-15);
            var query = await _watchlistRepository.GetQueryableAsync();
            
            // Fetch potential stale items
            var staleItems = query
                .Where(w => w.Status == WatchlistStatus.Watching && 
                           (w.LastModificationTime ?? w.CreationTime) < cutoff)
                .ToList();

            foreach (var item in staleItems)
            {
                // Avoid spamming: Check if we sent a reminder for this series in the last 7 days
                if (await RecentlyNotifiedAsync(item.UserId, NotificationType.Reminder, item.SerieId.ToString(), 7))
                    continue;

                var serie = await _serieRepository.GetAsync(item.SerieId);
                await _notificationManager.CreateAsync(
                    item.UserId,
                    "Inactivity Reminder",
                    $"You haven't updated {serie.Title} in a while.",
                    NotificationType.Reminder,
                    serie.Id.ToString());
            }
        }

        private async Task CheckPendingRemindersAsync()
        {
            // Pending series without start (e.g., added > 30 days ago)
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var query = await _watchlistRepository.GetQueryableAsync();

            var pendingItems = query
                .Where(w => w.Status == WatchlistStatus.Pending &&
                            w.CreationTime < cutoff)
                .ToList();

            foreach (var item in pendingItems)
            {
                if (await RecentlyNotifiedAsync(item.UserId, NotificationType.Reminder, item.SerieId.ToString(), 30))
                    continue;

                 await _notificationManager.CreateAsync(
                    item.UserId,
                    "Pending Series Reminder",
                    "You have series pending without starting.",
                    NotificationType.Reminder,
                    item.SerieId.ToString());
            }
        }

        private async Task CheckPersonalizedTrendsAsync()
        {
            // Pick users who haven't received a Trend notification in 7 days            
            // Simplified approach: Get high ratings (5 stars)
            var ratingQuery = await _ratingRepository.GetQueryableAsync();
            var highRatings = ratingQuery.Where(r => r.Score == 5).ToList();
            
            foreach (var rating in highRatings)
            {
                if (await RecentlyNotifiedAsync(rating.UserId, NotificationType.Trend, null, 14)) // Bi-weekly
                    continue;

                 var serie = await _serieRepository.GetAsync(rating.SerieId);
                 await _notificationManager.CreateAsync(
                    rating.UserId,
                    "Your Favorites",
                    $"{serie.Title} is one of your best rated series.",
                    NotificationType.Trend,
                    serie.Id.ToString());
            }
        }
        
        private async Task<bool> RecentlyNotifiedAsync(Guid userId, NotificationType type, string? relatedEntityId, int days)
        {
             var cutoff = DateTime.UtcNow.AddDays(-days);
             var query = await _notificationRepository.GetQueryableAsync();
             
             // Check for recent notification of same type (and optionally same series)
             return query.Any(n => n.UserId == userId && 
                                   n.Type == type && 
                                   n.CreationTime > cutoff && 
                                   (relatedEntityId == null || n.RelatedEntityId == relatedEntityId));
        }
    }
}
