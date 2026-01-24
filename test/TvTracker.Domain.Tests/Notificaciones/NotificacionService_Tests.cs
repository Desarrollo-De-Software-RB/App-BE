using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Threading.Tasks;
using TvTracker.Notificationes;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Xunit;

public class NotificationWorker_Tests
{
    // Subclass to expose protected DoWorkAsync for testing
    public class TestableNotificationWorker : NotificationWorker
    {
        public TestableNotificationWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            IUnitOfWorkManager unitOfWorkManager)
            : base(timer, serviceScopeFactory, unitOfWorkManager)
        {
        }

        public Task ExecuteDoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            return DoWorkAsync(workerContext);
        }
    }

    [Fact]
    public async Task Should_Call_Detection_Service()
    {
        // Arrange
        var seriesChangeDetectionService = Substitute.For<ISeriesChangeDetectionService>();
        
        var timer = Substitute.For<AbpAsyncTimer>();
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        
        // Setup UoW mock
        unitOfWorkManager.Begin(Arg.Any<AbpUnitOfWorkOptions>(), Arg.Any<bool>()).Returns(unitOfWork);

        // Setup ServiceProvider mock
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(ISeriesChangeDetectionService)).Returns(seriesChangeDetectionService);

        var workerContext = new PeriodicBackgroundWorkerContext(serviceProvider);

        var notificationWorker = new TestableNotificationWorker(
            timer,
            serviceScopeFactory,
            unitOfWorkManager
        );

        // Act
        await notificationWorker.ExecuteDoWorkAsync(workerContext);

        // Assert
        await seriesChangeDetectionService.Received(1).DetectChangesAsync();
        await unitOfWork.Received(1).CompleteAsync();
    }
}