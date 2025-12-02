import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SeriesComponent } from './series/series.component';
import { SearchSeriesComponent } from './search-series/search-series.component';
const routes: Routes = [
  { path: '', component: SeriesComponent },
  { path: 'search', component: SearchSeriesComponent }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SerieRoutingModule { }
