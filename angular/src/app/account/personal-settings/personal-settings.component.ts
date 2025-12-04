import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppAccountService } from '../../proxy/account/app-account.service';
import { ToasterService } from '@abp/ng.theme.shared';
import { RestService } from '@abp/ng.core';

@Component({
    selector: 'app-personal-settings',
    templateUrl: './personal-settings.component.html',
    styleUrls: ['./personal-settings.component.scss']
})
export class PersonalSettingsComponent implements OnInit {
    form: FormGroup;
    passwordForm: FormGroup;
    activeTab: 'profile' | 'password' = 'profile';

    constructor(
        private fb: FormBuilder,
        private appAccountService: AppAccountService,
        private toaster: ToasterService,
        private restService: RestService
    ) { }

    ngOnInit(): void {
        this.buildForm();
        this.buildPasswordForm();
        this.getProfile();
    }

    buildForm() {
        this.form = this.fb.group({
            userName: [''],
            email: ['', [Validators.required, Validators.email]],
            name: [''],
            surname: [''],
            phoneNumber: [''],
            profilePicture: ['']
        });
    }

    buildPasswordForm() {
        this.passwordForm = this.fb.group({
            currentPassword: ['', [Validators.required]],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            repeatNewPassword: ['', [Validators.required]]
        }, { validators: this.passwordMatchValidator });
    }

    passwordMatchValidator(g: FormGroup) {
        return g.get('newPassword').value === g.get('repeatNewPassword').value
            ? null : { mismatch: true };
    }

    getProfile() {
        this.restService.request<void, any>({
            method: 'GET',
            url: '/api/account/my-profile',
        }).subscribe(profile => {
            this.form.patchValue(profile);
            const extraProperties = profile.extraProperties;
            if (extraProperties && extraProperties.ProfilePicture) {
                this.form.patchValue({ profilePicture: extraProperties.ProfilePicture });
            }
        });
    }

    save() {
        if (this.form.invalid) return;

        this.appAccountService.updateProfile(this.form.value).subscribe({
            next: () => {
                this.toaster.success('Profile updated successfully.');
                this.form.markAsPristine();
            },
            error: (err) => {
                this.toaster.error(err.error?.error?.message || 'Update failed');
            }
        });
    }

    changePassword() {
        if (this.passwordForm.invalid) return;

        const { currentPassword, newPassword } = this.passwordForm.value;

        this.restService.request<any, void>({
            method: 'POST',
            url: '/api/account/my-profile/change-password',
            body: { currentPassword, newPassword }
        }).subscribe({
            next: () => {
                this.toaster.success('Password changed successfully.');
                this.passwordForm.reset();
            },
            error: (err) => {
                this.toaster.error(err.error?.error?.message || 'Password change failed');
            }
        });
    }

    setActiveTab(tab: 'profile' | 'password') {
        this.activeTab = tab;
    }
}
