import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import shipperService, { Load, TruckMatchResponse, BookingCreateRequest } from '../../services/shipper.service';
import { format } from 'date-fns';

interface BookingFormProps {
  load: Load;
  truck: TruckMatchResponse;
  onSuccess?: (bookingId: number) => void;
  onCancel?: () => void;
}

const BookingForm: React.FC<BookingFormProps> = ({ 
  load, 
  truck, 
  onSuccess, 
  onCancel 
}) => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [specificErrors, setSpecificErrors] = useState<Record<string, string>>({});
  
  // Form state
  const [proposedPrice, setProposedPrice] = useState<number | undefined>(load.price);
  const [currency, setCurrency] = useState<string>(load.currency || 'USD');
  const [notes, setNotes] = useState<string>('');
  const [expressBooking, setExpressBooking] = useState<boolean>(false);
  const [requestedPickupDate, setRequestedPickupDate] = useState<Date | undefined>(load.pickupDate ? new Date(load.pickupDate) : undefined);
  const [requestedDeliveryDate, setRequestedDeliveryDate] = useState<Date | undefined>(load.deliveryDate ? new Date(load.deliveryDate) : undefined);
  
  // For optimistic UI updates
  const [bookingSubmitted, setBookingSubmitted] = useState<boolean>(false);
  const [submittedBookingId, setSubmittedBookingId] = useState<number | null>(null);
  
  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};
    
    if (proposedPrice !== undefined && proposedPrice <= 0) {
      newErrors.proposedPrice = 'Price must be greater than 0';
    }
    
    if (requestedPickupDate && requestedDeliveryDate && requestedPickupDate > requestedDeliveryDate) {
      newErrors.dates = 'Pickup date cannot be after delivery date';
    }
    
    setSpecificErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      setError('Please correct the errors in the form');
      return;
    }
    
    setLoading(true);
    setError(null);
    setSpecificErrors({});
    
    const bookingRequest: BookingCreateRequest = {
      loadId: load.id,
      truckId: truck.id,
      proposedPrice,
      currency,
      notes: notes || undefined,
      expressBooking,
      requestedPickupDate,
      requestedDeliveryDate
    };
    
    try {
      // Optimistic UI update
      setBookingSubmitted(true);
      
      // Actual API call
      const newBooking = await shipperService.createBookingRequest(bookingRequest);
      
      // Update with real data
      setSubmittedBookingId(newBooking.id);
      
      if (onSuccess) {
        onSuccess(newBooking.id);
      } else {
        navigate('/shipper/dashboard');
      }
    } catch (err: any) {
      console.error('Error creating booking:', err);
      setBookingSubmitted(false);
      
      // Handle different types of errors
      if (err.response) {
        if (err.response.status === 400) {
          // Validation error
          const validationErrors = err.response.data?.errors || {};
          const errorFields: Record<string, string> = {};
          
          // Map server validation errors to form fields
          Object.entries(validationErrors).forEach(([key, messages]: [string, any]) => {
            const fieldName = key.charAt(0).toLowerCase() + key.slice(1);
            errorFields[fieldName] = Array.isArray(messages) ? messages[0] : messages;
          });
          
          setSpecificErrors(errorFields);
          setError('Please correct the validation errors');
        } else if (err.response.status === 404) {
          setError('The load or truck no longer exists');
        } else if (err.response.status === 409) {
          setError('This load has already been booked');
        } else {
          setError(`Server error: ${err.response.data?.message || err.message || 'An unknown error occurred'}`);
        }
      } else if (err.request) {
        // Network error
        setError('Network error. Please check your connection and try again.');
      } else {
        setError(err.message || 'Failed to create booking. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  if (bookingSubmitted && !error) {
    return (
      <div className="bg-white shadow overflow-hidden sm:rounded-lg">
        <div className="px-4 py-5 sm:px-6">
          <h3 className="text-lg leading-6 font-medium text-gray-900">
            Booking Submitted
          </h3>
          <p className="mt-1 max-w-2xl text-sm text-gray-500">
            Your booking request has been submitted successfully.
          </p>
        </div>
        <div className="border-t border-gray-200 px-4 py-5 sm:px-6">
          <div className="bg-green-50 border-l-4 border-green-400 p-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg className="h-5 w-5 text-green-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                </svg>
              </div>
              <div className="ml-3">
                <p className="text-sm text-green-700">
                  {submittedBookingId 
                    ? `Booking #${submittedBookingId} created. We're processing your request.`
                    : 'Your booking request is being processed.'}
                </p>
              </div>
            </div>
          </div>
          <div className="mt-4">
            <button
              type="button"
              onClick={() => navigate('/shipper/bookings')}
              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              View All Bookings
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white shadow overflow-hidden sm:rounded-lg">
      <div className="px-4 py-5 sm:px-6">
        <h3 className="text-lg leading-6 font-medium text-gray-900">
          Book Truck
        </h3>
        <p className="mt-1 max-w-2xl text-sm text-gray-500">
          Confirm booking details for load #{load.id} with truck {truck.registrationNumber}
        </p>
      </div>
      
      {error && (
        <div className="bg-red-50 border-l-4 border-red-400 p-4 mx-6 mb-4">
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
      
      <div className="border-t border-gray-200">
        <dl>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Load Description</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {load.description || `Load #${load.id}`}
            </dd>
          </div>
          <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Truck</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {truck.registrationNumber} - {truck.truckTypeName}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Company</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {truck.companyName || 'Independent Trucker'}
              {truck.companyRating && ` • Rating: ${truck.companyRating.toFixed(1)}/5`}
            </dd>
          </div>
          <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Driver</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {truck.driverName || 'Not assigned yet'}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Distance to Pickup</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {truck.distanceToPickup.toFixed(1)} km
            </dd>
          </div>
          <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Original Pickup Date</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {load.pickupDate ? format(new Date(load.pickupDate), 'PPp') : 'Not specified'}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt className="text-sm font-medium text-gray-500">Original Delivery Date</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {load.deliveryDate ? format(new Date(load.deliveryDate), 'PPp') : 'Not specified'}
            </dd>
          </div>
        </dl>
      </div>
      
      <form onSubmit={handleSubmit} className="border-t border-gray-200">
        <div className="px-4 py-5 bg-white sm:p-6">
          <div className="grid grid-cols-1 gap-6">
            <div>
              <label htmlFor="proposedPrice" className="block text-sm font-medium text-gray-700">
                Proposed Price
              </label>
              <div className="mt-1 relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span className="text-gray-500 sm:text-sm">$</span>
                </div>
                <input
                  type="number"
                  name="proposedPrice"
                  id="proposedPrice"
                  min="0"
                  step="0.01"
                  value={proposedPrice || ''}
                  onChange={(e) => setProposedPrice(e.target.value ? Number(e.target.value) : undefined)}
                  className={`mt-1 block pl-7 w-full shadow-sm sm:text-sm rounded-md ${specificErrors.proposedPrice ? 'border-red-300 text-red-900 placeholder-red-300 focus:ring-red-500 focus:border-red-500' : 'border-gray-300 focus:ring-indigo-500 focus:border-indigo-500'}`}
                />
              </div>
              {specificErrors.proposedPrice && (
                <p className="mt-2 text-sm text-red-600">{specificErrors.proposedPrice}</p>
              )}
            </div>

            <div>
              <label htmlFor="currency" className="block text-sm font-medium text-gray-700">
                Currency
              </label>
              <select
                id="currency"
                name="currency"
                value={currency}
                onChange={(e) => setCurrency(e.target.value)}
                className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              >
                <option value="USD">USD - US Dollar</option>
                <option value="EUR">EUR - Euro</option>
                <option value="GBP">GBP - British Pound</option>
                <option value="CAD">CAD - Canadian Dollar</option>
                <option value="AUD">AUD - Australian Dollar</option>
              </select>
            </div>

            <div>
              <label htmlFor="expressBooking" className="flex items-center text-sm font-medium text-gray-700">
                <input
                  type="checkbox"
                  id="expressBooking"
                  name="expressBooking"
                  checked={expressBooking}
                  onChange={(e) => setExpressBooking(e.target.checked)}
                  className="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded mr-2"
                />
                Express Booking (Priority Handling)
              </label>
              <p className="mt-1 text-sm text-gray-500">
                Express bookings are processed with highest priority and may incur additional fees.
              </p>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label htmlFor="requestedPickupDate" className="block text-sm font-medium text-gray-700">
                  Requested Pickup Date
                </label>
                <input
                  type="datetime-local"
                  name="requestedPickupDate"
                  id="requestedPickupDate"
                  value={requestedPickupDate ? format(requestedPickupDate, "yyyy-MM-dd'T'HH:mm") : ''}
                  onChange={(e) => setRequestedPickupDate(e.target.value ? new Date(e.target.value) : undefined)}
                  className={`mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm ${specificErrors.dates ? 'border-red-300' : ''}`}
                />
              </div>

              <div>
                <label htmlFor="requestedDeliveryDate" className="block text-sm font-medium text-gray-700">
                  Requested Delivery Date
                </label>
                <input
                  type="datetime-local"
                  name="requestedDeliveryDate"
                  id="requestedDeliveryDate"
                  value={requestedDeliveryDate ? format(requestedDeliveryDate, "yyyy-MM-dd'T'HH:mm") : ''}
                  onChange={(e) => setRequestedDeliveryDate(e.target.value ? new Date(e.target.value) : undefined)}
                  className={`mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm ${specificErrors.dates ? 'border-red-300' : ''}`}
                />
              </div>
            </div>
            {specificErrors.dates && (
              <p className="mt-2 text-sm text-red-600">{specificErrors.dates}</p>
            )}
            
            <div>
              <label htmlFor="notes" className="block text-sm font-medium text-gray-700">
                Special Instructions & Notes
              </label>
              <textarea
                id="notes"
                name="notes"
                rows={3}
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                className="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                placeholder="Any special instructions or notes for the driver"
              />
              <p className="mt-1 text-sm text-gray-500">
                Include any special handling instructions, access codes, specific requirements, etc.
              </p>
            </div>
          </div>
        </div>
        
        <div className="px-4 py-3 bg-gray-50 text-right sm:px-6">
          {onCancel && (
            <button
              type="button"
              onClick={onCancel}
              className="mr-2 inline-flex justify-center py-2 px-4 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:border-indigo-500"
            >
              Cancel
            </button>
          )}
          <button
            type="submit"
            disabled={loading}
            className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:border-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <>
                <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Processing...
              </>
            ) : 'Confirm Booking'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default BookingForm;