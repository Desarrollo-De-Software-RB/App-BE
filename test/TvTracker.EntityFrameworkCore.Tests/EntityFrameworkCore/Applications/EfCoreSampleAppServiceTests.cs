using TvTracker.Samples;
using Xunit;

namespace TvTracker.EntityFrameworkCore.Applications;

[Collection(TvTrackerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<TvTrackerEntityFrameworkCoreTestModule>
{

}
