import { Component, OnInit, ElementRef, HostListener } from '@angular/core';
import { AuthService, ConfigStateService, RestService } from '@abp/ng.core';
import { Observable, of } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
    selector: 'app-user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
    userProfile$: Observable<any>;

    isDropdownOpen = false;

    constructor(
        private config: ConfigStateService,
        private authService: AuthService,
        private router: Router,
        private restService: RestService,
        private eRef: ElementRef
    ) {
        console.log('UserProfileComponent initialized');
    }

    ngOnInit(): void {
        this.userProfile$ = this.config.getOne$('currentUser').pipe(
            map(currentUser => {
                if (!currentUser?.isAuthenticated) {
                    return null;
                }
                return currentUser;
            }),
            switchMap(currentUser => {
                if (!currentUser) return of(null);

                return this.restService.request({
                    method: 'GET',
                    url: '/api/account/my-profile',
                }).pipe(
                    map(profile => {
                        console.log('UserProfileComponent - Fetched Profile:', profile);
                        return profile;
                    }),
                    catchError(err => {
                        console.error('UserProfileComponent - Error fetching profile:', err);
                        return of(null);
                    })
                );
            })
        );
    }

    toggleDropdown() {
        this.isDropdownOpen = !this.isDropdownOpen;
    }

    @HostListener('document:click', ['$event'])
    clickout(event: any) {
        if (!this.eRef.nativeElement.contains(event.target)) {
            this.isDropdownOpen = false;
        }
    }

    navigateToMyAccount() {
        this.router.navigate(['/account/personal-settings']);
        this.isDropdownOpen = false;
    }

    logout() {
        this.authService.logout().subscribe();
        this.isDropdownOpen = false;
    }
}
