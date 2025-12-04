import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { WatchlistStatus } from '../../proxy/watchlists/models';

@Component({
    selector: 'app-add-to-watchlist-modal',
    templateUrl: './add-to-watchlist-modal.component.html',
    styleUrls: ['./add-to-watchlist-modal.component.scss']
})
export class AddToWatchlistModalComponent {
    @Input() title: string = '';
    @Input() currentStatus: WatchlistStatus | null = null;
    @Output() statusSelected = new EventEmitter<WatchlistStatus>();

    statuses = Object.keys(WatchlistStatus)
        .filter(k => isNaN(Number(k)))
        .map(key => ({
            key: key,
            value: WatchlistStatus[key as keyof typeof WatchlistStatus]
        }));

    selectedStatus: WatchlistStatus = WatchlistStatus.Pending;

    constructor(public activeModal: NgbActiveModal) { }

    ngOnInit() {
        if (this.currentStatus !== null) {
            this.selectedStatus = this.currentStatus;
        }
    }

    save() {
        this.activeModal.close(this.selectedStatus);
    }

    getStatusLabel(status: WatchlistStatus): string {
        return WatchlistStatus[status];
    }
}
