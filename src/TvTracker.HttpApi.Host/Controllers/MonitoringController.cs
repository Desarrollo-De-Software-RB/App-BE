using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvTracker.Monitoring;
using Volo.Abp.AspNetCore.Mvc;

namespace TvTracker.Controllers;

[Authorize(Policy = "TvTracker.AdminOptions")]
[Route("api/monitoring")]
public class MonitoringController : AbpController
{
    private readonly IApiMonitoringService _monitoringService;

    public MonitoringController(IApiMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    [HttpGet("stats")]
    public ActionResult<MonitoringStatsDto> GetStats()
    {
        return _monitoringService.GetStats();
    }

    [HttpGet("logs")]
    public async Task<IActionResult> DownloadLogs([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string? levels = null)
    {
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "logs.txt");
        
        if (!System.IO.File.Exists(logPath))
        {
            return NotFound("Log file not found.");
        }

        var allowedLevels = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(levels))
        {
            var splitLevels = levels.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var level in splitLevels)
            {
                allowedLevels.Add(level.Trim()); // Expected: INF, DBG, ERR, WRN, FTL
            }
        }

        // Default range if not provided: Last 30 days
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = endDate?.AddDays(1) ?? DateTime.Today.AddDays(1); // Add 1 day to include the end date fully

        try 
        {
            var memory = new MemoryStream();
            using (var writer = new StreamWriter(memory, leaveOpen: true))
            using (var stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // Basic parsing logic. Assuming standard text format: 
                    // 2023-10-27 10:00:00.123 +00:00 [INF] Message...
                    // Check if line starts with a date we can parse
                    
                    // Filter by Level
                    if (allowedLevels.Count > 0)
                    {
                        bool levelFound = false;
                        foreach (var lvl in allowedLevels)
                        {
                            if (line.Contains($"[{lvl}]"))
                            {
                                levelFound = true;
                                break;
                            }
                        }
                        if (!levelFound) continue;
                    }

                    // Filter by Date Range
                    // Try to parse the first 19 chars (YYYY-MM-DD HH:mm:ss)
                    if (line.Length > 19 && DateTime.TryParse(line.Substring(0, 19), out var logDate))
                    {
                        if (logDate < start || logDate >= end) continue;
                    }

                    await writer.WriteLineAsync(line);
                }
            }
            memory.Position = 0;
            return File(memory, "text/plain", $"logs_{start:yyyyMMdd}_{end.AddDays(-1):yyyyMMdd}.txt");
        }
        catch (IOException)
        {
            return StatusCode(500, "Could not access log file, it might be in use.");
        }
    }
}
