export enum UserType {
  Shipper = 'Shipper',
  Trucker = 'Trucker',
  Company = 'Company',
  Admin = 'Admin'
}

export enum TruckOwnerType {
  Individual = 'Individual',
  Company = 'Company'
}

// Base registration interface with common fields
export interface BaseRegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword?: string;
  firstName: string;
  lastName: string;
  userType: UserType;
}

// Shipper-specific registration interface
export interface ShipperRegisterRequest extends BaseRegisterRequest {
  userType: UserType.Shipper;
}

// Trucker-specific registration interface
export interface TruckerRegisterRequest extends BaseRegisterRequest {
  userType: UserType.Trucker;
  truckOwnerType: TruckOwnerType;
  licenseNumber: string;
  licenseExpiryDate: string | Date;
  experience?: number;
  phoneNumber?: string;
}

// Company-specific registration interface
export interface CompanyRegisterRequest extends BaseRegisterRequest {
  userType: UserType.Company;
  companyName: string;
  companyAddress: string;
  companyRegistrationNumber: string;
  companyContact: string;
}

// Login interface
export interface LoginRequest {
  username: string;
  password: string;
}

// Auth response interface
export interface AuthResponse {
  success: boolean;
  message: string;
  token: string;
  refreshToken: string;
  expiration: string;
  userId: string;
  username: string;
  roles: string[];
}

// User interface
export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  userType: UserType;
  truckOwnerType?: TruckOwnerType;
  companyName?: string;
  phoneNumber?: string;
  roles: string[];
  createdDate: string;
}