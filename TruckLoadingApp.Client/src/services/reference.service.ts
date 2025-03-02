import api from './api';

// Type definitions for reference data
export interface LoadType {
  id: number;
  name: string;
  description?: string;
}

export interface TruckType {
  id: number;
  name: string;
  description?: string;
  maxWeight?: number;
  maxVolume?: number;
}

export interface LoadTag {
  id: number;
  name: string;
  description?: string;
}

export interface Region {
  id: string;
  name: string;
  countryCode: string;
}

export interface Currency {
  code: string;
  name: string;
  symbol: string;
}

class ReferenceService {
  // Load types
  async getLoadTypes(): Promise<LoadType[]> {
    const response = await api.get<LoadType[]>('/reference/load-types');
    return response.data;
  }

  async getLoadTypeById(id: number): Promise<LoadType> {
    const response = await api.get<LoadType>(`/reference/load-types/${id}`);
    return response.data;
  }

  // Truck types
  async getTruckTypes(): Promise<TruckType[]> {
    const response = await api.get<TruckType[]>('/reference/truck-types');
    return response.data;
  }

  async getTruckTypeById(id: number): Promise<TruckType> {
    const response = await api.get<TruckType>(`/reference/truck-types/${id}`);
    return response.data;
  }

  // Load tags
  async getLoadTags(): Promise<LoadTag[]> {
    const response = await api.get<LoadTag[]>('/reference/load-tags');
    return response.data;
  }

  // Regions
  async getRegions(): Promise<Region[]> {
    const response = await api.get<Region[]>('/reference/regions');
    return response.data;
  }

  // Currencies
  async getCurrencies(): Promise<Currency[]> {
    const response = await api.get<Currency[]>('/reference/currencies');
    return response.data;
  }

  // Hazardous material classes
  async getHazardousMaterialClasses(): Promise<string[]> {
    const response = await api.get<string[]>('/reference/hazardous-material-classes');
    return response.data;
  }

  // Geocoding
  async geocodeAddress(address: string): Promise<{latitude: number, longitude: number}> {
    const response = await api.get(`/reference/geocode?address=${encodeURIComponent(address)}`);
    return response.data;
  }

  async reverseGeocode(latitude: number, longitude: number): Promise<string> {
    const response = await api.get(`/reference/reverse-geocode?latitude=${latitude}&longitude=${longitude}`);
    return response.data;
  }

  // Distance calculation
  async calculateDistance(
    originLat: number, 
    originLng: number, 
    destLat: number, 
    destLng: number
  ): Promise<{distance: number, duration: number}> {
    const response = await api.get(
      `/reference/distance?originLat=${originLat}&originLng=${originLng}&destLat=${destLat}&destLng=${destLng}`
    );
    return response.data;
  }
}

export default new ReferenceService(); 