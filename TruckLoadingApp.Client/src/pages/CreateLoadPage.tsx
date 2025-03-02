import React from 'react';
import { useNavigate } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import LoadForm from '../components/forms/LoadForm';
import { Load } from '../services/shipper.service';

const CreateLoadPage: React.FC = () => {
  const navigate = useNavigate();

  const handleSuccess = (load: Load) => {
    navigate('/shipper/dashboard');
  };

  const handleCancel = () => {
    navigate('/shipper/dashboard');
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <LoadForm 
            onSuccess={handleSuccess} 
            onCancel={handleCancel} 
          />
        </div>
      </div>
    </Layout>
  );
};

export default CreateLoadPage; 