import { Component } from '@angular/core';
import { SerieDto, SerieService } from '@proxy/series';
import { ListService, PagedResultDto } from '@abp/ng.core';

@Component({
    selector: 'app-search-series',
    templateUrl: './search-series.component.html',
    styleUrls: ['./search-series.component.scss'],
    providers: [ListService],
})
export class SearchSeriesComponent {
    series: SerieDto[] = [];
    searchTitle: string = '';
    searchGenre: string = '';
    searchType: string = '';
    hasSearched: boolean = false;

    constructor(private serieService: SerieService) { }

    search() {
        if (!this.searchTitle && !this.searchGenre) {
            return;
        }

        this.serieService.search(this.searchTitle, this.searchGenre, this.searchType).subscribe((response) => {
            this.series = response;
            this.hasSearched = true;
        });
    }

    handleImageError(serie: SerieDto) {
        serie.poster = 'N/A';
    }
}
