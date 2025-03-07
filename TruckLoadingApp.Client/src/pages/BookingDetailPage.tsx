import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { format } from 'date-fns';
import shipperService, { Booking, BookingHistory, BookingAuditRecord } from '../services/shipper.service';
import { FaTruck, FaBox, FaHistory, FaRegListAlt, FaTimes, FaCheck } from 'react-icons/fa';

const BookingDetailPage: React.FC = () => {
  const { bookingId } = useParams<{ bookingId: string }>();
  const navigate = useNavigate();
  const [booking, setBooking] = useState<Booking | null>(null);
  const [history, setHistory] = useState<BookingHistory[]>([]);
  const [auditTrail, setAuditTrail] = useState<BookingAuditRecord[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'details' | 'history' | 'audit'>('details');

  useEffect(() => {
    const fetchData = async () => {
      if (!bookingId) return;
      
      try {
        setLoading(true);
        
        const id = parseInt(bookingId);
        
        // Fetch booking details
        const bookingData = await shipperService.getBookingById(id);
        setBooking(bookingData);
        
        // Fetch booking history
        try {
          const historyData = await shipperService.getBookingHistory(id);
          setHistory(historyData);
        } catch (historyError) {
          console.warn('Could not fetch booking history:', historyError);
          // Don't set an error for the whole page if only history fails
        }
        
        // Fetch audit trail
        try {
          const auditData = await shipperService.getBookingAuditTrail(id);
          setAuditTrail(auditData);
        } catch (auditError) {
          console.warn('Could not fetch audit trail:', auditError);
          // Don't set an error for the whole page if only audit trail fails
        }
        
        setError(null);
      } catch (err: any) {
        console.error('Error fetching booking details:', err);
        setError(err.message || 'Failed to load booking details');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [bookingId]);

  const handleCancelBooking = async () => {
    if (!booking || !window.confirm('Are you sure you want to cancel this booking?')) return;
    
    try {
      await shipperService.cancelBooking(booking.id);
      // Refresh booking data after cancellation
      const updatedBooking = await shipperService.getBookingById(booking.id);
      setBooking(updatedBooking);
      window.alert('Booking cancelled successfully');
    } catch (err: any) {
      console.error('Error cancelling booking:', err);
      window.alert(`Failed to cancel booking: ${err.message || 'Unknown error'}`);
    }
  };

  const getStatusBadgeColor = (status: string): string => {
    status = status.toLowerCase();
    switch (status) {
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'confirmed':
      case 'accepted':
        return 'bg-green-100 text-green-800';
      case 'inprogress':
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

  if (loading) {
    return (
      <div className="min-h-screen flex justify-center items-center">
        <div className="animate-pulse flex flex-col items-center">
          <div className="h-12 w-12 mb-4">
            <svg className="animate-spin h-12 w-12 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
          </div>
          <p className="text-indigo-600 font-medium">Loading booking details...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex flex-col justify-center items-center">
        <div className="bg-red-50 border-l-4 border-red-400 p-4 mb-6 max-w-md">
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
        <button
          onClick={() => navigate('/shipper/bookings')}
          className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none"
        >
          View All Bookings
        </button>
      </div>
    );
  }

  if (!booking) {
    return (
      <div className="min-h-screen flex justify-center items-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-600">Booking Not Found</h2>
          <p className="mt-2 text-gray-500">The requested booking could not be found.</p>
          <button
            onClick={() => navigate('/shipper/bookings')}
            className="mt-4 inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none"
          >
            View All Bookings
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-6">
        <nav className="flex" aria-label="Breadcrumb">
          <ol className="flex items-center space-x-4">
            <li>
              <div>
                <Link to="/shipper/dashboard" className="text-gray-400 hover:text-gray-500">
                  Dashboard
                </Link>
              </div>
            </li>
            <li>
              <div className="flex items-center">
                <svg className="flex-shrink-0 h-5 w-5 text-gray-300" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20" aria-hidden="true">
                  <path d="M5.555 17.776l8-16 .894.448-8 16-.894-.448z" />
                </svg>
                <Link to="/shipper/bookings" className="ml-4 text-gray-400 hover:text-gray-500">Bookings</Link>
              </div>
            </li>
            <li>
              <div className="flex items-center">
                <svg className="flex-shrink-0 h-5 w-5 text-gray-300" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20" aria-hidden="true">
                  <path d="M5.555 17.776l8-16 .894.448-8 16-.894-.448z" />
                </svg>
                <span className="ml-4 text-gray-500" aria-current="page">
                  Booking #{booking.id}
                </span>
              </div>
            </li>
          </ol>
        </nav>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-lg mb-6">
        <div className="px-4 py-5 sm:px-6 flex justify-between items-center">
          <div>
            <h2 className="text-lg leading-6 font-medium text-gray-900">Booking Details</h2>
            <p className="mt-1 max-w-2xl text-sm text-gray-500">
              Booking #{booking.id} - Created on {format(new Date(booking.createdDate), 'PPp')}
            </p>
          </div>
          <div>
            <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadgeColor(booking.status)}`}>
              {booking.status.toUpperCase()}
            </span>
          </div>
        </div>

        <div className="border-t border-gray-200">
          <nav className="flex justify-center space-x-8 px-4 py-3" aria-label="Tabs">
            <button
              onClick={() => setActiveTab('details')}
              className={`${
                activeTab === 'details'
                  ? 'text-indigo-600 border-indigo-500'
                  : 'text-gray-500 border-transparent hover:text-gray-700 hover:border-gray-300'
              } whitespace-nowrap pb-3 px-1 border-b-2 font-medium text-sm flex items-center space-x-1`}
            >
              <FaTruck />
              <span className="ml-2">Details</span>
            </button>
            <button
              onClick={() => setActiveTab('history')}
              className={`${
                activeTab === 'history'
                  ? 'text-indigo-600 border-indigo-500'
                  : 'text-gray-500 border-transparent hover:text-gray-700 hover:border-gray-300'
              } whitespace-nowrap pb-3 px-1 border-b-2 font-medium text-sm flex items-center space-x-1`}
            >
              <FaHistory />
              <span className="ml-2">Status History</span>
            </button>
            <button
              onClick={() => setActiveTab('audit')}
              className={`${
                activeTab === 'audit'
                  ? 'text-indigo-600 border-indigo-500'
                  : 'text-gray-500 border-transparent hover:text-gray-700 hover:border-gray-300'
              } whitespace-nowrap pb-3 px-1 border-b-2 font-medium text-sm flex items-center space-x-1`}
            >
              <FaRegListAlt />
              <span className="ml-2">Audit Trail</span>
            </button>
          </nav>
        </div>

        {activeTab === 'details' && (
          <>
            <div className="border-t border-gray-200">
              <dl>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Load</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2 flex items-center">
                    <FaBox className="mr-2 text-gray-400" />
                    {booking.load?.description || `Load #${booking.loadId}`}
                  </dd>
                </div>
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Truck</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.truck?.registrationNumber || `Truck #${booking.truckId}`}
                  </dd>
                </div>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Driver</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.truck?.driverName || 'Not assigned yet'}
                  </dd>
                </div>
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Price</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.agreedPrice} {booking.currency}
                  </dd>
                </div>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Pricing Model</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.pricingType}
                  </dd>
                </div>
                {booking.load && (
                  <>
                    <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                      <dt className="text-sm font-medium text-gray-500">Pickup Address</dt>
                      <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                        {booking.load.pickupAddress || 'Not specified'}
                      </dd>
                    </div>
                    <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                      <dt className="text-sm font-medium text-gray-500">Pickup Date</dt>
                      <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                        {booking.load.pickupDate ? format(new Date(booking.load.pickupDate), 'PPp') : 'Not specified'}
                      </dd>
                    </div>
                    <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                      <dt className="text-sm font-medium text-gray-500">Delivery Address</dt>
                      <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                        {booking.load.deliveryAddress || 'Not specified'}
                      </dd>
                    </div>
                    <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                      <dt className="text-sm font-medium text-gray-500">Delivery Date</dt>
                      <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                        {booking.load.deliveryDate ? format(new Date(booking.load.deliveryDate), 'PPp') : 'Not specified'}
                      </dd>
                    </div>
                  </>
                )}
              </dl>
            </div>

            <div className="px-4 py-3 bg-gray-50 text-right sm:px-6 border-t border-gray-200">
              {booking.status.toLowerCase() === 'pending' && (
                <button
                  onClick={handleCancelBooking}
                  className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                >
                  Cancel Booking
                </button>
              )}
            </div>
          </>
        )}

        {activeTab === 'history' && (
          <div className="border-t border-gray-200 px-4 py-5 sm:p-6">
            {history.length > 0 ? (
              <div className="flow-root">
                <ul className="-mb-8">
                  {history.map((item, itemIdx) => (
                    <li key={item.id}>
                      <div className="relative pb-8">
                        {itemIdx !== history.length - 1 ? (
                          <span className="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200" aria-hidden="true"></span>
                        ) : null}
                        <div className="relative flex space-x-3">
                          <div>
                            <span className={`h-8 w-8 rounded-full flex items-center justify-center ring-8 ring-white ${
                              item.newStatus?.toLowerCase() === 'cancelled' ? 'bg-red-500' : 
                              item.newStatus?.toLowerCase() === 'completed' ? 'bg-green-500' : 
                              'bg-blue-500'
                            }`}>
                              {item.newStatus?.toLowerCase() === 'cancelled' ? <FaTimes className="h-4 w-4 text-white" /> :
                               item.newStatus?.toLowerCase() === 'completed' ? <FaCheck className="h-4 w-4 text-white" /> :
                               <FaHistory className="h-4 w-4 text-white" />}
                            </span>
                          </div>
                          <div className="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                            <div>
                              <p className="text-sm text-gray-500">
                                {item.changeDescription}{' '}
                                {item.previousStatus && item.newStatus && (
                                  <span className="font-medium text-gray-900">
                                    from <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadgeColor(item.previousStatus)}`}>{item.previousStatus}</span>{' '}
                                    to <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadgeColor(item.newStatus)}`}>{item.newStatus}</span>
                                  </span>
                                )}
                              </p>
                              {item.details && Object.keys(item.details).length > 0 && (
                                <div className="mt-2 text-sm text-gray-700">
                                  <ul className="list-disc pl-5 space-y-1">
                                    {Object.entries(item.details).map(([key, value]) => (
                                      <li key={key}>{key}: {value}</li>
                                    ))}
                                  </ul>
                                </div>
                              )}
                            </div>
                            <div className="text-right text-sm whitespace-nowrap text-gray-500">
                              <time dateTime={item.timestamp.toString()}>
                                {format(new Date(item.timestamp), 'PPp')}
                              </time>
                              <p>{item.changedBy}</p>
                            </div>
                          </div>
                        </div>
                      </div>
                    </li>
                  ))}
                </ul>
              </div>
            ) : (
              <div className="text-center py-6">
                <svg
                  className="mx-auto h-12 w-12 text-gray-400"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                  aria-hidden="true"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                  />
                </svg>
                <h3 className="mt-2 text-sm font-medium text-gray-900">No history available</h3>
                <p className="mt-1 text-sm text-gray-500">No status changes have been recorded yet.</p>
              </div>
            )}
          </div>
        )}

        {activeTab === 'audit' && (
          <div className="border-t border-gray-200">
            {auditTrail.length > 0 ? (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Field
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Old Value
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        New Value
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Changed By
                      </th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Changed At
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {auditTrail.map((record) => (
                      <tr key={record.id}>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                          {record.field}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {record.oldValue || '-'}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {record.newValue || '-'}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {record.changedBy}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {format(new Date(record.changedAt), 'PPp')}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className="text-center py-6">
                <svg
                  className="mx-auto h-12 w-12 text-gray-400"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                  aria-hidden="true"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                  />
                </svg>
                <h3 className="mt-2 text-sm font-medium text-gray-900">No audit trail available</h3>
                <p className="mt-1 text-sm text-gray-500">No modifications have been recorded for this booking.</p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default BookingDetailPage;