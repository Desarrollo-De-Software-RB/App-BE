using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TvTracker.Notificationes
{
    public interface IUserEngagementService : ITransientDependency
    {
        Task AnalyzeUserEngagementAsync();
    }
}
