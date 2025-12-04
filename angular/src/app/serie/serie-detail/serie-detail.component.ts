import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SerieService } from '../../proxy/series';
import { RatingService } from '../../proxy/series/rating.service';
import { SerieDto, RatingDto, CreateUpdateRatingDto } from '../../proxy/series/models';
import { ToasterService } from '@abp/ng.theme.shared';
import { ConfigStateService } from '@abp/ng.core';

@Component({
    selector: 'app-serie-detail',
    templateUrl: './serie-detail.component.html',
    styleUrls: ['./serie-detail.component.scss']
})
export class SerieDetailComponent implements OnInit {
    serie: SerieDto | null = null;
    loading = true;
    ratings: RatingDto[] = [];
    userRating: CreateUpdateRatingDto = { serieId: 0, score: 5, comment: '' };
    isSubmitting = false;

    constructor(
        private route: ActivatedRoute,
        private serieService: SerieService,
        private ratingService: RatingService,
        private toaster: ToasterService,
        private configState: ConfigStateService
    ) { }

    ngOnInit(): void {
        const imdbId = this.route.snapshot.params['imdbId'];
        if (imdbId) {
            this.serieService.getByImdbId(imdbId).subscribe({
                next: (res) => {
                    this.serie = res;
                    this.userRating.serieId = res.id;
                    this.loadRatings(res.id);
                    this.loading = false;
                },
                error: (err) => {
                    console.error(err);
                    this.loading = false;
                }
            });
        }
    }

    loadRatings(serieId: number): void {
        this.ratingService.getSeriesRatings(serieId).subscribe({
            next: (res) => {
                this.ratings = res;
                const currentUser = this.configState.getOne('currentUser');
                if (currentUser && currentUser.id) {
                    const myRating = this.ratings.find(r => r.userId === currentUser.id);
                    if (myRating) {
                        this.userRating.score = myRating.score;
                        this.userRating.comment = myRating.comment || '';
                    }
                }
            }
        });
    }

    submitRating(): void {
        if (!this.serie) return;
        this.isSubmitting = true;
        this.ratingService.rateSeries(this.userRating).subscribe({
            next: () => {
                this.toaster.success('Rating submitted successfully');
                this.loadRatings(this.serie!.id);
                this.isSubmitting = false;
            },
            error: (err) => {
                this.toaster.error('Error submitting rating');
                console.error(err);
                this.isSubmitting = false;
            }
        });
    }

    goBack(): void {
        history.back();
    }

    handleImageError(serie: SerieDto): void {
        serie.poster = 'N/A';
    }
}
