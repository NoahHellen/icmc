import type { GearCategory, StorageLocation } from '@api/common/enums';

export interface AddLogRequest {
  gearItemId: number;
  inspectedByUserId?: number;
  lentToUserId?: number;
  lentByUserId?: number;
  lentDate?: string;
  returnedDate?: string;
  notes?: string;
}

export interface LogbookDto {
  id: number;
  gearItemId: number;
  gearItemCategory?: GearCategory;
  gearItemToughTag?: string;
  gearItemModel?: string;
  gearItemBrand?: string;
  gearItemStorageLocation?: StorageLocation;
  inspectedByUserId?: number;
  inspectedByUserFullName?: string;
  inspectedByUserCid?: string;
  inspectedByUserEmail?: string;
  lentToUserId?: number;
  lentToUserFullName?: string;
  lentToUserCid?: string;
  lentToUserEmail?: string;
  lentByUserId?: number;
  lentByUserFullName?: string;
  lentByUserCid?: string;
  lentByUserEmail?: string;
  lentDate?: string;
  returnedDate?: string;
  notes?: string;
}

export interface SearchLogbookRequest {
  gearItemId?: number;
  gearItemCategory?: GearCategory;
  gearItemToughTag?: string;
  gearItemModel?: string;
  gearItemBrand?: string;
  gearItemStorageLocation?: StorageLocation;
  inspectedByUserId?: number;
  inspectedByUserFullName?: string;
  inspectedByUserCid?: string;
  inspectedByUserEmail?: string;
  lentToUserId?: number;
  lentToUserFullName?: string;
  lentToUserCid?: string;
  lentToUserEmail?: string;
  lentByUserId?: number;
  lentByUserFullName?: string;
  lentByUserCid?: string;
  lentByUserEmail?: string;
  lentDate?: string;
  returnedDate?: string;
  notes?: string;
  search?: string;
}
