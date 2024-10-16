using Xunit;

namespace TvTracker.EntityFrameworkCore;

[CollectionDefinition(TvTrackerTestConsts.CollectionDefinitionName)]
public class TvTrackerEntityFrameworkCoreCollection : ICollectionFixture<TvTrackerEntityFrameworkCoreFixture>
{

}
