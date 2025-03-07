import api from './api';
import { UserType } from '../types/auth.types';
import { GoodsTypeEnum } from '../models/enums';

// Type definitions for Load
export interface Load {
  id: number;
  weight: number;
  height?: number;
  width?: number;
  length?: number;
  description?: string;
  pickupDate: Date;
  deliveryDate: Date;
  specialRequirements?: string;
  goodsType: GoodsTypeEnum;
  loadTypeId: number;
  loadType?: { id: number; name: string };
  price?: number;
  currency?: string;
  region?: string;
  status: string;
  requiredTruckTypeId?: number;
  requiredTruckType?: { id: number; name: string };
  isStackable: boolean;
  requiresTemperatureControl: boolean;
  hazardousMaterialClass?: string;
  handlingInstructions?: string;
  isFragile: boolean;
  requiresStackingControl: boolean;
  stackingInstructions?: string;
  unNumber?: string;
  requiresCustomsDeclaration: boolean;
  customsDeclarationNumber?: string;
  createdDate: Date;
  updatedDate?: Date;
  pickupAddress?: string;
  pickupLatitude?: number;
  pickupLongitude?: number;
  deliveryAddress?: string;
  deliveryLatitude?: number;
  deliveryLongitude?: number;
}

// Request types
export interface LoadCreateRequest {
  weight: number;
  height?: number;
  width?: number;
  length?: number;
  description?: string;
  pickupDate: Date;
  deliveryDate: Date;
  specialRequirements?: string;
  goodsType: GoodsTypeEnum;
  loadTypeId: number;
  price?: number;
  currency?: string;
  region?: string;
  requiredTruckTypeId?: number;
  isStackable: boolean;
  requiresTemperatureControl: boolean;
  hazardousMaterialClass?: string;
  handlingInstructions?: string;
  isFragile: boolean;
  requiresStackingControl: boolean;
  stackingInstructions?: string;
  unNumber?: string;
  requiresCustomsDeclaration: boolean;
  customsDeclarationNumber?: string;
  pickupAddress?: string;
  pickupLatitude?: number;
  pickupLongitude?: number;
  deliveryAddress?: string;
  deliveryLatitude?: number;
  deliveryLongitude?: number;
}

export interface LoadUpdateRequest extends LoadCreateRequest {}

export interface TruckSearchRequest {
  originLatitude: number;
  originLongitude: number;
  destinationLatitude: number;
  destinationLongitude: number;
  weight: number;
  height?: number;
  width?: number;
  length?: number;
  pickupDate: Date;
  deliveryDate: Date;
}

export interface TruckSearchResponse {
  id: number;
  numberPlate: string;
  truckTypeId: number;
  truckType: string;
  loadCapacityWeight: number;
  loadCapacityVolume?: number;
  currentLatitude?: number;
  currentLongitude?: number;
  driverName?: string;
  companyName?: string;
  rating?: number;
  distanceToPickup?: number;
  estimatedArrivalTime?: Date;
}

export interface BookingCreateRequest {
  loadId: number;
  truckId: number;
  proposedPrice?: number;
  currency?: string;
  notes?: string;
  expressBooking?: boolean;
  requestedPickupDate?: Date;
  requestedDeliveryDate?: Date;
}

export interface Booking {
  id: number;
  loadId: number;
  load?: Load;
  truckId: number;
  truck?: any; // Define Truck interface if needed
  status: string;
  createdDate: Date;
  updatedDate?: Date;
  agreedPrice: number;
  pricingType: string;
  currency: string;
}

export interface TruckLocation {
  truckId: number;
  latitude: number;
  longitude: number;
  timestamp: Date;
  speed?: number;
  heading?: number;
  address?: string;
}

export interface TruckMatchResponse {
  id: number;
  registrationNumber: string;
  truckTypeId: number;
  truckTypeName: string;
  loadCapacityWeight: number;
  availableCapacityWeight: number;
  height?: number;
  width?: number;
  length?: number;
  volumeCapacity: number;
  hasRefrigeration: boolean;
  hasLiftgate: boolean;
  hasLoadingRamp: boolean;
  canTransportHazardousMaterials: boolean;
  hazardousMaterialsClasses?: string;
  driverName?: string;
  companyName?: string;
  companyRating?: number;
  distanceToPickup: number;
  estimatedTimeToPickup?: string;
}

export interface BookingHistory {
  id: number;
  bookingId: number;
  timestamp: Date;
  changeDescription: string;
  changedBy: string;
  previousStatus?: string;
  newStatus?: string;
  details?: Record<string, any>;
}

export interface BookingAuditRecord {
  id: number;
  bookingId: number;
  field: string;
  oldValue?: string;
  newValue?: string;
  changedBy: string;
  changedAt: Date;
}

class ShipperService {
  // Load management
  async createLoad(loadData: LoadCreateRequest): Promise<Load> {
    const response = await api.post<Load>('/shipper/loads', loadData);
    return response.data;
  }

  async getLoads(): Promise<Load[]> {
    const response = await api.get<Load[]>('/shipper/loads');
    return response.data;
  }

  async getLoadById(loadId: number): Promise<Load> {
    const response = await api.get<Load>(`/shipper/loads/${loadId}`);
    return response.data;
  }

  async updateLoad(loadId: number, loadData: LoadUpdateRequest): Promise<void> {
    await api.put(`/shipper/loads/${loadId}`, loadData);
  }

  async deleteLoad(loadId: number): Promise<void> {
    await api.delete(`/shipper/loads/${loadId}`);
  }

  // Truck search
  async searchTrucks(searchData: TruckSearchRequest): Promise<TruckSearchResponse[]> {
    const response = await api.post<TruckSearchResponse[]>('/shipper/search/trucks', searchData);
    return response.data;
  }

  // Booking management
  async createBookingRequest(request: BookingCreateRequest): Promise<Booking> {
    const response = await api.post<Booking>('/shipper/bookings', request);
    return response.data;
  }

  async getBookings(): Promise<Booking[]> {
    const response = await api.get<Booking[]>('/shipper/bookings');
    return response.data;
  }

  async getBookingById(bookingId: number): Promise<Booking> {
    const response = await api.get<Booking>(`/shipper/bookings/${bookingId}`);
    return response.data;
  }

  async cancelBooking(bookingId: number): Promise<void> {
    await api.delete(`/shipper/bookings/${bookingId}`);
  }

  // Tracking
  async getCurrentLoadLocation(loadId: number): Promise<TruckLocation | null> {
    const response = await api.get<TruckLocation>(`/shipper/tracking/${loadId}/current`);
    return response.data;
  }

  async getLoadLocationHistory(loadId: number): Promise<TruckLocation[]> {
    const response = await api.get<TruckLocation[]>(`/shipper/tracking/${loadId}/history`);
    return response.data;
  }

  // Get matching trucks for a load
  async getMatchingTrucksForLoad(loadId: number, maxDistanceKm: number = 50): Promise<TruckMatchResponse[]> {
    const response = await api.get<TruckMatchResponse[]>(`/load/${loadId}/matching-trucks?maxDistanceKm=${maxDistanceKm}`);
    return response.data;
  }

  // Booking history and audit trail
  async getBookingHistory(bookingId: number): Promise<BookingHistory[]> {
    const response = await api.get<BookingHistory[]>(`/shipper/bookings/${bookingId}/history`);
    return response.data;
  }
  
  async getBookingAuditTrail(bookingId: number): Promise<BookingAuditRecord[]> {
    const response = await api.get<BookingAuditRecord[]>(`/shipper/bookings/${bookingId}/audit`);
    return response.data;
  }
}

export default new ShipperService();