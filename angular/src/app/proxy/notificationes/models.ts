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
  AwardsChange = 4,
  RuntimeChange = 5,
  StatusChange = 6,
  UserActivity = 7,
  System = 8,
  Trend = 9,
  Reminder = 10
}

export enum NotificationChannel {
  InApp = 0,
  Email = 1
}
