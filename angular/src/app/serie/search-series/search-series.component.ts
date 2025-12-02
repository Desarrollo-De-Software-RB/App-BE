import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SerieDto, SerieService } from '@proxy/series';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { SearchStateService } from '../search-state.service';

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

    constructor(
        private serieService: SerieService,
        private router: Router,
        private searchStateService: SearchStateService
    ) { }

    ngOnInit() {
        this.restoreState();
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
}
