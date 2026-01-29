using System.Collections.Generic;

namespace TvTracker.Monitoring;

public interface IApiMonitoringService
{
    void RecordRequest(long durationMs, bool isError);
    void RecordExternalRequest(string apiName);
    MonitoringStatsDto GetStats();
}

public class MonitoringStatsDto
{
    public int TotalRequests { get; set; }
    public int TotalErrors { get; set; }
    public int LastMinuteErrors { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public long UptimeSeconds { get; set; }
    public Dictionary<string, int> ExternalRequests { get; set; } = new();
}
