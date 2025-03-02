import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';

// Validation schema
const CompanyRegisterSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email address')
    .required('Email is required'),
  password: Yup.string()
    .min(8, 'Password must be at least 8 characters')
    .required('Password is required'),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref('password')], 'Passwords must match')
    .required('Confirm password is required'),
  firstName: Yup.string()
    .required('First name is required'),
  lastName: Yup.string()
    .required('Last name is required'),
  companyName: Yup.string()
    .required('Company name is required'),
  companyAddress: Yup.string()
    .required('Company address is required'),
  companyRegistrationNumber: Yup.string()
    .required('Company registration number is required'),
  companyContact: Yup.string()
    .required('Company contact is required'),
});

const CompanyRegisterForm: React.FC = () => {
  const { registerCompany } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (values: any, { setSubmitting }: any) => {
    try {
      setError(null);
      await registerCompany(
        values.email,
        values.password,
        values.confirmPassword,
        values.firstName,
        values.lastName,
        values.companyName,
        values.companyAddress,
        values.companyRegistrationNumber,
        values.companyContact
      );
      navigate('/company/dashboard'); // Redirect to company dashboard after successful registration
    } catch (err: any) {
      console.error('Registration error:', err);
      
      // Handle different types of error responses
      if (err.response) {
        // The request was made and the server responded with a status code
        // that falls out of the range of 2xx
        const responseData = err.response.data;
        
        if (responseData.Message) {
          setError(responseData.Message);
        } else if (responseData.errors) {
          // Handle validation errors from ASP.NET Core
          const errorMessages = Object.values(responseData.errors).flat();
          setError(errorMessages.join(', '));
        } else if (responseData.Errors && Array.isArray(responseData.Errors)) {
          // Handle our custom error format
          setError(responseData.Errors.join(', '));
        } else {
          setError('Registration failed. Please check your information and try again.');
        }
      } else if (err.request) {
        // The request was made but no response was received
        setError('No response from server. Please check your internet connection and try again.');
      } else {
        // Something happened in setting up the request that triggered an Error
        setError('An error occurred during registration. Please try again later.');
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-md mx-auto p-6 bg-white rounded-lg shadow-md">
      <h2 className="text-2xl font-bold mb-6 text-center">Register as a Company</h2>
      
      {error && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">
          {error}
        </div>
      )}
      
      <Formik
        initialValues={{
          email: '',
          password: '',
          confirmPassword: '',
          firstName: '',
          lastName: '',
          companyName: '',
          companyAddress: '',
          companyRegistrationNumber: '',
          companyContact: '',
        }}
        validationSchema={CompanyRegisterSchema}
        onSubmit={handleSubmit}
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
                Admin First Name
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
                Admin Last Name
              </label>
              <Field
                type="text"
                name="lastName"
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
              <ErrorMessage name="lastName" component="div" className="mt-1 text-sm text-red-600" />
            </div>
            
            <div>
              <label htmlFor="companyName" className="block text-sm font-medium text-gray-700">
                Company Name
              </label>
              <Field
                type="text"
                name="companyName"
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
              <ErrorMessage name="companyName" component="div" className="mt-1 text-sm text-red-600" />
            </div>
            
            <div>
              <label htmlFor="companyAddress" className="block text-sm font-medium text-gray-700">
                Company Address
              </label>
              <Field
                type="text"
                name="companyAddress"
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
              <ErrorMessage name="companyAddress" component="div" className="mt-1 text-sm text-red-600" />
            </div>
            
            <div>
              <label htmlFor="companyRegistrationNumber" className="block text-sm font-medium text-gray-700">
                Company Registration Number
              </label>
              <Field
                type="text"
                name="companyRegistrationNumber"
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
              <ErrorMessage name="companyRegistrationNumber" component="div" className="mt-1 text-sm text-red-600" />
            </div>
            
            <div>
              <label htmlFor="companyContact" className="block text-sm font-medium text-gray-700">
                Company Contact
              </label>
              <Field
                type="text"
                name="companyContact"
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
              <ErrorMessage name="companyContact" component="div" className="mt-1 text-sm text-red-600" />
            </div>
            
            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                Password
              </label>
              <Field
                type="password"
                name="password"
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
              <ErrorMessage name="password" component="div" className="mt-1 text-sm text-red-600" />
            </div>
            
            <div>
              <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700">
                Confirm Password
              </label>
              <Field
                type="password"
                name="confirmPassword"
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              />
              <ErrorMessage name="confirmPassword" component="div" className="mt-1 text-sm text-red-600" />
            </div>
            
            <div>
              <button
                type="submit"
                disabled={isSubmitting}
                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
              >
                {isSubmitting ? 'Registering...' : 'Register'}
              </button>
            </div>
          </Form>
        )}
      </Formik>
      
      <div className="mt-4 text-center">
        <p className="text-sm text-gray-600">
          Already have an account?{' '}
          <button
            onClick={() => navigate('/login')}
            className="font-medium text-indigo-600 hover:text-indigo-500"
          >
            Sign in
          </button>
        </p>
      </div>
      
      <div className="mt-4 text-center">
        <p className="text-sm text-gray-600">
          Are you an individual trucker?{' '}
          <button
            onClick={() => navigate('/register/trucker')}
            className="font-medium text-indigo-600 hover:text-indigo-500"
          >
            Register as an Individual Trucker
          </button>
        </p>
      </div>
    </div>
  );
};

export default CompanyRegisterForm; 