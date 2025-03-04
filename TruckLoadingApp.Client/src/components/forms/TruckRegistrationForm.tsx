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

  const { register, handleSubmit, formState: { errors }, watch } = useForm<TruckRegistrationRequest>();

  // Load categories and all truck types on component mount
  useEffect(() => {
    const loadData = async () => {
      try {
        const [categoriesData, typesData] = await Promise.all([
          referenceService.getTruckCategories(),
          referenceService.getTruckTypes()
        ]);
        setCategories(categoriesData);
        setTruckTypes(typesData);
      } catch (error) {
        toast.error('Failed to load truck data');
      }
    };
    loadData();
  }, []);

  const onSubmit = async (data: TruckRegistrationRequest) => {
    setIsLoading(true);
    try {
      const result = await truckService.registerTruck(data);
      toast.success('Truck registered successfully');
      onSuccess?.(result.truckId);
    } catch (error) {
      toast.error('Failed to register truck');
    } finally {
      setIsLoading(false);
    }
  };

  // Group truck types by category
  const groupedTruckTypes = categories.map(category => ({
    category,
    types: truckTypes.filter(type => type.categoryId === category.id)
  }));

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      {/* Truck Type Selection */}
      <div>
        <label htmlFor="truckTypeId" className="block text-sm font-medium text-gray-700">
          Truck Type
        </label>
        <select
          id="truckTypeId"
          {...register('truckTypeId', { required: 'Truck type is required' })}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
        >
          <option value="">Select a truck type</option>
          {groupedTruckTypes.map(group => (
            <optgroup key={group.category.id} label={group.category.categoryName}>
              {group.types.map(type => (
                <option key={type.id} value={type.id}>
                  {type.name}
                  {type.description && ` - ${type.description}`}
                </option>
              ))}
            </optgroup>
          ))}
        </select>
        {errors.truckTypeId && (
          <p className="mt-1 text-sm text-red-600">{errors.truckTypeId.message}</p>
        )}
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
            {...register('availabilityStartDate', { required: 'Start date is required' })}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
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
            {...register('availabilityEndDate', { required: 'End date is required' })}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
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
          className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:bg-gray-400"
        >
          {isLoading ? 'Registering...' : 'Register Truck'}
        </button>
      </div>
    </form>
  );
};

export default TruckRegistrationForm; 