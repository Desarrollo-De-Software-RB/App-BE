import { Component, ElementRef, HostListener, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { NotificationDto, NotificationType } from '../../proxy/notificationes/models';
import { NotificationService } from '../../proxy/notificationes/notification.service';
import { Subscription } from 'rxjs';
import { NotificationStateService } from '../../services/notification-state.service';

interface NotificationItem extends NotificationDto {
    isAnimating?: boolean;
}

@Component({
    selector: 'app-notification-bell',
    templateUrl: './notification-bell.component.html',
    styleUrls: ['./notification-bell.component.scss']
})
export class NotificationBellComponent implements OnInit, OnDestroy {
    notifications: NotificationItem[] = [];
    unreadCount = 0;
    isDropdownOpen = false;
    private pollingSubscription: Subscription;

    constructor(
        private notificationService: NotificationService,
        private router: Router,
        private eRef: ElementRef,
        private authService: AuthService,
        private notificationStateService: NotificationStateService,
        private cdr: ChangeDetectorRef
    ) { }

    get hasLoggedIn(): boolean {
        return this.authService.isAuthenticated;
    }

    get unreadNotifications(): NotificationItem[] {
        return this.notifications.filter(n => !n.isRead);
    }

    ngOnInit(): void {
        if (!this.hasLoggedIn) {
            return;
        }

        // Initial load
        this.loadNotifications();

        // Subscribe to state changes
        this.pollingSubscription = this.notificationStateService.refresh$
            .subscribe(() => {
                this.loadNotifications();
            });
    }

    loadNotifications() {
        this.notificationService.getList().subscribe({
            next: (items) => {
                this.notifications = items;
                this.unreadCount = items.filter(n => !n.isRead).length;
                this.cdr.markForCheck();
            },
            error: (err) => console.error('Error fetching notifications', err)
        });
    }

    ngOnDestroy(): void {
        if (this.pollingSubscription) {
            this.pollingSubscription.unsubscribe();
        }
    }

    toggleDropdown() {
        this.isDropdownOpen = !this.isDropdownOpen;
        if (this.isDropdownOpen) {
            // Optional: Trigger refresh when opening
            this.refreshNotifications();
        }
    }

    refreshNotifications() {
        this.loadNotifications();
    }

    @HostListener('document:click', ['$event'])
    clickout(event: any) {
        if (!this.eRef.nativeElement.contains(event.target)) {
            this.isDropdownOpen = false;
        }
    }

    markAsRead(notification: NotificationItem, event: Event) {
        event.stopPropagation();
        if (!notification.isRead) {
            notification.isAnimating = true;

            // Wait for animation (consistent with page)
            setTimeout(() => {
                // Optimistic update
                notification.isRead = true;
                notification.isAnimating = false;
                this.unreadCount = this.notifications.filter(n => !n.isRead).length;
                this.cdr.markForCheck();

                this.notificationService.markAsRead(notification.id).subscribe({
                    error: () => {
                        notification.isRead = false;
                        this.unreadCount = this.notifications.filter(n => !n.isRead).length;
                        this.cdr.markForCheck();
                    }
                });
            }, 500); // 500ms delay for animation
        }
    }

    markAllAsRead() {
        this.notificationService.markAllAsRead().subscribe(() => {
            this.notifications.forEach(n => n.isRead = true);
            this.unreadCount = 0;
        });
    }

    viewAll() {
        this.router.navigate(['/notifications']);
        this.isDropdownOpen = false;
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
}
