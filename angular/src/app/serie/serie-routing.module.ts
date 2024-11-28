import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SeriesComponent } from './series/series.component';
const routes: Routes = [
  { path: '', component: SeriesComponent } // Ruta por defecto
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SerieRoutingModule { }
