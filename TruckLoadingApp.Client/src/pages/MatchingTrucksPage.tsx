import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, useLocation } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import shipperService, { Load, TruckMatchResponse } from '../services/shipper.service';

interface LocationState {
  load: Load;
}

const MatchingTrucksPage: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const location = useLocation();
  const state = location.state as LocationState;
  
  const [load, setLoad] = useState<Load | null>(state?.load || null);
  const [trucks, setTrucks] = useState<TruckMatchResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [maxDistance, setMaxDistance] = useState(50);

  useEffect(() => {
    const fetchData = async () => {
      if (!id) return;
      
      try {
        setLoading(true);
        // If we don't have the load data from state, fetch it
        if (!load) {
          const loadData = await shipperService.getLoadById(parseInt(id));
          setLoad(loadData);
        }
        
        // Fetch matching trucks
        const matchingTrucks = await shipperService.getMatchingTrucksForLoad(parseInt(id), maxDistance);
        setTrucks(matchingTrucks);
      } catch (err: any) {
        console.error('Error fetching data:', err);
        setError(err.message || 'Failed to load data');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, load, maxDistance]);

  const handleDistanceChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(event.target.value);
    if (!isNaN(value) && value > 0) {
      setMaxDistance(value);
    }
  };

  const handleBookTruck = (truckId: number) => {
    if (!load) return;
    navigate(`/loads/${load.id}/book/${truckId}`);
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
            <h1 className="text-2xl font-semibold text-gray-900">Matching Trucks</h1>
            <p className="mt-1 text-sm text-gray-500">
              Step 3: Select a truck for your load
            </p>
          </div>

          <div className="mb-6">
            <label htmlFor="maxDistance" className="block text-sm font-medium text-gray-700">
              Maximum Distance (km)
            </label>
            <div className="mt-1 flex rounded-md shadow-sm w-48">
              <input
                type="number"
                name="maxDistance"
                id="maxDistance"
                value={maxDistance}
                onChange={handleDistanceChange}
                min="1"
                className="flex-1 min-w-0 block w-full px-3 py-2 rounded-md focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm border-gray-300"
              />
            </div>
          </div>

          {trucks.length === 0 ? (
            <div className="text-center py-12">
              <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              <h3 className="mt-2 text-sm font-medium text-gray-900">No matching trucks found</h3>
              <p className="mt-1 text-sm text-gray-500">
                Try increasing the maximum distance or adjusting your load requirements.
              </p>
            </div>
          ) : (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <ul className="divide-y divide-gray-200">
                {trucks.map((truck) => (
                  <li key={truck.id}>
                    <div className="px-4 py-4 sm:px-6">
                      <div className="flex items-center justify-between">
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium text-indigo-600 truncate">
                            {truck.registrationNumber}
                          </p>
                          <p className="mt-1 text-sm text-gray-500">
                            {truck.truckTypeName} • {truck.companyName || 'Independent Trucker'}
                          </p>
                        </div>
                        <div className="ml-4 flex-shrink-0">
                          <button
                            onClick={() => handleBookTruck(truck.id)}
                            className="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                          >
                            Book Now
                          </button>
                        </div>
                      </div>
                      <div className="mt-2 sm:flex sm:justify-between">
                        <div className="sm:flex">
                          <p className="flex items-center text-sm text-gray-500">
                            Capacity: {truck.loadCapacityWeight}kg
                          </p>
                          <p className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:ml-6">
                            Available: {truck.availableCapacityWeight}kg
                          </p>
                        </div>
                        <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                          <p>
                            Distance: {Math.round(truck.distanceToPickup)}km • 
                            ETA: {truck.estimatedTimeToPickup || 'N/A'}
                          </p>
                        </div>
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          )}

          <div className="mt-8 flex justify-end space-x-4">
            <button
              type="button"
              onClick={() => navigate(`/loads/${load.id}/select-location`)}
              className="inline-flex items-center px-4 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Back
            </button>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default MatchingTrucksPage; 