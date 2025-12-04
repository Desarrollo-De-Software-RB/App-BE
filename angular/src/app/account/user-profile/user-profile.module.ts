import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserProfileComponent } from './user-profile.component';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { CoreModule } from '@abp/ng.core';

@NgModule({
    declarations: [UserProfileComponent],
    imports: [
        CommonModule,
        NgbDropdownModule,
        CoreModule
    ],
    exports: [UserProfileComponent]
})
// Module for User Profile
export class UserProfileModule { }
