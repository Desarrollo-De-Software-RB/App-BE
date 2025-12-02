import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SearchSeriesComponent } from './search-series/search-series.component';
import { SerieDetailComponent } from './serie-detail/serie-detail.component';

const routes: Routes = [
  { path: 'search', component: SearchSeriesComponent },
  { path: 'search/:imdbId', component: SerieDetailComponent }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SerieRoutingModule { }
