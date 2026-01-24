using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;

namespace TvTracker.Notificationes
{
    public class NotificationManager : DomainService
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly IRepository<NotificationPreference, Guid> _preferenceRepository;
        private readonly IEmailSender _emailSender;
        private readonly Volo.Abp.Identity.IIdentityUserRepository _userRepository;
        private readonly ILogger<NotificationManager> _logger;

        public NotificationManager(
            IRepository<Notification, Guid> notificationRepository,
            IRepository<NotificationPreference, Guid> preferenceRepository,
            IEmailSender emailSender,
            Volo.Abp.Identity.IIdentityUserRepository userRepository,
            ILogger<NotificationManager> logger)
        {
            _notificationRepository = notificationRepository;
            _preferenceRepository = preferenceRepository;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task CreateAsync(
            Guid userId,
            string title,
            string message,
            NotificationType type,
            string relatedEntityId = null)
        {
            var prefs = await _preferenceRepository.GetListAsync(p => p.UserId == userId && p.Type == type);
            var inAppPref = prefs.FirstOrDefault(p => p.Channel == NotificationChannel.InApp);
            var emailPref = prefs.FirstOrDefault(p => p.Channel == NotificationChannel.Email);

            bool sendInApp = inAppPref?.IsEnabled ?? true; 
            bool sendEmail = emailPref?.IsEnabled ?? false;

            if (sendInApp)
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    RelatedEntityId = relatedEntityId,
                    IsRead = false
                };
                await _notificationRepository.InsertAsync(notification);
            }

            if (sendEmail)
            {
                await SendEmailAsync(userId, title, message, type);
            }
        }

        private async Task SendEmailAsync(Guid userId, string subject, string body, NotificationType type)
        {
            try
            {
                var user = await _userRepository.FindAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    _logger.LogWarning($"[NotificationManager] User {userId} not found or has no email.");
                    return;
                }

                _logger.LogInformation($"[NotificationManager] Sending email to {user.Email}: {subject}");
                
                var emailBody = GetEmailTemplate(subject, body, type);
                await _emailSender.SendAsync(user.Email, subject, emailBody, isBodyHtml: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[NotificationManager] Failed to send email to user {userId}");
            }
        }

        private string GetEmailTemplate(string title, string message, NotificationType type)
        {
            // Colors based on type
            string headerColor = "#4e73df";
            string icon = "🔔";

            switch (type)
            {
                case NotificationType.RatingChange:
                    headerColor = "#f6c23e";
                    icon = "⭐";
                    break;
                case NotificationType.UserActivity:
                    headerColor = "#1cc88a";
                    icon = "✅";
                    break;
                case NotificationType.PosterChange:
                    headerColor = "#36b9cc";
                    icon = "🎨";
                    break;
                 case NotificationType.VotesChange:
                    headerColor = "#e74a3b";
                    icon = "📈";
                    break;
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #121212; color: #e0e0e0; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 20px auto; background-color: #1e1e1e; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.5); }}
        .header {{ background-color: {headerColor}; color: white; padding: 20px; text-align: center; font-size: 24px; font-weight: bold; }}
        .content {{ padding: 30px; font-size: 16px; line-height: 1.6; color: #cccccc; }}
        .footer {{ background-color: #121212; padding: 15px; text-align: center; font-size: 12px; color: #777; }}
        .button {{ display: inline-block; padding: 10px 20px; margin-top: 20px; background-color: {headerColor}; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
        .highlight {{ color: {headerColor}; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            {icon} {title}
        </div>
        <div class='content'>
            <p>Hello,</p>
            <p>{message}</p>
            <br>
            <p>Keep tracking your favorite shows on <span class='highlight'>TvTracker</span>!</p>
            <center><a href='http://localhost:4200' class='button'>Open App</a></center>
        </div>
        <div class='footer'>
            &copy; 2026 TvTracker. All rights reserved.
        </div>
    </div>
</body>
</html>";
        }
    }
}
