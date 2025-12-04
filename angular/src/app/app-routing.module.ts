import { authGuard, permissionGuard, eLayoutType } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonalSettingsComponent } from './account/personal-settings/personal-settings.component';
import { RegisterComponent } from './account/register/register.component';


const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
  },
  {
    path: 'account/personal-settings',
    component: PersonalSettingsComponent,
    canActivate: [authGuard]
  },

  {
    path: 'register',
    component: RegisterComponent,
    data: {
      layout: eLayoutType.empty
    }
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(m => m.AccountModule.forLazy()),
  },
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule),
  },
  {
    path: 'series',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./serie/serie.module').then(m => m.SerieModule),
  },
  {
    path: 'watchlist',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./watchlist/watchlist.module').then(m => m.WatchlistModule)
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {})],
  exports: [RouterModule],
})
export class AppRoutingModule { }
