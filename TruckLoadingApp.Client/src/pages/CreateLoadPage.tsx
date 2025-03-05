import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import LoadFormWizard from '../components/forms/LoadFormWizard';
import { LoadCreateRequest } from '../services/shipper.service';
import shipperService from '../services/shipper.service';

const CreateLoadPage: React.FC = () => {
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);

  const handleSuccess = async (loadData: LoadCreateRequest) => {
    try {
      const createdLoad = await shipperService.createLoad(loadData);
      navigate('/shipper/dashboard', {
        state: { message: 'Load created successfully!' }
      });
    } catch (error: any) {
      console.error('Error creating load:', error);
      setError(error.response?.data?.message || 'Failed to create load. Please try again.');
    }
  };

  const handleCancel = () => {
    navigate('/shipper/dashboard');
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="mb-6">
            <h1 className="text-2xl font-semibold text-gray-900">Create New Load</h1>
            <p className="mt-1 text-sm text-gray-500">
              Please fill in the load details using the form below.
            </p>
          </div>
          {error && (
            <div className="mb-4 rounded-md bg-red-50 p-4">
              <div className="flex">
                <div className="ml-3">
                  <h3 className="text-sm font-medium text-red-800">{error}</h3>
                </div>
              </div>
            </div>
          )}
          <LoadFormWizard 
            onSuccess={handleSuccess} 
            onCancel={handleCancel} 
          />
        </div>
      </div>
    </Layout>
  );
};

export default CreateLoadPage; 