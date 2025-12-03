import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppAccountService } from '../../proxy/account/app-account.service';
import { ToasterService } from '@abp/ng.theme.shared';
import { RestService } from '@abp/ng.core';

@Component({
    selector: 'app-personal-settings',
    templateUrl: './personal-settings.component.html',
})
export class PersonalSettingsComponent implements OnInit {
    form: FormGroup;

    constructor(
        private fb: FormBuilder,
        private appAccountService: AppAccountService,
        private toaster: ToasterService,
        private restService: RestService
    ) { }

    ngOnInit(): void {
        this.buildForm();
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
}
