import { Component, OnInit } from '@angular/core';
import { UserService } from '../proxy/users/user.service';
import { UserDto, UserFullDto, CreateUserDto, UpdateUserDto } from '../proxy/users/models';
import { PermissionService } from '@abp/ng.core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Confirmation, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';

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

    userForm: FormGroup;
    isEditMode = false;
    currentUserId: string | null = null;

    showPassword = false;
    showConfirmPassword = false;

    constructor(
        private userService: UserService,
        private permissionService: PermissionService,
        private modalService: NgbModal,
        private fb: FormBuilder,
        private confirmation: ConfirmationService,
        private toaster: ToasterService
    ) {
        this.createForm();
    }

    ngOnInit(): void {
        this.loadUsers();
        this.isAdmin = this.permissionService.getGrantedPolicy('TvTracker.AdminOptions');
    }

    loadUsers() {
        this.userService.getList().subscribe(users => {
            this.users = users;
        });
    }

    createForm() {
        this.userForm = this.fb.group({
            userName: ['', [Validators.required, Validators.maxLength(256)]],
            name: ['', [Validators.required, Validators.maxLength(64)]],
            surname: ['', [Validators.required, Validators.maxLength(64)]],
            email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
            password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$/)]],
            confirmPassword: [''],
            phoneNumber: ['', [Validators.maxLength(16)]],
            isActive: [true],
            profilePicture: ['']
        }, { validators: this.passwordMatchValidator });
    }

    passwordMatchValidator(form: FormGroup) {
        const password = form.get('password');
        const confirmPassword = form.get('confirmPassword');

        if (!password || !confirmPassword) {
            return null;
        }

        if (confirmPassword.errors && !confirmPassword.errors['mismatch']) {
            return null;
        }

        if (password.value !== confirmPassword.value) {
            confirmPassword.setErrors({ mismatch: true });
        } else {
            confirmPassword.setErrors(null);
        }

        return null;
    }

    togglePassword() {
        this.showPassword = !this.showPassword;
    }

    toggleConfirmPassword() {
        this.showConfirmPassword = !this.showConfirmPassword;
    }

    openDetails(user: UserDto, content: any) {
        if (!this.isAdmin) return;

        this.userService.get(user.id).subscribe(details => {
            this.selectedUser = details;
            this.modalService.open(content, { size: 'lg', windowClass: 'users-modal' });
        });
    }

    openCreateModal(content: any) {
        this.isEditMode = false;
        this.currentUserId = null;
        this.userForm.reset({ isActive: true });
        this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$/)]);
        this.userForm.get('password')?.updateValueAndValidity();
        this.userForm.get('confirmPassword')?.setValidators([Validators.required]);
        this.userForm.get('confirmPassword')?.updateValueAndValidity();
        this.modalService.open(content, { size: 'lg', windowClass: 'users-modal' });
    }

    openEditModal(user: UserDto, content: any, event: Event) {
        event.stopPropagation();
        this.isEditMode = true;
        this.currentUserId = user.id;

        this.userService.get(user.id).subscribe(details => {
            this.userForm.patchValue({
                userName: details.userName,
                name: details.name,
                surname: details.surname,
                email: details.email,
                phoneNumber: details.phoneNumber,
                isActive: details.isActive,
                profilePicture: details.profilePicture
            });

            // Password is not editable directly here, or make it optional
            this.userForm.get('password')?.clearValidators();
            this.userForm.get('password')?.updateValueAndValidity();
            this.userForm.get('confirmPassword')?.clearValidators();
            this.userForm.get('confirmPassword')?.updateValueAndValidity();

            this.modalService.open(content, { size: 'lg', windowClass: 'users-modal' });
        });
    }

    save(modal: any) {
        if (this.userForm.invalid) {
            return;
        }

        const formValue = this.userForm.value;

        if (this.isEditMode && this.currentUserId) {
            const input: UpdateUserDto = {
                userName: formValue.userName,
                name: formValue.name,
                surname: formValue.surname,
                email: formValue.email,
                phoneNumber: formValue.phoneNumber,
                isActive: formValue.isActive,
                profilePicture: formValue.profilePicture
            };

            this.userService.update(this.currentUserId, input).subscribe(() => {
                this.toaster.success('Usuario actualizado correctamente');
                this.loadUsers();
                modal.close();
            });
        } else {
            const input: CreateUserDto = {
                userName: formValue.userName,
                name: formValue.name,
                surname: formValue.surname,
                email: formValue.email,
                password: formValue.password,
                phoneNumber: formValue.phoneNumber,
                isActive: formValue.isActive,
                profilePicture: formValue.profilePicture
            };

            this.userService.create(input).subscribe(() => {
                this.toaster.success('Usuario creado correctamente');
                this.loadUsers();
                modal.close();
            });
        }
    }

    deleteUser(user: UserDto, event: Event) {
        event.stopPropagation();

        this.confirmation.warn(
            `¿Estás seguro de que deseas eliminar al usuario "${user.userName}"?`,
            'Confirmar eliminación'
        ).subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.userService.delete(user.id).subscribe(() => {
                    this.toaster.success('Usuario eliminado correctamente');
                    this.loadUsers();
                });
            }
        });
    }
}
