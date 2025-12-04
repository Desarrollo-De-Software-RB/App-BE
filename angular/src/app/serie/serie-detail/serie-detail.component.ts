import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SerieService } from '../../proxy/series';
import { SerieDto } from '../../proxy/series/models';
import { WatchlistService } from '../../proxy/watchlists/watchlist.service';
import { CreateUpdateWatchlistItemDto, WatchlistStatus, WatchlistItemDto } from '../../proxy/watchlists/models';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AddToWatchlistModalComponent } from '../../watchlist/add-to-watchlist-modal/add-to-watchlist-modal.component';

@Component({
    selector: 'app-serie-detail',
    templateUrl: './serie-detail.component.html',
    styleUrls: ['./serie-detail.component.scss']
})
export class SerieDetailComponent implements OnInit {
    serie: SerieDto | null = null;
    loading = true;
    watchlistItem: WatchlistItemDto | null = null;
    WatchlistStatus = WatchlistStatus;

    constructor(
        private route: ActivatedRoute,
        private serieService: SerieService,
        private watchlistService: WatchlistService,
        private modalService: NgbModal
    ) { }

    ngOnInit(): void {
        const imdbId = this.route.snapshot.params['imdbId'];
        if (imdbId) {
            this.serieService.getByImdbId(imdbId).subscribe({
                next: (res) => {
                    this.serie = res;
                    this.loading = false;
                },
                error: (err) => {
                    console.error(err);
                    this.loading = false;
                }
            });
            this.loadWatchlistStatus(imdbId);
        }
    }

    loadWatchlistStatus(imdbId: string) {
        this.watchlistService.getList().subscribe(items => {
            this.watchlistItem = items.find(i => i.serie && i.serie.imdbid === imdbId) || null;
        });
    }

    goBack(): void {
        history.back();
    }

    handleImageError(serie: SerieDto): void {
        serie.poster = 'N/A';
    }
    openAddToWatchlistModal() {
        if (!this.serie) return;

        const modalRef = this.modalService.open(AddToWatchlistModalComponent, { centered: true });
        modalRef.componentInstance.title = this.serie.title;

        if (this.watchlistItem) {
            modalRef.componentInstance.currentStatus = this.watchlistItem.status;
        }

        modalRef.result.then((status: WatchlistStatus) => {
            if (status !== undefined) {
                this.saveToWatchlist(status);
            }
        }, () => { });
    }

    saveToWatchlist(status: WatchlistStatus) {
        if (!this.serie) return;

        const input: CreateUpdateWatchlistItemDto = {
            imdbId: this.serie.imdbid || '',
            status: status
        };

        if (this.watchlistItem) {
            // Update
            this.watchlistService.updateStatus(input).subscribe(() => {
                this.loadWatchlistStatus(this.serie!.imdbid || '');
            });
        } else {
            // Add
            this.watchlistService.addItem(input).subscribe(() => {
                this.loadWatchlistStatus(this.serie!.imdbid || '');
            });
        }
    }

    getStatusLabel(status: WatchlistStatus): string {
        return WatchlistStatus[status];
    }
}
