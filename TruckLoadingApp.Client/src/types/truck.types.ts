export interface Truck {
  id: number;
  numberPlate: string;
  truckType: string;
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

export interface TruckType {
  id: number;
  name: string;
} 