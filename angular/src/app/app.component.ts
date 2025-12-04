import { Component, OnInit } from '@angular/core';
import { RoutesService, ConfigStateService } from '@abp/ng.core';
import { NavItemsService } from '@abp/ng.theme.shared';
import { UserProfileComponent } from './account/user-profile/user-profile.component';
import { ReplaceableComponentsService } from '@abp/ng.core';
import { eAccountComponents } from '@abp/ng.account';
import { RegisterComponent } from './account/register/register.component';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar></abp-loader-bar>
    <abp-dynamic-layout></abp-dynamic-layout>
  `,
})
export class AppComponent implements OnInit {
  constructor(
    private navItems: NavItemsService,
    private routes: RoutesService,
    private config: ConfigStateService,
    private replaceableComponents: ReplaceableComponentsService
  ) {
    this.replaceableComponents.add({
      component: RegisterComponent,
      key: eAccountComponents.Register,
    });

    this.navItems.addItems([
      {
        id: 'MyProfilePicture',
        order: 100,
        component: UserProfileComponent,
      },
    ]);
  }

  ngOnInit() {
    this.config.getOne$('currentUser').subscribe(currentUser => {
      const isAuthenticated = currentUser?.isAuthenticated;
      const searchRoute = this.routes.find(r => r.name === 'Busqueda de contenido');
      if (searchRoute) {
        this.routes.patch('Busqueda de contenido', {
          invisible: !isAuthenticated
        });
      }
    });

    // Force hide default avatar (temporary fix to identify the element)
    setInterval(() => {
      const avatars = document.querySelectorAll('.lpx-avatar');
      avatars.forEach((avatar: any) => {
        // Hide if it's NOT inside our custom component
        if (!avatar.closest('app-user-profile')) {
          // Hide the entire container of the default user menu
          const container = avatar.closest('.lpx-menu-item') || avatar.closest('.dropdown') || avatar.parentElement;
          if (container) {
            container.style.display = 'none';
          }
        }
      });

      // Also try to find by icon class if lpx-avatar is not used
      const userIcons = document.querySelectorAll('.bi-person-circle, .fa-user');
      userIcons.forEach((icon: any) => {
        if (!icon.closest('app-user-profile') && icon.closest('.lpx-header-right')) {
          const container = icon.closest('.lpx-menu-item') || icon.closest('.dropdown') || icon.parentElement;
          if (container) {
            container.style.display = 'none';
          }
        }
      });
    }, 50);
  }
}
