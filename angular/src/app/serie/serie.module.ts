import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { SeriesComponent } from './series/series.component';
import { SerieRoutingModule } from './serie-routing.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
@NgModule({
  declarations: [SeriesComponent],
  imports: [
    CommonModule,
    SharedModule,
    SerieRoutingModule,
    NgxDatatableModule
  ]
})
export class SerieModule { }