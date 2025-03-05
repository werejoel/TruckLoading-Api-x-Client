import React, { useState, useEffect, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import shipperService, { LoadCreateRequest, Load } from '../../services/shipper.service';
import referenceService from '../../services/reference.service';
import MapSelector from '../maps/MapSelector';

interface ExtendedLoadCreateRequest extends LoadCreateRequest {
  pickupAddress?: string;
  pickupLatitude?: number;
  pickupLongitude?: number;
  deliveryAddress?: string;
  deliveryLatitude?: number;
  deliveryLongitude?: number;
}

interface LoadFormWizardProps {
  initialData?: Partial<ExtendedLoadCreateRequest>;
  onSuccess?: (data: LoadCreateRequest) => void;
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
  description?: string;
  categoryId: number;
}

type Step = 'basic' | 'location' | 'schedule' | 'pricing';

const LoadFormWizard: React.FC<LoadFormWizardProps> = ({
  initialData,
  onSuccess,
  onCancel,
  isEdit = false
}) => {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState<Step>('basic');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loadTypes, setLoadTypes] = useState<LoadType[]>([]);
  const [truckTypes, setTruckTypes] = useState<TruckType[]>([]);
  
  // Ensure initialData.goodsType is a number
  const initialGoodsType = typeof initialData?.goodsType === 'string' 
    ? parseInt(initialData.goodsType, 10) || 0 
    : initialData?.goodsType || 0;

  const [formData, setFormData] = useState<ExtendedLoadCreateRequest>({
    weight: initialData?.weight || 0,
    height: initialData?.height,
    width: initialData?.width,
    length: initialData?.length,
    description: initialData?.description || '',
    pickupDate: initialData?.pickupDate || new Date(),
    deliveryDate: initialData?.deliveryDate || new Date(Date.now() + 86400000),
    specialRequirements: initialData?.specialRequirements || '',
    goodsType: initialGoodsType,
    loadTypeId: initialData?.loadTypeId || undefined,
    price: initialData?.price,
    currency: initialData?.currency || 'USD',
    region: initialData?.region || '',
    requiredTruckTypeId: initialData?.requiredTruckTypeId,
    isStackable: initialData?.isStackable || false,
    requiresTemperatureControl: initialData?.requiresTemperatureControl || false,
    hazardousMaterialClass: initialData?.hazardousMaterialClass !== undefined ? initialData.hazardousMaterialClass : 0, // Default to None (0)
    handlingInstructions: initialData?.handlingInstructions || '',
    isFragile: initialData?.isFragile || false,
    requiresStackingControl: initialData?.requiresStackingControl || false,
    stackingInstructions: initialData?.stackingInstructions || '',
    unNumber: initialData?.unNumber || '',
    requiresCustomsDeclaration: initialData?.requiresCustomsDeclaration || false,
    customsDeclarationNumber: initialData?.customsDeclarationNumber || '',
    pickupAddress: initialData?.pickupAddress || '',
    pickupLatitude: initialData?.pickupLatitude,
    pickupLongitude: initialData?.pickupLongitude,
    deliveryAddress: initialData?.deliveryAddress || '',
    deliveryLatitude: initialData?.deliveryLatitude,
    deliveryLongitude: initialData?.deliveryLongitude,
  });

  useEffect(() => {
    const fetchReferenceData = async () => {
      try {
        const [loadTypesData, truckTypesData] = await Promise.all([
          referenceService.getLoadTypes(),
          referenceService.getTruckTypes()
        ]);
        
        setLoadTypes(loadTypesData);
        setTruckTypes(truckTypesData);

        // Set the initial loadTypeId to the first available load type if none is selected
        if (!formData.loadTypeId && loadTypesData.length > 0) {
          setFormData(prev => ({
            ...prev,
            loadTypeId: loadTypesData[0].id
          }));
        }
      } catch (error: unknown) {
        console.error('Error fetching reference data:', error);
        setError(error instanceof Error ? error.message : 'Failed to load reference data. Please try again.');
      }
    };
    
    fetchReferenceData();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    
    if (type === 'checkbox') {
      const checked = (e.target as HTMLInputElement).checked;
      setFormData(prev => ({ ...prev, [name]: checked }));
    } else if (type === 'number' || name === 'goodsType' || name === 'loadTypeId' || name === 'requiredTruckTypeId' || name === 'hazardousMaterialClass') {
      // Ensure these fields are converted to numbers
      setFormData(prev => ({ ...prev, [name]: value === '' ? undefined : Number(value) }));
    } else {
      setFormData(prev => ({ ...prev, [name]: value }));
    }
  };

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: new Date(value) }));
  };

  const handlePickupLocationSelect = (location: { lat: number; lng: number; address?: string }) => {
    setFormData(prev => ({
      ...prev,
      pickupAddress: location.address || '',
      pickupLatitude: location.lat,
      pickupLongitude: location.lng
    }));
  };

  const handleDeliveryLocationSelect = (location: { lat: number; lng: number; address?: string }) => {
    setFormData(prev => ({
      ...prev,
      deliveryAddress: location.address || '',
      deliveryLatitude: location.lat,
      deliveryLongitude: location.lng
    }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    
    // Only process submission if we're on the pricing step
    if (currentStep !== 'pricing') {
      nextStep();
      return;
    }

    setLoading(true);
    setError('');

    // Validate required fields including loadTypeId
    if (!formData.weight || !formData.goodsType || !formData.pickupDate || !formData.deliveryDate) {
      setError('Please fill in all required fields');
      setLoading(false);
      return;
    }

    // Validate that loadTypeId exists and is valid
    if (!formData.loadTypeId || formData.loadTypeId === 0 || !loadTypes.some(lt => lt.id === formData.loadTypeId)) {
      setError('Please select a valid load type');
      setLoading(false);
      return;
    }

    // Validate location data
    if (!formData.pickupAddress || !formData.pickupLatitude || !formData.pickupLongitude ||
        !formData.deliveryAddress || !formData.deliveryLatitude || !formData.deliveryLongitude) {
      setError('Please provide complete pickup and delivery location information');
      setLoading(false);
      return;
    }

    try {
      // Create the final request object including location properties
      const requestData: LoadCreateRequest = {
        weight: formData.weight,
        height: formData.height,
        width: formData.width,
        length: formData.length,
        description: formData.description,
        pickupDate: formData.pickupDate,
        deliveryDate: formData.deliveryDate,
        specialRequirements: formData.specialRequirements,
        goodsType: formData.goodsType,
        loadTypeId: formData.loadTypeId,
        price: formData.price,
        currency: formData.currency,
        region: formData.region,
        requiredTruckTypeId: formData.requiredTruckTypeId,
        isStackable: formData.isStackable,
        requiresTemperatureControl: formData.requiresTemperatureControl,
        hazardousMaterialClass: formData.hazardousMaterialClass,
        handlingInstructions: formData.handlingInstructions,
        isFragile: formData.isFragile,
        requiresStackingControl: formData.requiresStackingControl,
        stackingInstructions: formData.stackingInstructions,
        unNumber: formData.unNumber,
        requiresCustomsDeclaration: formData.requiresCustomsDeclaration,
        customsDeclarationNumber: formData.customsDeclarationNumber,
        pickupAddress: formData.pickupAddress,
        pickupLatitude: formData.pickupLatitude,
        pickupLongitude: formData.pickupLongitude,
        deliveryAddress: formData.deliveryAddress,
        deliveryLatitude: formData.deliveryLatitude,
        deliveryLongitude: formData.deliveryLongitude
      };

      if (onSuccess) {
        await onSuccess(requestData);
      }
    } catch (err) {
      setError('Failed to create load. Please try again.');
      console.error('Error creating load:', err);
    } finally {
      setLoading(false);
    }
  };

  const nextStep = () => {
    switch (currentStep) {
      case 'basic':
        if (!formData.description || !formData.weight || !formData.goodsType) {
          setError('Please fill in all required basic information');
          return;
        }
        setCurrentStep('location');
        break;
      case 'location':
        if (!formData.pickupAddress || formData.pickupLatitude === undefined || formData.pickupLongitude === undefined ||
            !formData.deliveryAddress || formData.deliveryLatitude === undefined || formData.deliveryLongitude === undefined) {
          setError('Please provide complete pickup and delivery locations by searching or clicking on the map');
          return;
        }
        setCurrentStep('schedule');
        break;
      case 'schedule':
        if (!formData.pickupDate || !formData.deliveryDate) {
          setError('Please provide pickup and delivery dates');
          return;
        }
        setCurrentStep('pricing');
        break;
      case 'pricing':
        setCurrentStep('pricing');
        break;
    }
  };

  const prevStep = () => {
    switch (currentStep) {
      case 'location':
        setCurrentStep('basic');
        break;
      case 'schedule':
        setCurrentStep('location');
        break;
      case 'pricing':
        setCurrentStep('schedule');
        break;
    }
  };

  const renderBasicInfo = () => (
    <div className="space-y-6">
      <div>
        <label htmlFor="description" className="block text-sm font-medium text-gray-700">
          Description
        </label>
        <textarea
          id="description"
          name="description"
          rows={3}
          value={formData.description}
          onChange={handleChange}
          className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
        />
      </div>

      <div>
        <label htmlFor="goodsType" className="block text-sm font-medium text-gray-700">
          Goods Type
        </label>
        <select
          id="goodsType"
          name="goodsType"
          value={formData.goodsType}
          onChange={handleChange}
          required
          className="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
        >
          <option value="">Select goods type</option>
          <option value={1}>General</option>
          <option value={2}>Refrigerated</option>
          <option value={3}>Hazardous</option>
          <option value={4}>Fragile</option>
          <option value={5}>Oversized</option>
        </select>
      </div>

      <div>
        <label htmlFor="loadTypeId" className="block text-sm font-medium text-gray-700">
          Load Type
        </label>
        <select
          id="loadTypeId"
          name="loadTypeId"
          value={formData.loadTypeId || ''}
          onChange={(e) => {
            console.log('Load type changed:', e.target.value, 'as number:', Number(e.target.value));
            handleChange(e);
          }}
          required
          className="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
        >
          <option value="">Select load type</option>
          {loadTypes.length > 0 ? (
            loadTypes.map(type => (
              <option key={type.id} value={type.id}>{type.name}</option>
            ))
          ) : (
            <option value="" disabled>Loading load types...</option>
          )}
        </select>
        <div className="mt-1 text-xs text-gray-500">
          {loadTypes.length > 0 ? `${loadTypes.length} load types available` : 'Loading load types...'}
        </div>
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
            <option key={type.id} value={type.id}>
              {type.name}
              {type.description && ` - ${type.description}`}
            </option>
          ))}
        </select>
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

      <div className="space-y-4">
        <h4 className="text-lg font-medium text-gray-900">Special Requirements</h4>
        
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

        <div>
          <label htmlFor="hazardousMaterialClass" className="block text-sm font-medium text-gray-700">
            Hazardous Material Class
          </label>
          <select
            id="hazardousMaterialClass"
            name="hazardousMaterialClass"
            value={formData.hazardousMaterialClass}
            onChange={(e) => {
              setFormData(prev => ({
                ...prev,
                hazardousMaterialClass: Number(e.target.value)
              }));
            }}
            className="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
          >
            <option value="0">None</option>
            <option value="1">Explosives</option>
            <option value="2">Gases</option>
            <option value="3">Flammable Liquids</option>
            <option value="4">Flammable Solids</option>
            <option value="5">Oxidizing Substances</option>
            <option value="6">Toxic Substances</option>
            <option value="7">Radioactive Material</option>
            <option value="8">Corrosive Substances</option>
            <option value="9">Miscellaneous Dangerous Goods</option>
          </select>
        </div>

        <div>
          <label htmlFor="specialRequirements" className="block text-sm font-medium text-gray-700">
            Additional Requirements
          </label>
          <textarea
            id="specialRequirements"
            name="specialRequirements"
            rows={3}
            value={formData.specialRequirements}
            onChange={handleChange}
            className="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
            placeholder="Any other special requirements or instructions"
          />
        </div>
      </div>
    </div>
  );

  const renderLocationInfo = () => (
    <div className="space-y-6">
      <div>
        <h4 className="text-lg font-medium text-gray-900 mb-4">Pickup Location</h4>
        <MapSelector
          initialLocation={
            formData.pickupLatitude && formData.pickupLongitude
              ? {
                  lat: formData.pickupLatitude,
                  lng: formData.pickupLongitude,
                  address: formData.pickupAddress
                }
              : undefined
          }
          onLocationSelect={handlePickupLocationSelect}
          placeholder="Search pickup location..."
          className="mb-6"
        />
      </div>

      <div>
        <h4 className="text-lg font-medium text-gray-900 mb-4">Delivery Location</h4>
        <MapSelector
          initialLocation={
            formData.deliveryLatitude && formData.deliveryLongitude
              ? {
                  lat: formData.deliveryLatitude,
                  lng: formData.deliveryLongitude,
                  address: formData.deliveryAddress
                }
              : undefined
          }
          onLocationSelect={handleDeliveryLocationSelect}
          placeholder="Search delivery location..."
          className="mb-6"
        />
      </div>
    </div>
  );

  const renderScheduleInfo = () => (
    <div className="space-y-6">
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
    </div>
  );

  const renderPricingInfo = () => (
    <div className="space-y-6">
      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
        <div>
          <label htmlFor="price" className="block text-sm font-medium text-gray-700">Price</label>
          <input
            type="number"
            name="price"
            id="price"
            value={formData.price || ''}
            onChange={handleChange}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
            required
          />
        </div>
        <div>
          <label htmlFor="currency" className="block text-sm font-medium text-gray-700">Currency</label>
          <select
            name="currency"
            id="currency"
            value={formData.currency}
            onChange={handleChange}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
          >
            <option value="USD">USD</option>
            <option value="EUR">EUR</option>
            <option value="GBP">GBP</option>
          </select>
        </div>
      </div>
      <div className="flex justify-between pt-4">
        <button
          type="button"
          onClick={prevStep}
          className="inline-flex justify-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
        >
          Previous
        </button>
        <div className="flex space-x-3">
          {onCancel && (
            <button
              type="button"
              onClick={onCancel}
              className="inline-flex justify-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            >
              Cancel
            </button>
          )}
          <button
            type="submit"
            disabled={loading}
            className={`inline-flex justify-center rounded-md border border-transparent px-4 py-2 text-sm font-medium text-white shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 ${
              loading
                ? 'bg-indigo-400 cursor-not-allowed'
                : 'bg-indigo-600 hover:bg-indigo-700 focus:ring-indigo-500'
            }`}
          >
            {loading ? 'Creating...' : 'Create Load'}
          </button>
        </div>
      </div>
    </div>
  );

  const renderStepContent = () => {
    switch (currentStep) {
      case 'basic':
        return renderBasicInfo();
      case 'location':
        return renderLocationInfo();
      case 'schedule':
        return renderScheduleInfo();
      case 'pricing':
        return renderPricingInfo();
      default:
        return null;
    }
  };

  const isLastStep = currentStep === 'pricing';
  const isFirstStep = currentStep === 'basic';

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="mb-8">
        <nav className="flex justify-center" aria-label="Progress">
          <ol role="list" className="flex items-center">
            {['basic', 'location', 'schedule', 'pricing'].map((step, index) => (
              <li key={step} className={`relative ${index !== 0 ? 'ml-8' : ''}`}>
                <div className="flex items-center">
                  {index !== 0 && (
                    <div className="absolute inset-0 -left-8 flex items-center" aria-hidden="true">
                      <div className={`h-0.5 w-8 ${currentStep === step || ['location', 'schedule', 'pricing'].includes(currentStep) && index < ['basic', 'location', 'schedule', 'pricing'].indexOf(currentStep) ? 'bg-indigo-600' : 'bg-gray-200'}`} />
                    </div>
                  )}
                  <div
                    className={`relative flex h-8 w-8 items-center justify-center rounded-full border-2 ${
                      currentStep === step
                        ? 'border-indigo-600 bg-white'
                        : ['location', 'schedule', 'pricing'].includes(currentStep) && index < ['basic', 'location', 'schedule', 'pricing'].indexOf(currentStep)
                        ? 'border-indigo-600 bg-indigo-600'
                        : 'border-gray-300 bg-white'
                    }`}
                  >
                    <span
                      className={`h-2.5 w-2.5 rounded-full ${
                        currentStep === step
                          ? 'bg-indigo-600'
                          : ['location', 'schedule', 'pricing'].includes(currentStep) && index < ['basic', 'location', 'schedule', 'pricing'].indexOf(currentStep)
                          ? 'bg-white'
                          : 'bg-transparent'
                      }`}
                    />
                  </div>
                </div>
                <div className="absolute -bottom-8 w-32 text-center text-sm font-medium" style={{ left: '-48px' }}>
                  {step.charAt(0).toUpperCase() + step.slice(1)}
                </div>
              </li>
            ))}
          </ol>
        </nav>
      </div>

      <form onSubmit={handleSubmit} className="space-y-8 divide-y divide-gray-200">
        {error && (
          <div className="rounded-md bg-red-50 p-4 mb-4">
            <div className="flex">
              <div className="ml-3">
                <h3 className="text-sm font-medium text-red-800">{error}</h3>
              </div>
            </div>
          </div>
        )}

        <div className="space-y-8 divide-y divide-gray-200">
          <div>
            <div>
              <h3 className="text-lg leading-6 font-medium text-gray-900">
                {currentStep.charAt(0).toUpperCase() + currentStep.slice(1)} Information
              </h3>
              <p className="mt-1 text-sm text-gray-500">
                Please provide the {currentStep} details for your load.
              </p>
            </div>

            <div className="mt-6">
              {renderStepContent()}
            </div>
          </div>
        </div>

        <div className="pt-5">
          <div className="flex justify-between">
            <div>
              {!isFirstStep && (
                <button
                  type="button"
                  onClick={prevStep}
                  className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Previous
                </button>
              )}
            </div>
            <div className="flex">
              {onCancel && (
                <button
                  type="button"
                  onClick={onCancel}
                  className="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Cancel
                </button>
              )}
              {isLastStep ? (
                <button
                  type="submit"
                  disabled={loading}
                  className="ml-3 inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  {loading ? 'Saving...' : isEdit ? 'Update Load' : 'Create Load'}
                </button>
              ) : (
                <button
                  type="button"
                  onClick={nextStep}
                  className="ml-3 inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  Next
                </button>
              )}
            </div>
          </div>
        </div>
      </form>
    </div>
  );
};

export default LoadFormWizard;