import React from 'react';
import { Truck } from '../../types/truck.types';

interface TruckDetailsProps {
  truck: Truck;
  showBookButton?: boolean;
  onBookClick?: (truckId: number) => void;
}

const TruckDetails: React.FC<TruckDetailsProps> = ({ 
  truck, 
  showBookButton = false, 
  onBookClick 
}) => {
  return (
    <div className="bg-white shadow overflow-hidden sm:rounded-lg">
      <div className="px-4 py-5 sm:px-6">
        <h3 className="text-lg leading-6 font-medium text-gray-900">
          Truck Details
        </h3>
        <p className="mt-1 max-w-2xl text-sm text-gray-500">
          {truck.truckType?.category?.categoryName} - {truck.truckType?.name}
        </p>
      </div>
      <div className="border-t border-gray-200">
        <dl>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Number Plate</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {truck.numberPlate}
            </dd>
          </div>
          <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Load Capacity</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              Weight: {truck.loadCapacityWeight} kg
              {truck.loadCapacityVolume && ` | Volume: ${truck.loadCapacityVolume} m³`}
            </dd>
          </div>
          {(truck.height || truck.width || truck.length) && (
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Dimensions</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {truck.height && `Height: ${truck.height}m`}
                {truck.width && ` | Width: ${truck.width}m`}
                {truck.length && ` | Length: ${truck.length}m`}
              </dd>
            </div>
          )}
          <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Availability</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              From: {new Date(truck.availabilityStartDate).toLocaleDateString()}
              <br />
              Until: {new Date(truck.availabilityEndDate).toLocaleDateString()}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Status</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                truck.isApproved 
                  ? 'bg-green-100 text-green-800' 
                  : 'bg-yellow-100 text-yellow-800'
              }`}>
                {truck.isApproved ? 'Approved' : 'Pending Approval'}
              </span>
              <span className="ml-2 px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-blue-100 text-blue-800">
                {truck.operationalStatus}
              </span>
            </dd>
          </div>
          {truck.assignedDriverName && (
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Assigned Driver</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {truck.assignedDriverName}
              </dd>
            </div>
          )}
        </dl>
      </div>
      {showBookButton && (
        <div className="px-4 py-3 bg-gray-50 text-right sm:px-6">
          <button
            type="button"
            onClick={() => onBookClick?.(truck.id)}
            className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Book This Truck
          </button>
        </div>
      )}
    </div>
  );
};

export default TruckDetails; 