import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import shipperService, { Load, TruckMatchResponse } from '../../services/shipper.service';
import { format } from 'date-fns';

const ShipperDashboard: React.FC = () => {
  const { user } = useAuth();
  const [loads, setLoads] = useState<Load[]>([]);
  const [selectedLoad, setSelectedLoad] = useState<Load | null>(null);
  const [matchingTrucks, setMatchingTrucks] = useState<TruckMatchResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [maxDistance, setMaxDistance] = useState(50);
  const [activeTab, setActiveTab] = useState<'loads' | 'matching-trucks'>('loads');

  useEffect(() => {
    fetchLoads();
  }, []);

  const fetchLoads = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await shipperService.getLoads();
      setLoads(data);
    } catch (err: any) {
      console.error('Error fetching loads:', err);
      setError(err.message || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const handleLoadSelect = async (load: Load) => {
    setSelectedLoad(load);
    setActiveTab('loads');
  };

  const handleViewMatchingTrucks = async (load: Load) => {
    try {
      setLoading(true);
      setError(null);
      setSelectedLoad(load);
      const trucks = await shipperService.getMatchingTrucksForLoad(load.id, maxDistance);
      setMatchingTrucks(trucks);
      setActiveTab('matching-trucks');
    } catch (err: any) {
      console.error('Error fetching matching trucks:', err);
      setError(err.message || 'Failed to load matching trucks');
    } finally {
      setLoading(false);
    }
  };

  const formatDateTime = (date: Date | string) => {
    if (!date) return 'N/A';
    return format(new Date(date), 'MMM dd, yyyy HH:mm');
  };

  const formatDuration = (duration: string | undefined) => {
    if (!duration) return 'N/A';
    
    // Parse ISO 8601 duration format
    const matches = duration.match(/PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+)S)?/);
    if (!matches) return duration;
    
    const hours = matches[1] ? parseInt(matches[1]) : 0;
    const minutes = matches[2] ? parseInt(matches[2]) : 0;
    
    if (hours > 0) {
      return `${hours}h ${minutes}m`;
    } else {
      return `${minutes}m`;
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">Shipper Dashboard</h1>
      
      {/* Tabs */}
      <div className="border-b border-gray-200 mb-6">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('loads')}
            className={`${
              activeTab === 'loads'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            My Loads
          </button>
          {selectedLoad && (
            <button
              onClick={() => setActiveTab('matching-trucks')}
              className={`${
                activeTab === 'matching-trucks'
                  ? 'border-indigo-500 text-indigo-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
            >
              Matching Trucks
            </button>
          )}
        </nav>
      </div>

      {/* Error message */}
      {error && (
        <div className="bg-red-50 border-l-4 border-red-400 p-4 mb-6">
          <div className="flex">
            <div className="flex-shrink-0">
              <svg className="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
              </svg>
            </div>
            <div className="ml-3">
              <p className="text-sm text-red-700">{error}</p>
            </div>
          </div>
        </div>
      )}

      {/* Loading indicator */}
      {loading && (
        <div className="flex justify-center items-center py-8">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
        </div>
      )}

      {/* Tab Content */}
      {!loading && (
        <>
          {/* Loads Tab */}
          {activeTab === 'loads' && (
            <div>
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-semibold text-gray-800">My Loads</h2>
                <button
                  onClick={fetchLoads}
                  className="px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Refresh
                </button>
              </div>

              {loads.length === 0 ? (
                <div className="bg-white shadow overflow-hidden sm:rounded-md p-6 text-center">
                  <p className="text-gray-500">No loads found. Create a new load to get started.</p>
                </div>
              ) : (
                <div className="bg-white shadow overflow-hidden sm:rounded-md">
                  <ul className="divide-y divide-gray-200">
                    {loads.map((load) => (
                      <li key={load.id} className={`${selectedLoad?.id === load.id ? 'bg-indigo-50' : ''}`}>
                        <div className="px-4 py-4 sm:px-6">
                          <div className="flex items-center justify-between">
                            <div className="flex items-center">
                              <p className="text-sm font-medium text-indigo-600 truncate">
                                {load.description || `Load #${load.id}`}
                              </p>
                              <div className="ml-2 flex-shrink-0 flex">
                                <p className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full 
                                  ${load.status === 'Available' ? 'bg-green-100 text-green-800' : 
                                    load.status === 'Booked' ? 'bg-blue-100 text-blue-800' : 
                                    load.status === 'InTransit' ? 'bg-yellow-100 text-yellow-800' : 
                                    load.status === 'Delivered' ? 'bg-purple-100 text-purple-800' : 
                                    'bg-gray-100 text-gray-800'}`}
                                >
                                  {load.status}
                                </p>
                              </div>
                            </div>
                            <div className="flex space-x-2">
                              <button
                                onClick={() => handleLoadSelect(load)}
                                className="px-3 py-1 border border-gray-300 text-xs font-medium rounded text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                              >
                                Details
                              </button>
                              <button
                                onClick={() => handleViewMatchingTrucks(load)}
                                className="px-3 py-1 border border-transparent text-xs font-medium rounded text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                              >
                                Find Trucks
                              </button>
                            </div>
                          </div>
                          <div className="mt-2 sm:flex sm:justify-between">
                            <div className="sm:flex">
                              <p className="flex items-center text-sm text-gray-500">
                                <svg className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                  <path fillRule="evenodd" d="M10 2a8 8 0 100 16 8 8 0 000-16zm0 14a6 6 0 100-12 6 6 0 000 12z" clipRule="evenodd" />
                                </svg>
                                {load.weight} kg
                              </p>
                              <p className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:ml-6">
                                <svg className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                  <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                                </svg>
                                {load.pickupAddress || 'No pickup address'}
                              </p>
                            </div>
                            <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                              <svg className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                <path fillRule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clipRule="evenodd" />
                              </svg>
                              <p>
                                Pickup: {formatDateTime(load.pickupDate)}
                              </p>
                            </div>
                          </div>
                        </div>
                      </li>
                    ))}
                  </ul>
                </div>
              )}

              {/* Selected Load Details */}
              {selectedLoad && activeTab === 'loads' && (
                <div className="mt-6 bg-white shadow overflow-hidden sm:rounded-lg">
                  <div className="px-4 py-5 sm:px-6">
                    <h3 className="text-lg leading-6 font-medium text-gray-900">
                      Load Details
                    </h3>
                    <p className="mt-1 max-w-2xl text-sm text-gray-500">
                      Detailed information about the selected load.
                    </p>
                  </div>
                  <div className="border-t border-gray-200 px-4 py-5 sm:p-0">
                    <dl className="sm:divide-y sm:divide-gray-200">
                      <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                        <dt className="text-sm font-medium text-gray-500">Description</dt>
                        <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                          {selectedLoad.description || 'No description provided'}
                        </dd>
                      </div>
                      <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                        <dt className="text-sm font-medium text-gray-500">Weight</dt>
                        <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                          {selectedLoad.weight} kg
                        </dd>
                      </div>
                      <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                        <dt className="text-sm font-medium text-gray-500">Dimensions (H×W×L)</dt>
                        <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                          {selectedLoad.height ? `${selectedLoad.height} × ` : '- × '}
                          {selectedLoad.width ? `${selectedLoad.width} × ` : '- × '}
                          {selectedLoad.length ? selectedLoad.length : '-'} m
                        </dd>
                      </div>
                      <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                        <dt className="text-sm font-medium text-gray-500">Pickup</dt>
                        <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                          <div>{selectedLoad.pickupAddress || 'No address provided'}</div>
                          <div className="text-gray-500">{formatDateTime(selectedLoad.pickupDate)}</div>
                          {selectedLoad.pickupLatitude && selectedLoad.pickupLongitude && (
                            <div className="text-xs text-gray-500">
                              Lat: {selectedLoad.pickupLatitude.toFixed(6)}, Lng: {selectedLoad.pickupLongitude.toFixed(6)}
                            </div>
                          )}
                        </dd>
                      </div>
                      <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                        <dt className="text-sm font-medium text-gray-500">Delivery</dt>
                        <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                          <div>{selectedLoad.deliveryAddress || 'No address provided'}</div>
                          <div className="text-gray-500">{formatDateTime(selectedLoad.deliveryDate)}</div>
                          {selectedLoad.deliveryLatitude && selectedLoad.deliveryLongitude && (
                            <div className="text-xs text-gray-500">
                              Lat: {selectedLoad.deliveryLatitude.toFixed(6)}, Lng: {selectedLoad.deliveryLongitude.toFixed(6)}
                            </div>
                          )}
                        </dd>
                      </div>
                      <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                        <dt className="text-sm font-medium text-gray-500">Special Requirements</dt>
                        <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                          {selectedLoad.specialRequirements || 'None'}
                        </dd>
                      </div>
                      <div className="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                        <dt className="text-sm font-medium text-gray-500">Status</dt>
                        <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                          <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full 
                            ${selectedLoad.status === 'Available' ? 'bg-green-100 text-green-800' : 
                              selectedLoad.status === 'Booked' ? 'bg-blue-100 text-blue-800' : 
                              selectedLoad.status === 'InTransit' ? 'bg-yellow-100 text-yellow-800' : 
                              selectedLoad.status === 'Delivered' ? 'bg-purple-100 text-purple-800' : 
                              'bg-gray-100 text-gray-800'}`}
                          >
                            {selectedLoad.status}
                          </span>
                        </dd>
                      </div>
                    </dl>
                  </div>
                </div>
              )}
            </div>
          )}

          {/* Matching Trucks Tab */}
          {activeTab === 'matching-trucks' && selectedLoad && (
            <div>
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-semibold text-gray-800">
                  Matching Trucks for Load #{selectedLoad.id}
                </h2>
                <div className="flex items-center space-x-2">
                  <label htmlFor="maxDistance" className="text-sm text-gray-700">
                    Max Distance (km):
                  </label>
                  <input
                    id="maxDistance"
                    type="number"
                    min="1"
                    max="500"
                    value={maxDistance}
                    onChange={(e) => setMaxDistance(parseInt(e.target.value))}
                    className="w-20 px-2 py-1 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                  <button
                    onClick={() => handleViewMatchingTrucks(selectedLoad)}
                    className="px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                  >
                    Search
                  </button>
                </div>
              </div>

              {matchingTrucks.length === 0 ? (
                <div className="bg-white shadow overflow-hidden sm:rounded-md p-6 text-center">
                  <p className="text-gray-500">No matching trucks found. Try increasing the maximum distance or check your load details.</p>
                </div>
              ) : (
                <div className="bg-white shadow overflow-hidden sm:rounded-md">
                  <ul className="divide-y divide-gray-200">
                    {matchingTrucks.map((truck) => (
                      <li key={truck.id}>
                        <div className="px-4 py-4 sm:px-6">
                          <div className="flex items-center justify-between">
                            <div>
                              <h3 className="text-sm font-medium text-indigo-600">
                                {truck.registrationNumber} - {truck.truckTypeName}
                              </h3>
                              <p className="text-sm text-gray-500">
                                {truck.companyName || 'Independent Trucker'}
                                {truck.companyRating && ` • Rating: ${truck.companyRating.toFixed(1)}/5`}
                              </p>
                            </div>
                            <div className="flex flex-col items-end">
                              <span className="text-sm font-medium text-gray-900">
                                {truck.distanceToPickup.toFixed(1)} km away
                              </span>
                              <span className="text-xs text-gray-500">
                                Est. arrival: {formatDuration(truck.estimatedTimeToPickup)}
                              </span>
                            </div>
                          </div>
                          <div className="mt-2 sm:flex sm:justify-between">
                            <div className="sm:flex sm:space-x-4">
                              <div className="flex items-center text-sm text-gray-500">
                                <svg className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                  <path fillRule="evenodd" d="M10 2a8 8 0 100 16 8 8 0 000-16zm0 14a6 6 0 100-12 6 6 0 000 12z" clipRule="evenodd" />
                                </svg>
                                Capacity: {truck.availableCapacityWeight} / {truck.loadCapacityWeight} kg
                              </div>
                              <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                                <svg className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                  <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
                                </svg>
                                Driver: {truck.driverName || 'Not assigned'}
                              </div>
                            </div>
                            <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                              <div className="flex flex-wrap gap-1">
                                {truck.hasRefrigeration && (
                                  <span className="px-2 py-1 text-xs rounded-full bg-blue-100 text-blue-800">
                                    Refrigerated
                                  </span>
                                )}
                                {truck.hasLiftgate && (
                                  <span className="px-2 py-1 text-xs rounded-full bg-green-100 text-green-800">
                                    Liftgate
                                  </span>
                                )}
                                {truck.hasLoadingRamp && (
                                  <span className="px-2 py-1 text-xs rounded-full bg-yellow-100 text-yellow-800">
                                    Loading Ramp
                                  </span>
                                )}
                                {truck.canTransportHazardousMaterials && (
                                  <span className="px-2 py-1 text-xs rounded-full bg-red-100 text-red-800">
                                    Hazmat
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                          <div className="mt-2 flex justify-end">
                            <button
                              onClick={() => {
                                // Book the truck
                                alert(`Booking truck ${truck.registrationNumber} for load #${selectedLoad.id}`);
                                // In a real app, you would call a booking API here
                              }}
                              className="px-3 py-1 border border-transparent text-xs font-medium rounded text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                            >
                              Book Now
                            </button>
                          </div>
                        </div>
                      </li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default ShipperDashboard; 