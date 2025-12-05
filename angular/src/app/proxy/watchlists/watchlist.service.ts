import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { CreateUpdateWatchlistItemDto, WatchlistItemDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class WatchlistService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getList = () =>
        this.restService.request<any, WatchlistItemDto[]>({
            method: 'GET',
            url: '/api/app/watchlist-app-services',
        },
            { apiName: this.apiName });

    addItem = (input: CreateUpdateWatchlistItemDto) =>
        this.restService.request<any, WatchlistItemDto>({
            method: 'POST',
            url: '/api/app/watchlist-app-services',
            body: input,
        },
            { apiName: this.apiName });

    removeItem = (imdbId: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/watchlist-app-services/item/${imdbId}`,
        },
            { apiName: this.apiName });

    updateStatus = (input: CreateUpdateWatchlistItemDto) =>
        this.restService.request<any, void>({
            method: 'PUT',
            url: '/api/app/watchlist-app-services',
            body: input,
        },
            { apiName: this.apiName });
}
