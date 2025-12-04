import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { UserDto, UserFullDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    apiName = 'Default';

    getList = () =>
        this.restService.request<any, UserDto[]>({
            method: 'GET',
            url: '/api/app/user',
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, UserFullDto>({
            method: 'GET',
            url: `/api/app/user/${id}`,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
