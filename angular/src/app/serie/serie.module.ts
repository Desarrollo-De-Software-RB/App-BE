import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { SearchSeriesComponent } from './search-series/search-series.component';
import { SerieDetailComponent } from './serie-detail/serie-detail.component';
import { SerieRoutingModule } from './serie-routing.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

@NgModule({
  declarations: [SearchSeriesComponent, SerieDetailComponent],
  imports: [
    CommonModule,
    SharedModule,
    SerieRoutingModule,
    NgxDatatableModule
  ]
})
export class SerieModule { }