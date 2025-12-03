import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SerieService } from '../../proxy/series';
import { SerieDto } from '../../proxy/series/models';

@Component({
    selector: 'app-serie-detail',
    templateUrl: './serie-detail.component.html',
    styleUrls: ['./serie-detail.component.scss']
})
export class SerieDetailComponent implements OnInit {
    serie: SerieDto | null = null;
    loading = true;

    constructor(
        private route: ActivatedRoute,
        private serieService: SerieService
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
        }
    }

    goBack(): void {
        history.back();
    }

    handleImageError(serie: SerieDto): void {
        serie.poster = 'N/A';
    }
}
