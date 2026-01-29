using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TvTracker.Notificationes
{
    public interface INotificationAppService : IApplicationService
    {
        Task<List<NotificationDto>> GetListAsync();
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
        Task DeleteAsync(Guid id);
        Task DeleteAllReadAsync();
        Task DeleteAllUnreadAsync();
        Task DeleteAllAsync();
        Task<List<NotificationPreferenceDto>> GetPreferencesAsync();
        Task UpdatePreferencesAsync(List<NotificationPreferenceDto> input);
    }
}
