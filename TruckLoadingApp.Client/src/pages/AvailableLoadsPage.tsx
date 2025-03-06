import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import { FaSearch, FaTruck, FaMapMarkerAlt, FaCalendarAlt, FaWeightHanging, FaMoneyBillWave, FaList, FaMap } from 'react-icons/fa';
import truckerService, { AvailableLoad } from '../services/trucker.service';
import { format } from 'date-fns';
import LoadsMap from '../components/maps/LoadsMap';

const AvailableLoadsPage: React.FC = () => {
  const [loads, setLoads] = useState<AvailableLoad[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentLocation, setCurrentLocation] = useState<{ latitude: number; longitude: number } | null>(null);
  const [radius, setRadius] = useState(50); // Default radius in km
  const [viewMode, setViewMode] = useState<'list' | 'map'>('list');

  useEffect(() => {
    // Get current location
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setCurrentLocation({
            latitude: position.coords.latitude,
            longitude: position.coords.longitude
          });
        },
        (err) => {
          console.error('Error getting location:', err);
          setError('Unable to get your current location. Please enable location services.');
          // Use a default location or ask user to input location
          setCurrentLocation({ latitude: 0, longitude: 0 });
        }
      );
    } else {
      setError('Geolocation is not supported by your browser');
      setCurrentLocation({ latitude: 0, longitude: 0 });
    }
  }, []);

  useEffect(() => {
    const fetchLoads = async () => {
      if (!currentLocation) return;
      
      try {
        setLoading(true);
        setError(null);
        const availableLoads = await truckerService.searchAvailableLoads(
          currentLocation.latitude,
          currentLocation.longitude,
          radius
        );
        setLoads(availableLoads);
      } catch (err: any) {
        console.error('Error fetching available loads:', err);
        setError(err.message || 'Failed to load data');
      } finally {
        setLoading(false);
      }
    };

    if (currentLocation) {
      fetchLoads();
    }
  }, [currentLocation, radius]);

  const handleRadiusChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value);
    if (!isNaN(value) && value > 0) {
      setRadius(value);
    }
  };

  const formatDate = (dateString: string | Date) => {
    if (!dateString) return 'N/A';
    try {
      return format(new Date(dateString), 'MMM dd, yyyy');
    } catch (e) {
      return 'Invalid date';
    }
  };

  const filteredLoads = loads.filter(load => 
    (load.pickupLocation?.address || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (load.deliveryLocation?.address || '').toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Available Loads</h1>
          <p className="mt-2 text-gray-600">Browse and accept available load requests near you.</p>
        </div>

        {/* Error message */}
        {error && (
          <div className="mb-6 bg-red-50 border-l-4 border-red-400 p-4">
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

        {/* Search and filters */}
        <div className="bg-white shadow rounded-lg p-4 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label htmlFor="search" className="block text-sm font-medium text-gray-700">
                Search Locations
              </label>
              <div className="mt-1 relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <FaSearch className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type="text"
                  id="search"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="focus:ring-indigo-500 focus:border-indigo-500 block w-full pl-10 sm:text-sm border-gray-300 rounded-md"
                  placeholder="Search by pickup or delivery location"
                />
              </div>
            </div>
            <div>
              <label htmlFor="radius" className="block text-sm font-medium text-gray-700">
                Search Radius (km)
              </label>
              <div className="mt-1 relative rounded-md shadow-sm">
                <input
                  type="number"
                  id="radius"
                  value={radius}
                  onChange={handleRadiusChange}
                  min="1"
                  max="500"
                  className="focus:ring-indigo-500 focus:border-indigo-500 block w-full sm:text-sm border-gray-300 rounded-md"
                />
              </div>
            </div>
            <div className="flex items-end">
              <div className="w-full flex space-x-2">
                <button
                  onClick={() => setViewMode('list')}
                  className={`flex-1 flex justify-center items-center px-4 py-2 border rounded-md shadow-sm text-sm font-medium ${
                    viewMode === 'list'
                      ? 'bg-indigo-600 text-white border-indigo-600'
                      : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
                  }`}
                >
                  <FaList className="mr-2" />
                  List View
                </button>
                <button
                  onClick={() => setViewMode('map')}
                  className={`flex-1 flex justify-center items-center px-4 py-2 border rounded-md shadow-sm text-sm font-medium ${
                    viewMode === 'map'
                      ? 'bg-indigo-600 text-white border-indigo-600'
                      : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
                  }`}
                >
                  <FaMap className="mr-2" />
                  Map View
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Loading indicator */}
        {loading && (
          <div className="flex justify-center items-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
          </div>
        )}

        {/* No loads message */}
        {!loading && !error && filteredLoads.length === 0 && (
          <div className="bg-white shadow overflow-hidden sm:rounded-md p-6 text-center">
            <FaTruck className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No available loads found</h3>
            <p className="mt-1 text-sm text-gray-500">
              Try increasing your search radius or check back later.
            </p>
          </div>
        )}

        {/* Map View */}
        {!loading && !error && filteredLoads.length > 0 && viewMode === 'map' && (
          <div className="mb-6">
            <LoadsMap loads={filteredLoads} currentLocation={currentLocation} />
          </div>
        )}

        {/* List View */}
        {!loading && !error && filteredLoads.length > 0 && viewMode === 'list' && (
          <div className="bg-white shadow overflow-hidden sm:rounded-md">
            <ul className="divide-y divide-gray-200">
              {filteredLoads.map((load) => (
                <li key={load.id}>
                  <div className="px-4 py-4 sm:px-6 hover:bg-gray-50">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center">
                        <FaTruck className="h-5 w-5 text-gray-400 mr-3" />
                        <p className="text-sm font-medium text-indigo-600 truncate">
                          Load #{load.id}
                        </p>
                      </div>
                      <div className="ml-2 flex-shrink-0">
                        <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                          Available
                        </span>
                      </div>
                    </div>
                    <div className="mt-2 sm:flex sm:justify-between">
                      <div className="sm:flex sm:flex-col">
                        <div className="flex items-center text-sm text-gray-500">
                          <FaMapMarkerAlt className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                          <p>From: {load.pickupLocation?.address || 'Unknown location'}</p>
                        </div>
                        <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-2">
                          <FaMapMarkerAlt className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                          <p>To: {load.deliveryLocation?.address || 'Unknown location'}</p>
                        </div>
                      </div>
                      <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:flex-col sm:items-end">
                        <div className="flex items-center">
                          <FaCalendarAlt className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                          <p>Pickup: {formatDate(load.pickupDate)}</p>
                        </div>
                        <div className="mt-2 flex items-center">
                          <FaWeightHanging className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                          <p>Weight: {load.weight} kg</p>
                        </div>
                        {load.price && (
                          <div className="mt-2 flex items-center">
                            <FaMoneyBillWave className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                            <p>Price: {load.price} {load.currency || 'USD'}</p>
                          </div>
                        )}
                      </div>
                    </div>
                    <div className="mt-4 flex justify-end">
                      <Link
                        to={`/loads/${load.id}/details`}
                        className="inline-flex items-center px-3 py-1 border border-gray-300 text-sm leading-4 font-medium rounded text-gray-700 bg-white hover:bg-gray-50"
                      >
                        View Details
                      </Link>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        )}
      </div>
    </Layout>
  );
};

export default AvailableLoadsPage; 