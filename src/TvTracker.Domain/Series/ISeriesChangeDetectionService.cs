using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISeriesChangeDetectionService
{
    Task<List<Notification>> DetectChangesAsync();
}