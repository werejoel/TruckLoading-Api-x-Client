import api from './api';
import { Truck, TruckRegistrationRequest } from '../types/truck.types';

class TruckService {
  // Get all trucks for the current user
  async getCompanyTrucks(): Promise<Truck[]> {
    const response = await api.get<Truck[]>('/truck');
    return response.data;
  }

  // Register a new truck
  async registerTruck(truckData: TruckRegistrationRequest): Promise<{ message: string, truckId: number }> {
    const response = await api.post('/truck/register', truckData);
    return response.data;
  }

  // Get a truck by ID
  async getTruckById(id: number): Promise<Truck> {
    const response = await api.get<Truck>(`/truck/${id}`);
    return response.data;
  }

  // Get truck types
  async getTruckTypes(): Promise<{ id: number, name: string }[]> {
    const response = await api.get<{ id: number, name: string }[]>('/truck-types');
    return response.data;
  }
}

export default new TruckService(); 