import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

export interface MonitoringStatsDto {
  totalRequests: number;
  totalErrors: number;
  lastMinuteErrors: number;
  averageResponseTimeMs: number;
  uptimeSeconds: number;
}

@Injectable({
  providedIn: 'root'
})
export class MonitoringService {
  constructor(private restService: RestService) { }

  getStats(): Observable<MonitoringStatsDto> {
    return this.restService.request<void, MonitoringStatsDto>({
      method: 'GET',
      url: '/api/monitoring/stats',
    });
  }

  downloadLogs(startDate: string, endDate: string, levels: string[]): void {
    const params: any = { startDate, endDate };
    if (levels && levels.length > 0) {
      params.levels = levels.join(',');
    }

    this.restService.request({
      method: 'GET',
      url: '/api/monitoring/logs',
      params: params,
      responseType: 'blob',
    }).subscribe((response: any) => {
      this.downloadFile(response, `logs_${startDate}_${endDate}.txt`);
    });
  }

  private downloadFile(data: Blob, filename: string) {
    const blob = new Blob([data], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }
}
