import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import driverService from '../../services/driver.service';
import { Driver } from '../../types/driver.types';

// Validation schema for updating driver
const DriverUpdateSchema = Yup.object().shape({
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

const DriverDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [driver, setDriver] = useState<Driver | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [isEditing, setIsEditing] = useState(false);

  useEffect(() => {
    const fetchDriver = async () => {
      try {
        setLoading(true);
        setError(null);
        
        if (!id) {
          setError('Driver ID is missing');
          return;
        }
        
        const driverData = await driverService.getDriverById(parseInt(id));
        setDriver(driverData);
      } catch (err: any) {
        console.error('Error fetching driver:', err);
        const errorMessage = err.response?.data?.message || 
                            err.response?.data?.Message || 
                            err.message || 
                            'Failed to load driver data. Please try again later.';
        setError(errorMessage);
      } finally {
        setLoading(false);
      }
    };

    fetchDriver();
  }, [id]);

  const handleUpdateDriver = async (values: any) => {
    try {
      setError(null);
      setSuccessMessage(null);
      
      if (!id) {
        setError('Driver ID is missing');
        return;
      }
      
      // This is a placeholder for the actual update driver API call
      // You'll need to implement this in the driver service
      // const response = await driverService.updateDriver(parseInt(id), values);
      
      // For now, we'll just simulate a successful update
      setSuccessMessage('Driver updated successfully!');
      setDriver({ ...driver, ...values } as Driver);
      setIsEditing(false);
    } catch (err: any) {
      console.error('Error updating driver:', err);
      const errorMessage = err.response?.data?.message || 
                          err.response?.data?.Message || 
                          err.message || 
                          'An error occurred while updating the driver.';
      setError(errorMessage);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-500"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-100 text-red-700 p-4 rounded-md mb-4">
        <p>{error}</p>
        <button 
          onClick={() => navigate(-1)} 
          className="mt-2 text-sm text-indigo-600 hover:text-indigo-800"
        >
          Go Back
        </button>
      </div>
    );
  }

  if (!driver) {
    return (
      <div className="bg-yellow-100 text-yellow-700 p-4 rounded-md mb-4">
        <p>Driver not found</p>
        <button 
          onClick={() => navigate(-1)} 
          className="mt-2 text-sm text-indigo-600 hover:text-indigo-800"
        >
          Go Back
        </button>
      </div>
    );
  }

  return (
    <div className="bg-white shadow-md rounded-lg overflow-hidden">
      <div className="px-4 py-5 sm:px-6 flex justify-between items-center">
        <div>
          <h3 className="text-lg leading-6 font-medium text-gray-900">
            Driver Details
          </h3>
          <p className="mt-1 max-w-2xl text-sm text-gray-500">
            Personal and license information
          </p>
        </div>
        <div>
          <button
            onClick={() => navigate(-1)}
            className="mr-2 inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Back
          </button>
          <button
            onClick={() => setIsEditing(!isEditing)}
            className="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            {isEditing ? 'Cancel' : 'Edit'}
          </button>
        </div>
      </div>

      {successMessage && (
        <div className="mx-4 my-2 p-2 bg-green-100 text-green-700 rounded">
          {successMessage}
        </div>
      )}

      {isEditing ? (
        <div className="px-4 py-5 sm:p-6">
          <Formik
            initialValues={{
              firstName: driver.firstName,
              lastName: driver.lastName,
              licenseNumber: driver.licenseNumber,
              licenseExpiryDate: driver.licenseExpiryDate.split('T')[0], // Format date for input
              experience: driver.experience || 0,
              phoneNumber: driver.phoneNumber || '',
            }}
            validationSchema={DriverUpdateSchema}
            onSubmit={handleUpdateDriver}
          >
            {({ isSubmitting }) => (
              <Form className="space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
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
                </div>
                
                <div className="pt-4">
                  <button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
                  >
                    {isSubmitting ? 'Saving...' : 'Save Changes'}
                  </button>
                </div>
              </Form>
            )}
          </Formik>
        </div>
      ) : (
        <div className="border-t border-gray-200">
          <dl>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Full name</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {driver.firstName} {driver.lastName}
              </dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">License Number</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {driver.licenseNumber}
              </dd>
            </div>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">License Expiry</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {formatDate(driver.licenseExpiryDate)}
              </dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Experience</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {driver.experience ? `${driver.experience} years` : 'Not specified'}
              </dd>
            </div>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Phone Number</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {driver.phoneNumber || 'Not specified'}
              </dd>
            </div>
            <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Assigned Truck</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {driver.truckNumberPlate || 'Not assigned'}
              </dd>
            </div>
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
              <dt className="text-sm font-medium text-gray-500">Status</dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${
                  driver.isAvailable 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-yellow-100 text-yellow-800'
                }`}>
                  {driver.isAvailable ? 'Available' : 'On Duty'}
                </span>
              </dd>
            </div>
          </dl>
        </div>
      )}
    </div>
  );
};

export default DriverDetail; 