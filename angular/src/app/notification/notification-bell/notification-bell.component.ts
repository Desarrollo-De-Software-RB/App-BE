import { Component, ElementRef, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { NotificationDto } from '../../proxy/notificationes/models';
import { NotificationService } from '../../proxy/notificationes/notification.service';
import { interval, Subscription } from 'rxjs';
import { startWith, switchMap, mergeWith } from 'rxjs/operators';

@Component({
    selector: 'app-notification-bell',
    templateUrl: './notification-bell.component.html',
    styleUrls: ['./notification-bell.component.scss']
})
export class NotificationBellComponent implements OnInit, OnDestroy {
    notifications: NotificationDto[] = [];
    unreadCount = 0;
    isDropdownOpen = false;
    private pollingSubscription: Subscription;

    constructor(
        private notificationService: NotificationService,
        private router: Router,
        private eRef: ElementRef,
        private authService: AuthService
    ) { }

    get hasLoggedIn(): boolean {
        return this.authService.isAuthenticated;
    }

    ngOnInit(): void {
        if (!this.hasLoggedIn) {
            return;
        }

        // Poll every 60 seconds OR when manually triggered
        this.pollingSubscription = interval(60000)
            .pipe(
                startWith(0),
                mergeWith(this.notificationService.refresh$), // Merge with manual refresh trigger
                switchMap(() => this.notificationService.getList())
            )
            .subscribe({
                next: (items) => {
                    this.notifications = items;
                    this.unreadCount = items.filter(n => !n.isRead).length;
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
        this.notificationService.getList().subscribe(items => {
            this.notifications = items;
            this.unreadCount = items.filter(n => !n.isRead).length;
        });
    }

    @HostListener('document:click', ['$event'])
    clickout(event: any) {
        if (!this.eRef.nativeElement.contains(event.target)) {
            this.isDropdownOpen = false;
        }
    }

    markAsRead(notification: NotificationDto) {
        if (!notification.isRead) {
            this.notificationService.markAsRead(notification.id).subscribe(() => {
                notification.isRead = true;
                this.unreadCount = this.notifications.filter(n => !n.isRead).length;
            });
        }
    }

    viewAll() {
        this.router.navigate(['/notifications']);
        this.isDropdownOpen = false;
    }
}
