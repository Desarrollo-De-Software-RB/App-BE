import type { EntityDto } from '@abp/ng.core';

export interface UserDto extends EntityDto<string> {
  userName: string;
  profilePicture?: string;
}

export interface UserFullDto extends UserDto {
  name: string;
  surname: string;
  email: string;
  phoneNumber?: string;
  isActive: boolean;
  creationTime: string;
}

export interface CreateUserDto {
  userName: string;
  name?: string;
  surname?: string;
  email: string;
  password: string;
  phoneNumber?: string;
  isActive: boolean;
  profilePicture?: string;
}

export interface UpdateUserDto {
  userName: string;
  name?: string;
  surname?: string;
  email: string;
  phoneNumber?: string;
  isActive: boolean;
  profilePicture?: string;
}
