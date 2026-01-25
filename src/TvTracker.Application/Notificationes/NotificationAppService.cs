using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TvTracker.Notificationes
{
    [Authorize]
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly IRepository<NotificationPreference, Guid> _preferenceRepository;

        public NotificationAppService(
            IRepository<Notification, Guid> notificationRepository,
            IRepository<NotificationPreference, Guid> preferenceRepository)
        {
            _notificationRepository = notificationRepository;
            _preferenceRepository = preferenceRepository;
        }

        public async Task<List<NotificationDto>> GetListAsync()
        {
            if (CurrentUser.Id == null) return new List<NotificationDto>();

            var notifications = await _notificationRepository.GetListAsync(n => n.UserId == CurrentUser.Id.Value);
            
            // Order by creation time desc
            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(
                notifications.OrderByDescending(n => n.CreationTime).Take(50).ToList()
            );
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException();
            }

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task MarkAllAsReadAsync()
        {
            if (CurrentUser.Id == null) return;
            
            var notifications = await _notificationRepository.GetListAsync(n => n.UserId == CurrentUser.Id.Value && !n.IsRead);
            foreach (var n in notifications)
            {
                n.IsRead = true;
            }
            await _notificationRepository.UpdateManyAsync(notifications);
        }

        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("api/app/notification/delete-all-read")]
        public async Task DeleteAllReadAsync()
        {
             if (CurrentUser.Id == null) return;
             var notifications = await _notificationRepository.GetListAsync(n => n.UserId == CurrentUser.Id.Value && n.IsRead);
             if (notifications.Any())
             {
                 await _notificationRepository.DeleteManyAsync(notifications);
             }
        }

        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("api/app/notification/delete-all-unread")]
        public async Task DeleteAllUnreadAsync()
        {
             if (CurrentUser.Id == null) return;
             var notifications = await _notificationRepository.GetListAsync(n => n.UserId == CurrentUser.Id.Value && !n.IsRead);
             if (notifications.Any())
             {
                 await _notificationRepository.DeleteManyAsync(notifications);
             }
        }

        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("api/app/notification/delete-all")]
        public async Task DeleteAllAsync()
        {
             if (CurrentUser.Id == null) return;
             var notifications = await _notificationRepository.GetListAsync(n => n.UserId == CurrentUser.Id.Value);
             if (notifications.Any())
             {
                 await _notificationRepository.DeleteManyAsync(notifications);
             }
        }

        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("api/app/notification/{id}")]
        public async Task DeleteAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException();
            }
            await _notificationRepository.DeleteAsync(notification);
        }

        public async Task<List<NotificationPreferenceDto>> GetPreferencesAsync()
        {
             if (CurrentUser.Id == null) return new List<NotificationPreferenceDto>();
             
             var prefs = await _preferenceRepository.GetListAsync(p => p.UserId == CurrentUser.Id.Value);
             var dtos = new List<NotificationPreferenceDto>();

             // Enum all possibilities to return full matrix
             foreach (NotificationType type in Enum.GetValues(typeof(NotificationType)))
             {
                 foreach (NotificationChannel channel in Enum.GetValues(typeof(NotificationChannel)))
                 {
                     var existing = prefs.FirstOrDefault(p => p.Type == type && p.Channel == channel);
                     dtos.Add(new NotificationPreferenceDto
                     {
                         Type = type,
                         Channel = channel,
                         IsEnabled = existing?.IsEnabled ?? (channel == NotificationChannel.InApp) // Default InApp=True
                     });
                 }
             }

             return dtos;
        }

        public async Task UpdatePreferencesAsync(List<NotificationPreferenceDto> input)
        {
            if (CurrentUser.Id == null) return;

            var existingPrefs = await _preferenceRepository.GetListAsync(p => p.UserId == CurrentUser.Id.Value);
            
            foreach (var item in input)
            {
                var matches = existingPrefs.Where(p => p.Type == item.Type && p.Channel == item.Channel).ToList();
                if (matches.Any())
                {
                    // Update first, delete duplicates if any
                    var first = matches.First();
                    first.IsEnabled = item.IsEnabled;
                    await _preferenceRepository.UpdateAsync(first, autoSave: true);

                    if (matches.Count > 1)
                    {
                        var duplicates = matches.Skip(1);
                        await _preferenceRepository.DeleteManyAsync(duplicates, autoSave: true);
                    }
                }
                else
                {
                    await _preferenceRepository.InsertAsync(new NotificationPreference(
                        GuidGenerator.Create(),
                        CurrentUser.Id.Value,
                        item.Type,
                        item.Channel,
                        item.IsEnabled
                    ), autoSave: true);
                }
            }
        }
    }
}
