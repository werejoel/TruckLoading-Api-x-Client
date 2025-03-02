import api from './api';
import { Truck } from './trucker.service';

// Type definitions for Company
export interface Company {
  id: string;
  name: string;
  address: string;
  contactEmail: string;
  contactPhone?: string;
  registrationNumber?: string;
  taxIdentificationNumber?: string;
  isVerified: boolean;
  createdDate: Date;
}

export interface CompanyUpdateRequest {
  name: string;
  address: string;
  contactPhone?: string;
  registrationNumber?: string;
  taxIdentificationNumber?: string;
}

// Type definitions for Driver
export interface Driver {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  licenseNumber: string;
  licenseExpiryDate: Date;
  licenseType: string;
  isActive: boolean;
  truckId?: number;
  truck?: Truck;
  createdDate: Date;
}

export interface DriverCreateRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  licenseNumber: string;
  licenseExpiryDate: Date;
  licenseType: string;
}

export interface DriverUpdateRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  licenseNumber: string;
  licenseExpiryDate: Date;
  licenseType: string;
  isActive: boolean;
}

export interface AssignDriverRequest {
  driverId: string;
  truckId: number;
}

// Type definitions for Truck registration
export interface TruckRegistrationRequest {
  numberPlate: string;
  truckTypeId: number;
  loadCapacityWeight: number;
  loadCapacityVolume?: number;
}

class CompanyService {
  // Company profile management
  async getCompanyProfile(): Promise<Company> {
    const response = await api.get<Company>('/company/profile');
    return response.data;
  }

  async updateCompanyProfile(profileData: CompanyUpdateRequest): Promise<Company> {
    const response = await api.put<Company>('/company/profile', profileData);
    return response.data;
  }

  // Driver management
  async getCompanyDrivers(): Promise<Driver[]> {
    const response = await api.get<Driver[]>('/company/drivers');
    return response.data;
  }

  async getDriverById(driverId: string): Promise<Driver> {
    const response = await api.get<Driver>(`/company/drivers/${driverId}`);
    return response.data;
  }

  async registerDriver(driverData: DriverCreateRequest): Promise<Driver> {
    const response = await api.post<Driver>('/company/drivers', driverData);
    return response.data;
  }

  async updateDriver(driverId: string, driverData: DriverUpdateRequest): Promise<Driver> {
    const response = await api.put<Driver>(`/company/drivers/${driverId}`, driverData);
    return response.data;
  }

  async deactivateDriver(driverId: string): Promise<void> {
    await api.delete(`/company/drivers/${driverId}`);
  }

  // Truck management
  async getCompanyTrucks(): Promise<Truck[]> {
    const response = await api.get<Truck[]>('/company/trucks');
    return response.data;
  }

  async getTruckById(truckId: number): Promise<Truck> {
    const response = await api.get<Truck>(`/company/trucks/${truckId}`);
    return response.data;
  }

  async registerTruck(truckData: TruckRegistrationRequest): Promise<Truck> {
    const response = await api.post<Truck>('/company/trucks', truckData);
    return response.data;
  }

  async updateTruck(truckId: number, truckData: TruckRegistrationRequest): Promise<Truck> {
    const response = await api.put<Truck>(`/company/trucks/${truckId}`, truckData);
    return response.data;
  }

  async deleteTruck(truckId: number): Promise<void> {
    await api.delete(`/company/trucks/${truckId}`);
  }

  // Driver-Truck assignment
  async assignDriverToTruck(assignData: AssignDriverRequest): Promise<void> {
    await api.post('/company/assign-driver', assignData);
  }

  async unassignDriver(driverId: string): Promise<void> {
    await api.post(`/company/unassign-driver/${driverId}`);
  }

  // Analytics and reporting
  async getCompanyStatistics(): Promise<any> {
    const response = await api.get('/company/statistics');
    return response.data;
  }

  async getDriverPerformance(driverId: string, startDate?: Date, endDate?: Date): Promise<any> {
    let url = `/company/drivers/${driverId}/performance`;
    const params = new URLSearchParams();
    
    if (startDate) params.append('startDate', startDate.toISOString());
    if (endDate) params.append('endDate', endDate.toISOString());
    
    if (params.toString()) {
      url += `?${params.toString()}`;
    }
    
    const response = await api.get(url);
    return response.data;
  }

  async getTruckUtilization(truckId: number, startDate?: Date, endDate?: Date): Promise<any> {
    let url = `/company/trucks/${truckId}/utilization`;
    const params = new URLSearchParams();
    
    if (startDate) params.append('startDate', startDate.toISOString());
    if (endDate) params.append('endDate', endDate.toISOString());
    
    if (params.toString()) {
      url += `?${params.toString()}`;
    }
    
    const response = await api.get(url);
    return response.data;
  }
}

export default new CompanyService(); 