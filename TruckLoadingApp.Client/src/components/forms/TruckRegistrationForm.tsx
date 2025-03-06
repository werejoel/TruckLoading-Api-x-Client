import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { TruckRegistrationRequest, TruckType, TruckCategory } from '../../types/truck.types';
import { referenceService, truckService } from '../../services';
import { toast } from 'react-toastify';

interface TruckRegistrationFormProps {
  onSuccess?: (truckId: number) => void;
}

const TruckRegistrationForm: React.FC<TruckRegistrationFormProps> = ({ onSuccess }) => {
  const [categories, setCategories] = useState<TruckCategory[]>([]);
  const [truckTypes, setTruckTypes] = useState<TruckType[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [loadingError, setLoadingError] = useState<string | null>(null);
  const [apiError, setApiError] = useState<string | null>(null); // New state for API errors
  const [debugInfo, setDebugInfo] = useState<string | null>(null);
  const [showDebug, setShowDebug] = useState(false);

  const { register, handleSubmit, formState: { errors }, watch, setValue } = useForm<TruckRegistrationRequest>();
  
  // Watch the start date to validate the end date
  const startDate = watch('availabilityStartDate');

  const loadTruckData = async () => {
    try {
      console.log('Starting to load truck categories and types...');
      setLoadingError(null);
      setApiError(null); // Clear any previous API errors
      setIsLoading(true);
      
      // Load truck types directly without categories
      console.log('Fetching truck types...');
      const typesData = await referenceService.getTruckTypes();
      console.log('Received truck types:', typesData);
      
      if (!Array.isArray(typesData)) {
        console.error('Truck types data is not an array:', typesData);
        setLoadingError('Truck types data format is invalid');
        return;
      }
      
      // Filter out any inactive truck types
      const activeTypes = typesData.filter(type => type.isActive !== false);
      setTruckTypes(activeTypes);
      
      // If we have truck types, set the first one as default
      if (activeTypes.length > 0) {
        console.log('Setting default truck type:', activeTypes[0].id);
        setValue('truckTypeId', activeTypes[0].id);
      } else {
        console.warn('No active truck types found');
        setLoadingError('No active truck types available');
      }
      
      console.log('Successfully loaded all truck data');
      setDebugInfo(JSON.stringify({ types: typesData }, null, 2));
    } catch (error) {
      console.error('Error loading truck data:', error);
      setLoadingError(error instanceof Error ? error.message : 'Unknown error');
      toast.error('Failed to load truck data');
    } finally {
      setIsLoading(false);
    }
  };

  // Load truck types on component mount
  useEffect(() => {
    loadTruckData();
  }, []);

  const onSubmit = async (data: TruckRegistrationRequest) => {
    setIsLoading(true);
    setApiError(null); // Clear any previous API errors
    try {
      // Format dates properly before sending to API
      const formattedData = {
        ...data,
        // Convert string dates from form inputs to Date objects
        availabilityStartDate: new Date(data.availabilityStartDate),
        availabilityEndDate: new Date(data.availabilityEndDate)
      };
      
      console.log('Submitting truck registration with data:', formattedData);
      const result = await truckService.registerTruck(formattedData);
      console.log('Registration result:', result);
      toast.success('Truck registered successfully');
      onSuccess?.(result.truckId);
    } catch (error) {
      console.error('Error registering truck:', error);
      
      // Extract and display more specific error message if available
      let errorMessage = 'Failed to register truck';
      if (error && typeof error === 'object' && 'response' in error) {
        const axiosError = error as any;
        if (axiosError.response?.data?.message) {
          errorMessage = axiosError.response.data.message;
        } else if (axiosError.response?.data?.errors) {
          // Handle validation errors
          const validationErrors = axiosError.response.data.errors;
          errorMessage = Object.values(validationErrors).flat().join(', ');
        } else if (axiosError.response?.status === 400) {
          errorMessage = 'Invalid data submitted. Please check your form entries.';
        }
      }
      
      // Set the API error message to display in the form
      setApiError(errorMessage);
      toast.error(errorMessage);
      
      // Add detailed error to debug info if debug is enabled
      if (showDebug) {
        setDebugInfo(JSON.stringify({ 
          error: error,
          errorDetails: error && typeof error === 'object' && 'response' in error ? (error as any).response?.data : null
        }, null, 2));
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleDebugToggle = () => {
    setShowDebug(!showDebug);
  };

  const handleRefresh = () => {
    loadTruckData();
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      {/* API Error Message */}
      {apiError && (
        <div className="p-4 mb-4 text-sm text-red-700 bg-red-100 rounded-lg">
          <p className="font-bold">Error:</p>
          <p>{apiError}</p>
        </div>
      )}
      
      {/* Debug Information */}
      {loadingError && (
        <div className="p-4 mb-4 text-sm text-red-700 bg-red-100 rounded-lg">
          <p className="font-bold">Error loading data:</p>
          <p>{loadingError}</p>
          <button 
            type="button" 
            onClick={handleRefresh}
            className="mt-2 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
          >
            Retry Loading Data
          </button>
        </div>
      )}
      
      {/* Debug Toggle */}
      <div className="flex justify-end">
        <button 
          type="button" 
          onClick={handleDebugToggle}
          className="px-3 py-1 text-xs bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
        >
          {showDebug ? 'Hide Debug' : 'Show Debug'}
        </button>
      </div>
      
      {/* Debug Panel */}
      {showDebug && (
        <div className="p-4 mb-4 text-sm text-gray-700 bg-gray-100 rounded-lg">
          <h3 className="font-bold">Debug Information</h3>
          <div className="mt-2">
            <p>Truck Types: {truckTypes.length}</p>
          </div>
          <div className="mt-2">
            <button 
              type="button" 
              onClick={handleRefresh}
              className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 mr-2"
            >
              Refresh Data
            </button>
          </div>
          {debugInfo && (
            <div className="mt-4">
              <h4 className="font-semibold">API Response:</h4>
              <pre className="mt-2 p-2 bg-gray-800 text-white text-xs rounded overflow-auto max-h-60">
                {debugInfo}
              </pre>
            </div>
          )}
        </div>
      )}
      
      {/* Truck Type Selection */}
      <div>
        <label htmlFor="truckTypeId" className="block text-sm font-medium text-gray-700">
          Truck Type <span className="text-red-500">*</span>
        </label>
        <select
          id="truckTypeId"
          {...register('truckTypeId', { 
            required: 'Truck type is required',
            validate: value => value > 0 || 'Please select a valid truck type' 
          })}
          className={`mt-1 block w-full py-2 px-3 border ${errors.truckTypeId ? 'border-red-500' : 'border-gray-300'} bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm`}
          disabled={isLoading || truckTypes.length === 0}
        >
          <option value="">-- Select a truck type --</option>
          {truckTypes.length > 0 ? (
            truckTypes.map(type => (
              <option key={type.id} value={type.id}>
                {type.name}
                {type.description && ` - ${type.description}`}
              </option>
            ))
          ) : (
            <option disabled>Loading truck types...</option>
          )}
        </select>
        {errors.truckTypeId && (
          <p className="mt-1 text-sm text-red-600">{errors.truckTypeId.message}</p>
        )}
        <div className="mt-1 text-xs text-gray-500">
          {truckTypes.length > 0 
            ? `${truckTypes.length} truck type${truckTypes.length !== 1 ? 's' : ''} available` 
            : isLoading 
              ? 'Loading truck types...' 
              : 'No truck types available'}
        </div>
      </div>

      {/* Number Plate */}
      <div>
        <label htmlFor="numberPlate" className="block text-sm font-medium text-gray-700">
          Number Plate
        </label>
        <input
          type="text"
          id="numberPlate"
          {...register('numberPlate', { required: 'Number plate is required' })}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
        />
        {errors.numberPlate && (
          <p className="mt-1 text-sm text-red-600">{errors.numberPlate.message}</p>
        )}
      </div>

      {/* Load Capacity Weight */}
      <div>
        <label htmlFor="loadCapacityWeight" className="block text-sm font-medium text-gray-700">
          Load Capacity Weight (kg)
        </label>
        <input
          type="number"
          id="loadCapacityWeight"
          {...register('loadCapacityWeight', { 
            required: 'Load capacity weight is required',
            min: { value: 0, message: 'Weight must be positive' }
          })}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
        />
        {errors.loadCapacityWeight && (
          <p className="mt-1 text-sm text-red-600">{errors.loadCapacityWeight.message}</p>
        )}
      </div>

      {/* Load Capacity Volume */}
      <div>
        <label htmlFor="loadCapacityVolume" className="block text-sm font-medium text-gray-700">
          Load Capacity Volume (m³)
        </label>
        <input
          type="number"
          id="loadCapacityVolume"
          {...register('loadCapacityVolume', { 
            required: 'Load capacity volume is required',
            min: { value: 0, message: 'Volume must be positive' }
          })}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
        />
        {errors.loadCapacityVolume && (
          <p className="mt-1 text-sm text-red-600">{errors.loadCapacityVolume.message}</p>
        )}
      </div>

      {/* Dimensions (Optional) */}
      <div className="grid grid-cols-3 gap-4">
        <div>
          <label htmlFor="height" className="block text-sm font-medium text-gray-700">
            Height (m)
          </label>
          <input
            type="number"
            id="height"
            {...register('height', { min: { value: 0, message: 'Height must be positive' } })}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
          />
          {errors.height && (
            <p className="mt-1 text-sm text-red-600">{errors.height.message}</p>
          )}
        </div>
        <div>
          <label htmlFor="width" className="block text-sm font-medium text-gray-700">
            Width (m)
          </label>
          <input
            type="number"
            id="width"
            {...register('width', { min: { value: 0, message: 'Width must be positive' } })}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
          />
          {errors.width && (
            <p className="mt-1 text-sm text-red-600">{errors.width.message}</p>
          )}
        </div>
        <div>
          <label htmlFor="length" className="block text-sm font-medium text-gray-700">
            Length (m)
          </label>
          <input
            type="number"
            id="length"
            {...register('length', { min: { value: 0, message: 'Length must be positive' } })}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
          />
          {errors.length && (
            <p className="mt-1 text-sm text-red-600">{errors.length.message}</p>
          )}
        </div>
      </div>

      {/* Availability Dates */}
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label htmlFor="availabilityStartDate" className="block text-sm font-medium text-gray-700">
            Available From
          </label>
          <input
            type="date"
            id="availabilityStartDate"
            {...register('availabilityStartDate', { 
              required: 'Start date is required',
              validate: value => {
                const date = new Date(value);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                return date >= today || 'Start date cannot be in the past';
              }
            })}
            className={`mt-1 block w-full rounded-md ${errors.availabilityStartDate ? 'border-red-500' : 'border-gray-300'} shadow-sm focus:border-indigo-500 focus:ring-indigo-500`}
          />
          {errors.availabilityStartDate && (
            <p className="mt-1 text-sm text-red-600">{errors.availabilityStartDate.message}</p>
          )}
        </div>
        <div>
          <label htmlFor="availabilityEndDate" className="block text-sm font-medium text-gray-700">
            Available Until
          </label>
          <input
            type="date"
            id="availabilityEndDate"
            {...register('availabilityEndDate', { 
              required: 'End date is required',
              validate: value => {
                if (!startDate) return true;
                const endDate = new Date(value);
                const start = new Date(startDate);
                return endDate >= start || 'End date must be after start date';
              }
            })}
            className={`mt-1 block w-full rounded-md ${errors.availabilityEndDate ? 'border-red-500' : 'border-gray-300'} shadow-sm focus:border-indigo-500 focus:ring-indigo-500`}
          />
          {errors.availabilityEndDate && (
            <p className="mt-1 text-sm text-red-600">{errors.availabilityEndDate.message}</p>
          )}
        </div>
      </div>

      <div>
        <button
          type="submit"
          disabled={isLoading}
          className={`w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 ${
            isLoading ? 'opacity-50 cursor-not-allowed' : ''
          }`}
        >
          {isLoading ? 'Registering...' : 'Register Truck'}
        </button>
      </div>
    </form>
  );
};

export default TruckRegistrationForm;