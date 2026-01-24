import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { NotificationDto, NotificationPreferenceDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class NotificationService {
    apiName = 'Default';

    // Subject to trigger manual refreshes
    public refresh$ = new Subject<void>();

    constructor(private restService: RestService) { }

    notifyStateChange() {
        this.refresh$.next();
    }

    getList = () =>
        this.restService.request<any, NotificationDto[]>({
            method: 'GET',
            url: '/api/app/notification',
        },
            { apiName: this.apiName });

    markAsRead = (id: string) =>
        this.restService.request<any, void>({
            method: 'POST',
            url: `/api/app/notification/${id}/mark-as-read`,
        },
            { apiName: this.apiName });

    markAllAsRead = () =>
        this.restService.request<any, void>({
            method: 'POST',
            url: '/api/app/notification/mark-all-as-read',
        },
            { apiName: this.apiName });

    getPreferences = () =>
        this.restService.request<any, NotificationPreferenceDto[]>({
            method: 'GET',
            url: '/api/app/notification/preferences',
        },
            { apiName: this.apiName });

    updatePreferences = (input: NotificationPreferenceDto[]) =>
        this.restService.request<any, void>({
            method: 'PUT',
            url: '/api/app/notification/preferences',
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/notification/${id}`,
        },
            { apiName: this.apiName });

    deleteAllRead = () =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: '/api/app/notification/delete-all-read',
        },
            { apiName: this.apiName });

    deleteAllUnread = () =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: '/api/app/notification/delete-all-unread',
        },
            { apiName: this.apiName });

    deleteAll = () =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: '/api/app/notification/delete-all',
        },
            { apiName: this.apiName });
}
