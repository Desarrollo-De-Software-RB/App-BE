import type { CreateUpdateRatingDto, RatingDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class RatingService {
    apiName = 'Default';

    getSeriesRatings = (serieId: number, config?: Partial<Rest.Config>) =>
        this.restService.request<any, RatingDto[]>({
            method: 'GET',
            url: `/api/app/rating/series-ratings/${serieId}`,
        },
            { apiName: this.apiName, ...config });

    rateSeries = (input: CreateUpdateRatingDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, void>({
            method: 'POST',
            url: '/api/app/rating/rate-series',
            body: input,
        },
            { apiName: this.apiName, ...config });

    constructor(private restService: RestService) { }
}
