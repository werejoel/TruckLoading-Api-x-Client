import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { FaPlus, FaTruck, FaSearch, FaChevronDown, FaChevronUp, FaEdit, FaTrash } from 'react-icons/fa';
import Navbar from '../../components/layout/Navbar';
import shipperService, { Load } from '../../services/shipper.service';
import { format } from 'date-fns';

const LoadsPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [loads, setLoads] = useState<Load[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [expandedLoadId, setExpandedLoadId] = useState<number | null>(null);

  useEffect(() => {
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

    fetchLoads();
  }, []);

  const getStatusColor = (status: any) => {
    // Check if status exists and is a string before calling toLowerCase()
    if (typeof status !== 'string') {
      console.warn('Load status is not a string:', status);
      return 'bg-gray-100 text-gray-800';
    }

    switch (status.toLowerCase()) {
      case 'available':
        return 'bg-green-100 text-green-800';
      case 'booked':
        return 'bg-blue-100 text-blue-800';
      case 'intransit':
        return 'bg-purple-100 text-purple-800';
      case 'delivered':
        return 'bg-green-100 text-green-800';
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const formatDateTime = (dateString: Date | string) => {
    if (!dateString) return 'N/A';
    try {
      const date = typeof dateString === 'string' ? new Date(dateString) : dateString;
      return format(date, 'MMM dd, yyyy');
    } catch (e) {
      return 'Invalid date';
    }
  };

  const filteredLoads = loads.filter(load => 
    (load.description || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (load.pickupAddress || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (load.deliveryAddress || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
    (`load #${load.id}`).toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleLoadClick = (loadId: number) => {
    setExpandedLoadId(expandedLoadId === loadId ? null : loadId);
  };

  const handleDeleteLoad = async (loadId: number, event: React.MouseEvent) => {
    event.stopPropagation(); // Prevent card expansion
    if (window.confirm(`Are you sure you want to delete load #${loadId}?`)) {
      try {
        setLoading(true);
        await shipperService.deleteLoad(loadId);
        setLoads(loads.filter(load => load.id !== loadId));
      } catch (err: any) {
        console.error('Error deleting load:', err);
        setError(err.message || 'Failed to delete load');
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="space-y-6">
          {/* Page header */}
          <div className="md:flex md:items-center md:justify-between">
            <div className="flex-1 min-w-0">
              <h2 className="text-2xl font-bold leading-7 text-gray-900 sm:text-3xl sm:truncate">
                Loads Management
              </h2>
            </div>
            <div className="mt-4 flex md:mt-0 md:ml-4">
              <Link
                to="/loads/create"
                className="ml-3 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                <FaPlus className="mr-2 h-4 w-4" />
                Create Load
              </Link>
            </div>
          </div>

          {/* Search and filters */}
          <div className="bg-white shadow rounded-lg p-4">
            <div className="max-w-xl">
              <div className="mt-1 relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <FaSearch className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type="text"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="focus:ring-blue-500 focus:border-blue-500 block w-full pl-10 sm:text-sm border-gray-300 rounded-md"
                  placeholder="Search loads by reference, origin, or destination"
                />
              </div>
            </div>
          </div>

          {/* Loading indicator */}
          {loading && (
            <div className="flex justify-center items-center py-8">
              <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
            </div>
          )}

          {/* Error message */}
          {error && (
            <div className="rounded-md bg-red-50 p-4">
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

          {/* No loads message */}
          {!loading && !error && filteredLoads.length === 0 && (
            <div className="bg-white shadow overflow-hidden sm:rounded-md p-6 text-center">
              <p className="text-gray-500">No loads found. Create a new load to get started.</p>
              <Link
                to="/loads/create"
                className="mt-4 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                <FaPlus className="mr-2 h-4 w-4" />
                Create Load
              </Link>
            </div>
          )}

          {/* Loads list */}
          {!loading && !error && filteredLoads.length > 0 && (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <ul role="list" className="divide-y divide-gray-200">
                {filteredLoads.map((load) => (
                  <li key={load.id}>
                    <div 
                      onClick={() => handleLoadClick(load.id)} 
                      className="block hover:bg-gray-50 cursor-pointer transition-all duration-200 ease-in-out hover-card"
                    >
                      <div className="px-4 py-4 sm:px-6">
                        <div className="flex items-center justify-between">
                          <div className="flex items-center">
                            <FaTruck className="h-5 w-5 text-gray-400 mr-3" />
                            <p className="text-sm font-medium text-blue-600 truncate">
                              {load.description || `Load #${load.id}`}
                            </p>
                          </div>
                          <div className="flex items-center space-x-3">
                            <div className="ml-2 flex-shrink-0">
                              <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(load.status)}`}>
                                {typeof load.status === 'string' ? load.status.toUpperCase() : load.status?.toString?.() || 'UNKNOWN'}
                              </span>
                            </div>
                            {expandedLoadId === load.id ? <FaChevronUp className="h-4 w-4 text-gray-500" /> : <FaChevronDown className="h-4 w-4 text-gray-500" />}
                          </div>
                        </div>
                        <div className="mt-2 sm:flex sm:justify-between">
                          <div className="sm:flex">
                            <p className="flex items-center text-sm text-gray-500">
                              From: {load.pickupAddress || 'No address provided'}
                            </p>
                            <p className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:ml-6">
                              To: {load.deliveryAddress || 'No address provided'}
                            </p>
                          </div>
                          <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                            <p className="mr-4">Weight: {load.weight} kg</p>
                            <p>Pickup: {formatDateTime(load.pickupDate)}</p>
                          </div>
                        </div>

                        {/* Expanded details */}
                        {expandedLoadId === load.id && (
                          <div className="mt-4 border-t border-gray-200 pt-4 animate-fade-in">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-y-4 gap-x-8">
                              {/* Left column */}
                              <div>
                                <h4 className="font-medium text-gray-700">Load Details</h4>
                                <div className="mt-2 grid grid-cols-2 gap-2 text-sm">
                                  <div className="text-gray-600">ID:</div>
                                  <div>{load.id}</div>
                                  <div className="text-gray-600">Description:</div>
                                  <div>{load.description || 'N/A'}</div>
                                  <div className="text-gray-600">Weight:</div>
                                  <div>{load.weight} kg</div>
                                  <div className="text-gray-600">Dimensions:</div>
                                  <div>
                                    {load.height ? `${load.height} × ` : '- × '}
                                    {load.width ? `${load.width} × ` : '- × '}
                                    {load.length ? load.length : '-'} m
                                  </div>
                                  <div className="text-gray-600">Goods Type:</div>
                                  <div>{load.goodsType || 'N/A'}</div>
                                  <div className="text-gray-600">Special Requirements:</div>
                                  <div>{load.specialRequirements || 'None'}</div>
                                </div>
                              </div>
                              {/* Right column */}
                              <div>
                                <h4 className="font-medium text-gray-700">Shipping Information</h4>
                                <div className="mt-2 grid grid-cols-2 gap-2 text-sm">
                                  <div className="text-gray-600">Pickup Address:</div>
                                  <div>{load.pickupAddress || 'No address provided'}</div>
                                  <div className="text-gray-600">Pickup Date:</div>
                                  <div>{formatDateTime(load.pickupDate)}</div>
                                  <div className="text-gray-600">Delivery Address:</div>
                                  <div>{load.deliveryAddress || 'No address provided'}</div>
                                  <div className="text-gray-600">Delivery Date:</div>
                                  <div>{formatDateTime(load.deliveryDate)}</div>
                                  <div className="text-gray-600">Status:</div>
                                  <div>
                                    <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(load.status)}`}>
                                      {typeof load.status === 'string' ? load.status.toUpperCase() : load.status?.toString?.() || 'UNKNOWN'}
                                    </span>
                                  </div>
                                </div>
                              </div>
                            </div>

                            {/* Action buttons */}
                            <div className="mt-4 flex justify-end">
                              <Link
                                to={`/loads/edit/${load.id}`}
                                onClick={(e) => e.stopPropagation()}
                                className="px-3 py-1 border border-gray-300 text-xs font-medium rounded text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 mr-2 flex items-center"
                              >
                                <FaEdit className="mr-1" />
                                Edit
                              </Link>
                              <button
                                onClick={(e) => handleDeleteLoad(load.id, e)}
                                className="px-3 py-1 border border-gray-300 text-xs font-medium rounded text-red-700 bg-white hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 mr-2 flex items-center"
                              >
                                <FaTrash className="mr-1" />
                                Delete
                              </button>
                              <Link
                                to={`/loads/${load.id}${load.pickupLatitude && load.pickupLongitude && load.deliveryLatitude && load.deliveryLongitude ? '/matching-trucks' : '/select-location'}`}
                                onClick={(e) => e.stopPropagation()}
                                className="px-3 py-1 border border-transparent text-xs font-medium rounded text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 flex items-center"
                              >
                                Find Trucks
                              </Link>
                            </div>
                          </div>
                        )}
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default LoadsPage;