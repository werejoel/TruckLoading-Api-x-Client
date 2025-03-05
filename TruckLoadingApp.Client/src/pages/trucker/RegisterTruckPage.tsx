import React from 'react';
import { useNavigate } from 'react-router-dom';
import Layout from '../../components/layout/Layout';
import TruckRegistrationForm from '../../components/forms/TruckRegistrationForm';
import { toast } from 'react-toastify';

const RegisterTruckPage: React.FC = () => {
  const navigate = useNavigate();

  const handleSuccess = (truckId: number) => {
    toast.success('Truck registered successfully!');
    navigate('/trucker/dashboard');
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="mb-6">
            <h1 className="text-2xl font-semibold text-gray-900">Register New Truck</h1>
            <p className="mt-1 text-sm text-gray-500">
              Please provide your truck details. Make sure to select the appropriate category and type.
            </p>
          </div>
          <div className="bg-white shadow sm:rounded-lg">
            <div className="px-4 py-5 sm:p-6">
              <TruckRegistrationForm onSuccess={handleSuccess} />
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default RegisterTruckPage; 