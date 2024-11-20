using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;

namespace TvTracker.Notificationes
{
    public class NotificationWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly ISeriesChangeDetectionService _seriesChangeDetectionService;
        private readonly IRepository<Notification, Guid> _notificationRepository;

        public NotificationWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory, ISeriesChangeDetectionService seriesChangeDetectionService, IRepository<Notification, Guid> notificationRepository)
            : base(timer, serviceScopeFactory)
        {
            timer.Period = 3600000; // 1 hora en milisegundos
            _seriesChangeDetectionService = seriesChangeDetectionService;
            _notificationRepository = notificationRepository;
        }

        // Método para ejecutarse periódicamente
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            await SendNotificationsOnSeriesChange();
        }

        // Lógica para enviar notificaciones cuando detecta cambios en las series
        public async Task SendNotificationsOnSeriesChange()
        {
            var notifications = await _seriesChangeDetectionService.DetectChangesAsync();
            foreach (var notification in notifications)
            {
                await _notificationRepository.InsertAsync(notification);
            }
        }
    }
}
