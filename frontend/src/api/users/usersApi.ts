import queryClient from '@api/queryClient';
import { useMutation, useQuery } from '@tanstack/react-query';
import {
  addUser,
  createJwt,
  deleteUser,
  getUser,
  searchUsers,
  sendOtp,
  updateUser,
} from './usersService';
import type {
  AddUserRequest,
  CreateJwtRequest,
  SearchUsersRequest,
  SendOtpRequest,
  UpdateUserRequest,
  UserDto,
} from './usersTypes';

export function getUsersKey(id?: number): any[] {
  return ['users', id];
}

export function useGetUser(id: number) {
  return useQuery<UserDto>({
    queryKey: getUsersKey(id),
    queryFn: () => getUser(id),
  });
}

export function useSearchUsers(request: SearchUsersRequest) {
  return useQuery<UserDto[]>({
    queryKey: ['users', request],
    queryFn: () => searchUsers(request),
  });
}

export function useAddUser() {
  return useMutation({
    mutationFn: (request: AddUserRequest) => addUser(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
  });
}

export function useDeleteUser() {
  return useMutation({
    mutationFn: (id: number) => deleteUser(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
  });
}

export function useUpdateUser() {
  return useMutation({
    mutationFn: ({ id, request }: { id: number; request: UpdateUserRequest }) =>
      updateUser(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['gear-items'] });
    },
  });
}

export function useCreateJwt() {
  return useMutation({
    mutationFn: (request: CreateJwtRequest) => createJwt(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
  });
}

export function useSendOtp() {
  return useMutation({
    mutationFn: (request: SendOtpRequest) => sendOtp(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
  });
}
