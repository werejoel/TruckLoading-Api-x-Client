import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import shipperService, { LoadCreateRequest, Load } from '../../services/shipper.service';
import referenceService from '../../services/reference.service';

interface LoadFormProps {
  initialData?: Load;
  onSuccess?: (load: Load) => void;
  onCancel?: () => void;
  isEdit?: boolean;
}

interface LoadType {
  id: number;
  name: string;
}

interface TruckType {
  id: number;
  name: string;
}

const LoadForm: React.FC<LoadFormProps> = ({ 
  initialData, 
  onSuccess, 
  onCancel,
  isEdit = false 
}) => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loadTypes, setLoadTypes] = useState<LoadType[]>([]);
  const [truckTypes, setTruckTypes] = useState<TruckType[]>([]);
  
  const [formData, setFormData] = useState<LoadCreateRequest>({
    weight: initialData?.weight || 0,
    height: initialData?.height,
    width: initialData?.width,
    length: initialData?.length,
    description: initialData?.description || '',
    pickupDate: initialData?.pickupDate || new Date(),
    deliveryDate: initialData?.deliveryDate || new Date(Date.now() + 86400000), // Default to tomorrow
    specialRequirements: initialData?.specialRequirements || '',
    goodsType: initialData?.goodsType || '',
    loadTypeId: initialData?.loadTypeId || 1,
    price: initialData?.price,
    currency: initialData?.currency || 'USD',
    region: initialData?.region || '',
    requiredTruckTypeId: initialData?.requiredTruckTypeId,
    isStackable: initialData?.isStackable || false,
    requiresTemperatureControl: initialData?.requiresTemperatureControl || false,
    hazardousMaterialClass: initialData?.hazardousMaterialClass || '',
    handlingInstructions: initialData?.handlingInstructions || '',
    isFragile: initialData?.isFragile || false,
    requiresStackingControl: initialData?.requiresStackingControl || false,
    stackingInstructions: initialData?.stackingInstructions || '',
    unNumber: initialData?.unNumber || '',
    requiresCustomsDeclaration: initialData?.requiresCustomsDeclaration || false,
    customsDeclarationNumber: initialData?.customsDeclarationNumber || '',
  });

  // Additional fields for pickup and delivery locations
  const [pickupAddress, setPickupAddress] = useState(initialData?.pickupAddress || '');
  const [pickupLatitude, setPickupLatitude] = useState(initialData?.pickupLatitude || undefined);
  const [pickupLongitude, setPickupLongitude] = useState(initialData?.pickupLongitude || undefined);
  const [deliveryAddress, setDeliveryAddress] = useState(initialData?.deliveryAddress || '');
  const [deliveryLatitude, setDeliveryLatitude] = useState(initialData?.deliveryLatitude || undefined);
  const [deliveryLongitude, setDeliveryLongitude] = useState(initialData?.deliveryLongitude || undefined);

  useEffect(() => {
    const fetchReferenceData = async () => {
      try {
        const [loadTypesData, truckTypesData] = await Promise.all([
          referenceService.getLoadTypes(),
          referenceService.getTruckTypes()
        ]);
        
        setLoadTypes(loadTypesData);
        setTruckTypes(truckTypesData);
      } catch (err: any) {
        console.error('Error fetching reference data:', err);
        setError('Failed to load reference data. Please try again.');
      }
    };
    
    fetchReferenceData();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    
    if (type === 'checkbox') {
      const checked = (e.target as HTMLInputElement).checked;
      setFormData(prev => ({ ...prev, [name]: checked }));
    } else if (type === 'number') {
      setFormData(prev => ({ ...prev, [name]: value === '' ? undefined : Number(value) }));
    } else {
      setFormData(prev => ({ ...prev, [name]: value }));
    }
  };

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: new Date(value) }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    
    try {
      // Combine form data with location data
      const completeData = {
        ...formData,
        pickupAddress,
        pickupLatitude,
        pickupLongitude,
        deliveryAddress,
        deliveryLatitude,
        deliveryLongitude
      };
      
      let result;
      if (isEdit && initialData) {
        await shipperService.updateLoad(initialData.id, completeData);
        result = await shipperService.getLoadById(initialData.id);
      } else {
        result = await shipperService.createLoad(completeData);
      }
      
      if (onSuccess) {
        onSuccess(result);
      } else {
        navigate('/shipper/dashboard');
      }
    } catch (err: any) {
      console.error('Error saving load:', err);
      setError(err.message || 'Failed to save load. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white shadow overflow-hidden sm:rounded-lg">
      <div className="px-4 py-5 sm:px-6">
        <h3 className="text-lg leading-6 font-medium text-gray-900">
          {isEdit ? 'Edit Load' : 'Create New Load'}
        </h3>
        <p className="mt-1 max-w-2xl text-sm text-gray-500">
          {isEdit ? 'Update the details of your load' : 'Enter the details of your new load'}
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
      
      <form onSubmit={handleSubmit} className="border-t border-gray-200">
        <div className="px-4 py-5 bg-white sm:p-6">
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
            {/* Basic Information */}
            <div className="col-span-2">
              <h4 className="text-md font-medium text-gray-900 mb-4">Basic Information</h4>
            </div>
            
            <div>
              <label htmlFor="description" className="block text-sm font-medium text-gray-700">
                Description
              </label>
              <textarea
                id="description"
                name="description"
                rows={3}
                value={formData.description || ''}
                onChange={handleChange}
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>
            
            <div>
              <label htmlFor="goodsType" className="block text-sm font-medium text-gray-700">
                Goods Type
              </label>
              <input
                type="text"
                name="goodsType"
                id="goodsType"
                value={formData.goodsType}
                onChange={handleChange}
                required
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>
            
            <div>
              <label htmlFor="loadTypeId" className="block text-sm font-medium text-gray-700">
                Load Type
              </label>
              <select
                id="loadTypeId"
                name="loadTypeId"
                value={formData.loadTypeId}
                onChange={handleChange}
                required
                className="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              >
                {loadTypes.map(type => (
                  <option key={type.id} value={type.id}>{type.name}</option>
                ))}
              </select>
            </div>
            
            <div>
              <label htmlFor="requiredTruckTypeId" className="block text-sm font-medium text-gray-700">
                Required Truck Type
              </label>
              <select
                id="requiredTruckTypeId"
                name="requiredTruckTypeId"
                value={formData.requiredTruckTypeId || ''}
                onChange={handleChange}
                className="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              >
                <option value="">Any truck type</option>
                {truckTypes.map(type => (
                  <option key={type.id} value={type.id}>{type.name}</option>
                ))}
              </select>
            </div>
            
            {/* Dimensions */}
            <div className="col-span-2">
              <h4 className="text-md font-medium text-gray-900 mb-4 mt-4">Dimensions & Weight</h4>
            </div>
            
            <div>
              <label htmlFor="weight" className="block text-sm font-medium text-gray-700">
                Weight (kg)
              </label>
              <input
                type="number"
                name="weight"
                id="weight"
                min="0"
                step="0.01"
                value={formData.weight}
                onChange={handleChange}
                required
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>
            
            <div className="grid grid-cols-3 gap-4">
              <div>
                <label htmlFor="height" className="block text-sm font-medium text-gray-700">
                  Height (m)
                </label>
                <input
                  type="number"
                  name="height"
                  id="height"
                  min="0"
                  step="0.01"
                  value={formData.height || ''}
                  onChange={handleChange}
                  className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                />
              </div>
              <div>
                <label htmlFor="width" className="block text-sm font-medium text-gray-700">
                  Width (m)
                </label>
                <input
                  type="number"
                  name="width"
                  id="width"
                  min="0"
                  step="0.01"
                  value={formData.width || ''}
                  onChange={handleChange}
                  className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                />
              </div>
              <div>
                <label htmlFor="length" className="block text-sm font-medium text-gray-700">
                  Length (m)
                </label>
                <input
                  type="number"
                  name="length"
                  id="length"
                  min="0"
                  step="0.01"
                  value={formData.length || ''}
                  onChange={handleChange}
                  className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                />
              </div>
            </div>
            
            {/* Pickup & Delivery */}
            <div className="col-span-2">
              <h4 className="text-md font-medium text-gray-900 mb-4 mt-4">Pickup & Delivery</h4>
            </div>
            
            <div>
              <label htmlFor="pickupAddress" className="block text-sm font-medium text-gray-700">
                Pickup Address
              </label>
              <input
                type="text"
                id="pickupAddress"
                value={pickupAddress}
                onChange={(e) => setPickupAddress(e.target.value)}
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
              <div className="mt-2 grid grid-cols-2 gap-2">
                <div>
                  <label htmlFor="pickupLatitude" className="block text-sm font-medium text-gray-700">
                    Latitude
                  </label>
                  <input
                    type="number"
                    id="pickupLatitude"
                    step="0.000001"
                    value={pickupLatitude || ''}
                    onChange={(e) => setPickupLatitude(e.target.value ? Number(e.target.value) : undefined)}
                    className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                  />
                </div>
                <div>
                  <label htmlFor="pickupLongitude" className="block text-sm font-medium text-gray-700">
                    Longitude
                  </label>
                  <input
                    type="number"
                    id="pickupLongitude"
                    step="0.000001"
                    value={pickupLongitude || ''}
                    onChange={(e) => setPickupLongitude(e.target.value ? Number(e.target.value) : undefined)}
                    className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                  />
                </div>
              </div>
            </div>
            
            <div>
              <label htmlFor="deliveryAddress" className="block text-sm font-medium text-gray-700">
                Delivery Address
              </label>
              <input
                type="text"
                id="deliveryAddress"
                value={deliveryAddress}
                onChange={(e) => setDeliveryAddress(e.target.value)}
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
              <div className="mt-2 grid grid-cols-2 gap-2">
                <div>
                  <label htmlFor="deliveryLatitude" className="block text-sm font-medium text-gray-700">
                    Latitude
                  </label>
                  <input
                    type="number"
                    id="deliveryLatitude"
                    step="0.000001"
                    value={deliveryLatitude || ''}
                    onChange={(e) => setDeliveryLatitude(e.target.value ? Number(e.target.value) : undefined)}
                    className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                  />
                </div>
                <div>
                  <label htmlFor="deliveryLongitude" className="block text-sm font-medium text-gray-700">
                    Longitude
                  </label>
                  <input
                    type="number"
                    id="deliveryLongitude"
                    step="0.000001"
                    value={deliveryLongitude || ''}
                    onChange={(e) => setDeliveryLongitude(e.target.value ? Number(e.target.value) : undefined)}
                    className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                  />
                </div>
              </div>
            </div>
            
            <div>
              <label htmlFor="pickupDate" className="block text-sm font-medium text-gray-700">
                Pickup Date & Time
              </label>
              <input
                type="datetime-local"
                name="pickupDate"
                id="pickupDate"
                value={formData.pickupDate instanceof Date ? formData.pickupDate.toISOString().slice(0, 16) : new Date(formData.pickupDate).toISOString().slice(0, 16)}
                onChange={handleDateChange}
                required
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>
            
            <div>
              <label htmlFor="deliveryDate" className="block text-sm font-medium text-gray-700">
                Delivery Date & Time
              </label>
              <input
                type="datetime-local"
                name="deliveryDate"
                id="deliveryDate"
                value={formData.deliveryDate instanceof Date ? formData.deliveryDate.toISOString().slice(0, 16) : new Date(formData.deliveryDate).toISOString().slice(0, 16)}
                onChange={handleDateChange}
                required
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>
            
            {/* Special Requirements */}
            <div className="col-span-2">
              <h4 className="text-md font-medium text-gray-900 mb-4 mt-4">Special Requirements</h4>
            </div>
            
            <div className="col-span-2">
              <label htmlFor="specialRequirements" className="block text-sm font-medium text-gray-700">
                Special Requirements
              </label>
              <textarea
                id="specialRequirements"
                name="specialRequirements"
                rows={3}
                value={formData.specialRequirements || ''}
                onChange={handleChange}
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>
            
            <div>
              <div className="flex items-start">
                <div className="flex items-center h-5">
                  <input
                    id="isFragile"
                    name="isFragile"
                    type="checkbox"
                    checked={formData.isFragile}
                    onChange={handleChange}
                    className="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300 rounded"
                  />
                </div>
                <div className="ml-3 text-sm">
                  <label htmlFor="isFragile" className="font-medium text-gray-700">Fragile</label>
                  <p className="text-gray-500">The load contains fragile items that require careful handling</p>
                </div>
              </div>
            </div>
            
            <div>
              <div className="flex items-start">
                <div className="flex items-center h-5">
                  <input
                    id="isStackable"
                    name="isStackable"
                    type="checkbox"
                    checked={formData.isStackable}
                    onChange={handleChange}
                    className="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300 rounded"
                  />
                </div>
                <div className="ml-3 text-sm">
                  <label htmlFor="isStackable" className="font-medium text-gray-700">Stackable</label>
                  <p className="text-gray-500">The load can be stacked with other loads</p>
                </div>
              </div>
            </div>
            
            <div>
              <div className="flex items-start">
                <div className="flex items-center h-5">
                  <input
                    id="requiresTemperatureControl"
                    name="requiresTemperatureControl"
                    type="checkbox"
                    checked={formData.requiresTemperatureControl}
                    onChange={handleChange}
                    className="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300 rounded"
                  />
                </div>
                <div className="ml-3 text-sm">
                  <label htmlFor="requiresTemperatureControl" className="font-medium text-gray-700">Temperature Control</label>
                  <p className="text-gray-500">The load requires temperature-controlled transportation</p>
                </div>
              </div>
            </div>
            
            <div>
              <div className="flex items-start">
                <div className="flex items-center h-5">
                  <input
                    id="requiresCustomsDeclaration"
                    name="requiresCustomsDeclaration"
                    type="checkbox"
                    checked={formData.requiresCustomsDeclaration}
                    onChange={handleChange}
                    className="focus:ring-indigo-500 h-4 w-4 text-indigo-600 border-gray-300 rounded"
                  />
                </div>
                <div className="ml-3 text-sm">
                  <label htmlFor="requiresCustomsDeclaration" className="font-medium text-gray-700">Customs Declaration</label>
                  <p className="text-gray-500">The load requires customs declaration for international shipping</p>
                </div>
              </div>
            </div>
            
            {formData.requiresCustomsDeclaration && (
              <div>
                <label htmlFor="customsDeclarationNumber" className="block text-sm font-medium text-gray-700">
                  Customs Declaration Number
                </label>
                <input
                  type="text"
                  name="customsDeclarationNumber"
                  id="customsDeclarationNumber"
                  value={formData.customsDeclarationNumber || ''}
                  onChange={handleChange}
                  className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
                />
              </div>
            )}
            
            {/* Pricing */}
            <div className="col-span-2">
              <h4 className="text-md font-medium text-gray-900 mb-4 mt-4">Pricing</h4>
            </div>
            
            <div>
              <label htmlFor="price" className="block text-sm font-medium text-gray-700">
                Price
              </label>
              <input
                type="number"
                name="price"
                id="price"
                min="0"
                step="0.01"
                value={formData.price || ''}
                onChange={handleChange}
                className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>
            
            <div>
              <label htmlFor="currency" className="block text-sm font-medium text-gray-700">
                Currency
              </label>
              <select
                id="currency"
                name="currency"
                value={formData.currency || 'USD'}
                onChange={handleChange}
                className="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              >
                <option value="USD">USD</option>
                <option value="EUR">EUR</option>
                <option value="GBP">GBP</option>
                <option value="CAD">CAD</option>
                <option value="AUD">AUD</option>
                <option value="JPY">JPY</option>
              </select>
            </div>
          </div>
        </div>
        
        <div className="px-4 py-3 bg-gray-50 text-right sm:px-6">
          {onCancel && (
            <button
              type="button"
              onClick={onCancel}
              className="mr-2 inline-flex justify-center py-2 px-4 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Cancel
            </button>
          )}
          <button
            type="submit"
            disabled={loading}
            className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            {loading ? 'Saving...' : isEdit ? 'Update Load' : 'Create Load'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default LoadForm; 