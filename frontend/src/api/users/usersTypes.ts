import type { MemberType } from '@api/common/enums';

export interface AddUserRequest {
  cid: string;
  email?: string;
  fullName?: string;
  isAdmin?: boolean;
  memberType?: MemberType;
}

export interface SearchUsersRequest {
  cid?: string;
  email?: string;
  fullName?: string;
  isAdmin?: boolean;
  memberType?: MemberType;
  search?: string;
}

export interface UpdateUserRequest {
  id: number;
  cid?: string;
  email?: string;
  fullName?: string;
  isAdmin?: boolean;
  memberType?: MemberType;
}

export interface UserDto {
  id: number;
  cid?: string;
  email?: string;
  fullName?: string;
  isAdmin?: boolean;
  memberType?: MemberType;
}

export interface CreateJwtRequest {
  Cid: string;
  Otp: string;
}

export interface SendOtpRequest {
  Cid: string;
}
