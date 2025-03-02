export interface Driver {
  id: number;
  userId: string;
  companyId?: string;
  firstName: string;
  lastName: string;
  fullName: string;
  licenseNumber: string;
  licenseExpiryDate: string;
  experience?: number;
  safetyRating?: number;
  isAvailable: boolean;
  truckId?: number;
  truckNumberPlate?: string;
  phoneNumber?: string;
  createdDate: string;
  updatedDate?: string;
}

export interface DriverRegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  licenseNumber: string;
  licenseExpiryDate: string;
  experience?: number;
  phoneNumber?: string;
}

export interface AssignDriverRequest {
  driverId: number;
  truckId: number;
} 