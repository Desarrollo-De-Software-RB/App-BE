import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { UserDto, UserFullDto, CreateUserDto, UpdateUserDto } from './models';

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

    create = (input: CreateUserDto) =>
        this.restService.request<any, UserDto>({
            method: 'POST',
            url: '/api/app/user',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: UpdateUserDto) =>
        this.restService.request<any, UserDto>({
            method: 'PUT',
            url: `/api/app/user/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/user/${id}`,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
