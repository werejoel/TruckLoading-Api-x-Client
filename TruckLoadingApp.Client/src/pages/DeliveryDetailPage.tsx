import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import { FaTruck, FaMapMarkerAlt, FaCalendarAlt, FaMoneyBillWave, FaExclamationTriangle, FaArrowLeft } from 'react-icons/fa';
import truckerService from '../services/trucker.service';
import { Booking } from '../services/shipper.service';
import { format } from 'date-fns';

const DeliveryDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [booking, setBooking] = useState<Booking | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reportIssue, setReportIssue] = useState(false);
  const [issueDetails, setIssueDetails] = useState('');
  const [submittingIssue, setSubmittingIssue] = useState(false);

  useEffect(() => {
    const fetchBooking = async () => {
      if (!id) return;
      
      try {
        setLoading(true);
        setError(null);
        const data = await truckerService.getBookingById(parseInt(id));
        setBooking(data);
      } catch (err: any) {
        console.error('Error fetching booking:', err);
        setError(err.message || 'Failed to load data');
      } finally {
        setLoading(false);
      }
    };

    fetchBooking();
  }, [id]);

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

  const handleStartTransport = async () => {
    if (!booking) return;
    
    try {
      await truckerService.startTransport(booking.id);
      setBooking({ ...booking, status: 'In Transit' });
    } catch (err: any) {
      console.error('Error starting transport:', err);
      setError(err.message || 'Failed to start transport');
    }
  };

  const handleCompleteTransport = async () => {
    if (!booking) return;
    
    try {
      await truckerService.completeTransport(booking.id);
      setBooking({ ...booking, status: 'Completed' });
    } catch (err: any) {
      console.error('Error completing transport:', err);
      setError(err.message || 'Failed to complete transport');
    }
  };

  const handleSubmitIssue = async () => {
    if (!booking || !issueDetails.trim()) return;
    
    try {
      setSubmittingIssue(true);
      await truckerService.reportIssue(booking.id, issueDetails);
      setReportIssue(false);
      setIssueDetails('');
      // Show success message or update UI
    } catch (err: any) {
      console.error('Error reporting issue:', err);
      setError(err.message || 'Failed to report issue');
    } finally {
      setSubmittingIssue(false);
    }
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <button
          onClick={() => navigate('/my-deliveries')}
          className="mb-6 inline-flex items-center text-sm font-medium text-indigo-600 hover:text-indigo-500"
        >
          <FaArrowLeft className="mr-2" /> Back to Deliveries
        </button>

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

        {/* Loading indicator */}
        {loading && (
          <div className="flex justify-center items-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
          </div>
        )}

        {/* Booking not found */}
        {!loading && !error && !booking && (
          <div className="bg-white shadow overflow-hidden sm:rounded-md p-6 text-center">
            <FaTruck className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">Delivery not found</h3>
            <p className="mt-1 text-sm text-gray-500">
              The delivery you're looking for doesn't exist or you don't have permission to view it.
            </p>
          </div>
        )}

        {/* Booking details */}
        {!loading && !error && booking && (
          <div className="bg-white shadow overflow-hidden sm:rounded-lg">
            <div className="px-4 py-5 sm:px-6 flex justify-between items-center">
              <div>
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Delivery Details
                </h3>
                <p className="mt-1 max-w-2xl text-sm text-gray-500">
                  Booking #{booking.id}
                </p>
              </div>
              <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusBadgeColor(booking.status)}`}>
                {booking.status.toUpperCase()}
              </span>
            </div>
            <div className="border-t border-gray-200">
              <dl>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Load Description</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.load?.description || 'No description provided'}
                  </dd>
                </div>
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Pickup Location</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.load?.pickupAddress || 'No address provided'}
                  </dd>
                </div>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Delivery Location</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.load?.deliveryAddress || 'No address provided'}
                  </dd>
                </div>
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Pickup Date</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.load?.pickupDate ? formatDate(booking.load.pickupDate) : 'Not specified'}
                  </dd>
                </div>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Delivery Date</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.load?.deliveryDate ? formatDate(booking.load.deliveryDate) : 'Not specified'}
                  </dd>
                </div>
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Weight</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.load?.weight ? `${booking.load.weight} kg` : 'Not specified'}
                  </dd>
                </div>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Payment</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {booking.agreedPrice} {booking.currency} ({booking.pricingType})
                  </dd>
                </div>
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Booking Date</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {formatDate(booking.createdDate)}
                  </dd>
                </div>
                {booking.load?.specialRequirements && (
                  <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Special Requirements</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {booking.load.specialRequirements}
                    </dd>
                  </div>
                )}
              </dl>
            </div>

            {/* Action buttons */}
            <div className="px-4 py-5 sm:px-6 flex justify-end space-x-3">
              {booking.status.toLowerCase() === 'accepted' && (
                <button
                  onClick={handleStartTransport}
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700"
                >
                  Start Transport
                </button>
              )}
              
              {(booking.status.toLowerCase() === 'in transit' || booking.status.toLowerCase() === 'intransit') && (
                <button
                  onClick={handleCompleteTransport}
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700"
                >
                  Complete Delivery
                </button>
              )}
              
              {!reportIssue && (
                <button
                  onClick={() => setReportIssue(true)}
                  className="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md shadow-sm text-gray-700 bg-white hover:bg-gray-50"
                >
                  Report Issue
                </button>
              )}
            </div>

            {/* Report issue form */}
            {reportIssue && (
              <div className="px-4 py-5 sm:px-6 border-t border-gray-200">
                <h4 className="text-lg font-medium text-gray-900 mb-4">Report an Issue</h4>
                <div className="mb-4">
                  <label htmlFor="issueDetails" className="block text-sm font-medium text-gray-700 mb-2">
                    Issue Details
                  </label>
                  <textarea
                    id="issueDetails"
                    rows={4}
                    value={issueDetails}
                    onChange={(e) => setIssueDetails(e.target.value)}
                    className="shadow-sm focus:ring-indigo-500 focus:border-indigo-500 block w-full sm:text-sm border-gray-300 rounded-md"
                    placeholder="Describe the issue you're experiencing..."
                  />
                </div>
                <div className="flex justify-end space-x-3">
                  <button
                    type="button"
                    onClick={() => setReportIssue(false)}
                    className="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md shadow-sm text-gray-700 bg-white hover:bg-gray-50"
                  >
                    Cancel
                  </button>
                  <button
                    type="button"
                    onClick={handleSubmitIssue}
                    disabled={!issueDetails.trim() || submittingIssue}
                    className={`inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 ${
                      !issueDetails.trim() || submittingIssue ? 'opacity-50 cursor-not-allowed' : ''
                    }`}
                  >
                    {submittingIssue ? 'Submitting...' : 'Submit Issue'}
                  </button>
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </Layout>
  );
};

export default DeliveryDetailPage; 