using TvTracker.Samples;
using Xunit;

namespace TvTracker.EntityFrameworkCore.Domains;

[Collection(TvTrackerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<TvTrackerEntityFrameworkCoreTestModule>
{

}
