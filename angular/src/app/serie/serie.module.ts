import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { SeriesComponent } from './series/series.component';
import { SearchSeriesComponent } from './search-series/search-series.component';
import { SerieRoutingModule } from './serie-routing.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

@NgModule({
  declarations: [SeriesComponent, SearchSeriesComponent],
  imports: [
    CommonModule,
    SharedModule,
    SerieRoutingModule,
    NgxDatatableModule
  ]
})
export class SerieModule { }