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

    constructor(private serieService: SerieService) { }

    search() {
        if (!this.searchTitle && !this.searchGenre) {
            return;
        }

        this.serieService.search(this.searchTitle, this.searchGenre).subscribe((response) => {
            this.series = response;
        });
    }
}
