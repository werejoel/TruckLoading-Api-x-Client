import React from 'react';
import CompanyRegisterForm from '../components/auth/CompanyRegisterForm';

const CompanyRegisterPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <h1 className="text-center text-3xl font-extrabold text-gray-900">
          TruckLoading App
        </h1>
        <h2 className="mt-2 text-center text-sm text-gray-600">
          Register as a Company
        </h2>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <CompanyRegisterForm />
      </div>
    </div>
  );
};

export default CompanyRegisterPage; 