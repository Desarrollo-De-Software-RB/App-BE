import { Injectable } from '@angular/core';
import { SerieDto } from '@proxy/series';

@Injectable({
    providedIn: 'root'
})
export class SearchStateService {
    private _series: SerieDto[] = [];
    private _searchTitle: string = '';
    private _searchGenre: string = '';
    private _searchType: string = '';
    private _hasSearched: boolean = false;

    get series(): SerieDto[] {
        return this._series;
    }

    set series(value: SerieDto[]) {
        this._series = value;
    }

    get searchTitle(): string {
        return this._searchTitle;
    }

    set searchTitle(value: string) {
        this._searchTitle = value;
    }

    get searchGenre(): string {
        return this._searchGenre;
    }

    set searchGenre(value: string) {
        this._searchGenre = value;
    }

    get searchType(): string {
        return this._searchType;
    }

    set searchType(value: string) {
        this._searchType = value;
    }

    get hasSearched(): boolean {
        return this._hasSearched;
    }

    set hasSearched(value: boolean) {
        this._hasSearched = value;
    }

    clear() {
        this._series = [];
        this._searchTitle = '';
        this._searchGenre = '';
        this._searchType = '';
        this._hasSearched = false;
    }
}
