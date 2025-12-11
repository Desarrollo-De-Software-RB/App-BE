import { AuthService, PermissionService } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppAccountService } from '../proxy/account/app-account.service';
import { ToasterService } from '@abp/ng.theme.shared';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  registerForm: FormGroup;
  showRegister = false;
  isAdmin = false;

  // Splash Screen State
  showSplash = true;
  logoMoved = false;
  showContent = false;
  titleChars = 'Tracker'.split('');

  constructor(
    private authService: AuthService,
    private fb: FormBuilder,
    private appAccountService: AppAccountService,
    private toaster: ToasterService,
    private permissionService: PermissionService,
    private router: Router
  ) { }

  ngOnInit() {
    this.buildForm();
    if (this.hasLoggedIn) {
      this.checkAdmin();
    }

    // Start Animation Flow
    this.startSplashAnimation();
  }

  startSplashAnimation() {
    // Phase 1: Logo loads letter by letter (Total approx 2s)

    // Phase 2: Move Logo Up after animation completes
    setTimeout(() => {
      this.logoMoved = true;

      // Phase 3: Show Content
      setTimeout(() => {
        this.showContent = true;
      }, 800);
    }, 1200);
  }

  buildForm() {
    this.registerForm = this.fb.group({
      userName: ['', Validators.required],
      emailAddress: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      name: ['', Validators.required],
      surname: ['', Validators.required],
      profilePicture: ['']
    });
  }

  checkAdmin() {
    this.isAdmin = this.permissionService.getGrantedPolicy('TvTracker.AdminOptions');
  }

  login() {
    this.authService.navigateToLogin();
  }

  toggleRegister() {
    this.showRegister = !this.showRegister;
  }

  register() {
    if (this.registerForm.invalid) {
      return;
    }

    this.appAccountService.register(this.registerForm.value).subscribe({
      next: () => {
        this.toaster.success('Registration successful! Please login.');
        this.showRegister = false;
        this.login();
      },
      error: (err) => {
        this.toaster.error(err.error?.error?.message || 'Registration failed');
      }
    });
  }

  goToSearch() {
    this.router.navigate(['/series/search']);
  }
}
