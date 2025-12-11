import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateSerieDto {
  title?: string;
  year?: string;
  rated?: string;
  released?: string;
  runtime?: string;
  genre?: string;
  director?: string;
  writer?: string;
  actors?: string;
  plot?: string;
  language?: string;
  country?: string;
  awards?: string;
  poster?: string;
  metascore?: string;
  imdbRating: number;
  imdbVotes?: string;
  imdbid?: string;
  type?: string;
  totalSeasons: number;
}

export interface SerieDto extends EntityDto<number> {
  title?: string;
  year?: string;
  rated?: string;
  released?: string;
  runtime?: string;
  genre?: string;
  director?: string;
  writer?: string;
  actors?: string;
  plot?: string;
  language?: string;
  country?: string;
  awards?: string;
  poster?: string;
  metascore?: string;
  imdbRating: number;
  imdbVotes?: string;
  imdbid?: string;
  type?: string;
  totalSeasons: number;
}

export interface RatingDto extends EntityDto<number> {
  serieId: number;
  userId: string;
  userName?: string;
  score: number;
  comment?: string;
  profilePictureUrl?: string;
}

export interface CreateUpdateRatingDto {
  serieId: number;
  score: number;
  comment?: string;
}
