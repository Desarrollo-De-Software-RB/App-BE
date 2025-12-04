import { Component, OnInit } from '@angular/core';
import { UserService } from '../proxy/users/user.service';
import { UserDto, UserFullDto } from '../proxy/users/models';
import { PermissionService } from '@abp/ng.core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-users',
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
    users: UserDto[] = [];
    selectedUser: UserFullDto | null = null;
    isAdmin = false;
    defaultProfilePicture = 'https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png';

    constructor(
        private userService: UserService,
        private permissionService: PermissionService,
        private modalService: NgbModal
    ) { }

    ngOnInit(): void {
        this.userService.getList().subscribe(users => {
            this.users = users;
        });
        // Check if user has admin permission. 
        // We used TvTrackerPermissions.AdminOptions in the backend.
        this.isAdmin = this.permissionService.getGrantedPolicy('TvTracker.AdminOptions');
    }

    openDetails(user: UserDto, content: any) {
        if (!this.isAdmin) return;

        this.userService.get(user.id).subscribe(details => {
            this.selectedUser = details;
            this.modalService.open(content, { size: 'lg' });
        });
    }
}
