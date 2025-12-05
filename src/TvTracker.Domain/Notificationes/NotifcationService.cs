using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace TvTracker.Notificationes
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ISeriesChangeDetectionService _seriesChangeDetectionService;

        public NotificationService(INotificationRepository notificationRepository, ISeriesChangeDetectionService seriesChangeDetectionService)
        {
            _notificationRepository = notificationRepository;
            _seriesChangeDetectionService = seriesChangeDetectionService;
        }

        public async Task SendNotificationsAsync()
        {
            var notifications = await _seriesChangeDetectionService.DetectChangesAsync();
            foreach (var notification in notifications)
            {
                await _notificationRepository.InsertAsync(notification);
            }
        }
    }
}
