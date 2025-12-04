import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SerieDto, SerieService } from '@proxy/series';
import { ListService } from '@abp/ng.core';
import { SearchStateService } from '../search-state.service';
import { WatchlistService } from '../../proxy/watchlists/watchlist.service';
import { CreateUpdateWatchlistItemDto, WatchlistStatus, WatchlistItemDto } from '../../proxy/watchlists/models';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AddToWatchlistModalComponent } from '../../watchlist/add-to-watchlist-modal/add-to-watchlist-modal.component';

@Component({
    selector: 'app-search-series',
    templateUrl: './search-series.component.html',
    styleUrls: ['./search-series.component.scss'],
    providers: [ListService],
})
export class SearchSeriesComponent implements OnInit {
    series: SerieDto[] = [];
    searchTitle: string = '';
    searchGenre: string = '';
    searchType: string = '';
    hasSearched: boolean = false;
    userWatchlist: Map<string, WatchlistItemDto> = new Map();
    WatchlistStatus = WatchlistStatus;

    constructor(
        private serieService: SerieService,
        private router: Router,
        private searchStateService: SearchStateService,
        private watchlistService: WatchlistService,
        private modalService: NgbModal
    ) { }

    ngOnInit() {
        this.restoreState();
        this.loadUserWatchlist();
    }

    loadUserWatchlist() {
        this.watchlistService.getList().subscribe(items => {
            this.userWatchlist.clear();
            items.forEach(item => {
                if (item.serie && item.serie.imdbid) {
                    this.userWatchlist.set(item.serie.imdbid, item);
                }
            });
        });
    }

    getWatchlistItem(imdbId: string): WatchlistItemDto | undefined {
        return this.userWatchlist.get(imdbId);
    }

    isInWatchlist(imdbId: string): boolean {
        return this.userWatchlist.has(imdbId);
    }

    restoreState() {
        this.series = this.searchStateService.series;
        this.searchTitle = this.searchStateService.searchTitle;
        this.searchGenre = this.searchStateService.searchGenre;
        this.searchType = this.searchStateService.searchType;
        this.hasSearched = this.searchStateService.hasSearched;
    }

    search() {
        if (!this.searchTitle && !this.searchGenre) {
            return;
        }

        // Save search criteria
        this.searchStateService.searchTitle = this.searchTitle;
        this.searchStateService.searchGenre = this.searchGenre;
        this.searchStateService.searchType = this.searchType;

        this.serieService.search(this.searchTitle, this.searchGenre, this.searchType).subscribe((response) => {
            this.series = response;
            this.hasSearched = true;

            // Save results
            this.searchStateService.series = this.series;
            this.searchStateService.hasSearched = true;
        });
    }

    handleImageError(serie: SerieDto) {
        serie.poster = 'N/A';
    }

    viewDetails(imdbId: string) {
        this.router.navigate(['/series/search', imdbId]);
    }

    openAddToWatchlistModal(serie: SerieDto, event: Event) {
        event.stopPropagation();
        const modalRef = this.modalService.open(AddToWatchlistModalComponent, { centered: true });
        modalRef.componentInstance.title = serie.title;

        const existingItem = this.getWatchlistItem(serie.imdbid || '');
        if (existingItem) {
            modalRef.componentInstance.currentStatus = existingItem.status;
        }

        modalRef.result.then((status: WatchlistStatus) => {
            if (status !== undefined) {
                this.saveToWatchlist(serie, status);
            }
        }, () => { });
    }

    saveToWatchlist(serie: SerieDto, status: WatchlistStatus) {
        const input: CreateUpdateWatchlistItemDto = {
            imdbId: serie.imdbid || '',
            status: status
        };

        const existingItem = this.getWatchlistItem(serie.imdbid || '');

        if (existingItem) {
            // Update
            this.watchlistService.updateStatus(input).subscribe(() => {
                this.loadUserWatchlist(); // Reload to update local state
            });
        } else {
            // Add
            this.watchlistService.addItem(input).subscribe(() => {
                this.loadUserWatchlist(); // Reload to update local state
            });
        }
    }

    getStatusLabel(status: WatchlistStatus): string {
        return WatchlistStatus[status];
    }
}
