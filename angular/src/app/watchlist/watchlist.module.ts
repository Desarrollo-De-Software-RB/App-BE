import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { WatchlistRoutingModule } from './watchlist-routing.module';
import { WatchlistComponent } from './watchlist.component';


import { AddToWatchlistModalComponent } from './add-to-watchlist-modal/add-to-watchlist-modal.component';

@NgModule({
  declarations: [
    WatchlistComponent,
    AddToWatchlistModalComponent
  ],
  imports: [
    SharedModule,
    WatchlistRoutingModule
  ],
  exports: [
    AddToWatchlistModalComponent // Exporting so it can be used in other modules if needed (e.g. Search)
  ]
})
export class WatchlistModule { }
