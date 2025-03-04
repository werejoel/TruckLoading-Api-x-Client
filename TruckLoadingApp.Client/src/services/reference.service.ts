import api from './api';
import { TruckType, TruckCategory } from '../types/truck.types';

// Type definitions for reference data
export interface LoadType {
  id: number;
  name: string;
  description?: string;
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

  // Truck categories
  async getTruckCategories(): Promise<TruckCategory[]> {
    try {
      console.log('Fetching truck categories...');
      const response = await api.get<TruckCategory[]>('/reference/truck-categories');
      console.log('Received truck categories:', response.data);
      return response.data;
    } catch (error) {
      console.error('Error fetching truck categories:', error);
      throw error;
    }
  }

  async getTruckCategoryById(id: number): Promise<TruckCategory> {
    try {
      console.log(`Fetching truck category ${id}...`);
      const response = await api.get<TruckCategory>(`/reference/truck-categories/${id}`);
      console.log('Received truck category:', response.data);
      return response.data;
    } catch (error) {
      console.error(`Error fetching truck category ${id}:`, error);
      throw error;
    }
  }

  // Truck types
  async getTruckTypes(): Promise<TruckType[]> {
    try {
      console.log('Fetching all truck types...');
      const response = await api.get<TruckType[]>('/reference/truck-types');
      console.log('Received truck types:', response.data);
      return response.data;
    } catch (error) {
      console.error('Error fetching truck types:', error);
      throw error;
    }
  }

  async getTruckTypesByCategory(categoryId: number): Promise<TruckType[]> {
    try {
      console.log(`Fetching truck types for category ${categoryId}...`);
      const response = await api.get<TruckType[]>(`/reference/truck-categories/${categoryId}/types`);
      console.log('Received truck types:', response.data);
      return response.data;
    } catch (error) {
      console.error(`Error fetching truck types for category ${categoryId}:`, error);
      throw error;
    }
  }

  async getTruckTypeById(id: number): Promise<TruckType> {
    try {
      console.log(`Fetching truck type ${id}...`);
      const response = await api.get<TruckType>(`/reference/truck-types/${id}`);
      console.log('Received truck type:', response.data);
      return response.data;
    } catch (error) {
      console.error(`Error fetching truck type ${id}:`, error);
      throw error;
    }
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