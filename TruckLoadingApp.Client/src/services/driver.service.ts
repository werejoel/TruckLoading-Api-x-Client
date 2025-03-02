import api from './api';
import { Driver, DriverRegisterRequest, AssignDriverRequest } from '../types/driver.types';

class DriverService {
  // Get all drivers for the company
  async getCompanyDrivers(): Promise<Driver[]> {
    const response = await api.get<Driver[]>('/company/drivers');
    return response.data;
  }

  // Register a new driver
  async registerDriver(driverData: DriverRegisterRequest): Promise<{ message: string, driverId: number }> {
    const response = await api.post('/company/register-driver', driverData);
    return response.data;
  }

  // Assign a driver to a truck
  async assignDriverToTruck(assignData: AssignDriverRequest): Promise<{ message: string }> {
    const response = await api.post('/company/assign-driver', assignData);
    return response.data;
  }

  // Get a driver by ID
  async getDriverById(id: number): Promise<Driver> {
    const response = await api.get<Driver>(`/driver/${id}`);
    return response.data;
  }

  // Get available drivers
  async getAvailableDrivers(): Promise<Driver[]> {
    const response = await api.get<Driver[]>('/driver/available');
    return response.data;
  }
}

export default new DriverService(); 