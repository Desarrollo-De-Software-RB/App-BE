using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TvTracker.Monitoring;

namespace TvTracker.Middleware;

public class ApiMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiMonitoringService _monitoringService;

    public ApiMonitoringMiddleware(RequestDelegate next, IApiMonitoringService monitoringService)
    {
        _next = next;
        _monitoringService = monitoringService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip monitoring for metrics endpoint itself to avoid Heisenberg effect / noise
        if (context.Request.Path.StartsWithSegments("/api/monitoring"))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        bool isError = false;

        try
        {
            await _next(context);

            if (context.Response.StatusCode >= 400)
            {
                isError = true;
            }
        }
        catch (Exception)
        {
            isError = true;
            throw; // Re-throw to let global exception handler handle it, but we counted it as error
        }
        finally
        {
            stopwatch.Stop();
            _monitoringService.RecordRequest(stopwatch.ElapsedMilliseconds, isError);
        }
    }
}
