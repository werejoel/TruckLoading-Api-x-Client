import api from './api';
import { Load, Booking } from './shipper.service';

// Type definitions for Truck
export interface Truck {
  id: number;
  numberPlate: string;
  truckTypeId: number;
  truckType?: { id: number; name: string };
  loadCapacityWeight: number;
  loadCapacityVolume?: number;
  isApproved: boolean;
  operationalStatus: string;
  driverId?: string;
  companyId?: string;
  currentLatitude?: number;
  currentLongitude?: number;
  lastLocationUpdateTime?: Date;
}

export interface TruckUpdateRequest {
  numberPlate: string;
  truckTypeId: number;
  loadCapacityWeight: number;
  loadCapacityVolume?: number;
  operationalStatus: string;
}

export interface TruckLocationUpdate {
  latitude: number;
  longitude: number;
  speed?: number;
  heading?: number;
}

export interface AvailableLoad {
  id: number;
  weight: number;
  dimensions?: {
    height?: number;
    width?: number;
    length?: number;
  };
  pickupDate: Date;
  deliveryDate: Date;
  pickupLocation?: {
    latitude: number;
    longitude: number;
    address?: string;
  };
  deliveryLocation?: {
    latitude: number;
    longitude: number;
    address?: string;
  };
  price?: number;
  currency?: string;
  distance?: number;
  shipper?: {
    id: string;
    name: string;
    rating?: number;
  };
}

class TruckerService {
  // Truck management
  async getTruckerTrucks(): Promise<Truck[]> {
    const response = await api.get<Truck[]>('/trucker/trucks');
    return response.data;
  }

  async getTruckById(truckId: number): Promise<Truck> {
    const response = await api.get<Truck>(`/trucker/trucks/${truckId}`);
    return response.data;
  }

  async updateTruck(truckId: number, truckData: TruckUpdateRequest): Promise<void> {
    await api.put(`/trucker/trucks/${truckId}`, truckData);
  }

  async updateTruckLocation(truckId: number, locationData: TruckLocationUpdate): Promise<void> {
    await api.post(`/trucker/trucks/${truckId}/location`, locationData);
  }

  // Load search and booking
  async searchAvailableLoads(
    latitude: number,
    longitude: number,
    radius?: number,
    maxWeight?: number
  ): Promise<AvailableLoad[]> {
    const params = new URLSearchParams();
    params.append('latitude', latitude.toString());
    params.append('longitude', longitude.toString());
    if (radius) params.append('radius', radius.toString());
    if (maxWeight) params.append('maxWeight', maxWeight.toString());

    const response = await api.get<AvailableLoad[]>(`/trucker/loads/available?${params.toString()}`);
    return response.data;
  }

  async getLoadDetails(loadId: number): Promise<Load> {
    const response = await api.get<Load>(`/trucker/loads/${loadId}`);
    return response.data;
  }

  // Booking management
  async getBookings(): Promise<Booking[]> {
    const response = await api.get<Booking[]>('/trucker/bookings');
    return response.data;
  }

  async getBookingById(bookingId: number): Promise<Booking> {
    const response = await api.get<Booking>(`/trucker/bookings/${bookingId}`);
    return response.data;
  }

  async acceptBooking(bookingId: number): Promise<void> {
    await api.post(`/trucker/bookings/${bookingId}/accept`);
  }

  async rejectBooking(bookingId: number, reason?: string): Promise<void> {
    await api.post(`/trucker/bookings/${bookingId}/reject`, { reason });
  }

  async startTransport(bookingId: number): Promise<void> {
    await api.post(`/trucker/bookings/${bookingId}/start`);
  }

  async completeTransport(bookingId: number): Promise<void> {
    await api.post(`/trucker/bookings/${bookingId}/complete`);
  }

  async reportIssue(bookingId: number, issueDetails: string): Promise<void> {
    await api.post(`/trucker/bookings/${bookingId}/issue`, { issueDetails });
  }
}

export default new TruckerService(); 