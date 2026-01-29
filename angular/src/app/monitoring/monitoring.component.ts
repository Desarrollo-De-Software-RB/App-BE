import { Component, OnDestroy, OnInit } from '@angular/core';
import { MonitoringService, MonitoringStatsDto } from '../services/monitoring.service';
import { interval, Subscription } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';

@Component({
    selector: 'app-monitoring',
    templateUrl: './monitoring.component.html',
    styleUrls: ['./monitoring.component.scss']
})
export class MonitoringComponent implements OnInit, OnDestroy {
    stats: MonitoringStatsDto | null = null;
    subscription: Subscription = new Subscription();
    loading = true;
    showModal = false;
    logFilter = {
        startDate: new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().substring(0, 10),
        endDate: new Date().toISOString().substring(0, 10),
        levels: {
            INF: true,
            DBG: false,
            ERR: true,
            WRN: true,
            FTL: true
        }
    };

    constructor(private monitoringService: MonitoringService) { }

    ngOnInit(): void {
        // Refresh every 60 seconds
        this.subscription = interval(60000)
            .pipe(
                startWith(0),
                switchMap(() => this.monitoringService.getStats())
            )
            .subscribe({
                next: (data) => {
                    this.stats = data;
                    this.loading = false;
                },
                error: (err) => {
                    console.error('Failed to load stats', err);
                    this.loading = false;
                }
            });
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }

    downloadLogs(): void {
        this.showModal = true;
    }

    closeModal(): void {
        this.showModal = false;
    }

    confirmDownload(): void {
        const selectedLevels = Object.entries(this.logFilter.levels)
            .filter(([_, checked]) => checked)
            .map(([level]) => level);

        this.monitoringService.downloadLogs(this.logFilter.startDate, this.logFilter.endDate, selectedLevels);
        this.showModal = false;
    }

    formatUptime(seconds: number): string {
        const d = Math.floor(seconds / (3600 * 24));
        const h = Math.floor((seconds % (3600 * 24)) / 3600);
        const m = Math.floor((seconds % 3600) / 60);
        const s = Math.floor(seconds % 60);

        const parts = [];
        if (d > 0) parts.push(`${d}d`);
        if (h > 0) parts.push(`${h}h`);
        if (m > 0) parts.push(`${m}m`);
        parts.push(`${s}s`);

        return parts.join(' ');
    }
}
