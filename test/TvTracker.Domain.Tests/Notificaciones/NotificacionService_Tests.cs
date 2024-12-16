using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvTracker.Notificationes;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Xunit;

public class NotificationWorker_Tests
{
    [Fact]
    public async Task Should_Notify_On_Series_Change()
    {
        var userId = Guid.NewGuid();

        // Crear dos series de prueba
        var trackedSeries1 = new TrackedSeries
        {
            SeriesId = Guid.NewGuid(),
            Name = "Serie de prueba 1",
            UserId = userId,
            LastUpdated = DateTime.UtcNow
        };

        var trackedSeries2 = new TrackedSeries
        {
            SeriesId = Guid.NewGuid(),
            Name = "Serie de prueba 2",
            UserId = userId,
            LastUpdated = DateTime.UtcNow
        };

        // Arrange
        var seriesChangeDetectionService = Substitute.For<ISeriesChangeDetectionService>();
        var notificationRepository = Substitute.For<IRepository<Notification, Guid>>();

        // Simular el comportamiento de DetectChangesAsync
        var expectedNotification1 = new Notification
        {
            Message = $"La serie {trackedSeries1.Name} ha tenido actualizaciones.",
            Method = "PushNotification",
            UserId = userId
        };

        var expectedNotification2 = new Notification
        {
            Message = $"La serie {trackedSeries2.Name} ha tenido actualizaciones.",
            Method = "PushNotification",
            UserId = userId
        };

        seriesChangeDetectionService.DetectChangesAsync().Returns(new List<Notification>
    {
        expectedNotification1,
        expectedNotification2
    });

        // Instanciar el NotificationWorker
        var timer = Substitute.For<AbpAsyncTimer>();
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();

        var notificationWorker = new NotificationWorker(
            timer,
            serviceScopeFactory,
            seriesChangeDetectionService,
            notificationRepository
        );

        // Act
        await notificationWorker.SendNotificationsOnSeriesChange();

        // Assert
        await notificationRepository.Received(1).InsertAsync(expectedNotification1, Arg.Any<bool>());
        await notificationRepository.Received(1).InsertAsync(expectedNotification2, Arg.Any<bool>());
    }
}