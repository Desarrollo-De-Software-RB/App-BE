import { Component, OnInit } from '@angular/core';
import { RoutesService, ConfigStateService } from '@abp/ng.core';
import { NavItemsService } from '@abp/ng.theme.shared';
import { UserProfileComponent } from './account/user-profile/user-profile.component';

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
    private config: ConfigStateService
  ) {
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
  }
}
