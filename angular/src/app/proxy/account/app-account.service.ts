import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { IdentityUserDto } from '@abp/ng.identity/proxy';
import type { AppRegisterDto, AppUpdateProfileDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class AppAccountService {
    apiName = 'Default';

    register = (input: AppRegisterDto) =>
        this.restService.request<any, IdentityUserDto>({
            method: 'POST',
            url: '/api/app/app-account/register',
            body: input,
        },
            { apiName: this.apiName });



    updateProfile = (input: AppUpdateProfileDto) =>
        this.restService.request<any, IdentityUserDto>({
            method: 'PUT',
            url: '/api/app/app-account/profile',
            body: input,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
