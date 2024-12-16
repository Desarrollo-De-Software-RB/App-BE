using System.Threading.Tasks;

namespace TvTracker.Notificationes
{
    public interface INotificationService
    {
        Task SendNotificationsAsync();
    }
}