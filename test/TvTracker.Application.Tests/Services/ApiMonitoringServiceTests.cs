using System;
using System.Threading;
using Shouldly;
using TvTracker.Monitoring;
using Xunit;

namespace TvTracker.Monitoring;

public class ApiMonitoringServiceTests
{
    [Fact]
    public void Should_Track_Requests()
    {
        // Arrange
        var service = new ApiMonitoringService();

        // Act
        service.RecordRequest(100, false);
        service.RecordRequest(200, false);

        // Assert
        var stats = service.GetStats();
        stats.TotalRequests.ShouldBe(2);
        stats.TotalErrors.ShouldBe(0);
        stats.AverageResponseTimeMs.ShouldBe(150);
    }

    [Fact]
    public void Should_Track_Errors()
    {
        // Arrange
        var service = new ApiMonitoringService();

        // Act
        service.RecordRequest(100, true);

        // Assert
        var stats = service.GetStats();
        stats.TotalErrors.ShouldBe(1);
        stats.LastMinuteErrors.ShouldBe(1);
    }

    [Fact]
    public void Should_Track_External_Requests()
    {
        // Arrange
        var service = new ApiMonitoringService();

        // Act
        service.RecordExternalRequest("OMDB");
        service.RecordExternalRequest("OMDB");
        service.RecordExternalRequest("Other");

        // Assert
        var stats = service.GetStats();
        stats.ExternalRequests.ShouldContainKeyAndValue("OMDB", 2);
        stats.ExternalRequests.ShouldContainKeyAndValue("Other", 1);
    }

    [Fact]
    public void Should_Calculate_Average_Latency()
    {
        // Arrange
        var service = new ApiMonitoringService();

        // Act
        service.RecordRequest(10, false);
        service.RecordRequest(20, false);
        service.RecordRequest(30, false);

        // Assert
        var stats = service.GetStats();
        stats.AverageResponseTimeMs.ShouldBe(20);
    }
}
