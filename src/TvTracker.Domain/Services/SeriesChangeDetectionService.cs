using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Domain.Repositories;

public class SeriesChangeDetectionService : ISeriesChangeDetectionService
{
    private readonly IRepository<TrackedSeries, Guid> _trackedSeriesRepository;

    public SeriesChangeDetectionService(IRepository<TrackedSeries, Guid> trackedSeriesRepository)
    {
        _trackedSeriesRepository = trackedSeriesRepository;
    }

    public async Task<List<Notification>> DetectChangesAsync()
    {
        var trackedSeries = await _trackedSeriesRepository.GetListAsync();
        var notifications = new List<Notification>();

        foreach (var series in trackedSeries)
        {
            if (SeriesHasChanged(series))
            {
                var notification = new Notification
                {
                    Message = $"La serie {series.Name} ha tenido actualizaciones.",
                    Method = "PushNotification", // Por ejemplo
                    UserId = series.UserId
                };
                notifications.Add(notification);
            }
        }

        return notifications;
    }

    private bool SeriesHasChanged(TrackedSeries series)
    {
        // Implementar lógica para detectar cambios (ej. fecha de último episodio)
        return true; // Placeholder
    }
}
