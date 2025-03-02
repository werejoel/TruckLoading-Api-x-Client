import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const UnauthorizedPage: React.FC = () => {
  const navigate = useNavigate();
  const { hasRole } = useAuth();

  const handleBackToDashboard = () => {
    if (hasRole('Company')) {
      navigate('/company/dashboard');
    } else if (hasRole('Trucker')) {
      navigate('/dashboard'); // Can be changed to a trucker-specific dashboard later
    } else if (hasRole('Shipper')) {
      navigate('/dashboard'); // Can be changed to a shipper-specific dashboard later
    } else {
      navigate('/dashboard'); // Default fallback
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <h1 className="text-center text-3xl font-extrabold text-red-600">
          Unauthorized Access
        </h1>
        <h2 className="mt-2 text-center text-sm text-gray-600">
          You don't have permission to access this page
        </h2>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <div className="text-center">
            <p className="text-gray-700 mb-6">
              Sorry, you don't have the necessary permissions to view this page. Please contact an administrator if you believe this is an error.
            </p>
            <button
              onClick={handleBackToDashboard}
              className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              Return to Dashboard
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default UnauthorizedPage; 