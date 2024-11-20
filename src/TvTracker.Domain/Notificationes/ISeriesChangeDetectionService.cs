using System.Collections.Generic;
using System.Threading.Tasks;

namespace TvTracker.Notificationes
{
    public interface ISeriesChangeDetectionService
    {
        Task<List<Notification>> DetectChangesAsync();
    }
}