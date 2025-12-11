import { SerieDto } from '../series/models';
import { EntityDto } from '@abp/ng.core';

export interface WatchlistItemDto extends EntityDto<string> {
    userId: string;
    serieId: number;
    serie: SerieDto;
    status: WatchlistStatus;
}

export interface CreateUpdateWatchlistItemDto {
    imdbId: string;
    status: WatchlistStatus;
}

export enum WatchlistStatus {
    Completed = 0,
    Watching = 1,
    Pending = 2,
    Dropped = 3,
}
