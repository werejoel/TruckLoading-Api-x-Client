import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Navbar from '../components/layout/Navbar';
import { FaTruck, FaList, FaRoute, FaPlus, FaExclamationTriangle } from 'react-icons/fa';
import truckService from '../services/truck.service';
import truckerService from '../services/trucker.service';
import { Truck } from '../types/truck.types';
import { Booking } from '../services/shipper.service';
import { format } from 'date-fns';



//Main Function 
const TruckerDashboardPage: React.FC = () => {
  const { user } = useAuth();
  const [trucks, setTrucks] = useState<Truck[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  //useEffect Function
  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);
        
        // Fetch trucks
        const trucksData = await truckService.getCompanyTrucks();
        setTrucks(trucksData);
        
        // Fetch bookings
        const bookingsData = await truckerService.getBookings();
        setBookings(bookingsData);
      }
       catch (err: any) {
        console.error('Error fetching trucker data:', err);
        setError(err.message || 'Failed to load data');
      } 
      finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const getStatusBadgeColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'available':
        return 'bg-green-100 text-green-800';
      case 'in transit':
      case 'intransit':
        return 'bg-blue-100 text-blue-800';
      case 'maintenance':
        return 'bg-yellow-100 text-yellow-800';
      case 'out of service':
      case 'outofservice':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getBookingStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'accepted':
        return 'bg-green-100 text-green-800';
      case 'in transit':
      case 'intransit':
        return 'bg-blue-100 text-blue-800';
      case 'completed':
        return 'bg-indigo-100 text-indigo-800';
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
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

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Welcome, {user?.firstName || 'Trucker'}!</h1>
          <p className="mt-2 text-gray-600">Manage your deliveries and view available loads.</p>
        </div>

        {/* Error message */}
        {error && (
          <div className="mb-6 bg-red-50 border-l-4 border-red-400 p-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <FaExclamationTriangle className="h-5 w-5 text-red-400" />
              </div>
              <div className="ml-3">
                <p className="text-sm text-red-700">{error}</p>
              </div>
            </div>
          </div>
        )}

        {/* Quick Actions */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Available Loads Card */}
          <Link
            to="/available-loads"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaList className="text-blue-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">Available Loads</h2>
            </div>
            <p className="text-gray-600">Browse and accept available load requests.</p>
          </Link>

          {/* My Deliveries Card */}
          <Link
            to="/my-deliveries"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaTruck className="text-green-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">My Deliveries</h2>
            </div>
            <p className="text-gray-600">View and manage your current deliveries.</p>
          </Link>

          {/* Register Truck Card */}
          <Link
            to="/trucker/register-truck"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
          >
            <div className="flex items-center mb-4">
              <FaPlus className="text-purple-500 text-2xl mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">Register Truck</h2>
            </div>
            <p className="text-gray-600">Add a new truck to your fleet.</p>
          </Link>
        </div>

        {/* My Trucks Section */}
        <div className="mt-12">
          <div className="flex justify-between items-center mb-6">
            <h2 className="text-2xl font-bold text-gray-900">My Trucks</h2>
            <Link
              to="/trucker/register-truck"
              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700"
            >
              <FaPlus className="mr-2 -ml-1 h-4 w-4" />
              Add Truck
            </Link>
          </div>

          {loading ? (
            <div className="bg-white rounded-lg shadow-md p-6">
              <div className="animate-pulse flex space-x-4">
                <div className="flex-1 space-y-4 py-1">
                  <div className="h-4 bg-gray-200 rounded w-3/4"></div>
                  <div className="space-y-2">
                    <div className="h-4 bg-gray-200 rounded"></div>
                    <div className="h-4 bg-gray-200 rounded w-5/6"></div>
                  </div>
                </div>
              </div>
            </div>
          ) : trucks.length === 0 ? (
            <div className="bg-white rounded-lg shadow-md p-8 text-center">
              <FaTruck className="mx-auto h-12 w-12 text-gray-400" />
              <h3 className="mt-2 text-sm font-medium text-gray-900">No trucks registered</h3>
              <p className="mt-1 text-sm text-gray-500">
                Get started by registering your first truck.
              </p>
              <div className="mt-6">
                <Link
                  to="/trucker/register-truck"
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700"
                >
                  <FaPlus className="mr-2 -ml-1 h-4 w-4" />
                  Register Truck
                </Link>
              </div>
            </div>
          ) : (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <ul className="divide-y divide-gray-200">
                {trucks.map((truck) => (
                  <li key={truck.id}>
                    <div className="px-4 py-4 sm:px-6">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center">
                          <FaTruck className="h-5 w-5 text-gray-400 mr-3" />
                          <p className="text-sm font-medium text-indigo-600 truncate">
                            {truck.numberPlate}
                          </p>
                        </div>
                        <div className="ml-2 flex-shrink-0">
                          <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadgeColor(truck.operationalStatus)}`}>
                            {truck.operationalStatus.toUpperCase()}
                          </span>
                        </div>
                      </div>
                      <div className="mt-2 sm:flex sm:justify-between">
                        <div className="sm:flex">
                          <p className="flex items-center text-sm text-gray-500">
                            {truck.truckType?.name || `Type ID: ${truck.truckTypeId}`}
                          </p>
                          <p className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:ml-6">
                            Capacity: {truck.loadCapacityWeight} kg
                          </p>
                        </div>
                        <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                          <Link
                            to={`/trucker/trucks/${truck.id}`}
                            className="text-indigo-600 hover:text-indigo-900"
                          >
                            View Details
                          </Link>
                        </div>
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>

        {/* Recent Bookings Section */}
        <div className="mt-12">
          <h2 className="text-2xl font-bold text-gray-900 mb-6">Recent Bookings</h2>
          {loading ? (
            <div className="bg-white rounded-lg shadow-md p-6">
              <div className="animate-pulse flex space-x-4">
                <div className="flex-1 space-y-4 py-1">
                  <div className="h-4 bg-gray-200 rounded w-3/4"></div>
                  <div className="space-y-2">
                    <div className="h-4 bg-gray-200 rounded"></div>
                    <div className="h-4 bg-gray-200 rounded w-5/6"></div>
                  </div>
                </div>
              </div>
            </div>
          ) : bookings.length === 0 ? (
            <div className="bg-white rounded-lg shadow-md p-6 text-center">
              <p className="text-gray-600">No recent bookings to display.</p>
              <Link
                to="/available-loads"
                className="mt-4 inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700"
              >
                Find Available Loads
              </Link>
            </div>
          ) : (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <ul className="divide-y divide-gray-200">
                {bookings.slice(0, 5).map((booking) => (
                  <li key={booking.id}>
                    <div className="px-4 py-4 sm:px-6">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center">
                          <p className="text-sm font-medium text-indigo-600 truncate">
                            Booking #{booking.id}
                          </p>
                        </div>
                        <div className="ml-2 flex-shrink-0">
                          <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getBookingStatusColor(booking.status)}`}>
                            {booking.status.toUpperCase()}
                          </span>
                        </div>
                      </div>
                      <div className="mt-2 sm:flex sm:justify-between">
                        <div className="sm:flex">
                          <p className="flex items-center text-sm text-gray-500">
                            Load: {booking.load?.description || `#${booking.loadId}`}
                          </p>
                          <p className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:ml-6">
                            Price: {booking.agreedPrice} {booking.currency}
                          </p>
                        </div>
                        <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                          <Link
                            to={`/my-deliveries/${booking.id}`}
                            className="text-indigo-600 hover:text-indigo-900"
                          >
                            View Details
                          </Link>
                        </div>
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
              {bookings.length > 5 && (
                <div className="bg-gray-50 px-4 py-3 text-right sm:px-6">
                  <Link
                    to="/my-deliveries"
                    className="text-sm font-medium text-indigo-600 hover:text-indigo-500"
                  >
                    View all bookings
                  </Link>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TruckerDashboardPage; 