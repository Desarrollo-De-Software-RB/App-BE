using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;

using Volo.Abp.Uow; // Added using

namespace TvTracker.Notificationes
{
    public class NotificationWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificationWorker(
            AbpAsyncTimer timer, 
            IServiceScopeFactory serviceScopeFactory,
            IUnitOfWorkManager unitOfWorkManager)
            : base(timer, serviceScopeFactory)
        {
            timer.Period = 3600000; // 1 hour (Standard interval)
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            using (var uow = _unitOfWorkManager.Begin(new AbpUnitOfWorkOptions(), true))
            {
                // Resolve service from the scope created for this run
                var seriesDetectionService = workerContext.ServiceProvider.GetRequiredService<ISeriesChangeDetectionService>();
                var userEngagementService = workerContext.ServiceProvider.GetRequiredService<IUserEngagementService>();
             
                // Run detection
                await seriesDetectionService.DetectChangesAsync();
                
                // Run analysis for user engagement (Reminders, Trends)
                await userEngagementService.AnalyzeUserEngagementAsync();
                await uow.CompleteAsync();
            }
        }
    }
}
