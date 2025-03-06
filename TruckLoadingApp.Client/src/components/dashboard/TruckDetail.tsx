import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Truck } from '../../types/truck.types';
import truckService from '../../services/truck.service';

const TruckDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [truck, setTruck] = useState<Truck | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTruckDetails = async () => {
      try {
        setLoading(true);
        setError(null);
        
        if (!id) {
          setError('Truck ID is missing');
          return;
        }
        
        const truckData = await truckService.getTruckById(parseInt(id));
        setTruck(truckData);
      } catch (err: any) {
        console.error('Error fetching truck details:', err);
        const errorMessage = err.response?.data?.message || 
                            err.response?.data?.Message || 
                            err.message || 
                            'Failed to load truck details. Please try again later.';
        setError(errorMessage);
      } finally {
        setLoading(false);
      }
    };

    fetchTruckDetails();
  }, [id]);

  const handleBack = () => {
    navigate('/company/dashboard');
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-500"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-red-100 text-red-700 p-4 rounded mb-4">
          {error}
        </div>
        <button
          onClick={handleBack}
          className="px-4 py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
        >
          Back to Dashboard
        </button>
      </div>
    );
  }

  if (!truck) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-yellow-100 text-yellow-700 p-4 rounded mb-4">
          Truck not found
        </div>
        <button
          onClick={handleBack}
          className="px-4 py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
        >
          Back to Dashboard
        </button>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Truck Details</h1>
        <button
          onClick={handleBack}
          className="px-4 py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
        >
          Back to Dashboard
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="px-4 py-5 sm:px-6 bg-gray-50">
          <h3 className="text-lg leading-6 font-medium text-gray-900">
            {truck.numberPlate}
          </h3>
          <p className="mt-1 max-w-2xl text-sm text-gray-500">
            {typeof truck.truckType === 'string' ? truck.truckType : 'Unknown Type'}
          </p>
        </div>
        
        <div className="border-t border-gray-200">
          <dl>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Number Plate</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{truck.numberPlate}</dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Truck Type</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {typeof truck.truckType === 'string' ? truck.truckType : 'Unknown Type'}
              </dd>
            </div>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Load Capacity Weight</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {truck.loadCapacityWeight ? truck.loadCapacityWeight.toLocaleString() : 'N/A'} kg
              </dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Load Capacity Volume</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {truck.loadCapacityVolume ? truck.loadCapacityVolume.toLocaleString() : 'N/A'} m³
              </dd>
            </div>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Dimensions (H×W×L)</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {truck.height ? `${truck.height} m × ` : 'N/A × '}
                {truck.width ? `${truck.width} m × ` : 'N/A × '}
                {truck.length ? `${truck.length} m` : 'N/A'}
              </dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Availability Period</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {new Date(truck.availabilityStartDate).toLocaleDateString()} - {new Date(truck.availabilityEndDate).toLocaleDateString()}
              </dd>
            </div>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Status</dt>
              <dd className="mt-1 sm:mt-0 sm:col-span-2">
                <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                  truck.isApproved 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-yellow-100 text-yellow-800'
                }`}>
                  {truck.isApproved ? 'Approved' : 'Pending Approval'}
                </span>
              </dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Operational Status</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">{truck.operationalStatus}</dd>
            </div>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Assigned Driver</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {truck.assignedDriverName || 'Not assigned'}
              </dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Created Date</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {new Date(truck.createdDate).toLocaleDateString()}
              </dd>
            </div>
          </dl>
        </div>
      </div>
    </div>
  );
};

export default TruckDetail; 