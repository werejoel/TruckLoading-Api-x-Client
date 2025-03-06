import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import { FaSearch, FaTruck, FaMapMarkerAlt, FaCalendarAlt, FaExclamationTriangle } from 'react-icons/fa';
import truckerService from '../services/trucker.service';
import { Booking } from '../services/shipper.service';
import { format } from 'date-fns';

const MyDeliveriesPage: React.FC = () => {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await truckerService.getBookings();
        setBookings(data);
      } catch (err: any) {
        console.error('Error fetching bookings:', err);
        setError(err.message || 'Failed to load data');
      } finally {
        setLoading(false);
      }
    };

    fetchBookings();
  }, []);

  const formatDate = (dateString: string | Date) => {
    if (!dateString) return 'N/A';
    try {
      return format(new Date(dateString), 'MMM dd, yyyy');
    } catch (e) {
      return 'Invalid date';
    }
  };

  const getStatusBadgeColor = (status: string) => {
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

  const handleStartTransport = async (bookingId: number) => {
    try {
      await truckerService.startTransport(bookingId);
      // Update the booking status in the local state
      setBookings(bookings.map(booking => 
        booking.id === bookingId 
          ? { ...booking, status: 'In Transit' } 
          : booking
      ));
    } catch (err: any) {
      console.error('Error starting transport:', err);
      setError(err.message || 'Failed to start transport');
    }
  };

  const handleCompleteTransport = async (bookingId: number) => {
    try {
      await truckerService.completeTransport(bookingId);
      // Update the booking status in the local state
      setBookings(bookings.map(booking => 
        booking.id === bookingId 
          ? { ...booking, status: 'Completed' } 
          : booking
      ));
    } catch (err: any) {
      console.error('Error completing transport:', err);
      setError(err.message || 'Failed to complete transport');
    }
  };

  // Filter bookings based on search term and status filter
  const filteredBookings = bookings.filter(booking => {
    const matchesSearch = 
      (booking.load?.description || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
      (booking.load?.pickupAddress || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
      (booking.load?.deliveryAddress || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
      (`booking #${booking.id}`).toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesStatus = 
      statusFilter === 'all' || 
      booking.status.toLowerCase() === statusFilter.toLowerCase();
    
    return matchesSearch && matchesStatus;
  });

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">My Deliveries</h1>
          <p className="mt-2 text-gray-600">View and manage your current and past deliveries.</p>
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

        {/* Search and filters */}
        <div className="bg-white shadow rounded-lg p-4 mb-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label htmlFor="search" className="block text-sm font-medium text-gray-700">
                Search
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
                  placeholder="Search by load description or location"
                />
              </div>
            </div>
            <div>
              <label htmlFor="status" className="block text-sm font-medium text-gray-700">
                Status
              </label>
              <select
                id="status"
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value)}
                className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              >
                <option value="all">All Statuses</option>
                <option value="pending">Pending</option>
                <option value="accepted">Accepted</option>
                <option value="in transit">In Transit</option>
                <option value="completed">Completed</option>
                <option value="cancelled">Cancelled</option>
              </select>
            </div>
          </div>
        </div>

        {/* Loading indicator */}
        {loading && (
          <div className="flex justify-center items-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
          </div>
        )}

        {/* No bookings message */}
        {!loading && !error && filteredBookings.length === 0 && (
          <div className="bg-white shadow overflow-hidden sm:rounded-md p-6 text-center">
            <FaTruck className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No deliveries found</h3>
            <p className="mt-1 text-sm text-gray-500">
              {searchTerm || statusFilter !== 'all' 
                ? 'Try adjusting your search filters.' 
                : 'You have no deliveries yet. Check available loads to find work.'}
            </p>
            {!searchTerm && statusFilter === 'all' && (
              <div className="mt-6">
                <Link
                  to="/available-loads"
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700"
                >
                  Find Available Loads
                </Link>
              </div>
            )}
          </div>
        )}

        {/* Bookings list */}
        {!loading && !error && filteredBookings.length > 0 && (
          <div className="bg-white shadow overflow-hidden sm:rounded-md">
            <ul className="divide-y divide-gray-200">
              {filteredBookings.map((booking) => (
                <li key={booking.id}>
                  <div className="px-4 py-4 sm:px-6 hover:bg-gray-50">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center">
                        <FaTruck className="h-5 w-5 text-gray-400 mr-3" />
                        <p className="text-sm font-medium text-indigo-600 truncate">
                          {booking.load?.description || `Booking #${booking.id}`}
                        </p>
                      </div>
                      <div className="ml-2 flex-shrink-0">
                        <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadgeColor(booking.status)}`}>
                          {booking.status.toUpperCase()}
                        </span>
                      </div>
                    </div>
                    <div className="mt-2 sm:flex sm:justify-between">
                      <div className="sm:flex sm:flex-col">
                        <div className="flex items-center text-sm text-gray-500">
                          <FaMapMarkerAlt className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                          <p>From: {booking.load?.pickupAddress || 'Unknown location'}</p>
                        </div>
                        <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-2">
                          <FaMapMarkerAlt className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                          <p>To: {booking.load?.deliveryAddress || 'Unknown location'}</p>
                        </div>
                      </div>
                      <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0 sm:flex-col sm:items-end">
                        <div className="flex items-center">
                          <FaCalendarAlt className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" />
                          <p>Created: {formatDate(booking.createdDate)}</p>
                        </div>
                        <div className="mt-2 flex items-center">
                          <p>Price: {booking.agreedPrice} {booking.currency}</p>
                        </div>
                      </div>
                    </div>
                    <div className="mt-4 flex justify-end space-x-3">
                      <Link
                        to={`/my-deliveries/${booking.id}`}
                        className="inline-flex items-center px-3 py-1 border border-gray-300 text-sm leading-4 font-medium rounded text-gray-700 bg-white hover:bg-gray-50"
                      >
                        View Details
                      </Link>
                      
                      {booking.status.toLowerCase() === 'accepted' && (
                        <button
                          onClick={() => handleStartTransport(booking.id)}
                          className="inline-flex items-center px-3 py-1 border border-transparent text-sm leading-4 font-medium rounded text-white bg-indigo-600 hover:bg-indigo-700"
                        >
                          Start Transport
                        </button>
                      )}
                      
                      {booking.status.toLowerCase() === 'in transit' || booking.status.toLowerCase() === 'intransit' ? (
                        <button
                          onClick={() => handleCompleteTransport(booking.id)}
                          className="inline-flex items-center px-3 py-1 border border-transparent text-sm leading-4 font-medium rounded text-white bg-green-600 hover:bg-green-700"
                        >
                          Complete Delivery
                        </button>
                      ) : null}
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

export default MyDeliveriesPage; 