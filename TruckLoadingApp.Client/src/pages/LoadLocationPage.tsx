import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, useLocation } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import MapSelector from '../components/maps/MapSelector';
import shipperService, { Load } from '../services/shipper.service';

interface LocationState {
  load: Load;
}

const LoadLocationPage: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const location = useLocation();
  const state = location.state as LocationState;
  
  const [load, setLoad] = useState<Load | null>(state?.load || null);
  const [loading, setLoading] = useState(!state?.load);
  const [error, setError] = useState<string | null>(null);
  
  // Location states
  const [pickupLocation, setPickupLocation] = useState<{
    lat: number;
    lng: number;
    address: string;
  } | null>(null);
  
  const [deliveryLocation, setDeliveryLocation] = useState<{
    lat: number;
    lng: number;
    address: string;
  } | null>(null);

  useEffect(() => {
    const fetchLoad = async () => {
      if (!id) return;
      
      try {
        setLoading(true);
        const loadData = await shipperService.getLoadById(parseInt(id));
        setLoad(loadData);
      } catch (err: any) {
        console.error('Error fetching load:', err);
        setError(err.message || 'Failed to load data');
      } finally {
        setLoading(false);
      }
    };

    if (!state?.load && id) {
      fetchLoad();
    }
  }, [id, state?.load]);

  const handlePickupSelect = (location: { lat: number; lng: number; address: string }) => {
    setPickupLocation(location);
  };

  const handleDeliverySelect = (location: { lat: number; lng: number; address: string }) => {
    setDeliveryLocation(location);
  };

  const handleSubmit = async () => {
    if (!load || !pickupLocation || !deliveryLocation) {
      setError('Please select both pickup and delivery locations');
      return;
    }

    try {
      setLoading(true);
      // Update the load with location information
      const updatedLoad = await shipperService.updateLoad(load.id, {
        ...load,
        pickupLatitude: pickupLocation.lat,
        pickupLongitude: pickupLocation.lng,
        pickupAddress: pickupLocation.address,
        deliveryLatitude: deliveryLocation.lat,
        deliveryLongitude: deliveryLocation.lng,
        deliveryAddress: deliveryLocation.address
      });

      // Navigate to truck matching page
      navigate(`/loads/${load.id}/matching-trucks`, {
        state: { load: updatedLoad }
      });
    } catch (err: any) {
      console.error('Error updating load locations:', err);
      setError(err.message || 'Failed to update load locations');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="flex justify-center items-center min-h-screen">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
        </div>
      </Layout>
    );
  }

  if (error || !load) {
    return (
      <Layout>
        <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
          <div className="bg-red-50 border-l-4 border-red-400 p-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg className="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                </svg>
              </div>
              <div className="ml-3">
                <p className="text-sm text-red-700">{error || 'Load not found'}</p>
              </div>
            </div>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="mb-6">
            <h1 className="text-2xl font-semibold text-gray-900">Select Locations</h1>
            <p className="mt-1 text-sm text-gray-500">
              Step 2: Specify pickup and delivery locations for your load
            </p>
          </div>

          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
            <div>
              <h2 className="text-lg font-medium text-gray-900 mb-4">Pickup Location</h2>
              <MapSelector
                onLocationSelect={handlePickupSelect}
                initialLocation={pickupLocation}
                placeholder="Enter pickup location"
              />
              {pickupLocation && (
                <div className="mt-2 text-sm text-gray-500">
                  Selected: {pickupLocation.address}
                </div>
              )}
            </div>

            <div>
              <h2 className="text-lg font-medium text-gray-900 mb-4">Delivery Location</h2>
              <MapSelector
                onLocationSelect={handleDeliverySelect}
                initialLocation={deliveryLocation}
                placeholder="Enter delivery location"
              />
              {deliveryLocation && (
                <div className="mt-2 text-sm text-gray-500">
                  Selected: {deliveryLocation.address}
                </div>
              )}
            </div>
          </div>

          <div className="mt-8 flex justify-end space-x-4">
            <button
              type="button"
              onClick={() => navigate(`/loads/${load.id}/edit`)}
              className="inline-flex items-center px-4 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Back
            </button>
            <button
              type="button"
              onClick={handleSubmit}
              disabled={!pickupLocation || !deliveryLocation || loading}
              className={`inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 ${
                (!pickupLocation || !deliveryLocation || loading) ? 'opacity-50 cursor-not-allowed' : ''
              }`}
            >
              {loading ? 'Saving...' : 'Continue to Find Trucks'}
            </button>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default LoadLocationPage; 