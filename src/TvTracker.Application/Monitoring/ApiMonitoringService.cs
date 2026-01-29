using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace TvTracker.Monitoring;

public class ApiMonitoringService : IApiMonitoringService, ISingletonDependency
{
    private readonly ConcurrentQueue<long> _latencies = new();
    private readonly ConcurrentQueue<DateTime> _errorTimestamps = new();
    private readonly ConcurrentDictionary<string, int> _externalRequests = new();
    private int _totalRequests;
    private int _totalErrors;
    private DateTime _startTime;

    public ApiMonitoringService()
    {
        _startTime = DateTime.UtcNow;
    }

    public void RecordRequest(long durationMs, bool isError)
    {
        System.Threading.Interlocked.Increment(ref _totalRequests);
        
        // Keep last 1000 latencies for calculating average
        _latencies.Enqueue(durationMs);
        if (_latencies.Count > 1000)
        {
            _latencies.TryDequeue(out _);
        }

        if (isError)
        {
            System.Threading.Interlocked.Increment(ref _totalErrors);
            _errorTimestamps.Enqueue(DateTime.UtcNow);
        }

        while (_errorTimestamps.TryPeek(out var timestamp) && (DateTime.UtcNow - timestamp).TotalMinutes > 1)
        {
            _errorTimestamps.TryDequeue(out _);
        }
    }

    public void RecordExternalRequest(string apiName)
    {
        _externalRequests.AddOrUpdate(apiName, 1, (key, oldValue) => oldValue + 1);
    }

    public MonitoringStatsDto GetStats()
    {
        var now = DateTime.UtcNow;
        var uptime = now - _startTime;
        
        while (_errorTimestamps.TryPeek(out var timestamp) && (now - timestamp).TotalMinutes > 1)
        {
            _errorTimestamps.TryDequeue(out _);
        }

        double avgLatency = 0;
        if (!_latencies.IsEmpty)
        {
            avgLatency = _latencies.Average();
        }

        return new MonitoringStatsDto
        {
            TotalRequests = _totalRequests,
            TotalErrors = _totalErrors,
            LastMinuteErrors = _errorTimestamps.Count,
            AverageResponseTimeMs = Math.Round(avgLatency, 2),
            UptimeSeconds = (long)uptime.TotalSeconds,
            ExternalRequests = new Dictionary<string, int>(_externalRequests)
        };
    }
}

