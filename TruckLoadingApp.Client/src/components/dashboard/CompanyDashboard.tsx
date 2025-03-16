import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { Link } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import api from '../../services/api';
import driverService from '../../services/driver.service';
import truckService from '../../services/truck.service';
import { Driver, DriverRegisterRequest, AssignDriverRequest } from '../../types/driver.types';
import { Truck as TruckEntity, TruckRegistrationRequest, TruckType } from '../../types/truck.types';

// Types
interface Truck {
  id: number;
  numberPlate: string;
  truckType: string;
  loadCapacityWeight: number;
  isApproved: boolean;
  operationalStatus: string;
}

// Extended User interface to include company-specific fields
interface CompanyUser {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  companyName?: string;
  companyRegistrationNumber?: string;
}

// Validation schema for driver registration
const DriverRegistrationSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email address')
    .required('Email is required'),
  firstName: Yup.string()
    .required('First name is required'),
  lastName: Yup.string()
    .required('Last name is required'),
  licenseNumber: Yup.string()
    .required('License number is required'),
  licenseExpiryDate: Yup.date()
    .required('License expiry date is required')
    .min(new Date(), 'License must not be expired'),
  experience: Yup.number()
    .min(0, 'Experience cannot be negative')
    .nullable(),
  phoneNumber: Yup.string()
    .nullable(),
});

// Validation schema for assigning driver to truck
const AssignDriverSchema = Yup.object().shape({
  driverId: Yup.number()
    .required('Driver is required'),
  truckId: Yup.number()
    .required('Truck is required'),
});

// Validation schema for truck registration
const TruckRegistrationSchema = Yup.object().shape({
  truckTypeId: Yup.number()
    .required('Truck type is required')
    .min(1, 'Please select a truck type'),
  numberPlate: Yup.string()
    .required('Number plate is required')
    .max(50, 'Number plate cannot exceed 50 characters'),
  loadCapacityWeight: Yup.number()
    .required('Load capacity weight is required')
    .min(0, 'Load capacity weight cannot be negative'),
  loadCapacityVolume: Yup.number()
    .required('Load capacity volume is required')
    .min(0, 'Load capacity volume cannot be negative'),
  height: Yup.number()
    .min(0, 'Height cannot be negative')
    .nullable(),
  width: Yup.number()
    .min(0, 'Width cannot be negative')
    .nullable(),
  length: Yup.number()
    .min(0, 'Length cannot be negative')
    .nullable(),
  availabilityStartDate: Yup.date()
    .required('Availability start date is required'),
  availabilityEndDate: Yup.date()
    .required('Availability end date is required')
    .min(
      Yup.ref('availabilityStartDate'),
      'End date must be after start date'
    ),
});

const CompanyDashboard: React.FC = () => {
  const { user } = useAuth();
  const companyUser = user as CompanyUser;
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [trucks, setTrucks] = useState<Truck[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState('drivers');
  const [availableDrivers, setAvailableDrivers] = useState<Driver[]>([]);
  const [availableTrucks, setAvailableTrucks] = useState<Truck[]>([]);
  const [truckTypes, setTruckTypes] = useState<TruckType[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);

        try {
          // Fetch drivers associated with this company
          const driversData = await driverService.getCompanyDrivers();
          setDrivers(driversData);
        } catch (driversError: any) {
          console.error('Error fetching drivers:', driversError);
          // Don't set error yet, try to fetch trucks first
        }

        try {
          // Fetch trucks associated with this company
          const trucksResponse = await api.get('/company/trucks');
          setTrucks(trucksResponse.data);

          // Filter available trucks (those without assigned drivers)
          const availableTrucksData = trucksResponse.data.filter(
            (truck: Truck) => !drivers.some(driver => driver.truckId === truck.id)
          );
          setAvailableTrucks(availableTrucksData);
        } catch (trucksError: any) {
          console.error('Error fetching trucks:', trucksError);
          // Don't set error yet
        }

        try {
          // Fetch truck types
          const truckTypesData = await truckService.getTruckTypes();
          setTruckTypes(truckTypesData);
        } catch (truckTypesError: any) {
          console.error('Error fetching truck types:', truckTypesError);
        }

        try {
          // Fetch available drivers (those not assigned to trucks)
          const availableDriversData = await driverService.getAvailableDrivers();
          setAvailableDrivers(availableDriversData);
        } catch (availableDriversError: any) {
          console.error('Error fetching available drivers:', availableDriversError);
        }

        // If we got here without throwing, we succeeded at least partially
      } catch (err: any) {
        console.error('Error fetching company data:', err);
        const errorMessage = err.response?.data?.message ||
          err.response?.data?.Message ||
          err.message ||
          'Failed to load company data. Please try again later.';
        setError(errorMessage);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleRegisterDriver = async (values: DriverRegisterRequest, { resetForm, setSubmitting }: any) => {
    try {
      setError(null);
      setSuccessMessage(null);

      // Register a driver under this company
      const response = await driverService.registerDriver(values);

      if (response) {
        setSuccessMessage(`Driver registered successfully! ID: ${response.driverId}`);
        resetForm();

        // Refresh the drivers list
        const driversData = await driverService.getCompanyDrivers();
        setDrivers(driversData);
      } else {
        setError('Failed to register driver.');
      }
    } catch (err: any) {
      console.error('Error registering driver:', err);
      const errorMessage = err.response?.data?.message ||
        err.response?.data?.Message ||
        err.message ||
        'An error occurred while registering the driver.';
      setError(errorMessage);
    } finally {
      setSubmitting(false);
    }
  };

  const handleAssignDriver = async (values: AssignDriverRequest, { resetForm, setSubmitting }: any) => {
    try {
      setError(null);
      setSuccessMessage(null);

      // Assign driver to truck
      const response = await driverService.assignDriverToTruck(values);

      if (response) {
        setSuccessMessage(response.message || 'Driver assigned to truck successfully!');
        resetForm();

        // Refresh the data
        const driversData = await driverService.getCompanyDrivers();
        setDrivers(driversData);

        const availableDriversData = await driverService.getAvailableDrivers();
        setAvailableDrivers(availableDriversData);

        const trucksResponse = await api.get('/company/trucks');
        setTrucks(trucksResponse.data);

        // Update available trucks
        const availableTrucksData = trucksResponse.data.filter(
          (truck: Truck) => !driversData.some(driver => driver.truckId === truck.id)
        );
        setAvailableTrucks(availableTrucksData);
      } else {
        setError('Failed to assign driver to truck.');
      }
    } catch (err: any) {
      console.error('Error assigning driver to truck:', err);
      const errorMessage = err.response?.data?.message ||
        err.response?.data?.Message ||
        err.message ||
        'An error occurred while assigning the driver to truck.';
      setError(errorMessage);
    } finally {
      setSubmitting(false);
    }
  };

  const handleRegisterTruck = async (values: TruckRegistrationRequest, { resetForm, setSubmitting }: any) => {
    try {
      setError(null);
      setSuccessMessage(null);

      // Register a truck under this company
      const response = await truckService.registerTruck(values);

      if (response) {
        setSuccessMessage(`Truck registered successfully! ID: ${response.truckId}`);
        resetForm();

        // Refresh the trucks list
        const trucksResponse = await api.get('/company/trucks');
        setTrucks(trucksResponse.data);
      } else {
        setError('Failed to register truck.');
      }
    } catch (err: any) {
      console.error('Error registering truck:', err);
      const errorMessage = err.response?.data?.message ||
        err.response?.data?.Message ||
        err.message ||
        'An error occurred while registering the truck.';
      setError(errorMessage);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6 text-center">Company Dashboard</h1>
      <div className="bg-white rounded-lg shadow-md p-6 mb-8">
        <h2 className="text-xl font-semibold mb-4">Company Information</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <p className="text-gray-600">Company Name</p>
            <p className="font-medium">{companyUser?.companyName || 'N/A'}</p>
          </div>
          <div>
            <p className="text-gray-600">Admin</p>
            <p className="font-medium">{companyUser?.firstName} {companyUser?.lastName}</p>
          </div>
          <div>
            <p className="text-gray-600">Email</p>
            <p className="font-medium">{companyUser?.email}</p>
          </div>
          <div>
            <p className="text-gray-600">Registration Number</p>
            <p className="font-medium">{companyUser?.companyRegistrationNumber || 'N/A'}</p>
          </div>
        </div>
      </div>

      <div className="mb-6">
        <div className="border-b border-gray-200">
          <nav className="-mb-px flex space-x-8">
            <div style={{
              display: 'flex',
             overflow:"inherit",
              justifyContent:"center",
              paddingRight:"10px",
              scrollbarWidth: 'none',
              borderBottom: '1px solid #e5e7eb',
              backgroundColor:" #e5e7eb",
              borderRadius:"5px",
              marginBottom: '1rem'
            }}>
              <button
                onClick={() => setActiveTab('drivers')}
                style={{
                  position: 'relative',
                  padding: '1rem 1.25rem',
                  fontSize: '0.875rem',
                  fontWeight: activeTab === 'drivers' ? '600' : '500',
                  background: 'transparent',
                  color: activeTab === 'drivers' ? '#4f46e5' : '#6b7280',
                  border: 'none',
                  borderRadius:"5px",
                  borderBottom: `2px solid ${activeTab === 'drivers' ? '#4f46e5' : 'transparent'}`,
                  whiteSpace: 'nowrap',
                  cursor: 'pointer',
                  transition: 'all 0.15s ease'
                }}
                className={`${activeTab === 'drivers'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-white-500 hover:text-gray-700 hover:border-gray-300'
                  } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
              >
                Drivers
              </button>

              <button
                onClick={() => setActiveTab('trucks')}
                style={{
                  position: 'relative',
                  padding: '1rem 1.25rem',
                  fontSize: '0.875rem',
                  fontWeight: activeTab === 'trucks' ? '600' : '500',
                  background: 'transparent',
                  color: activeTab === 'trucks' ? '#4f46e5' : '#6b7280',
                  border: 'none',
                  borderBottom: `2px solid ${activeTab === 'trucks' ? '#4f46e5' : 'transparent'}`,
                  whiteSpace: 'nowrap',
                  cursor: 'pointer',
                  transition: 'all 0.15s ease'
                }}
                className={`${activeTab === 'trucks'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
              >
                Trucks
              </button>

              <button
                onClick={() => setActiveTab('register-driver')}
                style={{
                  position: 'relative',
                  padding: '1rem 1.25rem',
                  fontSize: '0.875rem',
                  fontWeight: activeTab === 'register-driver' ? '600' : '500',
                  background: 'transparent',
                  color: activeTab === 'register-driver' ? '#4f46e5' : '#6b7280',
                  border: 'none',
                  borderBottom: `2px solid ${activeTab === 'register-driver' ? '#4f46e5' : 'transparent'}`,
                  whiteSpace: 'nowrap',
                  cursor: 'pointer',
                  transition: 'all 0.15s ease'
                }}
                className={`${activeTab === 'register-driver'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
              >
                Register Driver
              </button>

              <button
                onClick={() => setActiveTab('register-truck')}
                style={{
                  position: 'relative',
                  padding: '1rem 1.25rem',
                  fontSize: '0.875rem',
                  fontWeight: activeTab === 'register-truck' ? '600' : '500',
                  background: 'transparent',
                  color: activeTab === 'register-truck' ? '#4f46e5' : '#6b7280',
                  border: 'none',
                  borderBottom: `2px solid ${activeTab === 'register-truck' ? '#4f46e5' : 'transparent'}`,
                  whiteSpace: 'nowrap',
                  cursor: 'pointer',
                  transition: 'all 0.15s ease'
                }}
                className={`${activeTab === 'register-truck'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
              >
                Register Truck
              </button>

              <button
                onClick={() => setActiveTab('assign-driver')}
                style={{
                  position: 'relative',
                  padding: '1rem 1.25rem',
                  fontSize: '0.875rem',
                  fontWeight: activeTab === 'assign-driver' ? '600' : '500',
                  background: 'transparent',
                  color: activeTab === 'assign-driver' ? '#4f46e5' : '#6b7280',
                  border: 'none',
                  borderBottom: `2px solid ${activeTab === 'assign-driver' ? '#4f46e5' : 'transparent'}`,
                  whiteSpace: 'nowrap',
                  cursor: 'pointer',
                  transition: 'all 0.15s ease'
                }}
                className={`${activeTab === 'assign-driver'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
              >
                Assign Driver
              </button>
            </div>
          </nav>
        </div>
      </div>

      {loading ? (
        <div className="flex justify-center items-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-500"></div>
        </div>
      ) : (
        <>
          {error && (
            <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">
              {error}
            </div>
          )}

          {successMessage && (
            <div className="mb-4 p-3 bg-green-100 text-green-700 rounded">
              {successMessage}
            </div>
          )}

          {activeTab === 'drivers' && (
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="px-4 py-5 sm:px-6">
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Company Drivers
                </h3>
                <p className="mt-1 max-w-2xl text-sm text-gray-500">
                  Manage your company's drivers
                </p>
              </div>

              {drivers.length === 0 ? (
                <div className="px-4 py-5 sm:p-6 text-center text-gray-500">
                  No drivers registered yet. Click on "Register Driver" to add one.
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Name
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          License
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Truck
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Status
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Actions
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {drivers.map((driver) => (
                        <tr key={driver.id}>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm font-medium text-gray-900">
                              {driver.firstName} {driver.lastName}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-500">
                              {driver.licenseNumber}
                              <span className="block text-xs">
                                Expires: {new Date(driver.licenseExpiryDate).toLocaleDateString()}
                              </span>
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-500">
                              {driver.truckNumberPlate || 'Not assigned'}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${driver.isAvailable
                                ? 'bg-green-100 text-green-800'
                                : 'bg-yellow-100 text-yellow-800'
                              }`}>
                              {driver.isAvailable ? 'Available' : 'On Duty'}
                            </span>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                            <Link to={`/company/drivers/${driver.id}`} className="text-indigo-600 hover:text-indigo-900 mr-4">
                              View
                            </Link>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {activeTab === 'trucks' && (
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="px-4 py-5 sm:px-6">
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Company Trucks
                </h3>
                <p className="mt-1 max-w-2xl text-sm text-gray-500">
                  Manage your company's trucks
                </p>
              </div>

              {trucks.length === 0 ? (
                <div className="px-4 py-5 sm:p-6 text-center text-gray-500">
                  No trucks registered yet.
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Number Plate
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Type
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Capacity (kg)
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Status
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Actions
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {trucks.map((truck) => (
                        <tr key={truck.id}>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm font-medium text-gray-900">
                              {truck.numberPlate}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-500">{truck.truckType}</div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-500">
                              {truck.loadCapacityWeight.toLocaleString()} kg
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${truck.isApproved
                                ? 'bg-green-100 text-green-800'
                                : 'bg-yellow-100 text-yellow-800'
                              }`}>
                              {truck.isApproved ? 'Approved' : 'Pending Approval'}
                            </span>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                            <Link to={`/company/trucks/${truck.id}`} className="text-indigo-600 hover:text-indigo-900 mr-4">
                              View
                            </Link>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {activeTab === 'register-driver' && (
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="px-4 py-5 sm:px-6">
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Register New Driver
                </h3>
                <p className="mt-1 max-w-2xl text-sm text-gray-500">
                  Add a new driver to your company
                </p>
              </div>

              <div className="px-4 py-5 sm:p-6">
                <Formik
                  initialValues={{
                    email: '',
                    firstName: '',
                    lastName: '',
                    licenseNumber: '',
                    licenseExpiryDate: '',
                    experience: undefined,
                    phoneNumber: '',
                  } as DriverRegisterRequest}
                  validationSchema={DriverRegistrationSchema}
                  onSubmit={handleRegisterDriver}
                >
                  {({ isSubmitting }) => (
                    <Form className="space-y-4">
                      <div>
                        <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                          Email
                        </label>
                        <Field
                          type="email"
                          name="email"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="email" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="firstName" className="block text-sm font-medium text-gray-700">
                          First Name
                        </label>
                        <Field
                          type="text"
                          name="firstName"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="firstName" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="lastName" className="block text-sm font-medium text-gray-700">
                          Last Name
                        </label>
                        <Field
                          type="text"
                          name="lastName"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="lastName" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="licenseNumber" className="block text-sm font-medium text-gray-700">
                          License Number
                        </label>
                        <Field
                          type="text"
                          name="licenseNumber"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="licenseNumber" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="licenseExpiryDate" className="block text-sm font-medium text-gray-700">
                          License Expiry Date
                        </label>
                        <Field
                          type="date"
                          name="licenseExpiryDate"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="licenseExpiryDate" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="experience" className="block text-sm font-medium text-gray-700">
                          Experience (years)
                        </label>
                        <Field
                          type="number"
                          name="experience"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="experience" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="phoneNumber" className="block text-sm font-medium text-gray-700">
                          Phone Number
                        </label>
                        <Field
                          type="text"
                          name="phoneNumber"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="phoneNumber" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div className="pt-4">
                        <button
                          type="submit"
                          disabled={isSubmitting}
                          className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
                        >
                          {isSubmitting ? 'Registering...' : 'Register Driver'}
                        </button>
                      </div>
                    </Form>
                  )}
                </Formik>
              </div>
            </div>
          )}

          {activeTab === 'register-truck' && (
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="px-4 py-5 sm:px-6">
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Register New Truck
                </h3>
                <p className="mt-1 max-w-2xl text-sm text-gray-500">
                  Add a new truck to your company fleet
                </p>
              </div>

              <div className="px-4 py-5 sm:p-6">
                <Formik
                  initialValues={{
                    truckTypeId: 0,
                    numberPlate: '',
                    loadCapacityWeight: 0,
                    loadCapacityVolume: 0,
                    height: undefined,
                    width: undefined,
                    length: undefined,
                    availabilityStartDate: new Date(),
                    availabilityEndDate: new Date(new Date().setFullYear(new Date().getFullYear() + 1)),
                  } as TruckRegistrationRequest}
                  validationSchema={TruckRegistrationSchema}
                  onSubmit={handleRegisterTruck}
                >
                  {({ isSubmitting }) => (
                    <Form className="space-y-4">
                      <div>
                        <label htmlFor="truckTypeId" className="block text-sm font-medium text-gray-700">
                          Truck Type
                        </label>
                        <Field
                          as="select"
                          name="truckTypeId"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        >
                          <option value="">Select a truck type</option>
                          {truckTypes.map(type => (
                            <option key={type.id} value={type.id}>
                              {type.name}
                            </option>
                          ))}
                        </Field>
                        <ErrorMessage name="truckTypeId" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="numberPlate" className="block text-sm font-medium text-gray-700">
                          Number Plate
                        </label>
                        <Field
                          type="text"
                          name="numberPlate"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="numberPlate" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="loadCapacityWeight" className="block text-sm font-medium text-gray-700">
                          Load Capacity Weight (kg)
                        </label>
                        <Field
                          type="number"
                          name="loadCapacityWeight"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="loadCapacityWeight" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div>
                        <label htmlFor="loadCapacityVolume" className="block text-sm font-medium text-gray-700">
                          Load Capacity Volume (m³)
                        </label>
                        <Field
                          type="number"
                          name="loadCapacityVolume"
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                        />
                        <ErrorMessage name="loadCapacityVolume" component="div" className="mt-1 text-sm text-red-600" />
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <div>
                          <label htmlFor="height" className="block text-sm font-medium text-gray-700">
                            Height (m)
                          </label>
                          <Field
                            type="number"
                            name="height"
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                          />
                          <ErrorMessage name="height" component="div" className="mt-1 text-sm text-red-600" />
                        </div>

                        <div>
                          <label htmlFor="width" className="block text-sm font-medium text-gray-700">
                            Width (m)
                          </label>
                          <Field
                            type="number"
                            name="width"
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                          />
                          <ErrorMessage name="width" component="div" className="mt-1 text-sm text-red-600" />
                        </div>

                        <div>
                          <label htmlFor="length" className="block text-sm font-medium text-gray-700">
                            Length (m)
                          </label>
                          <Field
                            type="number"
                            name="length"
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                          />
                          <ErrorMessage name="length" component="div" className="mt-1 text-sm text-red-600" />
                        </div>
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                          <label htmlFor="availabilityStartDate" className="block text-sm font-medium text-gray-700">
                            Availability Start Date
                          </label>
                          <Field
                            type="date"
                            name="availabilityStartDate"
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                          />
                          <ErrorMessage name="availabilityStartDate" component="div" className="mt-1 text-sm text-red-600" />
                        </div>

                        <div>
                          <label htmlFor="availabilityEndDate" className="block text-sm font-medium text-gray-700">
                            Availability End Date
                          </label>
                          <Field
                            type="date"
                            name="availabilityEndDate"
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                          />
                          <ErrorMessage name="availabilityEndDate" component="div" className="mt-1 text-sm text-red-600" />
                        </div>
                      </div>

                      <div className="pt-4">
                        <button
                          type="submit"
                          disabled={isSubmitting}
                          className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
                        >
                          {isSubmitting ? 'Registering...' : 'Register Truck'}
                        </button>
                      </div>
                    </Form>
                  )}
                </Formik>
              </div>
            </div>
          )}

          {activeTab === 'assign-driver' && (
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="px-4 py-5 sm:px-6">
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Assign Driver to Truck
                </h3>
                <p className="mt-1 max-w-2xl text-sm text-gray-500">
                  Assign an available driver to an available truck
                </p>
              </div>

              <div className="px-4 py-5 sm:p-6">
                {availableDrivers.length === 0 || availableTrucks.length === 0 ? (
                  <div className="text-center text-gray-500">
                    {availableDrivers.length === 0 ? 'No available drivers.' : ''}
                    {availableDrivers.length === 0 && availableTrucks.length === 0 ? ' ' : ''}
                    {availableTrucks.length === 0 ? 'No available trucks.' : ''}
                  </div>
                ) : (
                  <Formik
                    initialValues={{
                      driverId: 0,
                      truckId: 0,
                    }}
                    validationSchema={AssignDriverSchema}
                    onSubmit={handleAssignDriver}
                  >
                    {({ isSubmitting }) => (
                      <Form className="space-y-4">
                        <div>
                          <label htmlFor="driverId" className="block text-sm font-medium text-gray-700">
                            Driver
                          </label>
                          <Field
                            as="select"
                            name="driverId"
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                          >
                            <option value="">Select a driver</option>
                            {availableDrivers.map(driver => (
                              <option key={driver.id} value={driver.id}>
                                {driver.firstName} {driver.lastName} - {driver.licenseNumber}
                              </option>
                            ))}
                          </Field>
                          <ErrorMessage name="driverId" component="div" className="mt-1 text-sm text-red-600" />
                        </div>

                        <div>
                          <label htmlFor="truckId" className="block text-sm font-medium text-gray-700">
                            Truck
                          </label>
                          <Field
                            as="select"
                            name="truckId"
                            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                          >
                            <option value="">Select a truck</option>
                            {availableTrucks.map(truck => (
                              <option key={truck.id} value={truck.id}>
                                {truck.numberPlate} - {truck.truckType} ({truck.loadCapacityWeight} kg)
                              </option>
                            ))}
                          </Field>
                          <ErrorMessage name="truckId" component="div" className="mt-1 text-sm text-red-600" />
                        </div>

                        <div className="pt-4">
                          <button
                            type="submit"
                            disabled={isSubmitting}
                            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
                          >
                            {isSubmitting ? 'Assigning...' : 'Assign Driver to Truck'}
                          </button>
                        </div>
                      </Form>
                    )}
                  </Formik>
                )}
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default CompanyDashboard; 