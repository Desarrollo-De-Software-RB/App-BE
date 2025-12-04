import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { PermissionService } from '@abp/ng.core';

@Injectable({
    providedIn: 'root'
})
export class AdminGuard implements CanActivate {
    constructor(private permissionService: PermissionService, private router: Router) { }

    canActivate(): boolean {
        if (this.permissionService.getGrantedPolicy('TvTracker.AdminOptions')) {
            return true;
        }
        this.router.navigate(['/']);
        return false;
    }
}
