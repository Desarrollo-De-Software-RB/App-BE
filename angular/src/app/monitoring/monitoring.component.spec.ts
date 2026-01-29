import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MonitoringComponent } from './monitoring.component';
import { MonitoringService } from '../services/monitoring.service';
import { of } from 'rxjs';

describe('MonitoringComponent', () => {
    let component: MonitoringComponent;
    let fixture: ComponentFixture<MonitoringComponent>;
    let mockMonitoringService: jasmine.SpyObj<MonitoringService>;

    beforeEach(async () => {
        mockMonitoringService = jasmine.createSpyObj('MonitoringService', ['getStats', 'downloadLogs']);
        mockMonitoringService.getStats.and.returnValue(of({
            totalRequests: 100,
            totalErrors: 5,
            lastMinuteErrors: 1,
            averageResponseTimeMs: 120,
            uptimeSeconds: 3600
        }));

        await TestBed.configureTestingModule({
            declarations: [MonitoringComponent],
            providers: [
                { provide: MonitoringService, useValue: mockMonitoringService }
            ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(MonitoringComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load stats on init', () => {
        expect(mockMonitoringService.getStats).toHaveBeenCalled();
        expect(component.stats).toBeDefined();
        expect(component.stats?.totalRequests).toBe(100);
    });

    it('should format uptime correctly', () => {
        const formatted = component.formatUptime(3661); // 1h 1m 1s
        expect(formatted).toContain('1h');
        expect(formatted).toContain('1m');
        expect(formatted).toContain('1s');
    });

    it('should call downloadLogs on service with filter', () => {
        component.downloadLogs();

        // Set some dates
        component.logFilter.startDate = '2023-01-01';
        component.logFilter.endDate = '2023-01-31';
        component.logFilter.levels.INF = true;
        component.logFilter.levels.ERR = false;

        component.confirmDownload();

        expect(mockMonitoringService.downloadLogs).toHaveBeenCalledWith('2023-01-01', '2023-01-31', ['INF', 'WRN', 'FTL']);
    });
});
