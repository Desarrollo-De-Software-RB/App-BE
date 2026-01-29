import { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface NotificationDto extends EntityDto<string> {
  title: string;
  message: string;
  type: NotificationType;
  relatedEntityId: string;
  isRead: boolean;
  creationTime: string;
}

export interface NotificationPreferenceDto {
  type: NotificationType;
  channel: NotificationChannel;
  isEnabled: boolean;
}

export enum NotificationType {
  RatingChange = 0,
  VotesChange = 1,
  PosterChange = 2,
  PlotChange = 3,
  RuntimeChange = 4,
  StatusChange = 5,
  UserActivity = 6,
  System = 7,
  Trend = 8,
  Reminder = 9,
  UserRating = 10
}

export enum NotificationChannel {
  InApp = 0,
  Email = 1
}
