using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvTracker.EntityFrameworkCore;
using Xunit;

namespace TvTracker.Notificationes
{
    [Collection(TvTrackerTestConsts.CollectionDefinitionName)]
    public class EfCoreOmdbService_Tests : NotificationWorker_Tests
    {
    }
}