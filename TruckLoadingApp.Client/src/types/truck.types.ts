export interface TruckCategory {
  id: number;
  categoryName: string;
  isActive: boolean;
}

export interface TruckType {
  id: number;
  name: string;
  description?: string;
  categoryId: number;
  category?: TruckCategory;
  isActive: boolean;
}

export interface Truck {
  id: number;
  numberPlate: string;
  truckTypeId: number;
  truckType?: TruckType;
  loadCapacityWeight: number;
  loadCapacityVolume: number;
  height?: number;
  width?: number;
  length?: number;
  availabilityStartDate: string;
  availabilityEndDate: string;
  isApproved: boolean;
  operationalStatus: string;
  assignedDriverId?: number;
  assignedDriverName?: string;
  createdDate: string;
  updatedDate?: string;
}

export interface TruckRegistrationRequest {
  truckTypeId: number;
  numberPlate: string;
  loadCapacityWeight: number;
  loadCapacityVolume: number;
  height?: number;
  width?: number;
  length?: number;
  availabilityStartDate: Date;
  availabilityEndDate: Date;
} 