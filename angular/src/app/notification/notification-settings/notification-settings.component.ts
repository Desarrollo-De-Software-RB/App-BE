import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NotificationChannel, NotificationPreferenceDto, NotificationType } from '../../proxy/notificationes/models';
import { NotificationService } from '../../proxy/notificationes/notification.service';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-notification-settings',
    templateUrl: './notification-settings.component.html',
    styleUrls: ['./notification-settings.component.scss']
})
export class NotificationSettingsComponent implements OnInit {
    preferences: NotificationPreferenceDto[] = [];
    loading = false;
    saving = false;

    notificationTypes = Object.values(NotificationType)
        .filter(value => typeof value === 'number')
        .filter(value => value !== NotificationType.AwardsChange) as NotificationType[];

    notificationChannels = Object.values(NotificationChannel).filter(value => typeof value === 'number') as NotificationChannel[];

    @Output() close = new EventEmitter<void>();

    constructor(private notificationService: NotificationService) { }

    ngOnInit(): void {
        this.loadPreferences();
    }

    loadPreferences() {
        this.loading = true;
        this.notificationService.getPreferences()
            .pipe(finalize(() => this.loading = false))
            .subscribe(prefs => {
                this.preferences = prefs;
            });
    }

    getPreference(type: NotificationType, channel: NotificationChannel): NotificationPreferenceDto {
        let pref = this.preferences.find(p => p.type === type && p.channel === channel);
        if (!pref) {
            pref = { type, channel, isEnabled: channel === NotificationChannel.InApp };
            this.preferences.push(pref);
        }
        return pref;
    }

    save() {
        this.saving = true;
        this.notificationService.updatePreferences(this.preferences)
            .pipe(finalize(() => this.saving = false))
            .subscribe(() => {
                this.close.emit();
            });
    }

    cancel() {
        this.close.emit();
    }

    getTypeLabel(type: NotificationType): string {
        switch (type) {
            case NotificationType.RatingChange: return 'Rating Updates';
            case NotificationType.UserRating: return 'My Ratings';
            case NotificationType.UserActivity: return 'My Watchlist Actions';
            case NotificationType.PosterChange: return 'New Art/Poster';
            case NotificationType.StatusChange: return 'Status Updates (Renewed/Ended)';
            case NotificationType.VotesChange: return 'Popularity Spikes';
            case NotificationType.PlotChange: return 'Plot/Synopsis Updates';
            case NotificationType.RuntimeChange: return 'Runtime Adjustment';
            default: return NotificationType[type];
        }
    }

    getChannelLabel(channel: NotificationChannel): string {
        return NotificationChannel[channel];
    }
}
