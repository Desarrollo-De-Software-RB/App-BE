import { Component, OnInit } from '@angular/core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { AuthService } from '@abp/ng.core';
import { NotificationDto, NotificationType } from '../../proxy/notificationes/models';
import { NotificationService } from '../../proxy/notificationes/notification.service';

@Component({
    selector: 'app-notifications-page',
    templateUrl: './notifications-page.component.html',
    styleUrls: ['./notifications-page.component.scss']
})
export class NotificationsPageComponent implements OnInit {
    notifications: NotificationDto[] = [];
    filteredNotifications: NotificationDto[] = [];
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

    markAsRead(notification: NotificationDto) {
        if (!notification.isRead) {
            this.notificationService.markAsRead(notification.id).subscribe(() => {
                notification.isRead = true;
                this.updateLocalState();
            });
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
        if (!this.showSettings) {
        }
    }

    getIconForType(type: NotificationType): string {
        switch (type) {
            case NotificationType.RatingChange: return 'bi-star-fill text-warning';
            case NotificationType.VotesChange: return 'bi-graph-up-arrow text-info';
            case NotificationType.PosterChange: return 'bi-file-image text-primary';
            case NotificationType.StatusChange: return 'bi-info-circle-fill text-danger';
            case NotificationType.System: return 'bi-gear-fill text-secondary';
            default: return 'bi-bell-fill';
        }
    }
}
