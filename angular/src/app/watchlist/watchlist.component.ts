import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { WatchlistService } from '../proxy/watchlists/watchlist.service';
import { WatchlistItemDto, WatchlistStatus, CreateUpdateWatchlistItemDto } from '../proxy/watchlists/models';
import { ListService } from '@abp/ng.core';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AddToWatchlistModalComponent } from './add-to-watchlist-modal/add-to-watchlist-modal.component';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.scss'],
  providers: [ListService],
})
export class WatchlistComponent implements OnInit {
  items: WatchlistItemDto[] = [];
  filteredItems: WatchlistItemDto[] = [];
  selectedStatus: string = 'All';
  statuses = Object.keys(WatchlistStatus).filter(k => isNaN(Number(k)));
  WatchlistStatus = WatchlistStatus;

  constructor(
    private watchlistService: WatchlistService,
    private router: Router,
    private modalService: NgbModal,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit(): void {
    this.loadItems();
  }

  navigateToDetail(serie: any) {
    this.router.navigate(['/series/search', serie.imdbid]);
  }

  loadItems() {
    this.watchlistService.getList().subscribe((res) => {
      this.items = res;
      this.filterItems();
    });
  }

  filterItems() {
    if (this.selectedStatus === 'All') {
      this.filteredItems = this.items;
    } else {
      const statusEnum = WatchlistStatus[this.selectedStatus as keyof typeof WatchlistStatus];
      this.filteredItems = this.items.filter(item => item.status === statusEnum);
    }
  }

  onStatusChange(status: string) {
    this.selectedStatus = status;
    this.filterItems();
  }

  openAddToWatchlistModal(item: WatchlistItemDto, event: Event) {
    event.stopPropagation();
    const modalRef = this.modalService.open(AddToWatchlistModalComponent, { centered: true });
    modalRef.componentInstance.title = item.serie.title;
    modalRef.componentInstance.currentStatus = item.status;

    modalRef.result.then((status: WatchlistStatus) => {
      if (status !== undefined) {
        this.updateStatus(item, status);
      }
    }, () => { });
  }

  updateStatus(item: WatchlistItemDto, newStatus: WatchlistStatus) {
    const input: CreateUpdateWatchlistItemDto = {
      imdbId: item.serie.imdbid,
      status: newStatus
    };
    this.watchlistService.updateStatus(input).subscribe(() => {
      item.status = newStatus;
      this.filterItems(); // Re-filter in case the item should move
    });
  }

  removeItem(item: WatchlistItemDto, event: Event) {
    event.stopPropagation();
    this.confirmation.warn(
      `Are you sure you want to remove "${item.serie.title}" from your list?`,
      'Confirm deletion'
    ).subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.watchlistService.removeItem(item.serie.imdbid).subscribe(() => {
          this.items = this.items.filter(i => i.id !== item.id);
          this.filterItems();
        });
      }
    });
  }

  getStatusLabel(status: WatchlistStatus): string {
    return WatchlistStatus[status];
  }

  handleImageError(item: WatchlistItemDto) {
    if (item.serie) {
      item.serie.poster = 'N/A';
    }
  }
}
