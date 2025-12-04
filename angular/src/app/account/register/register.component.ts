import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppAccountService } from '../../proxy/account/app-account.service';
import { AppRegisterDto } from '../../proxy/account/models';
import { ToasterService } from '@abp/ng.theme.shared';
import { Router } from '@angular/router';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
    form: FormGroup;
    isSubmitted = false;
    showPassword = false;
    showConfirmPassword = false;

    togglePassword() {
        this.showPassword = !this.showPassword;
    }

    toggleConfirmPassword() {
        this.showConfirmPassword = !this.showConfirmPassword;
    }

    constructor(
        private fb: FormBuilder,
        private accountService: AppAccountService,
        private toaster: ToasterService,
        private router: Router
    ) {
        console.log('RegisterComponent constructor called');
    }

    ngOnInit(): void {
        console.log('RegisterComponent initialized');
        this.buildForm();
    }

    buildForm() {
        this.form = this.fb.group({
            userName: ['', [Validators.required, Validators.maxLength(256)]],
            name: ['', [Validators.required, Validators.maxLength(64)]],
            surname: ['', [Validators.required, Validators.maxLength(64)]],
            email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
            phoneNumber: ['', [Validators.maxLength(16)]],
            profilePicture: ['', []], // Explicitly no validators
            password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$/)]],
            confirmPassword: ['', [Validators.required]]
        }, {
            validators: this.passwordMatchValidator
        });
    }

    passwordMatchValidator(form: FormGroup) {
        const password = form.get('password');
        const confirmPassword = form.get('confirmPassword');

        if (password && confirmPassword && password.value !== confirmPassword.value) {
            confirmPassword.setErrors({ mismatch: true });
        } else {
            if (confirmPassword?.hasError('mismatch')) {
                delete confirmPassword.errors?.['mismatch'];
                if (!Object.keys(confirmPassword.errors || {}).length) {
                    confirmPassword.setErrors(null);
                }
            }
        }
        return null;
    }

    onSubmit() {
        this.isSubmitted = true;

        if (this.form.invalid) {
            return;
        }

        const formValue = this.form.value;
        const input: AppRegisterDto = {
            userName: formValue.userName,
            name: formValue.name,
            surname: formValue.surname,
            emailAddress: formValue.email,
            phoneNumber: formValue.phoneNumber || null,
            password: formValue.password,
            profilePicture: formValue.profilePicture || null
        };

        this.accountService.register(input).subscribe({
            next: () => {
                this.toaster.success('Usuario registrado exitosamente', 'Éxito');
                this.router.navigate(['/account/login']);
            },
            error: (err) => {
                this.isSubmitted = false;
                this.toaster.error(err.error?.error?.message || 'Ocurrió un error al registrarse', 'Error');
            }
        });
    }
}
