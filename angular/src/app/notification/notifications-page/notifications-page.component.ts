import { Component, OnInit } from '@angular/core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { AuthService } from '@abp/ng.core';
import { NotificationDto, NotificationType } from '../../proxy/notificationes/models';
import { NotificationService } from '../../proxy/notificationes/notification.service';

// Extend DTO for local animation state
interface NotificationItem extends NotificationDto {
    isAnimating?: boolean;
}

@Component({
    selector: 'app-notifications-page',
    templateUrl: './notifications-page.component.html',
    styleUrls: ['./notifications-page.component.scss']
})
export class NotificationsPageComponent implements OnInit {
    notifications: NotificationItem[] = [];
    filteredNotifications: NotificationItem[] = [];
    filter: 'all' | 'unread' | 'read' = 'unread'; // Default to unread
    loading = false;
    showSettings = false;

    constructor(
        private notificationService: NotificationService,
        private confirmation: ConfirmationService,
        private authService: AuthService
    ) { }

    ngOnInit(): void {
        if (!this.authService.isAuthenticated) {
            this.authService.navigateToLogin();
            return;
        }
        this.loadNotifications();
    }

    loadNotifications() {
        this.loading = true;
        this.notificationService.getList().subscribe(items => {
            this.notifications = items;
            this.filterNotifications();
            this.loading = false;
        });
    }

    filterNotifications() {
        if (this.filter === 'unread') {
            this.filteredNotifications = this.notifications.filter(n => !n.isRead);
        } else if (this.filter === 'read') {
            this.filteredNotifications = this.notifications.filter(n => n.isRead);
        } else {
            this.filteredNotifications = this.notifications;
        }
    }

    setFilter(filter: 'all' | 'unread' | 'read') {
        this.filter = filter;
        this.filterNotifications();
    }

    markAsRead(notification: NotificationItem) {
        if (!notification.isRead) {
            notification.isAnimating = true;

            // Wait for animation
            setTimeout(() => {
                this.notificationService.markAsRead(notification.id).subscribe(() => {
                    notification.isRead = true;
                    notification.isAnimating = false;
                    this.updateLocalState();
                });
            }, 800); // 800ms animation
        }
    }

    markAllAsRead() {
        this.notificationService.markAllAsRead().subscribe(() => {
            this.notifications.forEach(n => n.isRead = true);
            this.updateLocalState();
        });
    }

    deleteAllRead() {
        this.confirmation.warn('Are you sure you want to delete all read notifications?', 'Confirm Deletion').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.notificationService.deleteAllRead().subscribe(() => {
                    this.notifications = this.notifications.filter(n => !n.isRead);
                    this.updateLocalState();
                });
            }
        });
    }

    deleteAllUnread() {
        this.confirmation.warn('Are you sure you want to delete all UNREAD notifications?', 'Confirm Deletion').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.notificationService.deleteAllUnread().subscribe(() => {
                    this.notifications = this.notifications.filter(n => n.isRead);
                    this.updateLocalState();
                });
            }
        });
    }

    deleteAll() {
        this.confirmation.warn('Are you sure you want to delete ALL notifications?', 'Confirm Deletion').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.notificationService.deleteAll().subscribe(() => {
                    this.notifications = [];
                    this.updateLocalState();
                });
            }
        });
    }

    deleteNotification(notification: NotificationDto, event: Event) {
        event.stopPropagation();
        this.confirmation.warn('Are you sure you want to delete this notification?', 'Confirm Deletion').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.notificationService.delete(notification.id).subscribe(() => {
                    this.notifications = this.notifications.filter(n => n.id !== notification.id);
                    this.updateLocalState();
                });
            }
        });
    }

    private updateLocalState() {
        this.filterNotifications();
    }

    toggleSettings() {
        this.showSettings = !this.showSettings;
    }

    getIconForType(type: NotificationType): string {
        switch (type) {
            case NotificationType.RatingChange: return 'bi-star-fill text-warning';
            case NotificationType.UserRating: return 'bi-stars text-warning';
            case NotificationType.VotesChange: return 'bi-graph-up-arrow text-info';
            case NotificationType.PosterChange: return 'bi-file-image text-primary';
            case NotificationType.StatusChange: return 'bi-info-circle-fill text-success';
            case NotificationType.System: return 'bi-gear-fill text-secondary';
            case NotificationType.PlotChange: return 'bi-card-text text-danger';
            case NotificationType.AwardsChange: return 'bi-trophy-fill text-warning';
            case NotificationType.RuntimeChange: return 'bi-clock-fill text-success';
            case NotificationType.UserActivity: return 'bi-list-ul text-primary';
            case NotificationType.Trend: return 'bi-activity text-danger';
            case NotificationType.Reminder: return 'bi-alarm-fill text-warning';
            default: return 'bi-bell-fill';
        }
    }

    getTypeName(type: NotificationType): string {
        return NotificationType[type].replace(/([A-Z])/g, ' $1').trim();
    }
}
