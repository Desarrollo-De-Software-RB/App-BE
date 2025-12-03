import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class AdminService {
    apiName = 'Default';

    getDeveloperOptions = () =>
        this.restService.request<any, string>({
            method: 'POST',
            url: '/api/app/admin/developer-options',
            responseType: 'text',
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
