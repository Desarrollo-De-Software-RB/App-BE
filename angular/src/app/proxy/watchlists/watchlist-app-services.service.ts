import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class WatchlistAppServicesService {
  apiName = 'Default';
  

  addSerie = (serieId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/watchlist-app-services/serie/${serieId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
