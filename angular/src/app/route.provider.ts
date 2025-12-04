import { RoutesService, eLayoutType, AuthService } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  { provide: APP_INITIALIZER, useFactory: configureRoutes, deps: [RoutesService, AuthService], multi: true },
];

function configureRoutes(routes: RoutesService, authService: AuthService) {
  return () => {
    routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      }, {
        path: '/series/search',
        name: 'Busqueda de contenido',
        iconClass: 'fas fa-film',
        order: 2,
        layout: eLayoutType.application,
        invisible: true, // Will be managed dynamically in AppComponent
      },
      {
        path: '/watchlist',
        name: 'Mi Lista',
        iconClass: 'fas fa-list',
        order: 4,
        layout: eLayoutType.application,
        invisible: true,
      },
      {
        path: '/users',
        name: 'Usuarios',
        iconClass: 'fas fa-users',
        order: 3,
        layout: eLayoutType.application,
        invisible: true,
      },
    ]);
  };
}
