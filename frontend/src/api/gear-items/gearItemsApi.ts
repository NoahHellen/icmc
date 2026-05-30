import queryClient from '@api/queryClient';
import { type QueryKey, useMutation, useQuery } from '@tanstack/react-query';
import {
  addGearItem,
  deleteGearItem,
  getGearItem,
  searchGearItems,
  updateGearItem,
  uploadGearItemImage,
} from './gearItemsService';
import type {
  AddGearItemRequest,
  GearItemDto,
  SearchGearItemsRequest,
  UpdateGearItemRequest,
  UploadGearItemImageRequest,
} from './gearItemsTypes';

export function getGearItemsKey(
  id?: number,
  request?: SearchGearItemsRequest
): QueryKey {
  return ['gear-items', id, request];
}

export function useGetGearItem(id: number) {
  const query = useQuery<GearItemDto>({
    queryKey: getGearItemsKey(id),
    queryFn: () => getGearItem(id),
  });
  return query;
}

export function useSearchGearItems(request: SearchGearItemsRequest) {
  const query = useQuery<GearItemDto[]>({
    queryKey: getGearItemsKey(undefined, request),
    queryFn: () => searchGearItems(request),
    placeholderData: (previousData) => previousData,
  });
  return query;
}

export function useAddGearItem() {
  const mutation = useMutation({
    mutationFn: (request: AddGearItemRequest) => addGearItem(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gear-items'] });
    },
  });
  return mutation;
}

export function useDeleteGearItem() {
  const mutation = useMutation({
    mutationFn: (id: number) => deleteGearItem(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gear-items'] });
    },
  });
  return mutation;
}

export function useUpdateGearItem() {
  const mutation = useMutation({
    mutationFn: ({
      id,
      request,
    }: {
      id: number;
      request: UpdateGearItemRequest;
    }) => updateGearItem(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gear-items'] });
    },
  });
  return mutation;
}

export function useUploadGearItemImage() {
  const mutation = useMutation({
    mutationFn: ({
      id,
      request,
    }: {
      id: number;
      request: UploadGearItemImageRequest;
    }) => uploadGearItemImage(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gear-items'] });
    },
  });
  return mutation;
}
