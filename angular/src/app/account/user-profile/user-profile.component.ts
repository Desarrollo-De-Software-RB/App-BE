import { Component, OnInit, ElementRef, HostListener } from '@angular/core';
import { AuthService, ConfigStateService, RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
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
        // Fetch the full profile which contains extraProperties like ProfilePicture
        this.userProfile$ = this.restService.request({
            method: 'GET',
            url: '/api/account/my-profile',
        }).pipe(
            map(profile => {
                console.log('UserProfileComponent - Fetched Profile:', profile);
                return profile;
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
